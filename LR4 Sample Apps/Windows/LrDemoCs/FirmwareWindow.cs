using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace LrDemo
{
    [DataContract]
    class FwRelease
    {
        [DataMember]
        public string Manufacturer { get; set; }
        [DataMember]
        public string Product { get; set; }
        [DataMember]
        public string HardwareVersion { get; set; }
        [DataMember]
        public string FirmwareVersion { get; set; }
        [DataMember]
        public string ReleaseDate { get; set; }
        [DataMember]
        public string FileUrl { get; set; }
        [DataMember]
        public string FileName { get; set; }
    }

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

        private void btnUpdateFromWebClicked(object sender, EventArgs e)
        {
            DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(FwRelease[]));
            FileStream fs = new FileStream("catalog.json", FileMode.Open);
            FwRelease[] Catalog = (FwRelease[])js.ReadObject(fs);
            fs.Close();

            var client = new WebClient();
            client.DownloadFile(Catalog[0].FileUrl, Catalog[0].FileName);
            UpdateIsRunning = true;
            FwUpdater.UpdateFirmware(Catalog[0].FileName);
            UpdateIsRunning = false;
        }
    }
}
