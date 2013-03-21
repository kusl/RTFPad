namespace RTFPad
{
    partial class aboutForm
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
            this.title = new System.Windows.Forms.Label();
            this.authorLabel = new System.Windows.Forms.Label();
            this.mailLabel = new System.Windows.Forms.Label();
            this.authorName = new System.Windows.Forms.Label();
            this.authorMail = new System.Windows.Forms.LinkLabel();
            this.buttonOK = new System.Windows.Forms.Button();
            this.authorMailAlt = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // title
            // 
            this.title.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.title.AutoSize = true;
            this.title.Font = new System.Drawing.Font("Segoe UI", 25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.title.Location = new System.Drawing.Point(123, 9);
            this.title.Name = "title";
            this.title.Size = new System.Drawing.Size(131, 46);
            this.title.TabIndex = 0;
            this.title.Text = "RTFPad";
            // 
            // authorLabel
            // 
            this.authorLabel.AutoSize = true;
            this.authorLabel.Location = new System.Drawing.Point(33, 94);
            this.authorLabel.Name = "authorLabel";
            this.authorLabel.Size = new System.Drawing.Size(64, 13);
            this.authorLabel.TabIndex = 1;
            this.authorLabel.Text = "Created by: ";
            // 
            // mailLabel
            // 
            this.mailLabel.AutoSize = true;
            this.mailLabel.Location = new System.Drawing.Point(33, 117);
            this.mailLabel.Name = "mailLabel";
            this.mailLabel.Size = new System.Drawing.Size(41, 13);
            this.mailLabel.TabIndex = 2;
            this.mailLabel.Text = "E-mail: ";
            // 
            // authorName
            // 
            this.authorName.AutoSize = true;
            this.authorName.Location = new System.Drawing.Point(103, 94);
            this.authorName.Name = "authorName";
            this.authorName.Size = new System.Drawing.Size(91, 13);
            this.authorName.TabIndex = 3;
            this.authorName.Text = "Matija Lovreković";
            // 
            // authorMail
            // 
            this.authorMail.AutoSize = true;
            this.authorMail.Location = new System.Drawing.Point(103, 117);
            this.authorMail.Name = "authorMail";
            this.authorMail.Size = new System.Drawing.Size(93, 13);
            this.authorMail.TabIndex = 4;
            this.authorMail.TabStop = true;
            this.authorMail.Text = "mlovrekov@tvz.hr";
            this.authorMail.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.authorMail_LinkClicked);
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonOK.Location = new System.Drawing.Point(165, 233);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 5;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // authorMailAlt
            // 
            this.authorMailAlt.AutoSize = true;
            this.authorMailAlt.Location = new System.Drawing.Point(103, 140);
            this.authorMailAlt.Name = "authorMailAlt";
            this.authorMailAlt.Size = new System.Drawing.Size(123, 13);
            this.authorMailAlt.TabIndex = 6;
            this.authorMailAlt.TabStop = true;
            this.authorMailAlt.Text = "MrPlow442@yahoo.com";
            this.authorMailAlt.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.authorMailAlt_LinkClicked);
            // 
            // aboutForm
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonOK;
            this.ClientSize = new System.Drawing.Size(392, 268);
            this.Controls.Add(this.authorMailAlt);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.authorMail);
            this.Controls.Add(this.authorName);
            this.Controls.Add(this.mailLabel);
            this.Controls.Add(this.authorLabel);
            this.Controls.Add(this.title);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "aboutForm";
            this.ShowIcon = false;
            this.Text = "aboutForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label title;
        private System.Windows.Forms.Label authorLabel;
        private System.Windows.Forms.Label mailLabel;
        private System.Windows.Forms.Label authorName;
        private System.Windows.Forms.LinkLabel authorMail;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.LinkLabel authorMailAlt;
    }
}