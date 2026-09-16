/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    System Timer Resolution & Multimedia Interrupts Dialog
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Drawing;
using System.Windows.Forms;
using UninstallTools.SystemTools;

namespace BulkCrapUninstaller.Forms
{
    public sealed class SystemTimerResolutionOptimizerWindow : Form
    {
        private ListView _listView;
        private Button _btnRefresh;
        private Button _btnClose;
        private Label _lblStatus;

        public SystemTimerResolutionOptimizerWindow()
        {
            InitializeComponent();
            PerformAudit();
        }

        private void InitializeComponent()
        {
            Text = "System Timer Resolution & Interrupt Profiler - EBUninstaller Pro";
            Size = new Size(760, 440);
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            var panelTop = new Panel { Dock = DockStyle.Top, Height = 55, Padding = new Padding(12) };
            var lblTitle = new Label
            {
                Text = "High-Precision System Timer Resolution Profiler",
                Font = new Font(Font, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(12, 10)
            };
            var lblSub = new Label
            {
                Text = "Identifies running background applications requesting high-precision timer ticks (1.0ms/0.5ms) causing power drain.",
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
            _listView.Columns.Add("Process Name", 220);
            _listView.Columns.Add("PID", 80);
            _listView.Columns.Add("Timer Resolution Requested", 240);
            _listView.Columns.Add("Status", 180);

            var panelBottom = new Panel { Dock = DockStyle.Bottom, Height = 50, Padding = new Padding(10) };
            _lblStatus = new Label { Text = "Ready", AutoSize = true, Location = new Point(10, 16) };
            _btnRefresh = new Button { Text = "Profile Timers", Location = new Point(560, 10), Size = new Size(110, 28) };
            _btnRefresh.Click += (s, e) => PerformAudit();

            _btnClose = new Button { Text = "Close", Location = new Point(680, 10), Size = new Size(65, 28) };
            _btnClose.Click += (s, e) => Close();

            panelBottom.Controls.AddRange(new Control[] { _lblStatus, _btnRefresh, _btnClose });

            Controls.Add(_listView);
            Controls.Add(panelBottom);
            Controls.Add(panelTop);
        }

        private void PerformAudit()
        {
            _listView.Items.Clear();
            var res = SystemTimerResolutionOptimizerEngine.AuditTimerResolution();
            foreach (var p in res.ActiveTimerProcesses)
            {
                var lvi = new ListViewItem(p.ProcessName);
                lvi.SubItems.Add(p.ProcessId.ToString());
                lvi.SubItems.Add(p.RequestedResolution);
                lvi.SubItems.Add(p.Status);
                _listView.Items.Add(lvi);
            }
            _lblStatus.Text = $"Current timer resolution: {res.CurrentResolutionMs:F1} ms (Min: {res.MinimumResolutionMs:F1} ms, Max: {res.MaximumResolutionMs:F1} ms).";
        }
    }
}
