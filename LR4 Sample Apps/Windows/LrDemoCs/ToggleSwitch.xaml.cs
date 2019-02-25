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
    /// Interaction logic for ToggleSwitch.xaml
    /// </summary>
    public partial class ToggleSwitch : UserControl
    {
        Thickness LeftSide = new Thickness(-75, 0, 0, 0);
        Thickness RightSide = new Thickness(0, 0, -75, 0);
        SolidColorBrush Off = new SolidColorBrush(Color.FromRgb(160, 160, 160));
        SolidColorBrush On = new SolidColorBrush(Color.FromRgb(255, 127, 0));
        private bool Toggled = false;

        public ToggleSwitch()
        {
            InitializeComponent();
            Toggled = false;
            Back.Fill = Off;
            Dot.Margin = LeftSide;
        }

        public bool State
        {
            get
            {
                return this.Toggled;
            }
            set
            {
                StateChange(value);
            }
        }

        private void Dot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            StateChange(!Toggled);
        }

        private void Back_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            StateChange(!Toggled);
        }

        private void StateChange(bool NewState)
        {
            Toggled = NewState;
            if (Toggled)
            {
                Back.Fill = On;
                Dot.Margin = RightSide;
            }
            else
            {
                Back.Fill = Off;
                Dot.Margin = LeftSide;
            }
        }
    }
}
