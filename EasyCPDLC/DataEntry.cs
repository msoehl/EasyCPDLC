﻿using System;
using System.Windows.Forms;

namespace EasyCPDLC
{
    public partial class DataEntry : Form
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        public string hoppieLogonCode { get; set; }
        public int vatsimCID { get; set; }
        public bool remember { get; set; }
        public DataEntry(object _hoppieLogonCode = null, object _vatsimCID = null)
        {
            InitializeComponent();

            try
            {
                if (!(_hoppieLogonCode is null))
                {
                    hoppieCodeTextBox.Text = _hoppieLogonCode.ToString();
                }
                else
                {
                    throw new Exception();
                }
                if (!(_vatsimCID is null))
                {
                    vatsimCIDTextBox.Text = _vatsimCID.ToString();
                }
                else
                {
                    throw new Exception();
                }
                rememberCheckBox.Checked = true;
            }

            catch (Exception)
            {
                hoppieCodeTextBox.Text = "";
                vatsimCIDTextBox.Text = "";
                rememberCheckBox.Checked = false;
            }
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            try
            {
                hoppieLogonCode = hoppieCodeTextBox.Text;
                vatsimCID = Convert.ToInt32(vatsimCIDTextBox.Text);
                remember = rememberCheckBox.Checked;

                this.DialogResult = DialogResult.OK;
            }
            catch (FormatException)
            {
                MessageBox.Show("Invalid CID/Code, please check and try again.", "Error!", MessageBoxButtons.OK);
            }

        }

        private void vatsimCIDTextBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (vatsimCIDTextBox.Text.Length < 1 || hoppieCodeTextBox.Text.Length < 1)
                {
                    throw new FormatException();
                }

                Convert.ToInt32(vatsimCIDTextBox.Text);
                connectButton.Enabled = true;
            }
            catch (FormatException)
            {
                connectButton.Enabled = false;
            }
        }

        private void hoppieCodeTextBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (vatsimCIDTextBox.Text.Length < 1 || hoppieCodeTextBox.Text.Length < 1)
                {
                    throw new FormatException();
                }
                else
                {
                    connectButton.Enabled = true;
                }
            }
            catch (FormatException)
            {
                connectButton.Enabled = false;
            }
        }

        private void DataEntry_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
    }
}