using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;


namespace RTFPad
{
    public partial class findForm : Form
    {
        public findForm()
        {
            InitializeComponent();
        }

        private void findTextBox_TextChanged(object sender, EventArgs e)
        {
            if (this.textBoxFind.Text.Length > 0) this.buttonFind.Enabled = true;
            else this.buttonFind.Enabled = false;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonFind_Click(object sender, EventArgs e)
        {
            if (((rtfPadForm)this.Owner).tabControl.TabCount <= 0) return;
            if (((rtfPadForm)this.Owner).findText(this.textBoxFind.Text,
                                                        this.radioButtonDown.Checked,
                                                        this.checkBoxMatchCase.Checked) == false)
            {
                MessageBox.Show("Can't find \'" + this.textBoxFind.Text + "\'", "Find",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information,
                                MessageBoxDefaultButton.Button1);
            }
        }
    }
}