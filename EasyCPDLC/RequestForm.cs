﻿/*  EASYCPDLC: CPDLC Client for the VATSIM Network
    Copyright (C) 2021 Joshua Seagrave joshseagrave@googlemail.com

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/


using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;

namespace EasyCPDLC
{
    public partial class RequestForm : Form
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        private const int cGrip = 16;
        private const int cCaption = 32;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private ContextMenuStrip popupMenu = new ContextMenuStrip();
        private ToolStripMenuItem directRequestMenu;// = new ToolStripMenuItem();
        private ToolStripMenuItem levelRequestMenu;// = new ToolStripMenuItem();
        private ToolStripMenuItem speedRequestMenu;// = new ToolStripMenuItem();
        private ToolStripMenuItem whenCanWeRequestMenu;// = new ToolStripMenuItem();
        private Label dummyLabel;

        private MainForm parent;
        private PilotData userVATSIMData;
        private Color controlBackColor;
        private Color controlFrontColor;

        private Dictionary<string, string> rsnConversion = new Dictionary<string, string>();

        private Font controlFont;
        private Font controlFontBold;
        private Font textFont;
        private Font textFontBold;

        private bool _needsLogon;
        public bool needsLogon
        {
            get
            {
                return this._needsLogon;
            }

            set
            {
                this._needsLogon = value;

                if (this._needsLogon)
                {
                    logonButton.Text = "LOGON";
                    requestButton.Enabled = false;
                    reportButton.Enabled = false;
                }
                else
                {
                    logonButton.Text = "LOGOFF";
                    requestButton.Enabled = true;
                    reportButton.Enabled = true;
                }
            }
        }

        public RequestForm(MainForm parent)
        {
            InitializeComponent();
            this.parent = parent;
            this.TopMost = parent.TopMost;

            if (this.parent.currentATCUnit != null)
            {
                needsLogon = false;
            }
            else
            {
                needsLogon = true;
            }

            userVATSIMData = parent.userVATSIMData;
            controlBackColor = parent.controlBackColor;
            controlFrontColor = parent.controlFrontColor;
            controlFont = parent.controlFont;
            controlFontBold = new Font("Oxygen", 12.5F, FontStyle.Bold);
            textFont = parent.textFont;
            textFontBold = parent.textFontBold;

            dummyLabel = new Label();
            dummyLabel.Width = 0;
            dummyLabel.Height = 0;
            dummyLabel.Margin = new Padding(0, 0, 0, 0);

            rsnConversion.Add("DUE TO WX", "DUE TO WEATHER");
            rsnConversion.Add("DUE TO A/C PERFORMANCE", "DUE TO PERFORMANCE");

            InitialisePopupMenu();

        }

        private ToolStripMenuItem CreateMenuItem(string name)
        {
            ToolStripMenuItem _temp = new ToolStripMenuItem(name);
            _temp.BackColor = Color.FromArgb(28, 28, 28);
            _temp.ForeColor = controlFrontColor;
            _temp.Font = controlFontBold;
            //_temp.AutoSize = false;
            //_temp.Size = new Size(104, 37);

            return _temp;
        }

        private void InitialisePopupMenu()
        {

            popupMenu.BackColor = controlBackColor;
            popupMenu.ForeColor = controlFrontColor;
            popupMenu.Font = controlFontBold;
            popupMenu.ShowImageMargin = false;

            directRequestMenu = CreateMenuItem("DIRECT");
            directRequestMenu.Click += DirectRequestClick;
            levelRequestMenu = CreateMenuItem("LEVEL");
            levelRequestMenu.Click += LevelRequestClick;
            speedRequestMenu = CreateMenuItem("SPEED");
            speedRequestMenu.Click += SpeedRequestClick;
            whenCanWeRequestMenu = CreateMenuItem("WHEN CAN WE?");
            whenCanWeRequestMenu.Click += WhenCanWeRequestClick;
        }

        private void DirectRequestClick(object sender, EventArgs e)
        {
            directRadioButton.Checked = true;

            messageFormatPanel.Controls.Clear();
            messageFormatPanel.Controls.Add(createTemplate("RECIPIENT:"));
            messageFormatPanel.Controls.Add(createTextBox(parent.currentATCUnit, 4, true));
            messageFormatPanel.SetFlowBreak(messageFormatPanel.Controls[messageFormatPanel.Controls.Count - 1], true);
            messageFormatPanel.Controls.Add(createTemplate("REQUEST DIRECT TO "));
            messageFormatPanel.Controls.Add(createTextBox("", 5));
            messageFormatPanel.Controls.Add(dummyLabel);
            messageFormatPanel.SetFlowBreak(messageFormatPanel.Controls[messageFormatPanel.Controls.Count - 1], true);
            messageFormatPanel.Controls.Add(createCheckBox("DUE TO WX", "rsnParam"));
            messageFormatPanel.Controls.Add(createTemplate("   "));
            messageFormatPanel.Controls.Add(createCheckBox("DUE TO A/C PERFORMANCE", "rsnParam"));
            

        }

        private void LevelRequestClick(object sender, EventArgs e)
        {
            levelRadioButton.Checked = true;

            messageFormatPanel.Controls.Clear();
            messageFormatPanel.Controls.Add(createTemplate("RECIPIENT:"));
            messageFormatPanel.Controls.Add(createTextBox(parent.currentATCUnit, 4, true));
            messageFormatPanel.SetFlowBreak(messageFormatPanel.Controls[messageFormatPanel.Controls.Count - 1], true);
            messageFormatPanel.Controls.Add(createTemplate("REQUEST FL"));
            messageFormatPanel.Controls.Add(createTextBox("", 3, false, true));
            messageFormatPanel.SetFlowBreak(messageFormatPanel.Controls[messageFormatPanel.Controls.Count - 1], true);
            messageFormatPanel.Controls.Add(createCheckBox("DUE TO WX", "rsnParam"));
            messageFormatPanel.Controls.Add(createTemplate("   "));
            messageFormatPanel.Controls.Add(createCheckBox("DUE TO A/C PERFORMANCE", "rsnParam"));
        }

        private void SpeedRequestClick(object sender, EventArgs e)
        {
            speedRadioButton.Checked = true;

            messageFormatPanel.Controls.Clear();
            messageFormatPanel.Controls.Add(createTemplate("RECIPIENT:"));
            messageFormatPanel.Controls.Add(createTextBox(parent.currentATCUnit, 4, true));
            messageFormatPanel.SetFlowBreak(messageFormatPanel.Controls[messageFormatPanel.Controls.Count - 1], true);
            messageFormatPanel.Controls.Add(createTemplate("REQUEST"));
            messageFormatPanel.SetFlowBreak(messageFormatPanel.Controls[messageFormatPanel.Controls.Count - 1], false);
            messageFormatPanel.Controls.Add(createCheckBox("MACH: M0.", "unitParam"));
            messageFormatPanel.Controls.Add(createTextBox("", 2, false, true));
            messageFormatPanel.Controls.Add(createTemplate("   "));
            messageFormatPanel.Controls.Add(createCheckBox("SPEED: ", "unitParam"));
            messageFormatPanel.Controls.Add(createTextBox("", 3, false, true));
            messageFormatPanel.Controls.Add(createTemplate("KTS"));
            messageFormatPanel.SetFlowBreak(messageFormatPanel.Controls[messageFormatPanel.Controls.Count - 1], true);
            messageFormatPanel.Controls.Add(createCheckBox("DUE TO WX", "rsnParam"));
            messageFormatPanel.Controls.Add(createTemplate("   "));
            messageFormatPanel.Controls.Add(createCheckBox("DUE TO A/C PERFORMANCE", "rsnParam"));
        }

        private void WhenCanWeRequestClick(object sender, EventArgs e)
        {
            wcwRadioButton.Checked = true;

            messageFormatPanel.Controls.Clear();
            messageFormatPanel.Controls.Add(createTemplate("RECIPIENT:"));
            messageFormatPanel.Controls.Add(createTextBox(parent.currentATCUnit, 4, true));
            messageFormatPanel.SetFlowBreak(messageFormatPanel.Controls[messageFormatPanel.Controls.Count - 1], true);
            messageFormatPanel.Controls.Add(createTemplate("WHEN CAN WE EXPECT:"));
            messageFormatPanel.SetFlowBreak(messageFormatPanel.Controls[messageFormatPanel.Controls.Count - 1], true);
            messageFormatPanel.Controls.Add(createCheckBox("HIGHER LEVEL?", "wcwParam"));
            messageFormatPanel.Controls.Add(createTemplate("   "));
            messageFormatPanel.Controls.Add(createCheckBox("LOWER LEVEL?", "wcwParam"));
            messageFormatPanel.Controls.Add(createTemplate("   "));
            messageFormatPanel.Controls.Add(createCheckBox("BACK ON ROUTE?", "wcwParam"));
            messageFormatPanel.SetFlowBreak(messageFormatPanel.Controls[messageFormatPanel.Controls.Count - 1], true);
            messageFormatPanel.Controls.Add(createCheckBox("CLIMB TO: FL", "wcwParam"));
            messageFormatPanel.Controls.Add(createTextBox("", 3, false, true));
            messageFormatPanel.Controls.Add(createTemplate("   "));
            messageFormatPanel.Controls.Add(createCheckBox("DESCENT TO: FL", "wcwParam"));
            messageFormatPanel.Controls.Add(createTextBox("", 3, false, true));
            messageFormatPanel.SetFlowBreak(messageFormatPanel.Controls[messageFormatPanel.Controls.Count - 1], true);
            messageFormatPanel.Controls.Add(createCheckBox("SPEED: ", "wcwParam"));
            messageFormatPanel.Controls.Add(createTextBox("", 3, false, true));
            messageFormatPanel.Controls.Add(createTemplate("KTS"));
            messageFormatPanel.Controls.Add(createTemplate("   "));
            messageFormatPanel.Controls.Add(createCheckBox("MACH: M0.", "wcwParam"));
            messageFormatPanel.Controls.Add(createTextBox("", 2, false, true));
            messageFormatPanel.SetFlowBreak(messageFormatPanel.Controls[messageFormatPanel.Controls.Count - 1], true);
        }

            private void pdcButton_Click(object sender, EventArgs e)
        {
            messageFormatPanel.Controls.Clear();
            messageFormatPanel.Controls.Add(createTemplate("RECIPIENT:"));
            messageFormatPanel.Controls.Add(createTextBox(userVATSIMData.flight_plan.departure, 4));
            messageFormatPanel.SetFlowBreak(messageFormatPanel.Controls[messageFormatPanel.Controls.Count - 1], true);
            messageFormatPanel.Controls.Add(createTemplate("REQUEST PREDEP CLEARANCE"));
            messageFormatPanel.Controls.Add(createTextBox(userVATSIMData.callsign, 7));
            messageFormatPanel.Controls.Add(createTextBox(userVATSIMData.flight_plan.aircraft_short, 4));
            messageFormatPanel.SetFlowBreak(messageFormatPanel.Controls[messageFormatPanel.Controls.Count - 1], true);
            messageFormatPanel.Controls.Add(createTemplate("TO"));
            messageFormatPanel.Controls.Add(createTextBox(userVATSIMData.flight_plan.arrival, 4));
            messageFormatPanel.Controls.Add(createTemplate("AT"));
            messageFormatPanel.Controls.Add(createTextBox(userVATSIMData.flight_plan.departure, 4));
            messageFormatPanel.SetFlowBreak(messageFormatPanel.Controls[messageFormatPanel.Controls.Count - 1], true);
            messageFormatPanel.Controls.Add(createTemplate("STAND"));
            messageFormatPanel.Controls.Add(createTextBox("", 4));
            messageFormatPanel.Controls.Add(createTemplate("ATIS"));
            messageFormatPanel.Controls.Add(createTextBox("", 1));
            messageFormatPanel.SetFlowBreak(messageFormatPanel.Controls[messageFormatPanel.Controls.Count - 1], true);
            messageFormatPanel.Controls.Add(CreateMultiLineBox(""));

            pdcRadioButton.Checked = true;
        }

        private void reportButton_Click(object sender, EventArgs e)
        {
            reportRadioButton.Checked = true;
            messageFormatPanel.Controls.Clear();
            messageFormatPanel.Controls.Add(createTemplate("RECIPIENT:"));
            messageFormatPanel.Controls.Add(createTextBox(parent.currentATCUnit, 4, true));
            messageFormatPanel.SetFlowBreak(messageFormatPanel.Controls[messageFormatPanel.Controls.Count - 1], true);
            messageFormatPanel.Controls.Add(createTemplate("POSITION REPORT"));
            messageFormatPanel.SetFlowBreak(messageFormatPanel.Controls[messageFormatPanel.Controls.Count - 1], true);
            messageFormatPanel.Controls.Add(createTemplate("OVERHEAD"));
            messageFormatPanel.Controls.Add(createTextBox("", 7));
            messageFormatPanel.Controls.Add(createTemplate("AT"));
            messageFormatPanel.Controls.Add(createTextBox(DateTime.UtcNow.ToString("HHmm"), 4));
            messageFormatPanel.Controls.Add(createTemplate("Z"));
            messageFormatPanel.Controls.Add(createTemplate("FL"));
            messageFormatPanel.Controls.Add(createTextBox(userVATSIMData.flight_plan.altitude.Substring(0, 3), 3));
            messageFormatPanel.SetFlowBreak(messageFormatPanel.Controls[messageFormatPanel.Controls.Count - 1], true);
            messageFormatPanel.Controls.Add(createTemplate("NEXT"));
            messageFormatPanel.Controls.Add(createTextBox("", 7));
            messageFormatPanel.Controls.Add(createTemplate("AT"));
            messageFormatPanel.Controls.Add(createTextBox("", 4));
            messageFormatPanel.Controls.Add(createTemplate("Z"));
            messageFormatPanel.SetFlowBreak(messageFormatPanel.Controls[messageFormatPanel.Controls.Count - 1], true);
            messageFormatPanel.Controls.Add(createTemplate("THEN"));
            messageFormatPanel.Controls.Add(createTextBox("", 7));

        }

        private void logonButton_Click(object sender, EventArgs e)
        {
            messageFormatPanel.Controls.Clear();
            messageFormatPanel.Controls.Add(createTemplate("ATC UNIT:"));
            messageFormatPanel.Controls.Add(createTextBox(needsLogon ? "" : parent.currentATCUnit, 4));

            logonRadioButton.Checked = true;
        }

        private Label createTemplate(string _text)
        {
            Label _temp = new Label();
            _temp.BackColor = controlBackColor;
            _temp.ForeColor = controlFrontColor;
            _temp.Font = textFont;
            _temp.AutoSize = true;
            _temp.Text = _text;
            _temp.Top = 10;
            _temp.Height = 20;
            _temp.TextAlign = ContentAlignment.MiddleLeft;
            _temp.Padding = new Padding(0, 10, 0, 0);
            _temp.Margin = new Padding(0, 0, 0, 0);

            return _temp;
        }

        private UITextBox createTextBox(string _text, int _maxLength, bool _readOnly = false, bool _numsOnly = false)
        {
            UITextBox _temp = new UITextBox(controlFrontColor);

            _temp.BackColor = controlBackColor;
            _temp.ForeColor = controlFrontColor;
            _temp.Font = textFontBold;
            _temp.MaxLength = _maxLength;
            _temp.BorderStyle = BorderStyle.None;
            _temp.Text = _text;
            _temp.CharacterCasing = CharacterCasing.Upper;
            _temp.Top = 10;
            _temp.Padding = new Padding(3, 0, 3, -10);
            //_temp.Margin = new Padding(3, 5, 3, -10);
            _temp.Height = 20;
            _temp.ReadOnly = _readOnly;
            _temp.TextAlign = HorizontalAlignment.Center;

            if (_numsOnly)
            {
                _temp.KeyPress += NumsOnly;
            }

            using (Graphics G = _temp.CreateGraphics())
            {
                _temp.Width = (int)(_temp.MaxLength *
                              G.MeasureString("x", _temp.Font).Width);
            }

            return _temp;
        }

        private void NumsOnly(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private UICheckBox createCheckBox(string _text, string _group)
        {
            UICheckBox _temp = new UICheckBox(_group);

            _temp.BackColor = controlBackColor;
            _temp.ForeColor = controlFrontColor;
            _temp.Font = textFont;
            _temp.Text = _text;
            _temp.Padding = new Padding(3, 10, 3, -30);
            _temp.AutoSize = true;
            _temp.Click += DeselectCheckBox;
            return _temp;
        }

        private void DeselectCheckBox(object sender, EventArgs e)
        {
            UICheckBox _sender = (UICheckBox)sender;

            foreach (UICheckBox box in messageFormatPanel.Controls.OfType<UICheckBox>())
            {
                if (box.Text != _sender.Text && box.group == _sender.group)
                {
                    box.Checked = false;
                }
            }

        }

        private UITextBox CreateMultiLineBox(string _text)
        {
            UITextBox _temp = new UITextBox(controlFrontColor);
            _temp.BackColor = controlBackColor;
            _temp.ForeColor = controlFrontColor;
            _temp.Font = textFontBold;
            _temp.BorderStyle = BorderStyle.None;
            _temp.Width = messageFormatPanel.Width - 50;
            _temp.Multiline = true;
            _temp.WordWrap = true;
            _temp.Text = _text;
            _temp.MaxLength = 255;
            _temp.Height = 20;
            _temp.TextChanged += ExpandMultiLineBox;

            _temp.CharacterCasing = CharacterCasing.Upper;
            _temp.Padding = new Padding(3, 0, 3, -10);
            _temp.Margin = new Padding(3, 5, 3, -10);
            _temp.TextAlign = HorizontalAlignment.Left;

            return _temp;
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            messageFormatPanel.Controls.Clear();
        }

        private async void sendButton_Click(object sender, EventArgs e)
        {

            RadioButton radioBtn = radioContainer.Controls.OfType<RadioButton>()
                                       .Where(x => x.Checked).FirstOrDefault();

            if (radioBtn != null)
            {
                string _recipient = "";
                string _formatMessage = "";

                switch (radioBtn.Name)
                {

                    case "pdcRadioButton":

                        for (int i = 0; i < messageFormatPanel.Controls.Count - 2; i++)
                        {
                            if (messageFormatPanel.Controls[i].Text.Length < 1)
                            {
                                Console.WriteLine(i);
                                return;
                            }
                        }

                        _recipient = messageFormatPanel.Controls[1].Text;

                        for (int i = 2; i < messageFormatPanel.Controls.Count; i++)
                        {
                            _formatMessage += messageFormatPanel.Controls[i].Text + " ";
                        }
                        await parent.SendCPDLCMessage(_recipient, "TELEX", _formatMessage.Trim());
                        break;

                    case "logonRadioButton":

                        foreach (Control _control in messageFormatPanel.Controls)
                        {
                            if (_control.Text.Length < 1)
                            {
                                return;
                            }
                        }

                        _recipient = messageFormatPanel.Controls[1].Text;
                        if (needsLogon)
                        {
                            _formatMessage = String.Format("/data2/{0}//Y/REQUEST LOGON", parent.messageOutCounter);
                            parent.pendingLogon = _recipient;
                        }
                        else
                        {
                            _formatMessage = String.Format("/data2/{0}//N/LOGOFF", parent.messageOutCounter);
                            parent.currentATCUnit = null;

                        }
                        await parent.SendCPDLCMessage(_recipient, "CPDLC", _formatMessage);
                        parent.messageOutCounter += 1;

                        break;

                    case "requestRadioButton":

                        /*foreach (Control _control in messageFormatPanel.Controls)
                        {
                            if (_control.Text.Length < 1)
                            {
                                return;
                            }
                        }*/

                        _formatMessage = String.Format("/data2/{0}//Y/", parent.messageOutCounter);
                        _recipient = messageFormatPanel.Controls[1].Text;
                        string parsedMessage = ParseRequest();

                        if (parsedMessage == "")
                        {
                            parent.WriteMessage("ERROR PARSING CPDLC MESSAGE. NO MESSAGE SENT", "SYSTEM", "SYSTEM");
                            break;
                        }

                        _formatMessage += parsedMessage;

                        await parent.SendCPDLCMessage(_recipient, "CPDLC", _formatMessage);
                        parent.messageOutCounter += 1;

                        break;

                    case "reportRadioButton":

                        _formatMessage = String.Format("/data2/{0}//N/", parent.messageOutCounter);
                        _recipient = messageFormatPanel.Controls[1].Text;
                        string _messageContent = String.Format("POSITION REPORT PPOS {0} AT {1}Z FL{2} TO {3} AT {4}Z NEXT {5}",
                            messageFormatPanel.Controls[4].Text,
                            messageFormatPanel.Controls[6].Text,
                            messageFormatPanel.Controls[9].Text,
                            messageFormatPanel.Controls[11].Text,
                            messageFormatPanel.Controls[13].Text,
                            messageFormatPanel.Controls[16].Text);
                        _formatMessage += _messageContent;

                        await parent.SendCPDLCMessage(_recipient, "CPDLC", _formatMessage);
                        parent.messageOutCounter += 1;
                        break;



                    default:
                        break;



                }

                this.Close();
            }
            else
            {

            }
        }

        private string ParseRequest()
        {
            RadioButton radioBtn = requestContainer.Controls.OfType<RadioButton>()
                                      .Where(x => x.Checked).FirstOrDefault();

            UICheckBox dueToBox = messageFormatPanel.Controls.OfType<UICheckBox>()
                                   .Where(x => x.Checked && x.group == "rsnParam").FirstOrDefault();

            UICheckBox unitBox = messageFormatPanel.Controls.OfType<UICheckBox>()
                                   .Where(x => x.Checked && x.group == "unitParam").FirstOrDefault();

            UICheckBox wcwBox = messageFormatPanel.Controls.OfType<UICheckBox>()
                                   .Where(x => x.Checked && x.group == "wcwParam").FirstOrDefault();

            string _request = "";

            switch (radioBtn.Name)
            {
                case "levelRadioButton":

                    if (messageFormatPanel.Controls[3].Text == "")
                    {
                        return string.Empty;
                    }
                    _request = "REQUEST FL";
                    _request += messageFormatPanel.Controls[3].Text;                    

                    dueToBox = messageFormatPanel.Controls.OfType<UICheckBox>()
                                   .Where(x => x.Checked && x.group == "rsnParam").FirstOrDefault();
                   
                    if (dueToBox != default(UICheckBox))
                    {
                        _request += " " + rsnConversion[dueToBox.Text];
                    }
                    break;

                case "directRadioButton":

                    if (messageFormatPanel.Controls[3].Text == "")
                    {
                        return string.Empty;
                    }

                    _request = "REQUEST DIRECT TO ";
                    _request += messageFormatPanel.Controls[3].Text;

                    dueToBox = messageFormatPanel.Controls.OfType<UICheckBox>()
                                   .Where(x => x.Checked && x.group == "rsnParam").FirstOrDefault();

                    if (dueToBox != default(UICheckBox))
                    {
                        _request += " " + rsnConversion[dueToBox.Text];
                    }
                    break;

                case "speedRadioButton":

                    if(messageFormatPanel.Controls[messageFormatPanel.Controls.IndexOf(unitBox) + 1].Text == "")
                    {
                        return string.Empty;
                    }

                    _request += "REQUEST ";
                    if (unitBox != default(UICheckBox))
                    {
                        if(unitBox.Text == "MACH: M0.")
                        {
                            _request += "M" + messageFormatPanel.Controls[messageFormatPanel.Controls.IndexOf(unitBox) + 1].Text;
                        }
                        else
                        {
                            _request += messageFormatPanel.Controls[messageFormatPanel.Controls.IndexOf(unitBox) + 1].Text + "K";
                        }
                    }
                    else
                    {
                        return string.Empty;
                    }

                    dueToBox = messageFormatPanel.Controls.OfType<UICheckBox>()
                                   .Where(x => x.Checked && x.group == "rsnParam").FirstOrDefault();

                    if (dueToBox != default(UICheckBox))
                    {
                        _request += " " + rsnConversion[dueToBox.Text];
                    }
                    break;

                case "wcwRadioButton":

                    if(wcwBox is null)
                    {
                        return string.Empty;
                    }

                    _request = "WHEN CAN WE EXPECT ";

                    switch (wcwBox.Text)
                    {
                        case "HIGHER LEVEL?":
                            _request += "HIGHER LEVEL";
                            break;

                        case "LOWER LEVEL?":
                            _request += "LOWER LEVEL";
                            break;

                        case "BACK ON ROUTE?":
                            _request += "BACK ON ROUTE";
                            break;

                        case "CLIMB TO: FL":
                            _request += "CLIMB TO FL" + messageFormatPanel.Controls[messageFormatPanel.Controls.IndexOf(wcwBox) + 1];
                            break;

                        case "DESCENT TO: FL":
                            _request += "DESCENT TO FL" + messageFormatPanel.Controls[messageFormatPanel.Controls.IndexOf(wcwBox) + 1];
                            break;

                        case "MACH: M0.":
                            _request += "M" + messageFormatPanel.Controls[messageFormatPanel.Controls.IndexOf(wcwBox) + 1];
                            break;

                        case "SPEED: ":
                            _request += messageFormatPanel.Controls[messageFormatPanel.Controls.IndexOf(wcwBox) + 1] + "K";
                            break;

                        default:
                            break;
                    }

                    break;

                default:
                    break;
            }

            return _request;
        }

        private void WindowDrag(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void ExpandMultiLineBox(object sender, EventArgs e)
        {
            TextBox _sender = (TextBox)sender;
            // amount of padding to add
            const int padding = 3;
            // get number of lines (first line is 0, so add 1)
            int numLines = _sender.GetLineFromCharIndex(_sender.TextLength) + 1;
            // get border thickness
            int border = _sender.Height - _sender.ClientSize.Height;
            // set height (height of one line * number of lines + spacing)
            _sender.Height = _sender.Font.Height * numLines + padding + border;

            ScrollToBottom(messageFormatPanel);
        }

        private void ScrollToBottom(FlowLayoutPanel p)
        {
            using (Control c = new Control() { Parent = p, Dock = DockStyle.Bottom })
            {
                p.ScrollControlIntoView(c);
                c.Parent = null;
            }
        }

        private void requestButton_Click(object sender, EventArgs e)
        {
            requestRadioButton.Checked = true;

            popupMenu.Items.Clear();
            popupMenu.Items.Add(directRequestMenu);
            popupMenu.Items.Add(levelRequestMenu);
            popupMenu.Items.Add(speedRequestMenu);
            popupMenu.Items.Add(whenCanWeRequestMenu);
            //popupMenu.AutoSize = false;
            //popupMenu.Size = new Size(104, 114);
            popupMenu.Show(requestButton, new Point(0, requestButton.Height));
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x84)
            {  // Trap WM_NCHITTEST
                Point pos = new Point(m.LParam.ToInt32());
                pos = this.PointToClient(pos);
                if (pos.Y < cCaption)
                {
                    m.Result = (IntPtr)2;  // HTCAPTION
                    return;
                }
                if (pos.X >= this.ClientSize.Width - cGrip && pos.Y >= this.ClientSize.Height - cGrip)
                {
                    m.Result = (IntPtr)17; // HTBOTTOMRIGHT
                    return;
                }
            }
            base.WndProc(ref m);
        }

        
    }
}
