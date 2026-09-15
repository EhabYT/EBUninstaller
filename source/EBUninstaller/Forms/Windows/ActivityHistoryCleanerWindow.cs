/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Connected Devices Platform Activity History Cleaner Window
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Drawing;
using System.Windows.Forms;
using UninstallTools.JunkCleaner;

namespace BulkCrapUninstaller.Forms.Windows
{
    public sealed class ActivityHistoryCleanerWindow : Form
    {
        private Label _lblDatabases;
        private Label _lblSize;
        private Label _lblStatus;
        private Button _btnClean;
        private Button _btnRefresh;
        private Button _btnClose;
        private ActivityHistoryStats _stats;

        public ActivityHistoryCleanerWindow()
        {
            InitializeComponent();
            RefreshStats();
        }

        private void InitializeComponent()
        {
            Text = "Connected Devices Platform (CDP) Activity History Cleaner - EBUninstaller Pro";
            Size = new Size(800, 360);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(16) };

            var lblHeader = new Label
            {
                Text = "Windows User Activity History & Application Timeline Cache",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Location = new Point(16, 16),
                AutoSize = true
            };

            var lblDesc = new Label
            {
                Text = "Windows Connected Devices Platform (CDP) logs user application launches, accessed files,\nand device timeline states in ActivitiesCache.db. Purging removes persistent user tracking history\nand releases database storage from uninstalled applications.",
                Location = new Point(16, 46),
                Size = new Size(740, 50)
            };

            var group = new GroupBox
            {
                Text = "Activity History Storage Statistics",
                Location = new Point(16, 105),
                Size = new Size(740, 110)
            };

            _lblDatabases = new Label { Location = new Point(16, 30), AutoSize = true, Text = "Activity Databases: -" };
            _lblSize = new Label { Location = new Point(16, 62), AutoSize = true, Text = "Total Cache Size: -" };

            group.Controls.AddRange(new Control[] { _lblDatabases, _lblSize });

            _lblStatus = new Label
            {
                Location = new Point(16, 230),
                AutoSize = true,
                Font = new Font(Font, FontStyle.Bold),
                Text = "Status: Ready"
            };

            _btnClean = new Button
            {
                Text = "Purge Activity History",
                Font = new Font(Font, FontStyle.Bold),
                ForeColor = Color.DarkSlateBlue,
                Location = new Point(440, 265),
                Size = new Size(180, 32)
            };
            _btnClean.Click += OnCleanClick;

            _btnRefresh = new Button
            {
                Text = "Refresh",
                Location = new Point(630, 265),
                Size = new Size(70, 32)
            };
            _btnRefresh.Click += (s, e) => RefreshStats();

            _btnClose = new Button
            {
                Text = "Close",
                DialogResult = DialogResult.OK,
                Location = new Point(710, 265),
                Size = new Size(75, 32)
            };

            panel.Controls.AddRange(new Control[] { lblHeader, lblDesc, group, _lblStatus, _btnClean, _btnRefresh, _btnClose });
            Controls.Add(panel);
        }

        private void RefreshStats()
        {
            _stats = ActivityHistoryCleanerEngine.ScanActivityHistory();
            _lblDatabases.Text = $"Active CDP Databases: {_stats.DatabaseCount} file(s)";
            _lblSize.Text = $"Database Size on Disk: {FormatBytes(_stats.TotalSizeBytes)}";
            _lblStatus.Text = _stats.DatabaseCount > 0 ? "Application activity records found in ConnectedDevicesPlatform." : "Activity history is clear.";
            _lblStatus.ForeColor = _stats.DatabaseCount > 0 ? Color.DarkOrange : Color.DarkGreen;
        }

        private void OnCleanClick(object sender, EventArgs e)
        {
            var (files, bytes) = ActivityHistoryCleanerEngine.PurgeActivityHistory();
            MessageBox.Show($"Purged {files} activity database file(s) (Freed {FormatBytes(bytes)}).", "Activity History Cleared", MessageBoxButtons.OK, MessageBoxIcon.Information);
            RefreshStats();
        }

        private static string FormatBytes(long bytes)
        {
            if (bytes <= 0) return "0 B";
            if (bytes < 1024) return $"{bytes} B";
            if (bytes < 1024 * 1024) return $"{bytes / 1024.0:F1} KB";
            return $"{bytes / (1024.0 * 1024.0):F2} MB";
        }
    }
}
