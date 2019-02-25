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
    public partial class LineChart : UserControl
    {
        int numPoints = 100;        // Number of data points to plot across chart's width
        int numHorTickLines = 4;    // Number of horizontal tick lines exclusive of top and bottom of plot area
        int numVertTickLines = 9;   // Number of vertical tick lines exclusive of right and left of plot area
        int numYAxisLables = 4 + 2; // Extra one for bottom and top of axis
        int numXAxisLables = 9 + 2; // Extra one for left and right of axis
        private double[] plotData;
        private Line[] plotLine;
        private Line[] horTickLine;
        private TextBlock[] yAxisLable;
        private TextBlock[] xAxisLable;
        private double hScale, vScale, px1, py1, px2, py2;
        private double width, height, yMax, hTickSpacing, vTickSpacing, multiplier;

        public LineChart()
        {
            double x, y;
            InitializeComponent();
            plotLine = new Line[numPoints];
            plotData = new double[numPoints];
            horTickLine = new Line[numHorTickLines];
            yAxisLable = new TextBlock[numYAxisLables];
            xAxisLable = new TextBlock[numXAxisLables];

            // Add some dummy data to start with
            for (int i = 0; i < numPoints; i++)
                plotData[i] = 0;

            multiplier = 2.0;
            width = ChartCanvas.Width - 50;      // Width of plot area
            height = ChartCanvas.Height - 40;    // Height of plot area
            px1 = 40;                            // Upper left coord of plot area
            py1 = 10;
            px2 = px1 + width;                   // Lower right coord of plot area
            py2 = py1 + height;
            hScale = width / numPoints;          // Pixels of width per x unit
            hTickSpacing = hScale * (numVertTickLines + 1);
            CalculateYScale();

            SolidColorBrush brMargin = new SolidColorBrush(Color.FromRgb(31,33,35));
            SolidColorBrush brPlotBack = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            SolidColorBrush brAxis = new SolidColorBrush(Color.FromRgb(100, 100, 100));
            SolidColorBrush brTick = new SolidColorBrush(Color.FromRgb(30, 30, 30));
            SolidColorBrush brPlot = new SolidColorBrush(Color.FromRgb(255, 127, 0));
            SolidColorBrush brLabel = new SolidColorBrush(Color.FromRgb(150, 150, 150));
            ChartCanvas.Background = brMargin;

            // Draw box around plot area
            Polygon plot = new Polygon();
            plot.Points.Add(new Point(px1, py1));
            plot.Points.Add(new Point(px2, py1));
            plot.Points.Add(new Point(px2, py2));
            plot.Points.Add(new Point(px1, py2));
            plot.Stroke = brAxis;
            plot.Fill = brPlotBack;
            plot.StrokeThickness = 1;
            ChartCanvas.Children.Add(plot);

            // Draw vertical tick lines in plot area
            x = px2 - hTickSpacing;
            while (x > px1)
            {
                Line vTick = new Line();
                vTick.X1 = x;
                vTick.Y1 = py1;
                vTick.X2 = x;
                vTick.Y2 = py2;
                vTick.Stroke = brTick;
                vTick.StrokeThickness = 1;
                ChartCanvas.Children.Add(vTick);
                x -= hTickSpacing;
            }

            // Draw X axis labels
            x = px2;
            for (int i = 0; i < numXAxisLables; i++)
            {
                double val = i * (hTickSpacing / hScale);
                xAxisLable[i] = new TextBlock();
                xAxisLable[i].Text = String.Format("{0:F0}", val);
                xAxisLable[i].Foreground = brLabel;
                xAxisLable[i].FontSize = 8;
                Canvas.SetLeft(xAxisLable[i], x - 4);
                Canvas.SetTop(xAxisLable[i], py2 + 4);
                ChartCanvas.Children.Add(xAxisLable[i]);
                x -= hTickSpacing;
            }

            // Draw horizontal tick lines in plot area
            y = py1 + vTickSpacing;
            for (int i=0; i<numHorTickLines; i++)
            {
                horTickLine[i] = new Line();
                horTickLine[i].X1 = px1;
                horTickLine[i].Y1 = y;
                horTickLine[i].X2 = px2;
                horTickLine[i].Y2 = y;
                horTickLine[i].Stroke = brTick;
                horTickLine[i].StrokeThickness = (y < py2) ? 1 : 0;
                ChartCanvas.Children.Add(horTickLine[i]);
                y += vTickSpacing;
            }

            // Draw Y axis labels
            y = py2;
            for (int i = 0; i < numYAxisLables; i++)
            {
                double val = i * (vTickSpacing / vScale);
                yAxisLable[i] = new TextBlock();
                yAxisLable[i].Text = String.Format("{0:F1}", val);
                yAxisLable[i].Foreground = brLabel;
                yAxisLable[i].FontSize = 8;
                Canvas.SetRight(yAxisLable[i], width - px1 + 52);
                Canvas.SetTop(yAxisLable[i], y-6);
                ChartCanvas.Children.Add(yAxisLable[i]);
                y -= vTickSpacing;
            }

            // Draw initial plot line
            x = px2;
            for (int i = 0; i < numPoints; i++)
            {
                plotLine[i] = new Line();
                plotLine[i].X1 = x;
                plotLine[i].Y1 = py2;
                plotLine[i].X2 = x - hScale;
                plotLine[i].Y2 = py2;
                plotLine[i].Stroke = brPlot;
                plotLine[i].StrokeThickness = 1;
                ChartCanvas.Children.Add(plotLine[i]);
                x -= hScale;
            }
        }


        public void AddDataPoint (double val)
        {
            int i;
            double y;

            for (i = numPoints - 1; i > 0; i--)
                plotData[i] = plotData[i - 1];
            plotData[0] = val;

            if (CalculateYScale())
                AdjustHorizontalTickLines();

            plotLine[0].Y1 = py2 - plotData[0] * vScale * multiplier;
            for (i=1; i<numPoints; i++)
            {
                y = py2 - plotData[i] * vScale * multiplier;
                plotLine[i-1].Y2 = y;
                plotLine[i].Y1 = y;
            }
            plotLine[numPoints-1].Y2 = py2 - plotData[numPoints-1] * vScale * multiplier;
        }

        public void SetMultiplier (double mult)
        {
            multiplier = mult;
        }


        private bool CalculateYScale()  // Returns true if yMax changed
        {
            double oldYMax = yMax;
            double maxData = plotData.Max() * multiplier;
            if (maxData == 0)
                maxData = 2;

            double pow = Math.Floor(Math.Log10(maxData));
            double bas = maxData / (Math.Pow(10, pow));

            double step = 1;
            if (bas <= 2)
            {
                bas = 2;
                step = 0.5;
            }
            else if (bas < 5)
            {
                bas = 5;
                step = 1;
            }
            else
            {
                bas = 10;
                step = 2;
            }

            yMax = bas * Math.Pow(10, pow);      // Value for top of y-axis
            vScale = height / yMax;              // Pixels of height per y unit
            vTickSpacing = vScale * (step * Math.Pow(10, pow));
            return (yMax != oldYMax);
        }



        private void AdjustHorizontalTickLines()
        {
            double y;

            // Adjust tick lines
            y = py1 + vTickSpacing;
            for (int i = 0; i < numHorTickLines; i++)
            {
                horTickLine[i].X1 = px1;
                horTickLine[i].Y1 = y;
                horTickLine[i].X2 = px2;
                horTickLine[i].Y2 = y;
                horTickLine[i].StrokeThickness = (y < py2) ? 1 : 0;
                y += vTickSpacing;
            }

            // Adjust Y axis labels
            y = py2;
            string fmt;
            if (yMax < 0.05)
                fmt = "{0:F3}";
            else if (yMax < 0.5)
                fmt = "{0:F2}";
            else if (yMax < 5)
                fmt = "{0:F1}";
            else fmt = "{0:F0}";
            for (int i = 0; i < numYAxisLables; i++)
            {
                if (y >= py1)
                {
                    double val = i * (vTickSpacing / vScale);
                    yAxisLable[i].Text = String.Format(fmt, val);
                    Canvas.SetRight(yAxisLable[i], width - px1 + 52);
                    Canvas.SetTop(yAxisLable[i], y - 6);
                    y -= vTickSpacing;
                }
                else yAxisLable[i].Text = "";
            }


        }


    }
}
