/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Modern System Resource & Disk Health Telemetry Bar
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace BulkCrapUninstaller.Controls
{
    public sealed class ModernSystemResourceMetricsBar : UserControl
    {
        private Label _lblDisk;
        private Label _lblMemory;
        private Label _lblStatus;
        private Timer _refreshTimer;

        public ModernSystemResourceMetricsBar()
        {
            InitializeComponent();
            RefreshMetrics();

            _refreshTimer = new Timer { Interval = 10000 };
            _refreshTimer.Tick += (s, e) => RefreshMetrics();
            _refreshTimer.Start();
        }

        private void InitializeComponent()
        {
            Dock = DockStyle.Bottom;
            Height = 28;
            Font = new Font("Segoe UI", 8.5F, FontStyle.Regular, GraphicsUnit.Point);
            Padding = new Padding(8, 2, 8, 2);
            BackColor = Color.FromArgb(240, 240, 240);

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 1
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            _lblDisk = new Label
            {
                Text = "💾 System Drive (C:): Calculating...",
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                Margin = new Padding(0, 0, 16, 0)
            };
            layout.Controls.Add(_lblDisk, 0, 0);

            _lblMemory = new Label
            {
                Text = "💻 Virtual Memory: Monitored",
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                Margin = new Padding(0, 0, 16, 0)
            };
            layout.Controls.Add(_lblMemory, 1, 0);

            _lblStatus = new Label
            {
                Text = "🛡️ System Shield: Active (Denylist Protected)",
                ForeColor = Color.ForestGreen,
                Font = new Font(Font, FontStyle.Bold),
                AutoSize = true,
                Anchor = AnchorStyles.Right
            };
            layout.Controls.Add(_lblStatus, 3, 0);

            Controls.Add(layout);
        }

        public void RefreshMetrics()
        {
            try
            {
                var sysDrive = Path.GetPathRoot(Environment.SystemDirectory);
                var di = new DriveInfo(sysDrive);
                if (di.IsReady)
                {
                    var freeGb = di.AvailableFreeSpace / (1024.0 * 1024.0 * 1024.0);
                    var totalGb = di.TotalSize / (1024.0 * 1024.0 * 1024.0);
                    var freePercent = (di.AvailableFreeSpace * 100.0) / di.TotalSize;

                    _lblDisk.Text = $"💾 Boot Drive ({sysDrive}): {freeGb:F1} GB free of {totalGb:F1} GB ({freePercent:F0}% available)";
                    _lblDisk.ForeColor = freePercent < 15 ? Color.DarkOrange : Color.FromArgb(40, 40, 40);
                }

                _lblMemory.Text = $"💻 Working Set: {Environment.WorkingSet / (1024 * 1024)} MB | OS: {Environment.OSVersion.VersionString}";
            }
            catch { }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _refreshTimer?.Stop();
                _refreshTimer?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
