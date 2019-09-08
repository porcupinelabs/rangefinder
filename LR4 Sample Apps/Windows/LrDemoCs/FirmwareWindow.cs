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
using System.Diagnostics;

namespace LrDemo
{
    public partial class FirmwareWindow: Form
    {
        private ReleaseCatalog LrfFirmware;
        private FirmwareUpdate FwUpdater;
        private bool UpdateIsRunning;
        private string BrowsedToFilename;

        public FirmwareWindow(lr4 lx4Device, string CurrentFwVersion)
        {
            InitializeComponent();

            lbCurrentVersion.Text = CurrentFwVersion;

            FwMsgBox.AppendText("Messages...\n");
            UpdateIsRunning = false;
            FwUpdater = new FirmwareUpdate(lx4Device, FwMsgBox);

            cbReleases.Items.Add("Searching...");
            cbReleases.SelectedIndex = 0;
            LrfFirmware = new ReleaseCatalog();
            cbReleases.Items.Clear();
            if (LrfFirmware.Catalog.Length == 0)
            {
                cbReleases.Items.Add("Error connecting");
                cbReleases.SelectedIndex = 0;
            }
            else
            {
                for (int i = 0; i < LrfFirmware.Catalog.Length; i++)
                {
                    string s = LrfFirmware.Catalog[i].Product + " : "
                             + LrfFirmware.Catalog[i].FirmwareVersion + "    ("
                             + LrfFirmware.Catalog[i].ReleaseDate + ")";
                    cbReleases.Items.Add(s);
                }
                cbReleases.SelectedIndex = cbReleases.Items.Count-1;
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog BrowseFileDialog = new OpenFileDialog();
            BrowseFileDialog.Title = "Open firmware file";
            BrowseFileDialog.Filter = "Firmware files|*.fw";
            //BrowseFileDialog.InitialDirectory = @"C:\";
            if (BrowseFileDialog.ShowDialog() == DialogResult.OK)
            {
                BrowsedToFilename = BrowseFileDialog.FileName.ToString();
                if (BrowsedToFilename.Length < 40)
                    lbBrowsedToFile.Text = BrowsedToFilename;
                else lbBrowsedToFile.Text = "..." + BrowsedToFilename.Substring(BrowsedToFilename.Length - 30);
            }
        }

        private void btnUpdateFromFileClicked(object sender, EventArgs e)
        {
            if (!UpdateIsRunning)   // Prevent reentry on multiple clicks
            {
                UpdateIsRunning = true;
                FwMsgBox.AppendText("Updating firmware from local file\n");
                FwUpdater.UpdateFirmware(BrowsedToFilename);
                UpdateIsRunning = false;
            }
        }

        private void btnUpdateFromWebClicked(object sender, EventArgs e)
        {
            var client = new WebClient();
            try
            {
                int CatalogIndex = cbReleases.SelectedIndex;
                FwMsgBox.AppendText("Flashing firmware version " + LrfFirmware.Catalog[CatalogIndex].FirmwareVersion + "\n");

                // Download a firmware release binary to a local file and then flash it
                client.DownloadFile(LrfFirmware.Catalog[CatalogIndex].FileUrl, LrfFirmware.Catalog[CatalogIndex].FileName);
                UpdateIsRunning = true;
                FwUpdater.UpdateFirmware(LrfFirmware.Catalog[CatalogIndex].FileName);
                lbCurrentVersion.Text = LrfFirmware.Catalog[CatalogIndex].FirmwareVersion;
                UpdateIsRunning = false;
            }
            finally
            {
                client.Dispose();
            }
        }

        private void linkLabelRelNotes_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/porcupinelabs/rangefinder/tree/master/Firmware");
        }
    }

    [DataContract]
    public class FwRelease
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

    public class ReleaseCatalog
    {
        private string CatalogURL = "https://raw.githubusercontent.com/porcupinelabs/rangefinder/master/Firmware/catalog.json";
        public FwRelease[] Catalog;

        public ReleaseCatalog()
        {
            // Read firmware catalog from GitHub and use it to populate an array of available FwRelease objects
            var client = new WebClient();
            try
            {
                MemoryStream ms = new MemoryStream(client.DownloadData(CatalogURL));
                DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(FwRelease[]));
                Catalog = (FwRelease[])js.ReadObject(ms);
                ms.Close();
            }
            finally
            {
                client.Dispose();
            }
        }
    }
}
