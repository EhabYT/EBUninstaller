/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    .NET Framework NGEN Optimization Dialog
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Drawing;
using System.Windows.Forms;
using UninstallTools.SystemTools;

namespace BulkCrapUninstaller.Forms
{
    public sealed class DotNetNgenOptimizationWindow : Form
    {
        private ListView _listView;
        private Button _btnRefresh;
        private Button _btnOptimize;
        private Button _btnClose;
        private Label _lblStatus;

        public DotNetNgenOptimizationWindow()
        {
            InitializeComponent();
            RefreshList();
        }

        private void InitializeComponent()
        {
            Text = ".NET Native Image Generator (NGEN) Optimizer - EBUninstaller Pro";
            Size = new Size(740, 420);
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            var panelTop = new Panel { Dock = DockStyle.Top, Height = 55, Padding = new Padding(12) };
            var lblTitle = new Label
            {
                Text = ".NET Framework & NGEN Optimization Subsystem",
                Font = new Font(Font, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(12, 10)
            };
            var lblSub = new Label
            {
                Text = "Flushes and precompiles pending .NET native image compilation tasks to reduce background CPU usage.",
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
            _listView.Columns.Add("CLR Version", 160);
            _listView.Columns.Add("Architecture", 140);
            _listView.Columns.Add("NGEN Executable Path", 320);
            _listView.Columns.Add("Status", 100);

            var panelBottom = new Panel { Dock = DockStyle.Bottom, Height = 50, Padding = new Padding(10) };
            _lblStatus = new Label { Text = "Ready", AutoSize = true, Location = new Point(10, 16) };
            _btnRefresh = new Button { Text = "Refresh", Location = new Point(410, 10), Size = new Size(80, 28) };
            _btnRefresh.Click += (s, e) => RefreshList();

            _btnOptimize = new Button { Text = "Execute Queued Items", Location = new Point(495, 10), Size = new Size(155, 28) };
            _btnOptimize.Click += (s, e) => PerformOptimize();

            _btnClose = new Button { Text = "Close", Location = new Point(655, 10), Size = new Size(65, 28) };
            _btnClose.Click += (s, e) => Close();

            panelBottom.Controls.AddRange(new Control[] { _lblStatus, _btnRefresh, _btnOptimize, _btnClose });

            Controls.Add(_listView);
            Controls.Add(panelBottom);
            Controls.Add(panelTop);
        }

        private void RefreshList()
        {
            _listView.Items.Clear();
            var runtimes = DotNetNgenOptimizationEngine.DiscoverNgenRuntimes();
            foreach (var r in runtimes)
            {
                var lvi = new ListViewItem(r.FrameworkVersion);
                lvi.SubItems.Add(r.Architecture);
                lvi.SubItems.Add(r.NgenExecutablePath);
                lvi.SubItems.Add(r.Exists ? "Installed" : "Not Found");
                _listView.Items.Add(lvi);
            }
            _lblStatus.Text = $"Detected {runtimes.Count} .NET NGEN compiler targets.";
        }

        private void PerformOptimize()
        {
            _lblStatus.Text = "Optimizing NGEN queues, please wait...";
            Application.DoEvents();

            var res = DotNetNgenOptimizationEngine.ExecuteQueuedItems();
            MessageBox.Show($"Processed {res.QueuesProcessed} .NET NGEN queue(s). Background precompilation has completed.", "NGEN Optimization Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            RefreshList();
        }
    }
}
