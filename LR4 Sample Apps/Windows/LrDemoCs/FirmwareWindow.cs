using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LrDemo
{
    public partial class FirmwareWindow: Form
    {

        private FirmwareUpdate FwUpdater;
        private bool UpdateIsRunning;

        public FirmwareWindow(lr4 lx4Device)
        {
            InitializeComponent();
            FwMsgBox.AppendText("Messages...\n");
            UpdateIsRunning = false;
            FwUpdater = new FirmwareUpdate(lx4Device, FwMsgBox);
        }

        private void btnUpdateFromFileClicked(object sender, EventArgs e)
        {
            if (!UpdateIsRunning)   // Prevent reentry on multiple clicks
            {
                OpenFileDialog BrowseFileDialog = new OpenFileDialog();
                BrowseFileDialog.Title = "Open firmware file";
                BrowseFileDialog.Filter = "Firmware files|*.fw";
                BrowseFileDialog.InitialDirectory = @"C:\";
                if (BrowseFileDialog.ShowDialog() == DialogResult.OK)
                {
                    UpdateIsRunning = true;
                    FwUpdater.UpdateFirmware(BrowseFileDialog.FileName.ToString());
                    UpdateIsRunning = false;
                }


            }
        }
    }
}
