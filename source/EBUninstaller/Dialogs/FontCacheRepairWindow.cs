/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Font Cache Repair & Optimization Dialog
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Drawing;
using System.Windows.Forms;
using UninstallTools.SystemTools;

namespace BulkCrapUninstaller.Forms
{
    public sealed class FontCacheRepairWindow : Form
    {
        private ListView _listView;
        private Button _btnScan;
        private Button _btnRepair;
        private Button _btnClose;
        private Label _lblStatus;

        public FontCacheRepairWindow()
        {
            InitializeComponent();
            PerformScan();
        }

        private void InitializeComponent()
        {
            Text = "Windows Font Cache Diagnostics & Rebuilder - EBUninstaller Pro";
            Size = new Size(680, 420);
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            var panelTop = new Panel { Dock = DockStyle.Top, Height = 55, Padding = new Padding(12) };
            var lblTitle = new Label
            {
                Text = "Font Cache Repair & Integrity Manager",
                Font = new Font(Font, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(12, 10)
            };
            var lblSub = new Label
            {
                Text = "Scans and purges corrupted or bloated Windows & WPF font cache databases to resolve rendering issues.",
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
            _listView.Columns.Add("Cache Target File", 320);
            _listView.Columns.Add("Associated Service", 120);
            _listView.Columns.Add("Size (KB)", 80);
            _listView.Columns.Add("Status", 130);

            var panelBottom = new Panel { Dock = DockStyle.Bottom, Height = 50, Padding = new Padding(10) };
            _lblStatus = new Label { Text = "Ready", AutoSize = true, Location = new Point(10, 16) };
            _btnScan = new Button { Text = "Scan Caches", Location = new Point(360, 10), Size = new Size(95, 28) };
            _btnScan.Click += (s, e) => PerformScan();

            _btnRepair = new Button { Text = "Purge & Rebuild", Location = new Point(465, 10), Size = new Size(115, 28) };
            _btnRepair.Click += (s, e) => PerformRepair();

            _btnClose = new Button { Text = "Close", Location = new Point(590, 10), Size = new Size(70, 28) };
            _btnClose.Click += (s, e) => Close();

            panelBottom.Controls.AddRange(new Control[] { _lblStatus, _btnScan, _btnRepair, _btnClose });

            Controls.Add(_listView);
            Controls.Add(panelBottom);
            Controls.Add(panelTop);
        }

        private void PerformScan()
        {
            _listView.Items.Clear();
            var items = FontCacheRepairEngine.ScanFontCaches();
            foreach (var itm in items)
            {
                var lvi = new ListViewItem(itm.TargetPath);
                lvi.SubItems.Add(itm.ServiceName);
                lvi.SubItems.Add((itm.FileSizeBytes / 1024).ToString());
                lvi.SubItems.Add(itm.StatusDescription);
                _listView.Items.Add(lvi);
            }
            _lblStatus.Text = $"Audited {items.Count} font cache target(s).";
        }

        private void PerformRepair()
        {
            var res = FontCacheRepairEngine.RepairAndPurge();
            MessageBox.Show($"Purged {res.FilesPurged} cache file(s), reclaiming {res.BytesReclaimed / 1024} KB.\nFont cache services will regenerate clean databases automatically.", "Font Cache Rebuilt", MessageBoxButtons.OK, MessageBoxIcon.Information);
            PerformScan();
        }
    }
}
