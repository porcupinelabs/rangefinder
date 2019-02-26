
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

namespace LrDemo
{
	public partial class MainWindow : Window {

        private static Aimer Aim;
        private static readonly int[] SupportedDevices = new[] { 0xDD03 };
        private static lr4 lx4Device;
        private byte[] productInfoBuf = new byte[100];
        private bool LrfStarted = false;
        private static int measurementCount = 0;
        private static double[] UnitsMuliplierTable = new double[5] {3.28084, 1.0, 3.28084, 39.3701, 100}; // 0=Feet/Inches, 1=Meters, 2=Feet, 3=Inches, 4=Centimeters

        public MainWindow()
        {
			InitializeComponent();
            Aim = new Aimer(canvasAim);

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
            rsAimX.Changed += AimingChanged;
            rsAimY.Changed += AimingChanged;
        }

        private void ButtonStartClick(object sender, RoutedEventArgs e)
        {
            if (!LrfStarted)
            {
                lx4Device.SetConfigStart();
                if (cbMeasurementMode.SelectedIndex != 1)   // Measurement Mode not Single
                {
                    buttonStart.Content = "Stop";
                    LrfStarted = true;
                }
            }
            else
            {
                lx4Device.SetConfigStop();
                buttonStart.Content = "Start";
                LrfStarted = false;
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
                lx4Device.ReadAllConfigStart();
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
                case 0x03:  // GetConfigItem data
                    string getResult = Encoding.UTF8.GetString(data, 1, 7);
                    string[] parsedResult = getResult.Split('=');
                    int val;
                    if (Int32.TryParse(parsedResult[1], out val))
                        HandleNewCfgItemData(parsedResult[0], val);
                    lx4Device.ReadAllConfigNext();
                    break;
                case 0x04:  // Write config result
                    int result = data[1];
                    if (result == 0)
                        System.Windows.Forms.MessageBox.Show("Configuration settings were saved to the rangefinder's internal flash memory.  These settings will remain even after the rangefinder is unplugged / powered off.");
                    else
                        System.Windows.Forms.MessageBox.Show("There was an error while trying to save settings to the rangefinder's internal flash memory.");
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
            double Feet, Inches, Meters, Centimeters;
            int iFeet;
            ++measurementCount;
            textBoxCount.Text = System.Convert.ToString(measurementCount);

            int Millimeters = (data[2] << 8) + data[1];
            switch (cbUnits.SelectedIndex)
            {
                case 0:         // Feet and inches
                    Inches = Millimeters / 25.4;
                    iFeet = (int)(Inches / 12);
                    Inches -= iFeet * 12;
                    textBoxDistance.Text = String.Format("{0:0}' {1:F1}\"", iFeet, Inches);
                    break;
                case 1:         // Meters
                    Meters = (double)Millimeters * 0.001;
                    textBoxDistance.Text = String.Format("{0:F3} m", Meters);
                    break;
                case 2:         // Feet
                    Feet = ((double)Millimeters / 25.4) / 12;
                    textBoxDistance.Text = String.Format("{0:F2}'", Feet);
                    break;
                case 3:         // Inches
                    Inches = (double)Millimeters / 25.4;
                    textBoxDistance.Text = String.Format("{0:F1}\"", Inches);
                    break;
                case 4:         // Centimeters
                    Centimeters = (double)Millimeters / 10;
                    textBoxDistance.Text = String.Format("{0:F1} cm", Centimeters);
                    break;
               default:        // Error
                    textBoxDistance.Text = "Error";
                    break;
            }

            if (data[3] == 0)  // Not advanced mode (LR4)
            {
                textBoxStatus.Text = String.Format("{0:X4} {1:X2}", (data[4] << 8) + data[5], data[6]);
            }
            else    // Advanced mode (Micro LRF)
            {
                textBoxStatus.Text = String.Format("{0:X2}", data[6]);

                double SignalStrength = data[3] / 2.55;
                BarSignalStrength.SetBarPercent(SignalStrength);
                textBoxSignalStrength.Text = String.Format("{0:F0}%", SignalStrength);

                double AmbientIR = data[4] / 2.55;
                barAmbientIR.SetBarPercent(AmbientIR);
                textBoxAmbientIR.Text = String.Format("{0:F0}%", AmbientIR);

                double Uncertainty = data[5];
                barUncertainty.SetBarPercent(Uncertainty);
                textBoxUncertainty.Text = String.Format("{0:F1}mm", Uncertainty/10);
            }

            rangeChart.AddDataPoint((double)Millimeters/1000);
        }


        private void HandleNewCfgItemData(string cfgItem, int val)
        {
            switch (cfgItem)
            {
                case "run":
                    if (val == 0)
                    {
                        buttonStart.Content = "Start";
                        LrfStarted = false;
                    }
                    else
                    {
                        buttonStart.Content = "Stop";
                        LrfStarted = true;
                    }
                    break;
                case "uni":
                    cbUnits.SelectedIndex = val;
                    break;
                case "mod":
                    cbMeasurementMode.SelectedIndex = val;
                    break;
                case "int":
                    tbInterval.Text = val.ToString();
                    break;
                case "iun":
                    cbIntervalUnits.SelectedIndex = val;
                    break;
                case "trg":
                    cbTrigger.SelectedIndex = val;
                    break;
                case "kbd":
                    tsKeyboardEmulation.State = (val != 0);
                    break;
                case "dbl":
                    tsDoDoubleMeasurements.State = (val != 0);
                    break;
                case "nfl":
                    tsDontFilterErrors.State = (val != 0);
                    break;
                case "chg":
                    tsOnlySendChanges.State = (val != 0);
                    break;
                case "rmd":
                    cbRangeMode.SelectedIndex = val;
                    break;
                case "mrt":
                    slMeasurementRate.Value = val;
                    lblMeasurementRate.Content = "Measurement Rate: " + val.ToString() + " Hz";
                    break;
                case "bx1":
                    rsAimX.LoValue = val;
                    break;
                case "by1":
                    rsAimY.LoValue = val;
                    break;
                case "bx2":
                    rsAimX.HiValue = val;
                    rsAimX.DrawControl();
                    break;
                case "by2":
                    rsAimY.HiValue = val;
                    rsAimY.DrawControl();
                    Aim.SetAiming(rsAimX.LoValue, rsAimY.LoValue, rsAimX.HiValue, rsAimY.HiValue);
                    double Xdeg = (27 * (rsAimX.HiValue - rsAimX.LoValue) / 15);
                    double Ydeg = (27 * (rsAimY.HiValue - rsAimY.LoValue) / 15);
                    textBoxSignalBeamSteering.Text = String.Format("{0:F0}° x {1:F0}°", Xdeg, Ydeg);
                    break;

            }
        }
        

        private void cbUnits_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            uint val = (uint)cbUnits.SelectedIndex;  // 0=Feet/Inches, 1=Meters, 2=Feet, 3=Inches, 4=Centimeters
            if (lx4Device != null)
                lx4Device.SetConfigItem("uni", val);
            if ((val < 5) && (rangeChart != null))
                rangeChart.SetMultiplier(UnitsMuliplierTable[val]);
        }

        private void cbMeasurementMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            uint val = (uint)cbMeasurementMode.SelectedIndex;
            if (lx4Device != null)
                lx4Device.SetConfigItem("mod", val);
        }

        private void cbTrigger_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            uint val = (uint)cbTrigger.SelectedIndex;
            if (lx4Device != null)
                lx4Device.SetConfigItem("trg", val);
        }

        private void tbInterval_TextChanged(object sender, TextChangedEventArgs e)
        {
            uint val;
            if (UInt32.TryParse(tbInterval.Text, out val))
                if ((val > 0) && (val <= 99))
                    if (lx4Device != null)
                        lx4Device.SetConfigItem("int", val);
        }

        private void cbIntervalUnits_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            uint val = (uint)cbIntervalUnits.SelectedIndex;  // 0=Short, 1=Medium, 2=Long
            if (lx4Device != null)
                lx4Device.SetConfigItem("iun", val);
        }

        private void cbRangeMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            uint val = (uint)cbRangeMode.SelectedIndex;  // 0=Short, 1=Medium, 2=Long
            if (lx4Device != null)
                lx4Device.SetConfigItem("rmd", val);
        }

        private void tsKeyboardEmulation_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            uint val = (uint)(tsKeyboardEmulation.State ? 1 : 0);
            if (lx4Device != null)
                lx4Device.SetConfigItem("kbd", val);
        }

        private void tsDoDoubleMeasurements_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            uint val = (uint)(tsDoDoubleMeasurements.State ? 1 : 0);
            if (lx4Device != null)
                lx4Device.SetConfigItem("dbl", val);
        }

        private void tsDontFilterErrors_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            uint val = (uint)(tsDontFilterErrors.State ? 1 : 0);
            if (lx4Device != null)
                lx4Device.SetConfigItem("nfl", val);
        }

        private void tsOnlySendChanges_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            uint val = (uint)(tsOnlySendChanges.State ? 1 : 0);
            if (lx4Device != null)
                lx4Device.SetConfigItem("chg", val);
        }

        private void slMeasurementRate_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            uint val;
            val = (uint)slMeasurementRate.Value;
            if ((val > 0) && (val <= 100))
            {
                lblMeasurementRate.Content = "Measurement Rate: " + val.ToString() + " Hz";
                if (lx4Device != null)
                    lx4Device.SetConfigItem("mrt", val);
            }
        }

        private void AimingChanged(object sender, EventArgs e)
        {
            Aim.SetAiming(rsAimX.LoValue, rsAimY.LoValue, rsAimX.HiValue, rsAimY.HiValue);
            double Xdeg = (27 * (rsAimX.HiValue - rsAimX.LoValue) / 15);
            double Ydeg = (27 * (rsAimY.HiValue - rsAimY.LoValue) / 15);
            textBoxSignalBeamSteering.Text = String.Format("{0:F0}° x {1:F0}°", Xdeg, Ydeg);
            if (lx4Device != null)
            {
                lx4Device.SetConfigItem("bx1", (uint)rsAimX.LoValue);
                lx4Device.SetConfigItem("by1", (uint)rsAimY.LoValue);
                lx4Device.SetConfigItem("bx2", (uint)rsAimX.HiValue);
                lx4Device.SetConfigItem("by2", (uint)rsAimY.HiValue);
            }
        }

        private void btnSaveConfig_Click(object sender, RoutedEventArgs e)
        {
            if (lx4Device != null)
                lx4Device.SaveConfig();
        }
    }
}
