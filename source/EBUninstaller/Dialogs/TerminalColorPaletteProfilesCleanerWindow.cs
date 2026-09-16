/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Terminal & Console Profile Cleaner Dialog
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Drawing;
using System.Windows.Forms;
using UninstallTools.PrivacyCleaner;

namespace BulkCrapUninstaller.Forms
{
    public sealed class TerminalColorPaletteProfilesCleanerWindow : Form
    {
        private ListView _listView;
        private Button _btnScan;
        private Button _btnClean;
        private Button _btnClose;
        private Label _lblStatus;

        public TerminalColorPaletteProfilesCleanerWindow()
        {
            InitializeComponent();
            PerformScan();
        }

        private void InitializeComponent()
        {
            Text = "Console & Windows Terminal Profile Residuals - EBUninstaller Pro";
            Size = new Size(740, 430);
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            var panelTop = new Panel { Dock = DockStyle.Top, Height = 55, Padding = new Padding(12) };
            var lblTitle = new Label
            {
                Text = "Terminal & Console Application Registry Profiles",
                Font = new Font(Font, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(12, 10)
            };
            var lblSub = new Label
            {
                Text = "Scans and cleans orphaned console font/color registry profiles created by uninstalled CLI tools and shells.",
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
            _listView.Columns.Add("Application / Command Shell", 280);
            _listView.Columns.Add("Registry Location", 280);
            _listView.Columns.Add("Profile Type", 120);

            var panelBottom = new Panel { Dock = DockStyle.Bottom, Height = 50, Padding = new Padding(10) };
            _lblStatus = new Label { Text = "Ready", AutoSize = true, Location = new Point(10, 16) };
            _btnScan = new Button { Text = "Scan Profiles", Location = new Point(410, 10), Size = new Size(95, 28) };
            _btnScan.Click += (s, e) => PerformScan();

            _btnClean = new Button { Text = "Clean Custom", Location = new Point(515, 10), Size = new Size(110, 28) };
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
            var profiles = TerminalColorPaletteProfilesCleanerEngine.ScanConsoleProfiles();
            foreach (var p in profiles)
            {
                var lvi = new ListViewItem(p.TitlePreview);
                lvi.SubItems.Add(p.RegistryPath);
                lvi.SubItems.Add(p.IsCustomAppProfile ? "Custom App" : "Standard Shell");
                _listView.Items.Add(lvi);
            }
            _lblStatus.Text = $"Found {profiles.Count} console profile key(s).";
        }

        private void PerformClean()
        {
            var res = TerminalColorPaletteProfilesCleanerEngine.PurgeCustomConsoleProfiles();
            MessageBox.Show($"Purged {res.ProfilesCleaned} custom console profile(s).", "Profiles Cleaned", MessageBoxButtons.OK, MessageBoxIcon.Information);
            PerformScan();
        }
    }
}
