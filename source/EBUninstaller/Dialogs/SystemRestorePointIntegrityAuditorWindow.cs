/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    System Restore Point Integrity Auditor Dialog
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Drawing;
using System.Windows.Forms;
using UninstallTools.Backup;

namespace BulkCrapUninstaller.Forms
{
    public sealed class SystemRestorePointIntegrityAuditorWindow : Form
    {
        private ListView _listView;
        private Button _btnRefresh;
        private Button _btnClose;
        private Label _lblStatus;

        public SystemRestorePointIntegrityAuditorWindow()
        {
            InitializeComponent();
            PerformAudit();
        }

        private void InitializeComponent()
        {
            Text = "Windows System Restore Points & Snapshot Integrity - EBUninstaller Pro";
            Size = new Size(760, 440);
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            var panelTop = new Panel { Dock = DockStyle.Top, Height = 55, Padding = new Padding(12) };
            var lblTitle = new Label
            {
                Text = "Windows System Restore Points & Snapshot Chain",
                Font = new Font(Font, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(12, 10)
            };
            var lblSub = new Label
            {
                Text = "Audits restore point sequence numbers, trigger events, timestamps, and snapshot storage health.",
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
            _listView.Columns.Add("Seq #", 60);
            _listView.Columns.Add("Description / Checkpoint Name", 280);
            _listView.Columns.Add("Trigger Type", 170);
            _listView.Columns.Add("Creation Time", 160);
            _listView.Columns.Add("Integrity", 70);

            var panelBottom = new Panel { Dock = DockStyle.Bottom, Height = 50, Padding = new Padding(10) };
            _lblStatus = new Label { Text = "Ready", AutoSize = true, Location = new Point(10, 16) };
            _btnRefresh = new Button { Text = "Audit Restore Points", Location = new Point(530, 10), Size = new Size(140, 28) };
            _btnRefresh.Click += (s, e) => PerformAudit();

            _btnClose = new Button { Text = "Close", Location = new Point(675, 10), Size = new Size(70, 28) };
            _btnClose.Click += (s, e) => Close();

            panelBottom.Controls.AddRange(new Control[] { _lblStatus, _btnRefresh, _btnClose });

            Controls.Add(_listView);
            Controls.Add(panelBottom);
            Controls.Add(panelTop);
        }

        private void PerformAudit()
        {
            _listView.Items.Clear();
            var res = SystemRestorePointIntegrityAuditorEngine.AuditRestorePoints();
            foreach (var p in res.Points)
            {
                var lvi = new ListViewItem(p.SequenceNumber.ToString());
                lvi.SubItems.Add(p.Description);
                lvi.SubItems.Add(p.RestorePointType);
                lvi.SubItems.Add(p.CreationTime);
                lvi.SubItems.Add(p.IsValid ? "Valid" : "Corrupt");
                _listView.Items.Add(lvi);
            }
            _lblStatus.Text = $"Detected {res.TotalRestorePoints} system restore point(s).";
        }
    }
}
