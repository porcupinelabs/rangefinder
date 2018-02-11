using System;
using System.Collections.Generic;
using System.Threading;
using HidLibrary;
using System.Linq;

namespace LxDemo
{
    public class lr4 : IDisposable
    {
	    private const int Lx4VendorId = 0x0417;
        public int Instance;

	    public event InsertedEventHandler Inserted;
	    public event RemovedEventHandler Removed;
	    public event DataRecievedEventHandler DataRecieved;
        
        public delegate void InsertedEventHandler();
	    public delegate void RemovedEventHandler();
	    public delegate void DataRecievedEventHandler(byte[] data);

	    private readonly HidDevice lx4Device;
        private int _isReading;

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
            lx4Device.WriteReport(report);
        }

        public void EnableCalibrationData()
        {
            var report = lx4Device.CreateReport();
            report.ReportId = 0x00;
            report.Data[0] = 0x10;
            report.Data[1] = 0x01;
            lx4Device.WriteReport(report);
        }

        public void DisableCalibrationData()
        {
            var report = lx4Device.CreateReport();
            report.ReportId = 0x00;
            report.Data[0] = 0x10;
            report.Data[1] = 0x00;
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
