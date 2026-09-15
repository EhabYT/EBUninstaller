/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Most Recently Used (MRU) History Cleaner Dialog
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Drawing;
using System.Windows.Forms;
using UninstallTools.PrivacyCleaner;

namespace BulkCrapUninstaller.Forms
{
    public sealed class MruRecentHistoryCleanerWindow : Form
    {
        private ListView _listView;
        private Button _btnScan;
        private Button _btnClean;
        private Button _btnClose;
        private Label _lblStatus;

        public MruRecentHistoryCleanerWindow()
        {
            InitializeComponent();
            PerformScan();
        }

        private void InitializeComponent()
        {
            Text = "Most Recently Used (MRU) History & Trace Cleaner - EBUninstaller Pro";
            Size = new Size(760, 440);
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            var panelTop = new Panel { Dock = DockStyle.Top, Height = 55, Padding = new Padding(12) };
            var lblTitle = new Label
            {
                Text = "Windows Explorer & Common Dialog MRU History Cleaner",
                Font = new Font(Font, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(12, 10)
            };
            var lblSub = new Label
            {
                Text = "Scans and cleans Run dialog commands, Open/Save file history, and last-visited folder traces.",
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
            _listView.Columns.Add("MRU Category", 180);
            _listView.Columns.Add("Value / Target", 110);
            _listView.Columns.Add("Historical Data Content", 240);
            _listView.Columns.Add("Registry Location", 200);

            var panelBottom = new Panel { Dock = DockStyle.Bottom, Height = 50, Padding = new Padding(10) };
            _lblStatus = new Label { Text = "Ready", AutoSize = true, Location = new Point(10, 16) };
            _btnScan = new Button { Text = "Scan MRU", Location = new Point(440, 10), Size = new Size(95, 28) };
            _btnScan.Click += (s, e) => PerformScan();

            _btnClean = new Button { Text = "Purge MRU History", Location = new Point(545, 10), Size = new Size(130, 28) };
            _btnClean.Click += (s, e) => PerformClean();

            _btnClose = new Button { Text = "Close", Location = new Point(680, 10), Size = new Size(65, 28) };
            _btnClose.Click += (s, e) => Close();

            panelBottom.Controls.AddRange(new Control[] { _lblStatus, _btnScan, _btnClean, _btnClose });

            Controls.Add(_listView);
            Controls.Add(panelBottom);
            Controls.Add(panelTop);
        }

        private void PerformScan()
        {
            _listView.Items.Clear();
            var items = MruRecentHistoryCleanerEngine.ScanMruEntries();
            foreach (var itm in items)
            {
                var lvi = new ListViewItem(itm.Description);
                lvi.SubItems.Add(itm.ValueName);
                lvi.SubItems.Add(itm.DataPreview);
                lvi.SubItems.Add(itm.RegistryPath);
                _listView.Items.Add(lvi);
            }
            _lblStatus.Text = $"Found {items.Count} MRU history record(s).";
        }

        private void PerformClean()
        {
            var res = MruRecentHistoryCleanerEngine.PurgeMruHistory();
            MessageBox.Show($"Purged {res.EntriesCleaned} MRU history record(s) across {res.KeysScanned} registry locations.", "MRU Cleaned", MessageBoxButtons.OK, MessageBoxIcon.Information);
            PerformScan();
        }
    }
}
