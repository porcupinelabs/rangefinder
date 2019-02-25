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
    /// Interaction logic for BarMeter.xaml
    /// </summary>
    public partial class BarMeter : UserControl
    {
        private double Percent;

        public BarMeter()
        {
            InitializeComponent();
            Percent = 50;
        }

        public void SetBarPercent(double p)
        {
            if (p < 0)
                Percent = 0;
            else if (p > 100)
                Percent = 100;
            else Percent = p;
            Bar.Width = 5 * Percent;
        }
    }
}
