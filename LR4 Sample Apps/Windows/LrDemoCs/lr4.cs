using System;
using System.Collections.Generic;
using System.Threading;
using HidLibrary;
using System.Linq;
using System.Text;

namespace LrDemo
{
    public class lr4 : IDisposable
    {
        private const int Lx4VendorId = 0x0417;
        private uint InitState = 0;

        public event InsertedEventHandler Inserted;
        public event RemovedEventHandler Removed;
        public event DataRecievedEventHandler DataRecieved;

        public delegate void InsertedEventHandler();
        public delegate void RemovedEventHandler();
        public delegate void DataRecievedEventHandler(byte[] data);

        private readonly HidDevice lx4Device;
        private int _isReading;

        struct cfgItem
        {
            public string itemName;
            public int BlahBlah;
        }

        static cfgItem[] cfgItemTable = new cfgItem[] {
            new cfgItem{itemName = "000", BlahBlah = 0},    // Nothing (idle state)
            new cfgItem{itemName = "run", BlahBlah = 0},    // Run/Stop
            new cfgItem{itemName = "uni", BlahBlah = 0},    // Units
            new cfgItem{itemName = "mod", BlahBlah = 0},    // Mode
            new cfgItem{itemName = "int", BlahBlah = 0},    // Interval
            new cfgItem{itemName = "iun", BlahBlah = 0},    // Interval units
            new cfgItem{itemName = "trg", BlahBlah = 0},    // Trigger
            new cfgItem{itemName = "kbd", BlahBlah = 0},    // Keyboard emulation
            new cfgItem{itemName = "dbl", BlahBlah = 0},    // Do double measurements
            new cfgItem{itemName = "nfl", BlahBlah = 0},    // Don't filter errors
            new cfgItem{itemName = "chg", BlahBlah = 0},    // Only send changes
            new cfgItem{itemName = "rmd", BlahBlah = 0},    // Range mode
            new cfgItem{itemName = "mrt", BlahBlah = 0},    // Measurement rate
            new cfgItem{itemName = "bx1", BlahBlah = 0},    // Aiming X1
            new cfgItem{itemName = "by1", BlahBlah = 0},    // Aiming Y1
            new cfgItem{itemName = "bx2", BlahBlah = 0},    // Aiming X2
            new cfgItem{itemName = "by2", BlahBlah = 0}     // Aiming Y2
        };
    
	    public lr4(string devicePath) : this(HidDevices.GetDevice(devicePath)) { }

	    public lr4(HidDevice hidDevice)
	    {
		    lx4Device = hidDevice;

	        lx4Device.Inserted += Lx4Inserted;
            lx4Device.Removed += Lx4Removed;

		    if (!lx4Device.IsOpen)
                lx4Device.OpenDevice();
		    lx4Device.MonitorDeviceEvents = true;

		    BeginReadReport();
	    }

	    public string DevicePath { get { return lx4Device.DevicePath; } }
        public bool IsListening { get; private set; }
        public bool IsConnected { get { return lx4Device.IsConnected; } }

	    public static IEnumerable<lr4> Enumerate(int[] productIds)
	    {
            return HidDevices.Enumerate(Lx4VendorId, productIds).Select(x => new lr4(x));
	    }

        // Starts an initialization state machine that reads the current state all of the rangefinder's
        // configurable settings in order to initialize the app's UI the reflect the state these settings.
        public void ReadAllConfigStart()
        {
            InitState = 1;
            ReadAllConfigNext();

        }

        // Used by the initialization state machine to read the next setting from the rangefinder
        public void ReadAllConfigNext()
        {
            if (InitState == 0)
            {
                // Do nothing
            }
            else if ((InitState > 0) && (InitState < cfgItemTable.Length))
            {
                GetConfigItem(cfgItemTable[InitState].itemName);
                ++InitState;
            }
            else
            {
                InitState = 0;
                GetProductInfoSnippet(0);
            }
        }

        public void StartListen() { IsListening = true; }
	    public void StopListen() { IsListening = false; }

        public void SetConfigStart()
        {
            var report = lx4Device.CreateReport();
            report.ReportId = 0x00;
            report.Data[0] = 0x01;
            report.Data[1] = 0x01;  // ConfigValue (lsb)
            report.Data[2] = 0x80;  // ConfigValue (msb)
            report.Data[3] = 0x01;  // ConfigInterval (lsb)
            report.Data[4] = 0x00;  // ConfigInterval (msb)
            report.Data[5] = 0x01;  // Bit 0: Enable advanced mode
            lx4Device.WriteReport(report);
        }

        public void SetConfigStop()
        {
            var report = lx4Device.CreateReport();
            report.ReportId = 0x00;
            report.Data[0] = 0x01;
            report.Data[1] = 0x01;  // ConfigValue (lsb)
            report.Data[2] = 0x00;  // ConfigValue (msb)
            report.Data[3] = 0x01;  // ConfigInterval (lsb)
            report.Data[4] = 0x00;  // ConfigInterval (msb)
            report.Data[5] = 0x01;  // Bit 0: Enable advanced mode
            lx4Device.WriteReport(report);
        }

        public void SaveConfig()
        {
            var report = lx4Device.CreateReport();
            report.ReportId = 0x00;
            report.Data[0] = 0x02;
            lx4Device.WriteReport(report);
        }

        public void GetProductInfoSnippet(int offset)
        {
            var report = lx4Device.CreateReport();
            report.ReportId = 0x00;
            report.Data[0] = 0x03;
            report.Data[1] = (byte)offset;
            lx4Device.WriteReport(report);
        }

        public void SetConfigItem(String item, uint value)
        {
            if (InitState != 0)
                return;     // Init state machine is running so don't send commands to the rangefinder
            var report = lx4Device.CreateReport();
            String cmd = item + '=' + value.ToString();
            byte[] cmdBytes = Encoding.ASCII.GetBytes(cmd);
            byte[] reportBytes = new byte[cmdBytes.Length + 1];
            cmdBytes.CopyTo(reportBytes, 1);
            reportBytes[0] = 0x05;
            if (reportBytes.Length <= 8)
            {
                report.ReportId = 0x00;
                report.Data = reportBytes;
                lx4Device.WriteReport(report);
            }
        }

        public void GetConfigItem(String item)
        {
            var report = lx4Device.CreateReport();
            byte[] cmdBytes = Encoding.ASCII.GetBytes(item);
            byte[] reportBytes = new byte[cmdBytes.Length + 1];
            cmdBytes.CopyTo(reportBytes, 1);
            reportBytes[0] = 0x04;
            if (reportBytes.Length <= 8)
            {
                report.ReportId = 0x00;
                report.Data = reportBytes;
                lx4Device.WriteReport(report);
            }
        }

        public void InvokeBootloader()
        {
            var report = lx4Device.CreateReport();
            report.ReportId = 0x00;
            report.Data[0] = 0x81;
            lx4Device.WriteReport(report);
        }

        private void BeginReadReport()
	    {
		    if (Interlocked.CompareExchange(ref _isReading, 1, 0) == 1)
                return;
		    lx4Device.ReadReport(ReadReport);
	    }

        private void ReadReport(HidReport report)
	    {
            if (IsListening && report.Data.Length > 0 && DataRecieved != null)
                DataRecieved(report.Data);

            if (report.ReadStatus != HidDeviceData.ReadStatus.NotConnected)
                lx4Device.ReadReport(ReadReport);
            else _isReading = 0;
	    }

	    private void Lx4Inserted()
	    {
		    BeginReadReport();
		    if (Inserted != null) Inserted();
	    }

	    private void Lx4Removed()
	    {
		    if (Removed != null) Removed();
	    }

        public void Dispose()
        {
            lx4Device.CloseDevice();
            GC.SuppressFinalize(this);
        }

        ~lr4()
        {
            Dispose();
        }
    }
}
