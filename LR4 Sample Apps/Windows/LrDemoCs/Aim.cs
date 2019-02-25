using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LrDemo
{
    class Aimer
    {
        private double x1, y1, x2, y2, step;
        private Rectangle rect = new Rectangle();

        public Aimer (Canvas c)
        {
            SolidColorBrush brGrid = new SolidColorBrush(Color.FromRgb(100, 100, 100));
            SolidColorBrush brBeam = new SolidColorBrush(Color.FromArgb(64, 255, 0, 0));

            x1 = 10;  y1 = 10;
            x2 = 170; y2 = 170;
            step = (x2 - x1) / 16;
            int i;

            for (i=0; i<=16; i++)
            {
                Line l = new Line();
                l.X1 = x1;
                l.Y1 = y1 + (i * step);
                l.X2 = x2;
                l.Y2 = l.Y1;
                l.Stroke = brGrid;
                l.StrokeThickness = 1;
                c.Children.Add(l);
            }
            for (i = 0; i <= 16; i++)
            {
                Line l = new Line();
                l.X1 = x1 + (i * step);
                l.Y1 = y1;
                l.X2 = l.X1;
                l.Y2 = y2;
                l.Stroke = brGrid;
                l.StrokeThickness = 1;
                c.Children.Add(l);
            }

            rect.Fill = brBeam;
            rect.Width = x2 - x1;
            rect.Height = y2 - y1;
            Canvas.SetLeft(rect, x1);
            Canvas.SetTop(rect, y1);
            c.Children.Add(rect);
        }

        public void SetAiming(double ax1, double ay1, double ax2, double ay2)
        {
            // These value come in at 0..15
            ax1 = (ax1 * step) + x1;
            ay1 = (ay1 * step) + x1;
            ax2 = ((ax2+1) * step) + x1;
            ay2 = ((ay2+1) * step) + x1;

            rect.Width = ax2 - ax1;
            rect.Height = ay2 - ay1;
            Canvas.SetLeft(rect, ax1);
            Canvas.SetTop(rect, ay1);
        }
    }
}
