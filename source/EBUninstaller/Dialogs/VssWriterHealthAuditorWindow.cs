/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Volume Shadow Copy (VSS) Writer Health Auditor Dialog
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Drawing;
using System.Windows.Forms;
using UninstallTools.Backup;

namespace BulkCrapUninstaller.Forms
{
    public sealed class VssWriterHealthAuditorWindow : Form
    {
        private ListView _listView;
        private Button _btnRefresh;
        private Button _btnClose;
        private Label _lblStatus;

        public VssWriterHealthAuditorWindow()
        {
            InitializeComponent();
            PerformAudit();
        }

        private void InitializeComponent()
        {
            Text = "VSS Volume Shadow Copy Writers Health Auditor - EBUninstaller Pro";
            Size = new Size(760, 440);
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            var panelTop = new Panel { Dock = DockStyle.Top, Height = 55, Padding = new Padding(12) };
            var lblTitle = new Label
            {
                Text = "Volume Shadow Copy (VSS) Writer Diagnostic Subsystem",
                Font = new Font(Font, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(12, 10)
            };
            var lblSub = new Label
            {
                Text = "Verifies the operational status and error logs of all system VSS snapshot writers.",
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
            _listView.Columns.Add("Writer Name", 260);
            _listView.Columns.Add("Writer ID (GUID)", 240);
            _listView.Columns.Add("State", 120);
            _listView.Columns.Add("Last Error", 120);

            var panelBottom = new Panel { Dock = DockStyle.Bottom, Height = 50, Padding = new Padding(10) };
            _lblStatus = new Label { Text = "Ready", AutoSize = true, Location = new Point(10, 16) };
            _btnRefresh = new Button { Text = "Audit Writers", Location = new Point(560, 10), Size = new Size(110, 28) };
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
            var res = VssWriterHealthAuditorEngine.AuditWriters();
            foreach (var w in res.Writers)
            {
                var lvi = new ListViewItem(w.WriterName);
                lvi.SubItems.Add(w.WriterId);
                lvi.SubItems.Add(w.State);
                lvi.SubItems.Add(w.LastError);
                if (!w.IsHealthy)
                {
                    lvi.ForeColor = Color.Red;
                }
                _listView.Items.Add(lvi);
            }
            _lblStatus.Text = $"Audited {res.TotalWriters} VSS writers ({res.FailedWriters} in error state).";
        }
    }
}
