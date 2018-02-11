
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;

using System.IO;
using Microsoft.Win32;



namespace LxDemo
{
	public partial class MainWindow : Window {

        private static readonly int[] SupportedDevices = new[] { 0xDD03 };
        private static lr4 lx4Device;
        private byte[] productInfoBuf = new byte[100];
        private bool LrfStarted = false;

        private static int measurementCount = 0;

        public MainWindow()
        {
			InitializeComponent();

            lx4Device = lr4.Enumerate(SupportedDevices).FirstOrDefault();
            if (lx4Device != null)
            {
                lx4Device.Inserted += Lx4Inserted;
                lx4Device.DataRecieved += Lx4DataRecieved;
                lx4Device.Removed += Lx4Removed;
                lx4Device.StartListen();

                //lx4Device.StopListen();
                //lx4Device.Dispose();
            }
        }

        private void ButtonStartClick(object sender, RoutedEventArgs e)
        {
            if (!LrfStarted)
            {
                buttonStart.Content = "Stop";
                LrfStarted = true;
                lx4Device.SetConfigStart();
            }
            else
            {
                buttonStart.Content = "Start";
                LrfStarted = false;
                lx4Device.SetConfigStop();
            }
        }

        private void Lx4Inserted()
        {
            // Have to change threads in order to do any interaction with the user interface
            this.Dispatcher.Invoke((Action)(() =>
            {
                textBlockAttachStatus.Text = "Attached";
                buttonStart.Content = "Start";
                buttonStart.IsEnabled = true;
                lx4Device.GetProductInfoSnippet(0);
            }));
            LrfStarted = false;
            Console.WriteLine("LX4 attached");
        }

        private void Lx4Removed()
        {
            // Have to change threads in order to do any interaction with the user interface
            this.Dispatcher.Invoke((Action)(() =>
            {
                buttonStart.IsEnabled = false;
                buttonStart.Content = "Start";
                textBlockAttachStatus.Text = "Not Attached";
                textBlockManufacturer.Text = "";
                textBlockModel.Text = "";
                textBlockHwVersion.Text = "";
                textBlockFwVersion.Text = "";
                textBlockSerialNumber.Text = "";
            }));
            //Console.WriteLine("LX4 detached");
        }

        private void Lx4DataRecieved(byte[] data)
        {
            // Have to change threads in order to do any interaction with the user interface
            this.Dispatcher.Invoke((Action)(() =>
            {
                HandleLx4DataRecieved(data);
            }));
        }

        private void HandleLx4DataRecieved(byte[] data)
        {
            //Console.WriteLine(Encoding.ASCII.GetString(data));

            //Console.Write(data.Length);
            //foreach (var item in data)
            //    Console.Write(String.Format("{0,3:X2}", item));
            //Console.WriteLine();

            switch (data[0])
            {
                case 0x00:  // Distance measurement data
                    HandleDistanceData(data);
                    break;
                case 0x02:  // Product Info data
                    int offset = data[1];
                    for (int i = 2; i < data.Length; i++)
                        if (offset < productInfoBuf.Length)
                            productInfoBuf[offset++] = data[i];
                    if (offset < 80)
                        lx4Device.GetProductInfoSnippet(offset);
                    else HandleNewProductInfo();
                    break;
            }
        }

        private void HandleNewProductInfo()
        {
            string mfg   = Encoding.ASCII.GetString(productInfoBuf, 0, 30);
            string prod  = Encoding.ASCII.GetString(productInfoBuf, 30, 20);
            string hwVer = Encoding.ASCII.GetString(productInfoBuf, 50, 10);
            string fwVer = Encoding.ASCII.GetString(productInfoBuf, 60, 10);
            string ser   = Encoding.ASCII.GetString(productInfoBuf, 70, 10);
            char[] charsToTrim = {'\0'};
            textBlockManufacturer.Text = mfg.TrimEnd(charsToTrim);
            textBlockModel.Text        = "Model: " + prod.TrimEnd(charsToTrim);
            textBlockHwVersion.Text    = "Board: " + hwVer.TrimEnd(charsToTrim);
            textBlockFwVersion.Text    = "Firmware: " + fwVer.TrimEnd(charsToTrim);
            textBlockSerialNumber.Text = "Serial #: " + ser.TrimEnd(charsToTrim);
        }

        private void HandleDistanceData(byte[] data)
        {
            ++measurementCount;
            textBoxCount.Text = System.Convert.ToString(measurementCount);

            int millimeters = (data[2] << 8) + data[1];
            double meters = (double)millimeters * 0.001;
            textBoxDistance.Text = String.Format("{0,8:F3}", meters);
            textBoxStatus.Text = String.Format("{0:X4} {1:X2}", (data[4]<<8)+data[5], data[6]);
        }
    
    }
}
