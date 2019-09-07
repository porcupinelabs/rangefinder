using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using HidLibrary;

namespace LrDemo
{
    class FirmwareUpdate
    {
        private const int Lx4VendorId = 0x0417;
        private static readonly int[] SupportedBootloaders = new[] { 0xDD05 };
        private const int IN_REPORT_SIZE = 8;			// 8 byte report size
        private const int OUT_REPORT_SIZE = 65;			// 64 byte report size + one byte report ID
        private const int OUT_DATA_PER_PACKET = 60;		// 4 bytes of overhead in each packet, so 60 bytes of payload
        private const int CHUNK_SIZE = 0x4000;		    // Flash memory is written using this size at a time
        private const int CHUNK_WRAPPER_SIZE = CHUNK_SIZE + 48;
        private const int APPLICATION_SIZE = 0x00070000;// 448k (64k + 3 x 128k blocks)
        private const int NUM_CHUNKS = (APPLICATION_SIZE / CHUNK_SIZE); // 28 Chunks of 16k each

        // IN Report - Status Packet Definition
        //
        // 0x00 - Bootloader info
        //				Byte [0] = STS_BOOTLOADER_INFO
        //				Byte [1] = Bootloader version
        //				Byte [2] = Application status
        //				Byte [3] = Error code
        //				Byte [4] = 0
        //				Byte [5] = 0
        //				Byte [6] = 0
        //				Byte [7] = 0

        private const int STS_BOOTLOADER_INFO = 0x00;
        private const int STS_APPLICATION_INFO = 0x01;

        // OUT Report - Command Packet Defintion
        //
        // 0x00 - Get bootloader info:
        //				Byte [0] = 0x00
        //
        // 0x01 - Get application info:
        //				Byte [0] = 0x01
        //
        // 0x02 - Erase application:
        //				Byte [0] = 0x02
        //				Byte [1] = 0x55
        //				Byte [2] = 0xAA
        //				Byte [3] = 0x11
        //				Byte [4] = 0xFF
        //				Byte [5] = 0
        //				Byte [6] = 0
        //				Byte [7] = 0
        //
        // 0x03 - Write USB packet to chunk buffer:
        //				Byte [0] = 0x03
        //				Byte [2:1] = Buffer offset
        //				Byte [3] = Byte count
        //				Byte [3] = Data byte 0
        //				Byte [4] = Data byte 1
        //				Byte [5] = Data byte 2
        //				...
        //				Byte [63] = Data byte 59
        //
        // 0x04 - Write chunk buffer to flash:
        //				Byte [0] = 0x04
        //				Byte [4:1] = Flash offset
        //
        // 0x05 - Run application:
        //				Byte [0] = 0x05

        private const int CMD_GET_BOOTLOADER_INFO = 0x00;
        private const int CMD_GET_APPLICATION_INFO = 0x01;
        private const int CMD_ERASE_APPLICATION = 0x02;
        private const int CMD_WRITE_TO_BUFFER = 0x03;
        private const int CMD_WRITE_TO_FLASH = 0x04;
        private const int CMD_RUN_APPLICATION = 0x05;

        private const int BL_STATUS_FREE = 0x00;			// Boot loader is idle
        private const int BL_STATUS_BUSY = 0x01;			// Boot loader is busy
        private const int BL_STATUS_ERROR = 0xFF;           // Boot loader encountered an error

        private lr4 lx4Device;
        private TextBox MsgBox;
        private HidDevice BootloaderDevice;
        private int BootLoaderVersion;
        private int BootLoaderErrorCode;

        public FirmwareUpdate(lr4 lx4Device, TextBox MsgBox)
        {
            this.lx4Device = lx4Device;
            this.MsgBox = MsgBox;
        }

        public bool UpdateFirmware(string FwFilename)
        {
            BinaryReader InFile;
            int Chunk;

            // Check for the presence of the rangefinder device.  If found tell it to go to the bootloader.
            if (lx4Device != null)
                if (lx4Device.IsConnected)
                {
                    MsgBox.AppendText("Invoking bootloader\n");
                    lx4Device.InvokeBootloader();
                }

            // Wait for the bootloader device to show up.
            MsgBox.AppendText("Searching for rangefinder device bootloader\n");
            for (int i = 0; i < 50; i++)
            {
                MsgBox.AppendText(".");
                BootloaderDevice = HidDevices.Enumerate(Lx4VendorId, SupportedBootloaders).FirstOrDefault();
                if (BootloaderDevice != null)
                    break;
                Thread.Sleep(200);
            }
            MsgBox.AppendText("\n");
            if (BootloaderDevice == null)
            {
                MsgBox.AppendText("Could not find bootloader\n");
                return false;
            }
            else MsgBox.AppendText("Found bootloader\n");

            // Get the bootloader info
            if (GetBootloaderInfo() == false)
            {
                MsgBox.AppendText("Error getting bootloader info\n");
                return false;
            }
            MsgBox.AppendText("Bootloader version:"+BootLoaderVersion+"\n");

            // Open the firmware input file
            try
            {
                InFile = new BinaryReader(new FileStream(FwFilename, FileMode.Open));
            }
            catch (IOException e)
            {
                MsgBox.AppendText(e.Message+"\n");
                MsgBox.AppendText("Cannot open file.\n");
                return false;
            }

            // Erase the application flash area
            MsgBox.AppendText("Erasing flash\n");
            if (EraseApplication() == false)
            {
                MsgBox.AppendText("Error erasing flash\n");
                return false;
            }
            MsgBox.AppendText("Done\n");

            for (Chunk = 0; Chunk < NUM_CHUNKS; Chunk++)
            {
                byte[] ChunkData = InFile.ReadBytes(CHUNK_WRAPPER_SIZE);
                if (ChunkData.Length < CHUNK_WRAPPER_SIZE)
                    break;  // No such thing as a partial chunk

                MsgBox.AppendText("Chunk "+ Chunk + "\n");
                if (WriteChunkToBuffer(ChunkData) == false)
                {
                    MsgBox.AppendText("Error writing chunk to buffer\n");
                    return false;
                }

                if (WriteBufferToFlash(Chunk * CHUNK_SIZE) == false)
                {
                    MsgBox.AppendText("Error writing chunk to flash\n");
                    return false;
                }
            }
            InFile.Close();

            MsgBox.AppendText("Starting application\n");
            if (RunApplication() == false)
            {
                MsgBox.AppendText("Error starting application\n");
                return false;
            }
            MsgBox.AppendText("Done - Firmware update success\n");

            return true;
        }

        private bool WaitForStatus()
        {
            HidReport inReport = BootloaderDevice.ReadReport(1000);
            if (inReport.ReadStatus != HidDeviceData.ReadStatus.Success)
                return false;
            if (inReport.Data.Length != 8)
                return false;
            if (inReport.Data[0] == STS_BOOTLOADER_INFO)
            {
                BootLoaderVersion = inReport.Data[1];
                BootLoaderErrorCode = inReport.Data[3];
                if (BootLoaderErrorCode != 0)
                {
                    MsgBox.AppendText("Bootloader reported error:"+BootLoaderErrorCode+"\n");
                    return false;
                }
            }
            else
            {
                MsgBox.AppendText("Bootloader status packet=" + inReport.Data[0] + "\n");
                return false;
            }
            return true;
        }

        private bool GetBootloaderInfo()
        {
            HidReport outReport = BootloaderDevice.CreateReport();
            outReport.ReportId = 0x00;
            outReport.Data[0] = CMD_GET_BOOTLOADER_INFO;
            if (BootloaderDevice.WriteReport(outReport) == false)
                return false;
            return WaitForStatus();
        }

        private bool EraseApplication()
        {
            HidReport outReport = BootloaderDevice.CreateReport();
            outReport.ReportId = 0x00;
            outReport.Data[0] = CMD_ERASE_APPLICATION;
            outReport.Data[1] = 0x55;
            outReport.Data[2] = 0xAA;
            outReport.Data[3] = 0x11;
            outReport.Data[4] = 0xFF;
            if (BootloaderDevice.WriteReport(outReport) == false)
                return false;
            return WaitForStatus();
        }

        private bool WriteChunkToBuffer(byte[] ChunkData)
        {
            int ChunkOffset = 0x0000;
            int BytesRemaining = CHUNK_WRAPPER_SIZE;
            while (BytesRemaining > 0)
            {
                int PacketSize = Math.Min(OUT_DATA_PER_PACKET, BytesRemaining);
                byte[] PacketData = new byte[PacketSize];
                Array.Copy(ChunkData, PacketData, PacketSize);
                if (WritePacketToBuffer(PacketData, ChunkOffset, PacketSize) == false)
                    return false;
                ChunkOffset += PacketSize;
                BytesRemaining -= PacketSize;
                ChunkData = ChunkData.Skip(PacketSize).ToArray();
            }
            return true;
        }

        private bool WritePacketToBuffer(byte[] PacketData, int ChunkOffset, int ByteCount)
        {
            if (ByteCount > OUT_DATA_PER_PACKET)
                return false;   // Packet too big
            if ((ChunkOffset + ByteCount) > CHUNK_WRAPPER_SIZE)
                return false;   // Packet goes past end of chunk

            HidReport outReport = BootloaderDevice.CreateReport();
            outReport.ReportId = 0x00;
            outReport.Data[0] = CMD_WRITE_TO_BUFFER;
            outReport.Data[1] = (byte)(ChunkOffset & 0xFF);
            outReport.Data[2] = (byte)((ChunkOffset >> 8) & 0xFF);
            outReport.Data[3] = (byte)ByteCount;

            for (int i = 0; i < ByteCount; i++)
                outReport.Data[i + 4] = PacketData[i];

            if (BootloaderDevice.WriteReport(outReport) == false)
                return false;
            return WaitForStatus();
        }

        private bool WriteBufferToFlash(int Addr)
        {
            HidReport outReport = BootloaderDevice.CreateReport();
            outReport.ReportId = 0x00;
            outReport.Data[0] = CMD_WRITE_TO_FLASH;
            outReport.Data[1] = (byte)(Addr & 0xFF);
            outReport.Data[2] = (byte)((Addr >> 8) & 0xFF);
            outReport.Data[3] = (byte)((Addr >> 16) & 0xFF);
            outReport.Data[4] = (byte)((Addr >> 24) & 0xFF);
            if (BootloaderDevice.WriteReport(outReport) == false)
                return false;
            return WaitForStatus();
        }

        private bool RunApplication()
        {
            HidReport outReport = BootloaderDevice.CreateReport();
            outReport.ReportId = 0x00;
            outReport.Data[0] = CMD_RUN_APPLICATION;
            if (BootloaderDevice.WriteReport(outReport) == false)
                return false;
            return WaitForStatus();
        }
    }
}
