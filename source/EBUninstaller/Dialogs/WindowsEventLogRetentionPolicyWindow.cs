/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Windows Event Log Retention & Channel Health Dialog
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Drawing;
using System.Windows.Forms;
using UninstallTools.SystemTools;

namespace BulkCrapUninstaller.Forms
{
    public sealed class WindowsEventLogRetentionPolicyWindow : Form
    {
        private ListView _listView;
        private Button _btnRefresh;
        private Button _btnClearSelected;
        private Button _btnClose;
        private Label _lblStatus;

        public WindowsEventLogRetentionPolicyWindow()
        {
            InitializeComponent();
            PerformScan();
        }

        private void InitializeComponent()
        {
            Text = "Windows Event Log Channels & Retention Policy - EBUninstaller Pro";
            Size = new Size(760, 440);
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            var panelTop = new Panel { Dock = DockStyle.Top, Height = 55, Padding = new Padding(12) };
            var lblTitle = new Label
            {
                Text = "Windows Event Log Channel Health & Retention Manager",
                Font = new Font(Font, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(12, 10)
            };
            var lblSub = new Label
            {
                Text = "Audits system event log maximum sizes, circular buffer retention rules, and manages log channels.",
                ForeColor = Color.DimGray,
                AutoSize = true,
                Location = new Point(12, 30)
            };
            panelTop.Controls.AddRange(new Control[] { lblTitle, lblSub });

            _listView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true
            };
            _listView.Columns.Add("Log Channel Name", 300);
            _listView.Columns.Add("Max Size (KB)", 120);
            _listView.Columns.Add("Retention Policy", 180);
            _listView.Columns.Add("State", 100);

            var panelBottom = new Panel { Dock = DockStyle.Bottom, Height = 50, Padding = new Padding(10) };
            _lblStatus = new Label { Text = "Ready", AutoSize = true, Location = new Point(10, 16) };
            _btnRefresh = new Button { Text = "Refresh", Location = new Point(410, 10), Size = new Size(80, 28) };
            _btnRefresh.Click += (s, e) => PerformScan();

            _btnClearSelected = new Button { Text = "Clear Selected Log", Location = new Point(495, 10), Size = new Size(150, 28) };
            _btnClearSelected.Click += (s, e) => ClearSelected();

            _btnClose = new Button { Text = "Close", Location = new Point(650, 10), Size = new Size(65, 28) };
            _btnClose.Click += (s, e) => Close();

            panelBottom.Controls.AddRange(new Control[] { _lblStatus, _btnRefresh, _btnClearSelected, _btnClose });

            Controls.Add(_listView);
            Controls.Add(panelBottom);
            Controls.Add(panelTop);
        }

        private void PerformScan()
        {
            _listView.Items.Clear();
            var res = WindowsEventLogRetentionPolicyEngine.AuditEventLogChannels();
            foreach (var ch in res.Channels)
            {
                var lvi = new ListViewItem(ch.ChannelName);
                lvi.SubItems.Add(ch.MaxSizeKilobytes.ToString("N0"));
                lvi.SubItems.Add(ch.RetentionDescription);
                lvi.SubItems.Add(ch.IsEnabled ? "Enabled" : "Disabled");
                _listView.Items.Add(lvi);
            }
            _lblStatus.Text = $"Audited {res.TotalChannelsScanned} core event log channels.";
        }

        private void ClearSelected()
        {
            if (_listView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select an event log channel to clear.", "Select Channel", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var channelName = _listView.SelectedItems[0].Text;
            if (MessageBox.Show($"Are you sure you want to clear log channel '{channelName}'?", "Confirm Clear Log", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                var success = WindowsEventLogRetentionPolicyEngine.ClearEventLogChannel(channelName);
                MessageBox.Show(success ? $"Channel '{channelName}' cleared successfully." : $"Clear command issued for '{channelName}'.", "Event Log Updated", MessageBoxButtons.OK, MessageBoxIcon.Information);
                PerformScan();
            }
        }
    }
}
