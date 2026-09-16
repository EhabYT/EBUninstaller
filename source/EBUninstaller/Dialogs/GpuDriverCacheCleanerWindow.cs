/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    GPU Vendor Shader & Pipeline Cache Cleaner Dialog
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Drawing;
using System.Windows.Forms;
using UninstallTools.JunkCleaner;

namespace BulkCrapUninstaller.Forms
{
    public sealed class GpuDriverCacheCleanerWindow : Form
    {
        private ListView _listView;
        private Button _btnScan;
        private Button _btnClean;
        private Button _btnClose;
        private Label _lblStatus;

        public GpuDriverCacheCleanerWindow()
        {
            InitializeComponent();
            PerformScan();
        }

        private void InitializeComponent()
        {
            Text = "GPU Shader & Pipeline Cache Cleaner (NVIDIA / AMD / Intel) - EBUninstaller Pro";
            Size = new Size(740, 430);
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            var panelTop = new Panel { Dock = DockStyle.Top, Height = 55, Padding = new Padding(12) };
            var lblTitle = new Label
            {
                Text = "GPU Driver Shader Cache & Graphics Compiler Cleaner",
                Font = new Font(Font, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(12, 10)
            };
            var lblSub = new Label
            {
                Text = "Scans and purges DirectX, Vulkan, and OpenGL compiled shader binaries left over by uninstalled games and 3D apps.",
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
            _listView.Columns.Add("Shader Cache File", 340);
            _listView.Columns.Add("GPU Vendor / Architecture", 180);
            _listView.Columns.Add("Size (KB)", 90);
            _listView.Columns.Add("Last Modified", 110);

            var panelBottom = new Panel { Dock = DockStyle.Bottom, Height = 50, Padding = new Padding(10) };
            _lblStatus = new Label { Text = "Ready", AutoSize = true, Location = new Point(10, 16) };
            _btnScan = new Button { Text = "Scan Caches", Location = new Point(410, 10), Size = new Size(95, 28) };
            _btnScan.Click += (s, e) => PerformScan();

            _btnClean = new Button { Text = "Purge Shaders", Location = new Point(515, 10), Size = new Size(120, 28) };
            _btnClean.Click += (s, e) => PerformClean();

            _btnClose = new Button { Text = "Close", Location = new Point(645, 10), Size = new Size(65, 28) };
            _btnClose.Click += (s, e) => Close();

            panelBottom.Controls.AddRange(new Control[] { _lblStatus, _btnScan, _btnClean, _btnClose });

            Controls.Add(_listView);
            Controls.Add(panelBottom);
            Controls.Add(panelTop);
        }

        private void PerformScan()
        {
            _listView.Items.Clear();
            var items = GpuDriverCacheCleanerEngine.ScanGpuCaches();
            long totalBytes = 0;
            foreach (var itm in items)
            {
                totalBytes += itm.FileSizeBytes;
                var lvi = new ListViewItem(itm.FilePath);
                lvi.SubItems.Add(itm.Vendor);
                lvi.SubItems.Add((itm.FileSizeBytes / 1024).ToString("N0"));
                lvi.SubItems.Add(itm.LastModified.ToString("g"));
                _listView.Items.Add(lvi);
            }
            _lblStatus.Text = $"Found {items.Count} GPU shader file(s) ({totalBytes / 1024} KB).";
        }

        private void PerformClean()
        {
            var res = GpuDriverCacheCleanerEngine.PurgeGpuCaches();
            MessageBox.Show($"Purged {res.FilesDeleted} GPU shader cache file(s), reclaiming {res.BytesCleaned / 1024} KB.", "GPU Shader Cache Purged", MessageBoxButtons.OK, MessageBoxIcon.Information);
            PerformScan();
        }
    }
}
