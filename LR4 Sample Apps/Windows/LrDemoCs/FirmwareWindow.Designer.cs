namespace LrDemo
{
    partial class FirmwareWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.lbCurrentVersion = new System.Windows.Forms.Label();
            this.btnUpdateFromFile = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.FwMsgBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnUpdateFromWeb = new System.Windows.Forms.Button();
            this.cbReleases = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.linkLabelRelNotes = new System.Windows.Forms.LinkLabel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lbBrowsedToFile = new System.Windows.Forms.Label();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 338);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(123, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Current firmware version:";
            // 
            // lbCurrentVersion
            // 
            this.lbCurrentVersion.AutoSize = true;
            this.lbCurrentVersion.Location = new System.Drawing.Point(142, 338);
            this.lbCurrentVersion.Name = "lbCurrentVersion";
            this.lbCurrentVersion.Size = new System.Drawing.Size(31, 13);
            this.lbCurrentVersion.TabIndex = 3;
            this.lbCurrentVersion.Text = "1.2.3";
            // 
            // btnUpdateFromFile
            // 
            this.btnUpdateFromFile.Location = new System.Drawing.Point(149, 76);
            this.btnUpdateFromFile.Name = "btnUpdateFromFile";
            this.btnUpdateFromFile.Size = new System.Drawing.Size(87, 23);
            this.btnUpdateFromFile.TabIndex = 5;
            this.btnUpdateFromFile.Text = "Update";
            this.btnUpdateFromFile.UseVisualStyleBackColor = true;
            this.btnUpdateFromFile.Click += new System.EventHandler(this.btnUpdateFromFileClicked);
            // 
            // button3
            // 
            this.button3.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button3.Location = new System.Drawing.Point(482, 333);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 6;
            this.button3.Text = "Close";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // FwMsgBox
            // 
            this.FwMsgBox.BackColor = System.Drawing.Color.LightGray;
            this.FwMsgBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.FwMsgBox.Location = new System.Drawing.Point(281, 13);
            this.FwMsgBox.Multiline = true;
            this.FwMsgBox.Name = "FwMsgBox";
            this.FwMsgBox.Size = new System.Drawing.Size(276, 305);
            this.FwMsgBox.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(134, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Available firmware releases";
            // 
            // btnUpdateFromWeb
            // 
            this.btnUpdateFromWeb.Location = new System.Drawing.Point(20, 131);
            this.btnUpdateFromWeb.Name = "btnUpdateFromWeb";
            this.btnUpdateFromWeb.Size = new System.Drawing.Size(88, 23);
            this.btnUpdateFromWeb.TabIndex = 2;
            this.btnUpdateFromWeb.Text = "Update";
            this.btnUpdateFromWeb.UseVisualStyleBackColor = true;
            this.btnUpdateFromWeb.Click += new System.EventHandler(this.btnUpdateFromWebClicked);
            // 
            // cbReleases
            // 
            this.cbReleases.FormattingEnabled = true;
            this.cbReleases.Location = new System.Drawing.Point(14, 64);
            this.cbReleases.Name = "cbReleases";
            this.cbReleases.Size = new System.Drawing.Size(225, 21);
            this.cbReleases.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 20);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(148, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Update firmware from the web";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.LightGray;
            this.panel1.Controls.Add(this.linkLabelRelNotes);
            this.panel1.Controls.Add(this.btnUpdateFromWeb);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.cbReleases);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Location = new System.Drawing.Point(12, 13);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(263, 169);
            this.panel1.TabIndex = 10;
            // 
            // linkLabelRelNotes
            // 
            this.linkLabelRelNotes.AutoSize = true;
            this.linkLabelRelNotes.Location = new System.Drawing.Point(20, 98);
            this.linkLabelRelNotes.Name = "linkLabelRelNotes";
            this.linkLabelRelNotes.Size = new System.Drawing.Size(96, 13);
            this.linkLabelRelNotes.TabIndex = 10;
            this.linkLabelRelNotes.TabStop = true;
            this.linkLabelRelNotes.Text = "View release notes";
            this.linkLabelRelNotes.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelRelNotes_LinkClicked);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.LightGray;
            this.panel2.Controls.Add(this.lbBrowsedToFile);
            this.panel2.Controls.Add(this.btnBrowse);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.btnUpdateFromFile);
            this.panel2.Location = new System.Drawing.Point(12, 188);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(263, 130);
            this.panel2.TabIndex = 11;
            // 
            // lbBrowsedToFile
            // 
            this.lbBrowsedToFile.AutoSize = true;
            this.lbBrowsedToFile.Location = new System.Drawing.Point(16, 47);
            this.lbBrowsedToFile.Name = "lbBrowsedToFile";
            this.lbBrowsedToFile.Size = new System.Drawing.Size(97, 13);
            this.lbBrowsedToFile.TabIndex = 11;
            this.lbBrowsedToFile.Text = "No file selected yet";
            this.lbBrowsedToFile.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(20, 76);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(88, 23);
            this.btnBrowse.TabIndex = 10;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 18);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(148, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Update firmware from local file";
            // 
            // FirmwareWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkGray;
            this.ClientSize = new System.Drawing.Size(570, 370);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.FwMsgBox);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.lbCurrentVersion);
            this.Controls.Add(this.label1);
            this.Name = "FirmwareWindow";
            this.Text = "Firmware Update";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbCurrentVersion;
        private System.Windows.Forms.Button btnUpdateFromFile;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox FwMsgBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnUpdateFromWeb;
        private System.Windows.Forms.ComboBox cbReleases;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.LinkLabel linkLabelRelNotes;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lbBrowsedToFile;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Label label4;
    }
}