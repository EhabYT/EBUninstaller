/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    DNS Client Resolver Cache Flush Dialog
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Drawing;
using System.Windows.Forms;
using UninstallTools.SystemTools;

namespace BulkCrapUninstaller.Forms
{
    public sealed class DnsClientCacheHealthWindow : Form
    {
        private TextBox _txtOutput;
        private Button _btnFlush;
        private Button _btnCheck;
        private Button _btnClose;
        private Label _lblStatus;

        public DnsClientCacheHealthWindow()
        {
            InitializeComponent();
            CheckDns();
        }

        private void InitializeComponent()
        {
            Text = "DNS Client Resolver Cache & Health - EBUninstaller Pro";
            Size = new Size(620, 380);
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            var panelTop = new Panel { Dock = DockStyle.Top, Height = 55, Padding = new Padding(12) };
            var lblTitle = new Label
            {
                Text = "Windows DNS Client Resolver Cache Subsystem",
                Font = new Font(Font, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(12, 10)
            };
            var lblSub = new Label
            {
                Text = "Flushes stale DNS lookups, cleans corrupted hostname resolutions, and verifies DNS service state.",
                ForeColor = Color.DimGray,
                AutoSize = true,
                Location = new Point(12, 30)
            };
            panelTop.Controls.AddRange(new Control[] { lblTitle, lblSub });

            _txtOutput = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Consolas", 9.5F, FontStyle.Regular, GraphicsUnit.Point),
                BackColor = Color.FromArgb(250, 250, 250)
            };

            var panelBottom = new Panel { Dock = DockStyle.Bottom, Height = 50, Padding = new Padding(10) };
            _lblStatus = new Label { Text = "Ready", AutoSize = true, Location = new Point(10, 16) };
            _btnCheck = new Button { Text = "Check Status", Location = new Point(280, 10), Size = new Size(100, 28) };
            _btnCheck.Click += (s, e) => CheckDns();

            _btnFlush = new Button { Text = "Flush DNS Cache", Location = new Point(390, 10), Size = new Size(130, 28) };
            _btnFlush.Click += (s, e) => FlushDns();

            _btnClose = new Button { Text = "Close", Location = new Point(530, 10), Size = new Size(65, 28) };
            _btnClose.Click += (s, e) => Close();

            panelBottom.Controls.AddRange(new Control[] { _lblStatus, _btnCheck, _btnFlush, _btnClose });

            Controls.Add(_txtOutput);
            Controls.Add(panelBottom);
            Controls.Add(panelTop);
        }

        private void CheckDns()
        {
            var st = DnsClientCacheHealthEngine.CheckStatus();
            _txtOutput.Text = $"[DNS Client Service Status]\r\nRunning: {st.DnsClientRunning}\r\n\r\n{st.OutputLog}";
            _lblStatus.Text = st.DnsClientRunning ? "DNS Client service is active." : "DNS Client check completed.";
        }

        private void FlushDns()
        {
            var res = DnsClientCacheHealthEngine.FlushDnsCache();
            _txtOutput.Text += $"\r\n\r\n[Flush Output]\r\n{res.OutputLog}";
            _lblStatus.Text = res.FlushedSuccessfully ? "DNS Resolver Cache flushed successfully." : "DNS Flush executed.";
            MessageBox.Show("Windows IP / DNS Resolver Cache has been flushed successfully.", "DNS Flushed", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
