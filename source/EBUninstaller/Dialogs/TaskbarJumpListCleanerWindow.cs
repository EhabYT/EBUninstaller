/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Taskbar Jump List & Recent Document Cleaner Dialog
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Drawing;
using System.Windows.Forms;
using UninstallTools.PrivacyCleaner;

namespace BulkCrapUninstaller.Forms
{
    public sealed class TaskbarJumpListCleanerWindow : Form
    {
        private ListView _listView;
        private Button _btnScan;
        private Button _btnClean;
        private Button _btnClose;
        private Label _lblStatus;

        public TaskbarJumpListCleanerWindow()
        {
            InitializeComponent();
            PerformScan();
        }

        private void InitializeComponent()
        {
            Text = "Taskbar Jump List & Recent Documents Cleaner - EBUninstaller Pro";
            Size = new Size(720, 430);
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            var panelTop = new Panel { Dock = DockStyle.Top, Height = 55, Padding = new Padding(12) };
            var lblTitle = new Label
            {
                Text = "Taskbar Jump List & Recent Activity Cleaner",
                Font = new Font(Font, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(12, 10)
            };
            var lblSub = new Label
            {
                Text = "Purges pinned application jump lists, automatic destinations, and recent document caches.",
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
            _listView.Columns.Add("Destination Item", 280);
            _listView.Columns.Add("Type", 180);
            _listView.Columns.Add("Size (Bytes)", 100);
            _listView.Columns.Add("Modified Date", 130);

            var panelBottom = new Panel { Dock = DockStyle.Bottom, Height = 50, Padding = new Padding(10) };
            _lblStatus = new Label { Text = "Ready", AutoSize = true, Location = new Point(10, 16) };
            _btnScan = new Button { Text = "Scan Lists", Location = new Point(410, 10), Size = new Size(95, 28) };
            _btnScan.Click += (s, e) => PerformScan();

            _btnClean = new Button { Text = "Purge Jump Lists", Location = new Point(515, 10), Size = new Size(120, 28) };
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
            var items = TaskbarJumpListCleanerEngine.ScanJumpLists();
            long totalBytes = 0;
            foreach (var itm in items)
            {
                totalBytes += itm.FileSizeBytes;
                var lvi = new ListViewItem(itm.FileName);
                lvi.SubItems.Add(itm.JumpListType);
                lvi.SubItems.Add(itm.FileSizeBytes.ToString("N0"));
                lvi.SubItems.Add(itm.LastModified.ToString("g"));
                _listView.Items.Add(lvi);
            }
            _lblStatus.Text = $"Found {items.Count} jump list record(s) ({totalBytes / 1024} KB).";
        }

        private void PerformClean()
        {
            var res = TaskbarJumpListCleanerEngine.PurgeJumpLists();
            MessageBox.Show($"Purged {res.TotalItemsDeleted} jump list file(s), reclaiming {res.BytesCleaned / 1024} KB.", "Jump Lists Cleaned", MessageBoxButtons.OK, MessageBoxIcon.Information);
            PerformScan();
        }
    }
}
