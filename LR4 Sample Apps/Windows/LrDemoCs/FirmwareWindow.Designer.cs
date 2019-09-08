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
            this.label2 = new System.Windows.Forms.Label();
            this.btnUpdateFromWeb = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnUpdateFromFile = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.FwMsgBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(123, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Current firmware version:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(118, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Latest firmware version:";
            // 
            // btnUpdateFromWeb
            // 
            this.btnUpdateFromWeb.Location = new System.Drawing.Point(15, 82);
            this.btnUpdateFromWeb.Name = "btnUpdateFromWeb";
            this.btnUpdateFromWeb.Size = new System.Drawing.Size(160, 23);
            this.btnUpdateFromWeb.TabIndex = 2;
            this.btnUpdateFromWeb.Text = "Update firmware from web";
            this.btnUpdateFromWeb.UseVisualStyleBackColor = true;
            this.btnUpdateFromWeb.Click += new System.EventHandler(this.btnUpdateFromWebClicked);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(144, 19);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(31, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "1.2.3";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(144, 46);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(31, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "4.5.6";
            // 
            // btnUpdateFromFile
            // 
            this.btnUpdateFromFile.Location = new System.Drawing.Point(15, 121);
            this.btnUpdateFromFile.Name = "btnUpdateFromFile";
            this.btnUpdateFromFile.Size = new System.Drawing.Size(160, 23);
            this.btnUpdateFromFile.TabIndex = 5;
            this.btnUpdateFromFile.Text = "Update firmware from file";
            this.btnUpdateFromFile.UseVisualStyleBackColor = true;
            this.btnUpdateFromFile.Click += new System.EventHandler(this.btnUpdateFromFileClicked);
            // 
            // button3
            // 
            this.button3.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button3.Location = new System.Drawing.Point(60, 161);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 6;
            this.button3.Text = "Close";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // FwMsgBox
            // 
            this.FwMsgBox.Location = new System.Drawing.Point(193, 10);
            this.FwMsgBox.Multiline = true;
            this.FwMsgBox.Name = "FwMsgBox";
            this.FwMsgBox.Size = new System.Drawing.Size(266, 186);
            this.FwMsgBox.TabIndex = 7;
            // 
            // FirmwareWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Menu;
            this.ClientSize = new System.Drawing.Size(475, 205);
            this.Controls.Add(this.FwMsgBox);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.btnUpdateFromFile);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnUpdateFromWeb);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "FirmwareWindow";
            this.Text = "Firmware Update";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnUpdateFromWeb;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnUpdateFromFile;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox FwMsgBox;
    }
}