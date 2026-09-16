/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    DirectInput Controller Profile Cleaner Dialog
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Drawing;
using System.Windows.Forms;
using UninstallTools.JunkCleaner;

namespace BulkCrapUninstaller.Forms
{
    public sealed class DirectInputControllerProfileCleanerWindow : Form
    {
        private ListView _listView;
        private Button _btnScan;
        private Button _btnClean;
        private Button _btnClose;
        private Label _lblStatus;

        public DirectInputControllerProfileCleanerWindow()
        {
            InitializeComponent();
            PerformScan();
        }

        private void InitializeComponent()
        {
            Text = "DirectInput Game Controller Calibration Residuals - EBUninstaller Pro";
            Size = new Size(740, 430);
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            var panelTop = new Panel { Dock = DockStyle.Top, Height = 55, Padding = new Padding(12) };
            var lblTitle = new Label
            {
                Text = "DirectInput & Game Controller Calibration Residuals",
                Font = new Font(Font, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(12, 10)
            };
            var lblSub = new Label
            {
                Text = "Audits and purges stale game controller mapping and joystick calibration registry trees.",
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
            _listView.Columns.Add("Device / Controller Identifier", 280);
            _listView.Columns.Add("Device GUID", 240);
            _listView.Columns.Add("Calibration", 140);

            var panelBottom = new Panel { Dock = DockStyle.Bottom, Height = 50, Padding = new Padding(10) };
            _lblStatus = new Label { Text = "Ready", AutoSize = true, Location = new Point(10, 16) };
            _btnScan = new Button { Text = "Scan Devices", Location = new Point(410, 10), Size = new Size(95, 28) };
            _btnScan.Click += (s, e) => PerformScan();

            _btnClean = new Button { Text = "Purge Profiles", Location = new Point(515, 10), Size = new Size(110, 28) };
            _btnClean.Click += (s, e) => PerformClean();

            _btnClose = new Button { Text = "Close", Location = new Point(635, 10), Size = new Size(65, 28) };
            _btnClose.Click += (s, e) => Close();

            panelBottom.Controls.AddRange(new Control[] { _lblStatus, _btnScan, _btnClean, _btnClose });

            Controls.Add(_listView);
            Controls.Add(panelBottom);
            Controls.Add(panelTop);
        }

        private void PerformScan()
        {
            _listView.Items.Clear();
            var items = DirectInputControllerProfileCleanerEngine.ScanDirectInputProfiles();
            foreach (var itm in items)
            {
                var lvi = new ListViewItem(itm.JoystickName);
                lvi.SubItems.Add(itm.DeviceGuid);
                lvi.SubItems.Add(itm.HasCalibrationData ? "Custom Calibration" : "Standard Profile");
                _listView.Items.Add(lvi);
            }
            _lblStatus.Text = $"Found {items.Count} game controller profile(s).";
        }

        private void PerformClean()
        {
            var res = DirectInputControllerProfileCleanerEngine.PurgeDirectInputProfiles();
            MessageBox.Show($"Purged {res.ProfilesDeleted} stale controller profile(s).", "Profiles Purged", MessageBoxButtons.OK, MessageBoxIcon.Information);
            PerformScan();
        }
    }
}
