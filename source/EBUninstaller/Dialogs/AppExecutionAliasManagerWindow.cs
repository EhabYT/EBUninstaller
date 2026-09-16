/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    App Execution Alias & App Paths Manager Dialog
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Drawing;
using System.Windows.Forms;
using UninstallTools.SystemTools;

namespace BulkCrapUninstaller.Forms
{
    public sealed class AppExecutionAliasManagerWindow : Form
    {
        private ListView _listView;
        private Button _btnScan;
        private Button _btnClose;
        private Label _lblStatus;

        public AppExecutionAliasManagerWindow()
        {
            InitializeComponent();
            PerformScan();
        }

        private void InitializeComponent()
        {
            Text = "App Execution Aliases & App Paths Residuals - EBUninstaller Pro";
            Size = new Size(760, 440);
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            var panelTop = new Panel { Dock = DockStyle.Top, Height = 55, Padding = new Padding(12) };
            var lblTitle = new Label
            {
                Text = "Windows App Execution Aliases & App Paths Residuals",
                Font = new Font(Font, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(12, 10)
            };
            var lblSub = new Label
            {
                Text = "Audits system execution aliases and registry App Paths to identify broken links left by uninstalled software.",
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
            _listView.Columns.Add("Alias Name / Command", 180);
            _listView.Columns.Add("Target Executable Path", 320);
            _listView.Columns.Add("Source Category", 160);
            _listView.Columns.Add("Status", 80);

            var panelBottom = new Panel { Dock = DockStyle.Bottom, Height = 50, Padding = new Padding(10) };
            _lblStatus = new Label { Text = "Ready", AutoSize = true, Location = new Point(10, 16) };
            _btnScan = new Button { Text = "Scan Aliases", Location = new Point(560, 10), Size = new Size(110, 28) };
            _btnScan.Click += (s, e) => PerformScan();

            _btnClose = new Button { Text = "Close", Location = new Point(680, 10), Size = new Size(65, 28) };
            _btnClose.Click += (s, e) => Close();

            panelBottom.Controls.AddRange(new Control[] { _lblStatus, _btnScan, _btnClose });

            Controls.Add(_listView);
            Controls.Add(panelBottom);
            Controls.Add(panelTop);
        }

        private void PerformScan()
        {
            _listView.Items.Clear();
            var res = AppExecutionAliasManagerEngine.ScanExecutionAliases();
            foreach (var a in res.Items)
            {
                var lvi = new ListViewItem(a.AliasName);
                lvi.SubItems.Add(a.TargetPath);
                lvi.SubItems.Add(a.SourceCategory);
                lvi.SubItems.Add(a.TargetExists ? "Valid" : "Broken Target");
                if (!a.TargetExists) lvi.ForeColor = Color.Red;
                _listView.Items.Add(lvi);
            }
            _lblStatus.Text = $"Detected {res.TotalAliasesFound} execution alias(es) ({res.BrokenAliasesCount} broken targets).";
        }
    }
}
