/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Windows Memory Diagnostic & RAM Allocation Auditor Window
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Drawing;
using System.Windows.Forms;
using UninstallTools.SystemTools;

namespace BulkCrapUninstaller.Forms.Windows
{
    public sealed class MemoryDiagnosticAuditWindow : Form
    {
        private Label _lblTotalRam;
        private Label _lblFreeRam;
        private Label _lblSticks;
        private Label _lblSpeed;
        private Label _lblStatus;
        private Button _btnScheduleTest;
        private Button _btnRefresh;
        private Button _btnClose;
        private MemoryDiagnosticInfo _info;

        public MemoryDiagnosticAuditWindow()
        {
            InitializeComponent();
            RefreshData();
        }

        private void InitializeComponent()
        {
            Text = "Windows Memory Diagnostic & Hardware RAM Auditor - EBUninstaller Pro";
            Size = new Size(800, 360);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(16) };

            var lblHeader = new Label
            {
                Text = "Windows Memory Diagnostic (MdSched.exe) & Physical RAM Health",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Location = new Point(16, 16),
                AutoSize = true
            };

            var lblDesc = new Label
            {
                Text = "Corrupted RAM modules or aggressive memory timings can cause random application crashes,\nuninstallation failures, and blue screen bugchecks (BSOD). Running Windows Memory Diagnostic\nchecks physical RAM sectors for bitflips and memory address faults on boot.",
                Location = new Point(16, 46),
                Size = new Size(740, 50)
            };

            var group = new GroupBox
            {
                Text = "Physical Memory (RAM) Allocation",
                Location = new Point(16, 105),
                Size = new Size(740, 110)
            };

            _lblTotalRam = new Label { Location = new Point(16, 28), AutoSize = true, Text = "Total Physical RAM: -" };
            _lblFreeRam = new Label { Location = new Point(16, 56), AutoSize = true, Text = "Free Available RAM: -" };
            _lblSticks = new Label { Location = new Point(380, 28), AutoSize = true, Text = "RAM Modules Detected: -" };
            _lblSpeed = new Label { Location = new Point(380, 56), AutoSize = true, Text = "Configured Memory Clock: -" };

            group.Controls.AddRange(new Control[] { _lblTotalRam, _lblFreeRam, _lblSticks, _lblSpeed });

            _lblStatus = new Label
            {
                Location = new Point(16, 230),
                AutoSize = true,
                Font = new Font(Font, FontStyle.Bold),
                Text = "Status: Ready"
            };

            _btnScheduleTest = new Button
            {
                Text = "Launch Memory Diagnostic...",
                Font = new Font(Font, FontStyle.Bold),
                ForeColor = Color.DarkSlateBlue,
                Location = new Point(380, 265),
                Size = new Size(230, 32)
            };
            _btnScheduleTest.Click += OnScheduleTestClick;

            _btnRefresh = new Button
            {
                Text = "Refresh",
                Location = new Point(620, 265),
                Size = new Size(80, 32)
            };
            _btnRefresh.Click += (s, e) => RefreshData();

            _btnClose = new Button
            {
                Text = "Close",
                DialogResult = DialogResult.OK,
                Location = new Point(710, 265),
                Size = new Size(75, 32)
            };

            panel.Controls.AddRange(new Control[] { lblHeader, lblDesc, group, _lblStatus, _btnScheduleTest, _btnRefresh, _btnClose });
            Controls.Add(panel);
        }

        private void RefreshData()
        {
            _info = MemoryDiagnosticAuditEngine.QueryMemoryDiagnosticState();
            _lblTotalRam.Text = $"Total Physical RAM: {FormatBytes((long)_info.TotalPhysicalMemoryBytes)}";
            _lblFreeRam.Text = $"Available Free RAM: {FormatBytes((long)_info.FreePhysicalMemoryBytes)}";
            _lblSticks.Text = $"Physical DIMM Modules: {_info.MemoryStickCount}";
            _lblSpeed.Text = $"Memory Speed & Clock: {_info.MemorySpeedAndType}";

            var usedPercent = _info.TotalPhysicalMemoryBytes > 0 ? 100.0 - ((double)_info.FreePhysicalMemoryBytes * 100.0 / _info.TotalPhysicalMemoryBytes) : 0;
            _lblStatus.Text = $"Current RAM Utilization: {usedPercent:F1}%";
            _lblStatus.ForeColor = usedPercent > 85 ? Color.DarkOrange : Color.DarkGreen;
        }

        private void OnScheduleTestClick(object sender, EventArgs e)
        {
            MemoryDiagnosticAuditEngine.ScheduleMemoryTestOnNextReboot();
        }

        private static string FormatBytes(long bytes)
        {
            if (bytes <= 0) return "0 B";
            if (bytes < 1024) return $"{bytes} B";
            if (bytes < 1024 * 1024) return $"{bytes / 1024.0:F1} KB";
            if (bytes < 1024 * 1024 * 1024) return $"{bytes / (1024.0 * 1024.0):F2} MB";
            return $"{bytes / (1024.0 * 1024.0 * 1024.0):F2} GB";
        }
    }
}
