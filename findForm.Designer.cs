namespace RTFPad
{
    partial class findForm
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
            this.findLabel = new System.Windows.Forms.Label();
            this.textBoxFind = new System.Windows.Forms.TextBox();
            this.checkBoxMatchCase = new System.Windows.Forms.CheckBox();
            this.findDirectionGroupBox = new System.Windows.Forms.GroupBox();
            this.radioButtonDown = new System.Windows.Forms.RadioButton();
            this.radioButtonUp = new System.Windows.Forms.RadioButton();
            this.buttonFind = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.findDirectionGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // findLabel
            // 
            this.findLabel.AutoSize = true;
            this.findLabel.Location = new System.Drawing.Point(13, 24);
            this.findLabel.Name = "findLabel";
            this.findLabel.Size = new System.Drawing.Size(56, 13);
            this.findLabel.TabIndex = 0;
            this.findLabel.Text = "Fi&nd what:";
            // 
            // textBoxFind
            // 
            this.textBoxFind.Location = new System.Drawing.Point(75, 21);
            this.textBoxFind.Name = "textBoxFind";
            this.textBoxFind.Size = new System.Drawing.Size(200, 20);
            this.textBoxFind.TabIndex = 1;
            this.textBoxFind.TextChanged += new System.EventHandler(this.findTextBox_TextChanged);
            // 
            // checkBoxMatchCase
            // 
            this.checkBoxMatchCase.AutoSize = true;
            this.checkBoxMatchCase.Checked = true;
            this.checkBoxMatchCase.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxMatchCase.Location = new System.Drawing.Point(16, 89);
            this.checkBoxMatchCase.Name = "checkBoxMatchCase";
            this.checkBoxMatchCase.Size = new System.Drawing.Size(82, 17);
            this.checkBoxMatchCase.TabIndex = 2;
            this.checkBoxMatchCase.Text = "Match &case";
            this.checkBoxMatchCase.UseVisualStyleBackColor = true;
            // 
            // findDirectionGroupBox
            // 
            this.findDirectionGroupBox.Controls.Add(this.radioButtonDown);
            this.findDirectionGroupBox.Controls.Add(this.radioButtonUp);
            this.findDirectionGroupBox.Location = new System.Drawing.Point(155, 56);
            this.findDirectionGroupBox.Name = "findDirectionGroupBox";
            this.findDirectionGroupBox.Size = new System.Drawing.Size(120, 50);
            this.findDirectionGroupBox.TabIndex = 3;
            this.findDirectionGroupBox.TabStop = false;
            this.findDirectionGroupBox.Text = "Direction";
            // 
            // radioButtonDown
            // 
            this.radioButtonDown.AutoSize = true;
            this.radioButtonDown.Checked = true;
            this.radioButtonDown.Location = new System.Drawing.Point(61, 27);
            this.radioButtonDown.Name = "radioButtonDown";
            this.radioButtonDown.Size = new System.Drawing.Size(53, 17);
            this.radioButtonDown.TabIndex = 1;
            this.radioButtonDown.TabStop = true;
            this.radioButtonDown.Text = "&Down";
            this.radioButtonDown.UseVisualStyleBackColor = true;
            // 
            // radioButtonUp
            // 
            this.radioButtonUp.AutoSize = true;
            this.radioButtonUp.Location = new System.Drawing.Point(6, 27);
            this.radioButtonUp.Name = "radioButtonUp";
            this.radioButtonUp.Size = new System.Drawing.Size(39, 17);
            this.radioButtonUp.TabIndex = 0;
            this.radioButtonUp.Text = "&Up";
            this.radioButtonUp.UseVisualStyleBackColor = true;
            // 
            // buttonFind
            // 
            this.buttonFind.Enabled = false;
            this.buttonFind.Location = new System.Drawing.Point(298, 24);
            this.buttonFind.Name = "buttonFind";
            this.buttonFind.Size = new System.Drawing.Size(75, 23);
            this.buttonFind.TabIndex = 4;
            this.buttonFind.Text = "&Find";
            this.buttonFind.UseVisualStyleBackColor = true;
            this.buttonFind.Click += new System.EventHandler(this.buttonFind_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(298, 53);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // findForm
            // 
            this.AcceptButton = this.buttonFind;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(394, 118);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonFind);
            this.Controls.Add(this.findDirectionGroupBox);
            this.Controls.Add(this.checkBoxMatchCase);
            this.Controls.Add(this.textBoxFind);
            this.Controls.Add(this.findLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "findForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Find";
            this.findDirectionGroupBox.ResumeLayout(false);
            this.findDirectionGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label findLabel;
        private System.Windows.Forms.TextBox textBoxFind;
        private System.Windows.Forms.CheckBox checkBoxMatchCase;
        private System.Windows.Forms.GroupBox findDirectionGroupBox;
        private System.Windows.Forms.RadioButton radioButtonUp;
        private System.Windows.Forms.RadioButton radioButtonDown;
        private System.Windows.Forms.Button buttonFind;
        private System.Windows.Forms.Button buttonCancel;
    }
}