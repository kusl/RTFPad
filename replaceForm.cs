using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RTFPad
{
    public partial class replaceForm : Form
    {
        public replaceForm()
        {
            InitializeComponent();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBoxFind_TextChanged(object sender, EventArgs e)
        {
            if (this.textBoxFind.Text.Length > 0)
            {
                this.buttonFindNext.Enabled     = true;
                this.buttonReplace.Enabled      = true;
                this.buttonReplaceAll.Enabled   = true;
            }
            else
            {
                this.buttonFindNext.Enabled     = false;
                this.buttonReplace.Enabled      = false;
                this.buttonReplaceAll.Enabled   = false;
            }
        }

        private void buttonFindNext_Click(object sender, EventArgs e)
        {
            if (((rtfPadForm)this.Owner).tabControl.TabCount <= 0) return;
            if (((rtfPadForm)this.Owner).findText(this.textBoxFind.Text,
                                                        true,
                                                        this.checkBoxMatchCase.Checked) == false)
            {
                MessageBox.Show("Can't find \'" + this.textBoxFind.Text + "'\'", "Find",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information,
                                MessageBoxDefaultButton.Button1);
            }
        }

        private void buttonReplace_Click(object sender, EventArgs e)
        {
            if (((rtfPadForm)this.Owner).tabControl.TabCount <= 0) return;
            if (((rtfPadForm)this.Owner).replaceText(this.textBoxFind.Text, 
                                                      this.textBoxReplace.Text,
                                                      this.checkBoxMatchCase.Checked) == false)
           {
               MessageBox.Show("Can't find \'" + this.textBoxFind.Text + "'\'", "Find",
               MessageBoxButtons.OK,
               MessageBoxIcon.Information,
               MessageBoxDefaultButton.Button1);
           }
        }

        private void buttonReplaceAll_Click(object sender, EventArgs e)
        {
            if (((rtfPadForm)this.Owner).tabControl.TabCount <= 0) return;
            ((rtfPadForm)this.Owner).replaceAllInText(this.textBoxFind.Text, this.textBoxReplace.Text, this.checkBoxMatchCase.Checked);
        }

    }
}
