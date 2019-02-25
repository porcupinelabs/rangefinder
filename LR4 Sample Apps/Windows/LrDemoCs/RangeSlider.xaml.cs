using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LrDemo
{
    /// <summary>
    /// Interaction logic for RangeSlider.xaml
    /// </summary>
    public partial class RangeSlider : UserControl
    {
        public event EventHandler Changed;
        public double LoValue { get; set; }
        public double HiValue { get; set; }
        public double RangeMin { get; set; }
        public double RangeMax { get; set; }
        public double LoHiSep { get; set; }

        private Point DragStart;
        private bool Dragging, Vertical;
        private double DragStartOffset;
        private double LoPosition, HiPosition, SepPosition, BackMin, BackMax;

        public RangeSlider()
        {
            RangeMin = 0;
            RangeMax = 15;
            LoHiSep = 4;
            LoValue = RangeMin;
            HiValue = RangeMax;
            Dragging = false;
            Vertical = false;
            InitializeComponent();
        }

        public void DrawControl ()
        {
            GetCurPositions();
            LoPosition = BackMin + ((BackMax - BackMin) * ((LoValue - RangeMin) / (RangeMax - RangeMin)));
            HiPosition = BackMin + ((BackMax - BackMin) * ((HiValue - RangeMin) / (RangeMax - RangeMin)));

            double RelPos = LoPosition - BackMin;
            DotLo.Margin = new Thickness(RelPos, 0, 0, 0);
            Center.Margin = new Thickness(RelPos + 9, 0, 0, 0);
            Center.Width = HiPosition - LoPosition;
            DotHi.Margin = new Thickness(0, 0, BackMax - HiPosition, 0);
        }


        private void FireChanged()
        {
            if (Changed != null)
                Changed(this, new System.EventArgs());
        }

        private void DotLo_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            GetCurPositions();
            DragStart = Mouse.GetPosition(Application.Current.MainWindow);
            DragStartOffset = (Vertical ? DragStart.Y : DragStart.X) - LoPosition - 9;
            Dragging = true;
            Mouse.Capture(DotLo);
        }

        private void DotLo_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Dragging = false;
            Mouse.Capture(null);
        }

        private void DotLo_MouseMove(object sender, MouseEventArgs e)
        {
            if (Dragging)
            {
                double PrevPosition = LoPosition;
                Point DragCur = Mouse.GetPosition(Application.Current.MainWindow);
                LoPosition = (Vertical ? DragCur.Y : DragCur.X) - DragStartOffset;
                LoPosition = MinMax(LoPosition, BackMin, HiPosition);

                LoValue = Math.Round(RangeMin + ((RangeMax - RangeMin) * ((LoPosition - BackMin) / (BackMax - BackMin))));
                if (LoValue > HiValue - 3)
                    LoValue = HiValue - 3;
                LoPosition = BackMin + ((BackMax - BackMin) * ((LoValue - RangeMin) / (RangeMax - RangeMin)));

                if (PrevPosition != LoPosition)
                {
                    //Console.WriteLine("Draw");
                    // Draw the dot and center bar
                    double RelPos = LoPosition - BackMin;
                    DotLo.Margin = new Thickness(RelPos, 0, 0, 0);
                    Center.Margin = new Thickness(RelPos + 9, 0, 0, 0);
                    Center.Width = HiPosition - LoPosition + 9;
                    FireChanged();
                }
            }
        }



        private void DotHi_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            GetCurPositions();
            DragStart = Mouse.GetPosition(Application.Current.MainWindow);
            DragStartOffset = (Vertical ? DragStart.Y : DragStart.X) - HiPosition - 9;
            Dragging = true;
            Mouse.Capture(DotHi);
        }

        private void DotHi_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Dragging = false;
            Mouse.Capture(null);
        }

        private void DotHi_MouseMove(object sender, MouseEventArgs e)
        {
            if (Dragging)
            {
                double PrevPosition = HiPosition;
                Point DragCur = Mouse.GetPosition(Application.Current.MainWindow);
                HiPosition = (Vertical ? DragCur.Y : DragCur.X) - DragStartOffset;
                HiPosition = MinMax(HiPosition, LoPosition, BackMax);

                HiValue = Math.Round(RangeMin + ((RangeMax - RangeMin) * ((HiPosition - BackMin) / (BackMax - BackMin))));
                if (HiValue < LoValue + 3)
                    HiValue = LoValue + 3;
                HiPosition = BackMin + ((BackMax - BackMin) * ((HiValue - RangeMin) / (RangeMax - RangeMin)));

                if (PrevPosition != HiPosition)
                {
                    // Draw the dot and center bar
                    DotHi.Margin = new Thickness(0, 0, BackMax - HiPosition, 0);
                    Center.Width = HiPosition - LoPosition - 9;
                    FireChanged();
                }
            }
        }





        private void Back_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void GetCurPositions()
        {
            Point pl, ph, pb;
            pl = DotLo.TransformToAncestor(Application.Current.MainWindow).Transform(new Point(0, 0));
            ph = DotHi.TransformToAncestor(Application.Current.MainWindow).Transform(new Point(0, 0));
            pb = Back.TransformToAncestor(Application.Current.MainWindow).Transform(new Point(0, 0));

            if (pl.Y == ph.Y)   // Is control horizontal?
            {
                Vertical = false;
                LoPosition = pl.X;
                HiPosition = ph.X;
                BackMin = pb.X + 9;
                BackMax = pb.X + Back.Width - 9;
            }
            else                // Control is vertical
            {
                Vertical = true;
                LoPosition = pl.Y;
                HiPosition = ph.Y;
                BackMin = pb.Y + 9;
                BackMax = pb.Y + Back.Width - 9;
            }
            SepPosition = LoHiSep * ((BackMax - BackMin) / (RangeMax - RangeMin));
        }

        private double MinMax (double v, double min, double max)
        {
            if (v < min)
                return min;
            if (v > max)
                return max;
            return v;
        }

    }
}
