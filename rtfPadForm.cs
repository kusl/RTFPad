using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using PrintPreviewRichTextBox;

namespace RTFPad
{
    public partial class rtfPadForm : Form
    {
        #region variables
        public const int MAX_TABS = 12;
        private Dictionary<string, StreamReader> openedFileInTab = new Dictionary<string, StreamReader>();
        private Dictionary<string, string> fileNameInTab = new Dictionary<string, string>();
        private Dictionary<string, int> fileTypeInTab = new Dictionary<string, int>();
        private Dictionary<string, int> selectedColorInTab = new Dictionary<string, int>();
        private Dictionary<string, string> textToFindInTab = new Dictionary<string, string>();
        private Dictionary<string, bool> matchCaseInTab = new Dictionary<string, bool>();
        protected internal TabControl tabControl = new TabControl();
        private FontFamily[] fontList;
        private string[] colorList = System.Enum.GetNames(typeof(KnownColor));
        #endregion

        #region constructor
        public rtfPadForm()
        {
            InitializeComponent();
            this.fontList = FontFamily.Families;
            foreach (FontFamily font in fontList)
            {
                this.toolStripCBoxFont.Items.Add(font.Name);
            }
            foreach (string color in colorList)
            {
                Color boja = Color.FromKnownColor((KnownColor)System.Enum.Parse(typeof(KnownColor), color));
                if (boja.IsSystemColor == false && boja.Name != Color.Transparent.Name)
                    this.toolStripFontColor.DropDownItems.Add(color);
                    
            }
            foreach (ToolStripDropDownItem entry in this.toolStripFontColor.DropDownItems)
            {
                entry.BackColor = Color.FromKnownColor((KnownColor)System.Enum.Parse(typeof(KnownColor), entry.Text));
                Color c = entry.BackColor;
                int Luminance = (int)(0.2126 * c.R + 0.7152 * c.G + 0.0722 * c.B);
                if (Luminance > 50)
                {
                    entry.ForeColor = Color.Black;
                }
                else
                {
                    entry.ForeColor = Color.White;
                }

            }
            this.newTab();
            this.rtb_SelectionChanged(this, new EventArgs());
        }
        #endregion

        #region File

        /* File Exit */
        private void menuFileExit_Click(object sender, EventArgs e)
        {
            if (this.tabControl.TabCount <= 0) return;

            for (int i = this.tabControl.TabCount; i >= 0; --i)
            {
                this.tabControl.SelectedIndex = i;
                RichTextBox rtb = (RichTextBox)this.tabControl.SelectedTab.Controls[0];

                if (rtb.Text != (string)rtb.Tag)
                {

                    DialogResult result = MessageBox.Show("Do you wish to save changes to " + this.tabControl.SelectedTab.Text + " ?",
                                     "RTFPad", MessageBoxButtons.YesNoCancel);

                    if (result == DialogResult.Yes)
                    {
                        menuFileSave_Click(sender, e);
                    }
                    else if (result == DialogResult.Cancel)
                    {
                        return;
                    }
                }
            }
            this.Close();
        }

        /* File New */
        private void menuFileNew_Click(object sender, EventArgs e)
        {
            this.newTab();
        }

        /* File Open */
        private void menuFileOpen_Click(object sender, EventArgs e)
        {
            if (this.dialogOpen.ShowDialog() == DialogResult.OK)
            {
                bool exists = false;
                for (int i = 0; i < this.tabControl.TabCount; i++)
                {
                    if (this.tabControl.TabPages[i].Text == this.dialogOpen.SafeFileName)
                    {
                        exists = true;
                        this.tabControl.SelectedIndex = i;
                    }
                }

                if (exists == false)
                {
                    if (this.tabControl.TabCount < MAX_TABS) menuFileNew_Click(sender, e);
                    else
                    {
                        DialogResult mBoxResult = MessageBox.Show("Max tab count reached! Do you wish to open the file in the current tab?",
                                                                    "RTFPad", MessageBoxButtons.YesNoCancel);
                        switch (mBoxResult)
                        {
                            case DialogResult.Yes: this.closeTab();
                                                   this.newTab();
                                                   break;
                            case DialogResult.No: MessageBox.Show("Close a tab", "RTFPad", MessageBoxButtons.OK);
                                                  return; 
                            case DialogResult.Cancel: return;
                        }

                    }
                    RichTextBox rtb = (RichTextBox)this.tabControl.SelectedTab.Controls[0];

                    StreamReader value;
                    openedFileInTab.TryGetValue(this.tabControl.SelectedTab.Text, out value);
                    if (value != null)
                        this.openedFileInTab[this.tabControl.SelectedTab.Text].Close();

                    this.fileTypeInTab.Remove(this.tabControl.SelectedTab.Text);
                    this.fileNameInTab.Remove(this.tabControl.SelectedTab.Text);
                    this.openedFileInTab.Remove(this.tabControl.SelectedTab.Text);
                    this.selectedColorInTab.Remove(this.tabControl.SelectedTab.Text);
                    this.textToFindInTab.Remove(this.tabControl.SelectedTab.Text);
                    this.matchCaseInTab.Remove(this.tabControl.SelectedTab.Text);

                    this.tabControl.SelectedTab.Text = this.dialogOpen.SafeFileName;
                    int fileType = 0;
                    if (this.dialogOpen.FilterIndex == 1)
                    {
                        rtb.LoadFile(this.dialogOpen.FileName, RichTextBoxStreamType.RichText);
                        fileType = 1;
                    }
                    else
                    {
                        rtb.LoadFile(this.dialogOpen.FileName, RichTextBoxStreamType.PlainText);
                        fileType = 2;
                    }
                    rtb.Tag = rtb.Text;
                    this.openedFileInTab[this.tabControl.SelectedTab.Text] = new StreamReader(this.dialogOpen.FileName);
                    this.tabControl.SelectedTab.Text                        = this.dialogOpen.SafeFileName;
                    this.fileNameInTab[this.tabControl.SelectedTab.Text]    = this.dialogOpen.FileName;
                    this.Text                                               = "RTFPad - " + this.tabControl.SelectedTab.Text;
                    this.fileTypeInTab[this.tabControl.SelectedTab.Text]   = fileType;
                    this.rtb_SelectionChanged(sender, e);
                    this.tabControl_SelectedIndexChanged(sender, e);
                }
            }
        }

        /* File Save As */
        private void menuFileSaveAs_Click(object sender, EventArgs e)
        {
            RichTextBox rtb = (RichTextBox)this.tabControl.SelectedTab.Controls[0];
            if (this.dialogSave.ShowDialog() == DialogResult.OK)
            {
                StreamReader value;
                openedFileInTab.TryGetValue(this.tabControl.SelectedTab.Text, out value);
                if (value != null)
                    this.openedFileInTab[this.tabControl.SelectedTab.Text].Close();

                this.fileTypeInTab.Remove(this.tabControl.SelectedTab.Text);
                this.fileNameInTab.Remove(this.tabControl.SelectedTab.Text);
                this.openedFileInTab.Remove(this.tabControl.SelectedTab.Text);
                this.selectedColorInTab.Remove(this.tabControl.SelectedTab.Text);
                this.textToFindInTab.Remove(this.tabControl.SelectedTab.Text);
                this.matchCaseInTab.Remove(this.tabControl.SelectedTab.Text);

                this.tabControl.SelectedTab.Text = this.dialogSave.FileName.Substring(this.dialogSave.FileName.LastIndexOf('\\') + 1);

                string currentTabKey = this.tabControl.SelectedTab.Text;
                if (this.dialogSave.FilterIndex == 1)
                {
                    rtb.SaveFile(this.dialogSave.FileName, RichTextBoxStreamType.RichText);
                    this.fileTypeInTab[currentTabKey] = 1;
                }
                else
                {
                    rtb.SaveFile(this.dialogSave.FileName, RichTextBoxStreamType.PlainText);
                    this.fileTypeInTab[currentTabKey] = 2;
                }
                rtb.Tag = rtb.Text;
                this.openedFileInTab[currentTabKey] = new StreamReader(this.dialogSave.FileName);
                this.fileNameInTab[currentTabKey] = this.dialogSave.FileName;
                this.Text = "RTFPad - " + this.dialogSave.FileName.
                                                                Substring(this.dialogSave.FileName.LastIndexOf('\\') + 1);
                this.rtb_SelectionChanged(sender, e);
                this.tabControl_SelectedIndexChanged(sender, e);
            }
        }

        /* File Save */
        private void menuFileSave_Click(object sender, EventArgs e)
        {
            RichTextBox rtb = (RichTextBox)this.tabControl.SelectedTab.Controls[0];
            string currentTabKey = this.tabControl.SelectedTab.Text;
            StreamReader value;
            this.openedFileInTab.TryGetValue(currentTabKey, out value);
            if (value != null && this.fileNameInTab[currentTabKey] != null)
            {
                this.openedFileInTab[currentTabKey].Close();
                if (this.fileTypeInTab[currentTabKey] == 1) rtb.SaveFile(this.fileNameInTab[currentTabKey], RichTextBoxStreamType.RichText);
                else rtb.SaveFile(this.fileNameInTab[currentTabKey], RichTextBoxStreamType.PlainText);
                this.openedFileInTab[currentTabKey] = new StreamReader(this.fileNameInTab[currentTabKey]);
            }
            else this.menuFileSaveAs_Click(sender, e);
        }

        /* File Close Current Tab */
        private void menuFileCloseCurrentTab_Click(object sender, EventArgs e)
        {
            this.closeTab();
            changeUIState_CloseCurrentTabButton();
        }

        /* File menu click event */
        private void menuFile_Click(object sender, EventArgs e)
        {
            changeUIState_CloseCurrentTabButton();
        }

        /* File Print Preview */
        private void menuFilePrintPreview_Click(object sender, EventArgs e)
        {
            if (this.tabControl.TabCount <= 0) return;
            RichTextBox rtb = (RichTextBox)this.tabControl.SelectedTab.Controls[0];
            RichTextBoxDocument document = new RichTextBoxDocument(rtb);

            dialogPrintPreview.Document = document;
            dialogPrintPreview.ShowDialog();
        }

        /* File Print */
        private void menuFilePrint_Click(object sender, EventArgs e)
        {
            if (this.tabControl.TabCount <= 0) return;
            RichTextBox rtb = (RichTextBox)this.tabControl.SelectedTab.Controls[0];
            RichTextBoxDocument document = new RichTextBoxDocument(rtb);

            dialogPrint.Document = document;
            if (dialogPrint.ShowDialog() == DialogResult.OK)
            {
                document.Print();
            }
        }

        /* File Page Setup */
        private void menuFilePageSetup_Click(object sender, EventArgs e)
        {
            if (this.tabControl.TabCount <= 0) return;
            RichTextBox rtb = (RichTextBox)this.tabControl.SelectedTab.Controls[0];
            RichTextBoxDocument document = new RichTextBoxDocument(rtb);

            dialogPageSetup.Document = document;
            dialogPageSetup.PageSettings = new System.Drawing.Printing.PageSettings();
            dialogPageSetup.PrinterSettings = new System.Drawing.Printing.PrinterSettings();
            dialogPageSetup.ShowDialog();
        }
        #endregion

        #region Edit

        /* Edit Undo */
        private void menuEditUndo_Click(object sender, EventArgs e)
        {
            if (tabControl.TabCount <= 0) return;
            RichTextBox rtb = (RichTextBox)this.tabControl.SelectedTab.Controls[0];
            if (rtb.CanUndo) rtb.Undo();
        }

        /* Edit Redo */
        private void menuEditRedo_Click(object sender, EventArgs e)
        {
            if (tabControl.TabCount <= 0) return;
            RichTextBox rtb = (RichTextBox)this.tabControl.SelectedTab.Controls[0];
            if (rtb.CanRedo) rtb.Redo();
        }

        /* Edit Cut */
        private void menuEditCut_Click(object sender, EventArgs e)
        {
            if (tabControl.TabCount <= 0) return;
            RichTextBox rtb = (RichTextBox)this.tabControl.SelectedTab.Controls[0];
            rtb.Cut();
        }

        /* Edit Copy */
        private void menuEditCopy_Click(object sender, EventArgs e)
        {
            if (tabControl.TabCount <= 0) return;
            RichTextBox rtb = (RichTextBox)this.tabControl.SelectedTab.Controls[0];
            rtb.Copy();
        }

        /*  Edit Paste */
        private void menuEditPaste_Click(object sender, EventArgs e)
        {
            if (tabControl.TabCount <= 0) return;
            RichTextBox rtb = (RichTextBox)this.tabControl.SelectedTab.Controls[0];
            rtb.Paste();
        }

        /* Edit Find */
        private void menuEditFind_Click(object sender, EventArgs e)
        {
            findForm findControl = new findForm();
            findControl.Owner = this;
            findControl.Show();
        }

        /* Edit Find Next */
        private void menuEditFindNext_Click(object sender, EventArgs e)
        {
            bool isTextInMap = this.textToFindInTab.ContainsKey(this.tabControl.SelectedTab.Text);
            bool isCaseInMap = this.matchCaseInTab.ContainsKey(this.tabControl.SelectedTab.Text);
            if ((!isTextInMap || !isCaseInMap))
            {
                menuEditFind_Click(sender, e);
            }
            else if (this.textToFindInTab[this.tabControl.SelectedTab.Text] == "")
            {
                menuEditFind_Click(sender, e);
            }
            else
            {
                bool postojiText = findText(this.textToFindInTab[this.tabControl.SelectedTab.Text], true, this.matchCaseInTab[this.tabControl.SelectedTab.Text]);
                if (!postojiText)
                {
                    MessageBox.Show("Can't find \'" + this.textToFindInTab[this.tabControl.SelectedTab.Text] + "\'", "Find",
                                     MessageBoxButtons.OK,
                                     MessageBoxIcon.Information,
                                     MessageBoxDefaultButton.Button1);
                }
            }
        }

        /* Edit Clear */
        private void menuEditClear_Click(object sender, EventArgs e)
        {
            if (tabControl.TabCount <= 0) return;
            RichTextBox rtb = (RichTextBox)this.tabControl.SelectedTab.Controls[0];
            rtb.SelectedText = "";
        }

        /* Edit Select All */
        private void menuEditSelectAll_Click(object sender, EventArgs e)
        {
            if (tabControl.TabCount <= 0) return;
            RichTextBox rtb = (RichTextBox)this.tabControl.SelectedTab.Controls[0];
            rtb.SelectAll();
        }

        /* Edit Replace */
        private void menuEditReplace_Click(object sender, EventArgs e)
        {
            replaceForm replaceControl = new replaceForm();
            replaceControl.Owner = this;
            replaceControl.Show();
        }
        #endregion

        #region View
        /* View Word Wrap */
        private void menuViewWordWrap_Click(object sender, EventArgs e)
        {
            if (this.tabControl.TabCount <= 0) return;
            RichTextBox rtb = (RichTextBox)this.tabControl.SelectedTab.Controls[0];

            if (rtb.WordWrap)
            {
                rtb.WordWrap = false;
                menuViewWordWrap.Checked = false;
            }
            else
            {
                rtb.WordWrap = true;
                menuViewWordWrap.Checked = true;
            }
        }
        #endregion

        #region Format

        /* Format Font */
        private void menuFormatFont_Click(object sender, EventArgs e)
        {
            if (this.tabControl.TabCount <= 0) return;
            RichTextBox rtb = (RichTextBox)this.tabControl.SelectedTab.Controls[0];

            if (this.dialogFont.ShowDialog() == DialogResult.OK)
            {
                rtb.SelectionFont = this.dialogFont.Font;
                rtb.SelectionColor = this.dialogFont.Color;
            }
        }

        /* Format Color */
        private void menuFormatColor_Click(object sender, EventArgs e)
        {
            if (this.tabControl.TabCount <= 0) return;
            RichTextBox rtb = (RichTextBox)this.tabControl.SelectedTab.Controls[0];

            if (this.dialogColor.ShowDialog() == DialogResult.OK)
            {
                rtb.SelectionColor = this.dialogColor.Color;
            }
        }
        #endregion

        #region Help
        /* Help About */
        private void aboutRTFPadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            aboutForm aboutDialog = new aboutForm();
            aboutDialog.Owner = this;
            aboutDialog.StartPosition = FormStartPosition.CenterParent;
            aboutDialog.ShowDialog();
        }
        #endregion

        #region Context Menu

        /* Context Menu New Tab */
        private void contextMenuNew_Click(object sender, EventArgs e)
        {
            this.newTab();
        }


        /* Context Menu Close Tab */
        private void contextMenuClose_Click(object sender, EventArgs e)
        {
            if (this.tabControl.TabCount <= 0) return;
            RichTextBox rtb = (RichTextBox)this.tabControl.SelectedTab.Controls[0];

            if (rtb.Text != (string)rtb.Tag)
            {
                DialogResult result = MessageBox.Show("Do you wish to save changes to " + this.tabControl.SelectedTab.Text + " ?",
                                 "RTFPad", MessageBoxButtons.YesNoCancel);

                if (result == DialogResult.Yes)
                {
                    menuFileSave_Click(sender, e);
                }
                else if (result == DialogResult.Cancel)
                {
                    return;
                }
            }
            this.closeTab();
            changeUIState_CloseCurrentTabButton();
        }
        #endregion

        #region Tool Strip

        /* Tool Strip New Tab */
        private void toolStripNew_Click(object sender, EventArgs e)
        {
            this.newTab();
            changeUIState_CloseCurrentTabButton();
        }

        /* Tool Strip Open */
        private void toolStripOpen_Click(object sender, EventArgs e)
        {
            this.menuFileOpen_Click(sender, e);
        }

        /* Tool Strip Save */
        private void toolStripSave_Click(object sender, EventArgs e)
        {
            this.menuFileSave_Click(sender, e);
        }

        /* Tool Strip Font */
        private void toolStripCBoxFont_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.tabControl.TabCount > 0)
            {
                RichTextBox rtb = (RichTextBox)this.tabControl.SelectedTab.Controls[0];
                try
                {
                    rtb.SelectionFont = new Font(this.fontList[this.toolStripCBoxFont.SelectedIndex],
                                                 int.Parse(this.toolStripCBoxFontSize.SelectedItem.ToString()),
                                                 rtb.Font.Style, rtb.Font.Unit);
                }
                catch
                {

                }
                finally
                {
                    rtb.Focus();
                }

            }
        }

        /* Tool Strip Font Size */
        private void toolStripCBoxFontSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.tabControl.TabCount <= 0) return;
            RichTextBox rtb = (RichTextBox)this.tabControl.SelectedTab.Controls[0];
            try
            {
                rtb.SelectionFont = new Font(rtb.SelectionFont.FontFamily,
                                            int.Parse(this.toolStripCBoxFontSize.SelectedItem.ToString()),
                                            rtb.Font.Style, rtb.Font.Unit);
            }
            catch
            {

            }
            finally
            {
                rtb.Focus();
            }
        }

        /* Tool Strip Color */
        private void toolStripFontColor_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (this.tabControl.TabCount <= 0) return;

            RichTextBox rtb = (RichTextBox)this.tabControl.SelectedTab.Controls[0];
            rtb.SelectionColor = Color.FromKnownColor((KnownColor)System.Enum.Parse(typeof(KnownColor), e.ClickedItem.Text));

            if (!this.selectedColorInTab.ContainsKey(this.tabControl.SelectedTab.Text))
                this.selectedColorInTab.Add(this.tabControl.SelectedTab.Text, default(int));

            int index;
            this.selectedColorInTab.TryGetValue(this.tabControl.SelectedTab.Text, out index);

            ((ToolStripMenuItem)this.toolStripFontColor.DropDownItems[index]).Checked = false;
            this.selectedColorInTab[this.tabControl.SelectedTab.Text] = this.toolStripFontColor.
                                                                        DropDownItems.IndexOf((ToolStripMenuItem)e.ClickedItem);
            ((ToolStripMenuItem)e.ClickedItem).Checked = true;
        }

        /* Tool Strip Undo */
        private void toolStripUndo_Click(object sender, EventArgs e)
        {
            menuEditUndo_Click(sender, e);
        }

        /* Tool Strip Redo */
        private void toolStripRedo_Click(object sender, EventArgs e)
        {
            menuEditRedo_Click(sender, e);
        }

        /* Tool Strip Cut */
        private void toolStripCut_Click(object sender, EventArgs e)
        {
            menuEditCut_Click(sender, e);
        }

        /* Tool Strip Copy */
        private void toolStripCopy_Click(object sender, EventArgs e)
        {
            menuEditCopy_Click(sender, e);
        }

        /* Tool Strip Paste */
        private void toolStripPaste_Click(object sender, EventArgs e)
        {
            menuEditPaste_Click(sender, e);
        }

        /* Tool Strip Find */
        private void toolStripFind_Click(object sender, EventArgs e)
        {
            menuEditFind_Click(sender, e);
        }

        /* Tool Strip Print Preview */
        private void toolStripPrintPreview_Click(object sender, EventArgs e)
        {
            menuFilePrintPreview_Click(sender, e);
        }

        /* Tool Strip Print */
        private void toolStripPrint_Click(object sender, EventArgs e)
        {
            menuFilePrint_Click(sender, e);
        }

        /* Tool Strip Bold */
        private void toolStripBold_Click(object sender, EventArgs e)
        {
            if (this.tabControl.TabCount <= 0) return;
            RichTextBox rtb = (RichTextBox)this.tabControl.SelectedTab.Controls[0];

            if (!rtb.SelectionFont.Bold)
            {
                rtb.SelectionFont = new Font(this.fontList[this.toolStripCBoxFont.SelectedIndex],
                                                 int.Parse(this.toolStripCBoxFontSize.SelectedItem.ToString()),
                                                 FontStyle.Bold, rtb.Font.Unit);
                this.toolStripBold.Checked = true;
            }
            else
            {
                rtb.SelectionFont = new Font(this.fontList[this.toolStripCBoxFont.SelectedIndex],
                                 int.Parse(this.toolStripCBoxFontSize.SelectedItem.ToString()),
                                 rtb.Font.Style, rtb.Font.Unit);
                this.toolStripBold.Checked = false;
            }

        }

        /* Tool Strip Italic */
        private void toolStripItalic_Click(object sender, EventArgs e)
        {
            if (this.tabControl.TabCount <= 0) return;
            RichTextBox rtb = (RichTextBox)this.tabControl.SelectedTab.Controls[0];

            if (!rtb.SelectionFont.Italic)
            {
                rtb.SelectionFont = new Font(this.fontList[this.toolStripCBoxFont.SelectedIndex],
                                                 int.Parse(this.toolStripCBoxFontSize.SelectedItem.ToString()),
                                                 FontStyle.Italic, rtb.Font.Unit);
                this.toolStripItalic.Checked = true;
            }
            else
            {
                rtb.SelectionFont = new Font(this.fontList[this.toolStripCBoxFont.SelectedIndex],
                                 int.Parse(this.toolStripCBoxFontSize.SelectedItem.ToString()),
                                 rtb.Font.Style, rtb.Font.Unit);
                this.toolStripItalic.Checked = false;
            }
        }

        /* Tool Strip Underline */
        private void toolStripUnderline_Click(object sender, EventArgs e)
        {
            if (this.tabControl.TabCount <= 0) return;
            RichTextBox rtb = (RichTextBox)this.tabControl.SelectedTab.Controls[0];

            if (!rtb.SelectionFont.Underline)
            {
                rtb.SelectionFont = new Font(this.fontList[this.toolStripCBoxFont.SelectedIndex],
                                                 int.Parse(this.toolStripCBoxFontSize.SelectedItem.ToString()),
                                                 FontStyle.Underline, rtb.Font.Unit);
                this.toolStripUnderline.Checked = true;
            }
            else
            {
                rtb.SelectionFont = new Font(this.fontList[this.toolStripCBoxFont.SelectedIndex],
                                 int.Parse(this.toolStripCBoxFontSize.SelectedItem.ToString()),
                                 rtb.Font.Style, rtb.Font.Unit);
                this.toolStripUnderline.Checked = false;
            }
        }

        /* Tool Strip Strikethrough */
        private void toolStripStrikethrough_Click(object sender, EventArgs e)
        {
            if (this.tabControl.TabCount <= 0) return;
            RichTextBox rtb = (RichTextBox)this.tabControl.SelectedTab.Controls[0];

            if (!rtb.SelectionFont.Strikeout)
            {
                rtb.SelectionFont = new Font(this.fontList[this.toolStripCBoxFont.SelectedIndex],
                                                 int.Parse(this.toolStripCBoxFontSize.SelectedItem.ToString()),
                                                 FontStyle.Strikeout, rtb.Font.Unit);
                this.toolStripStrikethrough.Checked = true;
            }
            else
            {
                rtb.SelectionFont = new Font(this.fontList[this.toolStripCBoxFont.SelectedIndex],
                                 int.Parse(this.toolStripCBoxFontSize.SelectedItem.ToString()),
                                 rtb.Font.Style, rtb.Font.Unit);
                this.toolStripStrikethrough.Checked = false;
            }
        }

        /* Tool Strip Align Left */
        private void toolStripAlignLeft_Click(object sender, EventArgs e)
        {
            if (this.tabControl.TabCount <= 0) return;
            RichTextBox rtb = (RichTextBox)this.tabControl.SelectedTab.Controls[0];

            if (rtb.SelectionAlignment != HorizontalAlignment.Left)
            {
                rtb.SelectionAlignment = HorizontalAlignment.Left;
                this.toolStripAlignLeft.Checked = true;
            }
            else
            {
                this.toolStripAlignLeft.Checked = false;
            }

        }

        /* Tool Strip Align Center */
        private void toolStripAlignCenter_Click(object sender, EventArgs e)
        {
            if (this.tabControl.TabCount <= 0) return;
            RichTextBox rtb = (RichTextBox)this.tabControl.SelectedTab.Controls[0];

            if (rtb.SelectionAlignment != HorizontalAlignment.Center)
            {
                rtb.SelectionAlignment = HorizontalAlignment.Center;
                this.toolStripAlignCenter.Checked = true;
            }
            else
            {
                this.toolStripAlignCenter.Checked = false;
            }
        }

        /* Tool Strip Align Right*/
        private void toolStripAlignRight_Click(object sender, EventArgs e)
        {
            if (this.tabControl.TabCount <= 0) return;
            RichTextBox rtb = (RichTextBox)this.tabControl.SelectedTab.Controls[0];

            if (rtb.SelectionAlignment != HorizontalAlignment.Right)
            {
                rtb.SelectionAlignment = HorizontalAlignment.Right;
                this.toolStripAlignRight.Checked = true;
            }
            else
            {
                this.toolStripAlignRight.Checked = false;
            }
        }

        /* Tool Strip Bullet */
        private void toolStripBullet_Click(object sender, EventArgs e)
        {
            if (this.tabControl.TabCount <= 0) return;
            RichTextBox rtb = (RichTextBox)this.tabControl.SelectedTab.Controls[0];

            if (!rtb.SelectionBullet)
            {
                rtb.SelectionBullet = true;
            }
            else
            {
                rtb.SelectionBullet = false;
            }
        }
        #endregion

        #region Helper Functions

        /* Function which creates a new tab and a rich text box in it */
        private void newTab()
        {
            if (this.tabControl.TabCount != 0)
            {
                StreamReader value;
                openedFileInTab.TryGetValue(this.tabControl.SelectedTab.Text, out value);
                if (value != null)
                    this.openedFileInTab[this.tabControl.SelectedTab.Text].Close();
            }

            RichTextBox rtb = new RichTextBox();
            if (this.tabControl.TabCount < MAX_TABS)
            {
                this.tabControl.TabPages.Add(this.tabControl.TabCount == 0 ?
                                              "New Tab" : "New Tab " + this.tabControl.TabCount.ToString());
                this.tabControl.TabPages[(this.tabControl.TabCount - 1)].Controls.Add(rtb);
                this.tabControl.SelectedIndex = this.tabControl.TabCount - 1;

                rtb.Dock = DockStyle.Fill;
                rtb.Multiline = true;
                rtb.WordWrap = true;
                rtb.ScrollBars = RichTextBoxScrollBars.Both;
                rtb.EnableAutoDragDrop = true;
                rtb.AcceptsTab = true;
                rtb.AutoWordSelection = true;
                rtb.DetectUrls = true;
                rtb.HideSelection = false;
                rtb.Tag = rtb.Text;

                rtb.SelectionChanged += new EventHandler(rtb_SelectionChanged);
                rtb.TextChanged += new EventHandler(rtb_TextChanged);
                rtb.KeyDown += new KeyEventHandler(rtb_KeyDown);
                this.Text = "RTFPad - " + this.tabControl.SelectedTab.Text;
                fileTypeInTab[this.tabControl.SelectedTab.Text] = 1;
                changeUIState_CloseCurrentTabButton();
                tabControl_SelectedIndexChanged(this, new EventArgs());
                this.ActiveControl = rtb;
                rtb.Focus();
            }
            else
            {
                MessageBox.Show("You cannot create any more tabs!", "RTFPad", MessageBoxButtons.OK);
            }
        }

        /* Function which closes the current tab
         */
        private void closeTab()
        {
            if (this.tabControl.TabCount <= 0)
            {
                MessageBox.Show("There are no tabs", "Cannot Close Tab", MessageBoxButtons.OK);
                return;
            }
            string tabKey = this.tabControl.SelectedTab.Text;
            this.fileNameInTab.Remove(tabKey);
            this.fileTypeInTab.Remove(tabKey);
            this.openedFileInTab.Remove(tabKey);
            this.selectedColorInTab.Remove(tabKey);
            this.textToFindInTab.Remove(tabKey);
            this.matchCaseInTab.Remove(tabKey);
            //this.tabControl.TabPages.RemoveByKey("tab " + this.tabControl.SelectedIndex.ToString());
            this.tabControl.TabPages.RemoveAt(this.tabControl.SelectedIndex);
        }

        /* Function which finds the first occurence of textToFind in the textbox
         * Used by: Find, Find Next, Replace, Replace All
         */
        protected internal bool findText(string textToFind, bool searchDirectionDown, bool matchCase)
        {
            RichTextBox rtb = (RichTextBox)this.tabControl.SelectedTab.Controls[0];
            StringComparison comparator = matchCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

            this.textToFindInTab[this.tabControl.SelectedTab.Text] = textToFind;
            this.matchCaseInTab[this.tabControl.SelectedTab.Text] = matchCase;
            int startIndex = rtb.SelectionStart;
            int length = textToFind.Length;
            if (searchDirectionDown)
            {
                if (rtb.SelectedText.Length == 0 || rtb.SelectedText.ToLower() != textToFind.ToLower())
                    startIndex = rtb.Text.IndexOf(textToFind, rtb.SelectionStart, comparator);
                else
                {
                    int offset = rtb.SelectionStart + 1;
                    startIndex = rtb.Text.IndexOf(textToFind, offset, comparator);
                }
            }
            else
            {
                startIndex = rtb.Text.LastIndexOf(textToFind, rtb.SelectionStart, comparator);
            }
            if (startIndex == -1 || startIndex >= rtb.Text.Length) return false;
            rtb.Select(startIndex, length);
            rtb.ScrollToCaret();
            return true;
        }

        /* Function which replaces the first occurence of textToReplace with replaceWith
         * Used by: Replace, Replace All
         */
        protected internal bool replaceText(string textToReplace, string replaceWith, bool matchCase)
        {
            RichTextBox rtb = (RichTextBox)this.tabControl.SelectedTab.Controls[0];
            if (rtb.SelectedText.Length == 0 || rtb.SelectedText.ToLower() != textToReplace.ToLower())
            {
                return findText(textToReplace, true, matchCase);
            }
            else
            {
                rtb.SelectedText = replaceWith;
                return true;
            }
        }

        /* Replaces all matching text with another
         * Used by: Replace All
         */
        protected internal void replaceAllInText(string textToReplace, string replaceWith, bool matchCase)
        {
            RichTextBox rtb = (RichTextBox)this.tabControl.SelectedTab.Controls[0];
            rtb.SelectionStart = 0;
            rtb.SelectionLength = 0;
            while (this.findText(textToReplace, true, matchCase))
            {
                replaceText(textToReplace, replaceWith, matchCase);
            }
        }

        /* Enables/Disables the close current tab button in menu */
        private void changeUIState_CloseCurrentTabButton()
        {
            if (this.tabControl.TabCount > 0)
                this.menuFileCloseCurrentTab.Enabled = true;
            else
                this.menuFileCloseCurrentTab.Enabled = false;
        }

        /* Key shortcuts to Bold,Italic and Underline
         * 
         */
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case (Keys.Control | Keys.B):
                    this.toolStripBold_Click(this, new EventArgs()); break;

                case (Keys.Control | Keys.I):
                    this.toolStripItalic_Click(this, new EventArgs()); break;

                case (Keys.Control | Keys.U):
                    this.toolStripUnderline_Click(this, new EventArgs()); break;

            }
            return base.ProcessCmdKey(ref msg, keyData);
        }


        /* Blocks Ctrl + i in tab to act as the tab key
         * 
         */
        private void rtb_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.tabControl.TabCount <= 0) return;
            RichTextBox rtb = (RichTextBox)this.tabControl.SelectedTab.Controls[0];
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.I)
            {
                e.SuppressKeyPress = true;
            }
        }
        #endregion

        #region General Events

        private void rtb_TextChanged(Object sender, EventArgs e)
        {
            RichTextBox rtb = (RichTextBox)this.tabControl.SelectedTab.Controls[0];
            if (rtb.TextLength == 0)
            {
                this.menuEditFind.Enabled = false;
                this.toolStripFind.Enabled = false;
                this.menuEditFindNext.Enabled = false;
                this.menuEditReplace.Enabled = false;
                this.menuEditSelectAll.Enabled = false;

            }
            else
            {
                this.menuEditFind.Enabled = true;
                this.toolStripFind.Enabled = true;
                this.menuEditFindNext.Enabled = true;
                this.menuEditReplace.Enabled = true;
                this.menuEditSelectAll.Enabled = true;
            }
            if (rtb.SelectionAlignment == HorizontalAlignment.Left) this.toolStripAlignLeft.Checked = true;
            else this.toolStripAlignLeft.Checked = false;

            if (rtb.SelectionAlignment == HorizontalAlignment.Center) this.toolStripAlignCenter.Checked = true;
            else this.toolStripAlignCenter.Checked = false;

            if (rtb.SelectionAlignment == HorizontalAlignment.Right) this.toolStripAlignRight.Checked = true;
            else this.toolStripAlignRight.Checked = false;
        }

        /* Event which watches for the selection change 
         * Used for finding the current font,style,color,etc..
         */
        private void rtb_SelectionChanged(Object sender, EventArgs e)
        {
            try
            {
                RichTextBox rtb = (RichTextBox)this.tabControl.SelectedTab.Controls[0];
                this.toolStripCBoxFont.SelectedItem = rtb.SelectionFont.FontFamily.Name;
                this.toolStripCBoxFontSize.SelectedItem = Math.Round(rtb.SelectionFont.Size).ToString();

                foreach (ToolStripMenuItem item in this.toolStripFontColor.DropDownItems)
                {
                    item.Checked = false;
                    if (item.Text == rtb.SelectionColor.Name)
                    {
                        item.Checked = true;
                    }
                }

                if (rtb.TextLength == 0)
                {
                    this.menuEditFind.Enabled      = false;
                    this.toolStripFind.Enabled     = false;
                    this.menuEditFindNext.Enabled  = false;
                    this.menuEditReplace.Enabled   = false;
                    this.menuEditSelectAll.Enabled = false;
                    
                }
                else
                {
                    this.menuEditFind.Enabled      = true;
                    this.toolStripFind.Enabled     = true;
                    this.menuEditFindNext.Enabled  = true;
                    this.menuEditReplace.Enabled   = true;
                    this.menuEditSelectAll.Enabled = true;
                }

                if (rtb.CanUndo)
                {
                    this.menuEditUndo.Enabled  = true;
                    this.toolStripUndo.Enabled = true;
                }
                else
                {
                    this.menuEditUndo.Enabled  = false;
                    this.toolStripUndo.Enabled = false;
                }

                if (rtb.CanRedo)
                {
                    this.menuEditRedo.Enabled  = true;
                    this.toolStripRedo.Enabled = true;
                }
                else
                {
                    this.menuEditRedo.Enabled  = false;
                    this.toolStripRedo.Enabled = false;
                }

                if (rtb.SelectionLength == 0)
                {
                    this.menuEditClear.Enabled = false;
                    this.menuEditCut.Enabled   = false;
                    this.toolStripCut.Enabled  = false;
                    this.menuEditCopy.Enabled  = false;
                    this.toolStripCopy.Enabled = false;
                }
                else
                {
                    this.menuEditClear.Enabled = true;
                    this.menuEditCut.Enabled   = true;
                    this.toolStripCut.Enabled  = true;
                    this.menuEditCopy.Enabled  = true;
                    this.toolStripCopy.Enabled = true;
                }
                if (rtb.SelectionFont.Bold) this.toolStripBold.Checked = true;
                else this.toolStripBold.Checked = false;

                if (rtb.SelectionFont.Italic) this.toolStripItalic.Checked = true;
                else this.toolStripItalic.Checked = false;

                if (rtb.SelectionFont.Underline) this.toolStripUnderline.Checked = true;
                else this.toolStripUnderline.Checked = false;

                if (rtb.SelectionFont.Strikeout) this.toolStripStrikethrough.Checked = true;
                else this.toolStripStrikethrough.Checked = false;

                if (rtb.SelectionAlignment == HorizontalAlignment.Left) this.toolStripAlignLeft.Checked = true;
                else this.toolStripAlignLeft.Checked = false;

                if (rtb.SelectionAlignment == HorizontalAlignment.Center) this.toolStripAlignCenter.Checked = true;
                else this.toolStripAlignCenter.Checked = false;

                if (rtb.SelectionAlignment == HorizontalAlignment.Right) this.toolStripAlignRight.Checked = true;
                else this.toolStripAlignRight.Checked = false;
            }
            catch
            {
            }
        }

        /* Event which watches the changes in the tab selections
         * Used for: Changing the main form title, for activating/deactivating 
         * tool strip buttons, etc;
         */
        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.tabControl.TabCount > 0)
            {
                this.Text = "RTFPad - " + this.tabControl.SelectedTab.Text;
                this.rtb_SelectionChanged(sender, e);
                this.menuEditPaste.Enabled          = true;
                this.toolStripPaste.Enabled         = true;
                this.menuFilePrint.Enabled          = true;
                this.toolStripPrint.Enabled         = true;
                this.menuFilePrintPreview.Enabled   = true;
                this.toolStripPrintPreview.Enabled  = true;
                this.menuFilePageSetup.Enabled      = true;
                this.menuViewWordWrap.Enabled       = true;

                if (!this.fileTypeInTab.ContainsKey(this.tabControl.SelectedTab.Text))
                {
                    this.fileTypeInTab[this.tabControl.SelectedTab.Text] = 1;
                }
                if (this.fileTypeInTab[this.tabControl.SelectedTab.Text] == 2)
                {
                    this.toolStripCBoxFont.Visible      = false;
                    this.toolStripCBoxFontSize.Visible  = false;
                    this.toolStripFontColor.Visible     = false;
                    this.toolStripBold.Visible          = false;
                    this.toolStripItalic.Visible        = false;
                    this.toolStripUnderline.Visible     = false;
                    this.toolStripStrikethrough.Visible = false;
                }
                else if (this.fileTypeInTab[this.tabControl.SelectedTab.Text] == 1)
                {
                    this.toolStripCBoxFont.Visible      = true;
                    this.toolStripCBoxFontSize.Visible  = true;
                    this.toolStripFontColor.Visible     = true;
                    this.toolStripBold.Visible          = true;
                    this.toolStripItalic.Visible        = true;
                    this.toolStripUnderline.Visible     = true;
                    this.toolStripStrikethrough.Visible = true;
                }
            }
            else
            {
                this.menuEditFind.Enabled           = false;
                this.toolStripFind.Enabled          = false;
                this.menuEditFindNext.Enabled       = false;
                this.menuEditReplace.Enabled        = false;
                this.menuEditSelectAll.Enabled      = false;
                this.menuEditUndo.Enabled           = false;
                this.toolStripUndo.Enabled          = false;
                this.menuEditRedo.Enabled           = false;
                this.toolStripRedo.Enabled          = false;
                this.menuEditClear.Enabled          = false;
                this.menuEditCut.Enabled            = false;
                this.toolStripCut.Enabled           = false;
                this.menuEditCopy.Enabled           = false;
                this.toolStripCopy.Enabled          = false;
                this.menuEditPaste.Enabled          = false;
                this.toolStripPaste.Enabled         = false;
                this.menuFilePrint.Enabled          = false;
                this.toolStripPrint.Enabled         = false;
                this.menuFilePrintPreview.Enabled   = false;
                this.toolStripPrintPreview.Enabled  = false;
                this.menuFilePageSetup.Enabled      = false;
                this.menuViewWordWrap.Enabled       = false;
                this.toolStripAlignLeft.Checked     = false;
                this.toolStripAlignCenter.Checked   = false;
                this.toolStripAlignRight.Checked    = false;
            }
        }

        /* Checks if the mouse is over the tab header
         * Used to display the context menu for tabs
         */
        private void tabControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                for (int i = 0; i < tabControl.TabCount; i++)
                {
                    Rectangle r = tabControl.GetTabRect(i);
                    if (r.Contains(e.Location))
                    {
                        Point pos = this.PointToClient(Cursor.Position);
                        if (this.tabControl.SelectedIndex != i) this.tabControl.SelectedIndex = i;
                        tabMenu.Show(this, pos, ToolStripDropDownDirection.Right);
                    }
                }
            }
        }

        /* Event which watches for the change in form size
         * Used for styling the Tool Strip so it's displayed in two rows when possible
         */
        private void rtfPadForm_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized || this.Size.Width > 900 )
            {
                toolStripSeparator5.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            }
            else
            {
                toolStripSeparator5.Margin = new System.Windows.Forms.Padding(0, 0, 500, 0);
            }
        }
        #endregion

        #region Tool Strip Mouse Over Events

        private void toolStripNew_MouseEnter(object sender, EventArgs e)
        {
            this.statusStripInfoLabel.Text = "Create a new Rich Text Format document";
        }

        private void toolStripOpen_MouseEnter(object sender, EventArgs e)
        {
            this.statusStripInfoLabel.Text = "Open a Rich Text Format or Plain Text file in a new tab";
        }

        private void toolStripSave_MouseEnter(object sender, EventArgs e)
        {
            this.statusStripInfoLabel.Text = "Save the active document";
        }

        private void toolStripPrint_MouseEnter(object sender, EventArgs e)
        {
            this.statusStripInfoLabel.Text = "Print the active document";
        }

        private void toolStripPrintPreview_MouseEnter(object sender, EventArgs e)
        {
            this.statusStripInfoLabel.Text = "Displays full pages";
        }

        private void toolStripFind_MouseEnter(object sender, EventArgs e)
        {
            this.statusStripInfoLabel.Text = "Finds the specified Text";
        }

        private void toolStripCut_MouseEnter(object sender, EventArgs e)
        {
            this.statusStripInfoLabel.Text = "Cuts the selection and puts it on the Clipboard";
        }

        private void toolStripCopy_MouseEnter(object sender, EventArgs e)
        {
            this.statusStripInfoLabel.Text = "Copies the selection and puts it on the Clipboard";
        }

        private void toolStripPaste_MouseEnter(object sender, EventArgs e)
        {
            this.statusStripInfoLabel.Text = "Inserts Clipboard contents";
        }

        private void toolStripUndo_MouseEnter(object sender, EventArgs e)
        {
            this.statusStripInfoLabel.Text = "Reverses the last action";
        }

        private void toolStripRedo_MouseEnter(object sender, EventArgs e)
        {
            this.statusStripInfoLabel.Text = "Redoes the next saved action";
        }

        private void toolStripBold_MouseEnter(object sender, EventArgs e)
        {
            this.statusStripInfoLabel.Text = "Makes the selection Bold(toggle)";
        }

        private void toolStripItalic_MouseEnter(object sender, EventArgs e)
        {
            this.statusStripInfoLabel.Text = "Makes the selection Italic(toggle)";
        }

        private void toolStripUnderline_MouseEnter(object sender, EventArgs e)
        {
            this.statusStripInfoLabel.Text = "Makes the selection Underlined(toggle)";
        }

        private void toolStripStrikethrough_MouseEnter(object sender, EventArgs e)
        {
            this.statusStripInfoLabel.Text = "Makes the selection Strikeout(toggle)";
        }

        private void toolStripFontColor_MouseEnter(object sender, EventArgs e)
        {
            this.statusStripInfoLabel.Text = "Formats the selection with a color";
        }

        private void toolStripNew_MouseLeave(object sender, EventArgs e)
        {
            this.statusStripInfoLabel.Text = "";
        }

        private void toolStripOpen_MouseLeave(object sender, EventArgs e)
        {
            this.statusStripInfoLabel.Text = "";
        }

        private void toolStripSave_MouseLeave(object sender, EventArgs e)
        {
            this.statusStripInfoLabel.Text = "";
        }

        private void toolStripPrint_MouseLeave(object sender, EventArgs e)
        {
            this.statusStripInfoLabel.Text = "";
        }

        private void toolStripPrintPreview_MouseLeave(object sender, EventArgs e)
        {
            this.statusStripInfoLabel.Text = "";
        }

        private void toolStripFind_MouseLeave(object sender, EventArgs e)
        {
            this.statusStripInfoLabel.Text = "";
        }

        private void toolStripCut_MouseLeave(object sender, EventArgs e)
        {
            this.statusStripInfoLabel.Text = "";
        }

        private void toolStripCopy_MouseLeave(object sender, EventArgs e)
        {
            this.statusStripInfoLabel.Text = "";
        }

        private void toolStripPaste_MouseLeave(object sender, EventArgs e)
        {
            this.statusStripInfoLabel.Text = "";
        }

        private void toolStripUndo_MouseLeave(object sender, EventArgs e)
        {
            this.statusStripInfoLabel.Text = "";
        }

        private void toolStripRedo_MouseLeave(object sender, EventArgs e)
        {
            this.statusStripInfoLabel.Text = "";
        }

        private void toolStripBold_MouseLeave(object sender, EventArgs e)
        {
            this.statusStripInfoLabel.Text = "";
        }

        private void toolStripItalic_MouseLeave(object sender, EventArgs e)
        {
            this.statusStripInfoLabel.Text = "";
        }

        private void toolStripUnderline_MouseLeave(object sender, EventArgs e)
        {
            this.statusStripInfoLabel.Text = "";
        }

        private void toolStripStrikethrough_MouseLeave(object sender, EventArgs e)
        {
            this.statusStripInfoLabel.Text = "";
        }

        private void toolStripFontColor_MouseLeave(object sender, EventArgs e)
        {
            this.statusStripInfoLabel.Text = "";
        }

        private void toolStripAlignLeft_MouseEnter(object sender, EventArgs e)
        {
            this.statusStripInfoLabel.Text = "Left-justifies selection";
        }

        private void toolStripAlignLeft_MouseLeave(object sender, EventArgs e)
        {
            this.statusStripInfoLabel.Text = "";
        }

        private void toolStripAlignCenter_MouseEnter(object sender, EventArgs e)
        {
            this.statusStripInfoLabel.Text = "Right-justifies selection";
        }

        private void toolStripAlignCenter_MouseLeave(object sender, EventArgs e)
        {
            this.statusStripInfoLabel.Text = "";
        }

        private void toolStripAlignRight_MouseEnter(object sender, EventArgs e)
        {
            this.statusStripInfoLabel.Text = "Right-justifies selection";
        }

        private void toolStripAlignRight_MouseLeave(object sender, EventArgs e)
        {
            this.statusStripInfoLabel.Text = "";
        }

        private void toolStripBullet_MouseEnter(object sender, EventArgs e)
        {
            this.statusStripInfoLabel.Text = "Inserts a bullet on this line";
        }

        private void toolStripBullet_MouseLeave(object sender, EventArgs e)
        {
            this.statusStripInfoLabel.Text = "";
        }
        #endregion
    }
}