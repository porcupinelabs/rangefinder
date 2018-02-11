
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Media;

using System.IO;
using Microsoft.Win32;


namespace LxDemo
{
	public partial class MainWindow : Window {

        private static readonly int[] SupportedDevices = new[] { 0xDD03 };
//        private static lr4 lx4Device;
        private byte[] productInfoBuf1 = new byte[100];
        private byte[] productInfoBuf2 = new byte[100];
        private bool LrfStarted = false;

        private static int measurementCount1 = 0;
        private static int measurementCount2 = 0;
        private static int CANVAS_HEIGHT = 510;

        private static List<lr4> lx4List = new List<lr4>();

        private static Tank tank1, tank2;

        public MainWindow()
        {
            InitializeComponent();
            tank1 = new Tank("Tank1", tankCanvas, 10, 10);
            tank2 = new Tank("Tank2", tankCanvas, 270, 10);

            var lx4ListAll = lr4.Enumerate(SupportedDevices).ToList();
            foreach (lr4 x in lx4ListAll)
                if (x.DevicePath.Contains("&mi_00"))    // Only care about interface 0 on each LR4
                    lx4List.Add(x);

            if (lx4List.Count >= 1)
            {
                lx4List[0].Instance = 0;
                lx4List[0].Inserted += Lx4Inserted1;
                lx4List[0].DataRecieved += Lx4DataRecieved1;
                lx4List[0].Removed += Lx4Removed1;
                lx4List[0].StartListen();
            }
            if (lx4List.Count >= 2)
            {
                lx4List[1].Instance = 1;
                lx4List[1].Inserted += Lx4Inserted2;
                lx4List[1].DataRecieved += Lx4DataRecieved2;
                lx4List[1].Removed += Lx4Removed2;
                lx4List[1].StartListen();
            }

            textBoxTankHeight.Text = tank1.GetHeight().ToString("F1");
            textBoxTankCapacity.Text = tank1.GetCapacity().ToString("F1");

            textBoxTankHeight2.Text = tank2.GetHeight().ToString("F1");
            textBoxTankCapacity2.Text = tank2.GetCapacity().ToString("F1");

            SolidColorBrush darkBlueBrush = new SolidColorBrush();
            darkBlueBrush.Color = Color.FromRgb(0x2B, 0x3C, 0x59);

            int canvasWidth = 480;
            int canvasHeight = CANVAS_HEIGHT;
            int canvasCenterX = canvasWidth / 2;

            int x1 = 10;
            int x2 = canvasWidth - 10;
            int y1 = 533;
            int y2 = 17;

            for (int i = 0; i <= 10; i++)
            {
                int y = y2 - (i * (y2 - y1)) / 10;
                DrawLine(x1, y, canvasCenterX - 15, y, darkBlueBrush);
                DrawLine(x2, y, canvasCenterX + 15, y, darkBlueBrush);

                TextBlock textBlock = new TextBlock();
                textBlock.Text = Convert.ToString(100 - i * 10) + "%";
                Canvas.SetLeft(textBlock, 0);
                Canvas.SetTop(textBlock, y - 7);
                textBlock.Width = canvasWidth;

                textBlock.TextAlignment = TextAlignment.Center;
                textBlock.Foreground = darkBlueBrush;
                tankCanvas.Children.Add(textBlock);

            }
        }


        private void DrawLine(int x1, int y1, int x2, int y2, SolidColorBrush b)
        {
            Line myLine = new Line();
            myLine.Stroke = b;
            myLine.X1 = x1;
            myLine.Y1 = y1;
            myLine.X2 = x2;
            myLine.Y2 = y1;
            myLine.StrokeThickness = 1;
            tankCanvas.Children.Add(myLine);
        }


        private void ButtonStartClick(object sender, RoutedEventArgs e)
        {
            if (!LrfStarted)
            {
                buttonStart.Content = "Stop";
                LrfStarted = true;
                //lx4Device.SetConfigStart();
                foreach (lr4 x in lx4List)
                    x.SetConfigStart();
            }
            else
            {
                buttonStart.Content = "Start";
                LrfStarted = false;
                //lx4Device.SetConfigStop();
                foreach (lr4 x in lx4List)
                    x.SetConfigStop();
            }
        }

        private void Lx4Inserted1()
        {
            // Have to change threads in order to do any interaction with the user interface
            this.Dispatcher.Invoke((Action)(() =>
            {
                textBlockAttachStatus.Text = "Attached";
                buttonStart.Content = "Start";
                buttonStart.IsEnabled = true;
                lx4List[0].GetProductInfoSnippet(0);
            }));
            LrfStarted = false;
            Console.WriteLine("LX4 #1 attached");
        }
        private void Lx4Inserted2()
        {
            // Have to change threads in order to do any interaction with the user interface
            this.Dispatcher.Invoke((Action)(() =>
            {
                textBlockAttachStatus2.Text = "Attached";
                buttonStart.Content = "Start";
                buttonStart.IsEnabled = true;
                lx4List[1].GetProductInfoSnippet(0);
            }));
            LrfStarted = false;
            Console.WriteLine("LX4 #2 attached");
        }

        private void Lx4Removed1()
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
        }
        private void Lx4Removed2()
        {
            // Have to change threads in order to do any interaction with the user interface
            this.Dispatcher.Invoke((Action)(() =>
            {
                //buttonStart.IsEnabled = false;
                //buttonStart.Content = "Start";
                textBlockAttachStatus2.Text = "Not Attached";
                textBlockManufacturer2.Text = "";
                textBlockModel2.Text = "";
                textBlockHwVersion2.Text = "";
                textBlockFwVersion2.Text = "";
                textBlockSerialNumber2.Text = "";
            }));
        }

        private void Lx4DataRecieved1(byte[] data)
        {
            // Have to change threads in order to do any interaction with the user interface
            this.Dispatcher.Invoke((Action)(() =>
            {
                HandleLx4DataRecieved(data, 0);
            }));
        }
        private void Lx4DataRecieved2(byte[] data)
        {
            // Have to change threads in order to do any interaction with the user interface
            this.Dispatcher.Invoke((Action)(() =>
            {
                HandleLx4DataRecieved(data, 1);
            }));
        }

        private void HandleLx4DataRecieved(byte[] data, int Instance)
        {
            //Console.WriteLine(Encoding.ASCII.GetString(data));

            //Console.Write(data.Length);
            //foreach (var item in data)
            //    Console.Write(String.Format("{0,3:X2}", item));
            //Console.WriteLine();

            switch (data[0])
            {
                case 0x00:  // Distance measurement data
                    HandleDistanceData(data, Instance);
                    break;
                case 0x02:  // Product Info data
                    int offset = data[1];
                    for (int i = 2; i < data.Length; i++)
                        if (offset < productInfoBuf1.Length)
                        {
                            if (Instance == 0)
                                productInfoBuf1[offset++] = data[i];
                            else productInfoBuf2[offset++] = data[i];
                        }
                    if (offset < 80)
                        lx4List[Instance].GetProductInfoSnippet(offset);
                    else HandleNewProductInfo(Instance);
                    break;
            }
        }

        private void HandleNewProductInfo(int Instance)
        {
            if (Instance == 0)
            {
                string mfg = Encoding.ASCII.GetString(productInfoBuf1, 0, 30);
                string prod = Encoding.ASCII.GetString(productInfoBuf1, 30, 20);
                string hwVer = Encoding.ASCII.GetString(productInfoBuf1, 50, 10);
                string fwVer = Encoding.ASCII.GetString(productInfoBuf1, 60, 10);
                string ser = Encoding.ASCII.GetString(productInfoBuf1, 70, 10);
                char[] charsToTrim = { '\0' };
                textBlockManufacturer.Text = mfg.TrimEnd(charsToTrim);
                textBlockModel.Text = "Model: " + prod.TrimEnd(charsToTrim);
                textBlockHwVersion.Text = "Board: " + hwVer.TrimEnd(charsToTrim);
                textBlockFwVersion.Text = "Firmware: " + fwVer.TrimEnd(charsToTrim);
                textBlockSerialNumber.Text = "Serial #: " + ser.TrimEnd(charsToTrim);
            }
            else
            {
                string mfg = Encoding.ASCII.GetString(productInfoBuf2, 0, 30);
                string prod = Encoding.ASCII.GetString(productInfoBuf2, 30, 20);
                string hwVer = Encoding.ASCII.GetString(productInfoBuf2, 50, 10);
                string fwVer = Encoding.ASCII.GetString(productInfoBuf2, 60, 10);
                string ser = Encoding.ASCII.GetString(productInfoBuf2, 70, 10);
                char[] charsToTrim = { '\0' };
                textBlockManufacturer2.Text = mfg.TrimEnd(charsToTrim);
                textBlockModel2.Text = "Model: " + prod.TrimEnd(charsToTrim);
                textBlockHwVersion2.Text = "Board: " + hwVer.TrimEnd(charsToTrim);
                textBlockFwVersion2.Text = "Firmware: " + fwVer.TrimEnd(charsToTrim);
                textBlockSerialNumber2.Text = "Serial #: " + ser.TrimEnd(charsToTrim);
            }
        }

        private void HandleDistanceData(byte[] data, int Instance)
        {
            if (Instance == 0)
            {
                ++measurementCount1;
                textBoxCount.Text = System.Convert.ToString(measurementCount1);
                int millimeters = (data[2] << 8) + data[1];
                double meters = (double)millimeters * 0.001;
                textBoxDistance.Text = String.Format("{0,8:F3}", meters);
                textBoxStatus.Text = String.Format("{0:X4} {1:X2}", (data[4] << 8) + data[5], data[6]);

                try
                {
                    tank1.SetTankLevelUsingDistance(meters);
                    double vol = tank1.GetFilledWeight();
                    MeasTxt.Text = String.Format("{0,8:F1}", vol);
                    
                    //double TankHeight = Convert.ToDouble(textBoxTankHeight.Text);
                    //double TankCapacity = Convert.ToDouble(textBoxTankCapacity.Text);
                    //double TankOffset = Convert.ToDouble(textBoxOffset.Text);
                    //double meas = ((TankHeight - (meters + TankOffset)) / TankHeight) * TankCapacity;
                    //MeasTxt.Text = String.Format("{0,8:F1}", meas);
                    //double h = ((TankHeight - (meters + TankOffset)) / TankHeight) * CANVAS_HEIGHT - 20;
                    //rectangle1.Height = h;
                    //Canvas.SetTop(rectangle1, CANVAS_HEIGHT - 10 - h);
                }
                catch
                {
                    MeasTxt.Text = "Error";
                }
            }
            else
            {
                ++measurementCount2;
                textBoxCount2.Text = System.Convert.ToString(measurementCount2);
                int millimeters = (data[2] << 8) + data[1];
                double meters = (double)millimeters * 0.001;
                textBoxDistance2.Text = String.Format("{0,8:F3}", meters);
                textBoxStatus2.Text = String.Format("{0:X4} {1:X2}", (data[4] << 8) + data[5], data[6]);

                try
                {
                    tank2.SetTankLevelUsingDistance(meters);
                    double vol = tank2.GetFilledWeight();
                    MeasTxt2.Text = String.Format("{0,8:F1}", vol);

                    //double TankHeight = Convert.ToDouble(textBoxTankHeight2.Text);
                    //double TankCapacity = Convert.ToDouble(textBoxTankCapacity2.Text);
                    //double TankOffset = Convert.ToDouble(textBoxOffset2.Text);
                    //double meas = ((TankHeight - (meters + TankOffset)) / TankHeight) * TankCapacity;
                    //MeasTxt2.Text = String.Format("{0,8:F1}", meas);
                    //double h = ((TankHeight - (meters + TankOffset)) / TankHeight) * CANVAS_HEIGHT - 20;
                    //rectangle2.Height = h;
                    //Canvas.SetTop(rectangle2, CANVAS_HEIGHT - 10 - h);
                }
                catch
                {
                    MeasTxt2.Text = "Error";
                }
            }

        }

        private void checkBoxFillingClick(object sender, RoutedEventArgs e)
        {
            tank1.SetFillingMode(checkBoxFilling.IsChecked.HasValue && checkBoxFilling.IsChecked.Value);
        }

        private void checkBoxFillingClick2(object sender, RoutedEventArgs e)
        {
            tank2.SetFillingMode(checkBoxFilling2.IsChecked.HasValue && checkBoxFilling2.IsChecked.Value);
        }

        /*
        private void sliderChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double f = slider1.Value;
            tank1.SetTankLevelUsingPercent(f);
            double vol = tank1.GetFilledWeight();
            MeasTxt.Text = String.Format("{0,8:F1}", vol);

            tank2.SetTankLevelUsingPercent(f);
            vol = tank2.GetFilledWeight();
            MeasTxt2.Text = String.Format("{0,8:F1}", vol);
        }
        */
    }

}
