/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Connected User Experiences & Telemetry (DiagTrack) Hardening Dialog
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Drawing;
using System.Windows.Forms;
using UninstallTools.SecurityHardening;

namespace BulkCrapUninstaller.Forms
{
    public sealed class WindowsTelemetryDiagTrackWindow : Form
    {
        private ListView _listView;
        private Button _btnRefresh;
        private Button _btnDisable;
        private Button _btnRestore;
        private Button _btnClose;
        private Label _lblStatus;

        public WindowsTelemetryDiagTrackWindow()
        {
            InitializeComponent();
            RefreshServices();
        }

        private void InitializeComponent()
        {
            Text = "Diagnostic Data & Telemetry (DiagTrack) Hardening - EBUninstaller Pro";
            Size = new Size(740, 430);
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            var panelTop = new Panel { Dock = DockStyle.Top, Height = 55, Padding = new Padding(12) };
            var lblTitle = new Label
            {
                Text = "Windows Telemetry & Diagnostic Data Collection Hardening",
                Font = new Font(Font, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(12, 10)
            };
            var lblSub = new Label
            {
                Text = "Manage Connected User Experiences and Telemetry (DiagTrack) services and DataCollection policies.",
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
            _listView.Columns.Add("Service Identifier", 160);
            _listView.Columns.Add("Display Name", 230);
            _listView.Columns.Add("Status", 90);
            _listView.Columns.Add("Purpose", 230);

            var panelBottom = new Panel { Dock = DockStyle.Bottom, Height = 50, Padding = new Padding(10) };
            _lblStatus = new Label { Text = "Ready", AutoSize = true, Location = new Point(10, 16) };
            _btnRefresh = new Button { Text = "Refresh", Location = new Point(350, 10), Size = new Size(80, 28) };
            _btnRefresh.Click += (s, e) => RefreshServices();

            _btnDisable = new Button { Text = "Harden / Disable", Location = new Point(435, 10), Size = new Size(120, 28) };
            _btnDisable.Click += (s, e) => ApplyHardening(true);

            _btnRestore = new Button { Text = "Restore Default", Location = new Point(560, 10), Size = new Size(100, 28) };
            _btnRestore.Click += (s, e) => ApplyHardening(false);

            _btnClose = new Button { Text = "Close", Location = new Point(665, 10), Size = new Size(60, 28) };
            _btnClose.Click += (s, e) => Close();

            panelBottom.Controls.AddRange(new Control[] { _lblStatus, _btnRefresh, _btnDisable, _btnRestore, _btnClose });

            Controls.Add(_listView);
            Controls.Add(panelBottom);
            Controls.Add(panelTop);
        }

        private void RefreshServices()
        {
            _listView.Items.Clear();
            var services = WindowsTelemetryDiagTrackEngine.GetTelemetryServices();
            foreach (var s in services)
            {
                var lvi = new ListViewItem(s.ServiceName);
                lvi.SubItems.Add(s.DisplayName);
                lvi.SubItems.Add(s.IsActive ? "Active" : "Disabled");
                lvi.SubItems.Add(s.Description);
                _listView.Items.Add(lvi);
            }
            _lblStatus.Text = $"Audited {services.Count} telemetry service channels.";
        }

        private void ApplyHardening(bool disable)
        {
            var res = WindowsTelemetryDiagTrackEngine.ApplyPrivacyHardening(disable);
            MessageBox.Show(disable
                ? $"Telemetry services hardened & DataCollection policy applied.\nAdjusted {res.ServicesAdjusted} services and set {res.RegistryPoliciesSet} policy entries."
                : $"Default telemetry settings restored.\nAdjusted {res.ServicesAdjusted} services.",
                "Telemetry Policy Updated", MessageBoxButtons.OK, MessageBoxIcon.Information);
            RefreshServices();
        }
    }
}
