/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Print Spooler Health & Stuck Job Repair Dialog
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Drawing;
using System.Windows.Forms;
using UninstallTools.SystemTools;

namespace BulkCrapUninstaller.Forms
{
    public sealed class PrintSpoolerHealthRepairWindow : Form
    {
        private ListView _listView;
        private Button _btnScan;
        private Button _btnRepair;
        private Button _btnClose;
        private Label _lblStatus;

        public PrintSpoolerHealthRepairWindow()
        {
            InitializeComponent();
            PerformScan();
        }

        private void InitializeComponent()
        {
            Text = "Windows Print Spooler Diagnostics & Repair - EBUninstaller Pro";
            Size = new Size(680, 420);
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            var panelTop = new Panel { Dock = DockStyle.Top, Height = 55, Padding = new Padding(12) };
            var lblTitle = new Label
            {
                Text = "Print Spooler Health & Stuck Queue Manager",
                Font = new Font(Font, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(12, 10)
            };
            var lblSub = new Label
            {
                Text = "Scans for locked print spooler files (.spl / .shd) that prevent printer uninstallation or lock system resources.",
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
            _listView.Columns.Add("Spool File", 180);
            _listView.Columns.Add("Type", 180);
            _listView.Columns.Add("Size (Bytes)", 110);
            _listView.Columns.Add("Created Date", 160);

            var panelBottom = new Panel { Dock = DockStyle.Bottom, Height = 50, Padding = new Padding(10) };
            _lblStatus = new Label { Text = "Ready", AutoSize = true, Location = new Point(10, 16) };
            _btnScan = new Button { Text = "Scan Queue", Location = new Point(360, 10), Size = new Size(95, 28) };
            _btnScan.Click += (s, e) => PerformScan();

            _btnRepair = new Button { Text = "Reset Spooler", Location = new Point(465, 10), Size = new Size(115, 28) };
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
            var jobs = PrintSpoolerHealthRepairEngine.ScanStuckJobs();
            foreach (var j in jobs)
            {
                var lvi = new ListViewItem(j.FileName);
                lvi.SubItems.Add(j.JobType);
                lvi.SubItems.Add(j.FileSizeBytes.ToString("N0"));
                lvi.SubItems.Add(j.CreatedTime.ToString("g"));
                _listView.Items.Add(lvi);
            }
            _lblStatus.Text = jobs.Count == 0 ? "Print spooler queue is clean." : $"Found {jobs.Count} stuck print spooler item(s).";
        }

        private void PerformRepair()
        {
            var res = PrintSpoolerHealthRepairEngine.RepairAndPurgeSpooler();
            MessageBox.Show($"Purged {res.JobsPurged} stuck spool item(s) and restarted the Print Spooler service.", "Spooler Reset Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            PerformScan();
        }
    }
}
