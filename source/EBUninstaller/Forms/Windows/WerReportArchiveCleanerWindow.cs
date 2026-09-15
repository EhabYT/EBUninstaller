/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    WER ReportArchive & Application Hang Diagnostics Cleaner Window
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BrightIdeasSoftware;
using UninstallTools.JunkCleaner;

namespace BulkCrapUninstaller.Forms.Windows
{
    public sealed class WerReportArchiveCleanerWindow : Form
    {
        private FastObjectListView _folvReports;
        private Label _lblSummary;
        private Button _btnClean;
        private Button _btnRefresh;
        private Button _btnClose;
        private List<WerReportArchiveItem> _items = new();

        public WerReportArchiveCleanerWindow()
        {
            InitializeComponent();
            ScanReports();
        }

        private void InitializeComponent()
        {
            Text = "Windows Error Reporting (WER) ReportArchive Cleaner - EBUninstaller Pro";
            Size = new Size(950, 480);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(720, 380);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(12)
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // List
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Summary
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Buttons

            _folvReports = new FastObjectListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = true,
                GridLines = true
            };

            var colApp = new OLVColumn("Crashing Application", nameof(WerReportArchiveItem.ApplicationName)) { Width = 230 };
            var colType = new OLVColumn("Report Type", nameof(WerReportArchiveItem.EventType)) { Width = 120 };
            var colSize = new OLVColumn("Report Size", nameof(WerReportArchiveItem.SizeBytes))
            {
                Width = 110,
                AspectToStringConverter = v => FormatBytes((long)v)
            };
            var colDate = new OLVColumn("Timestamp", nameof(WerReportArchiveItem.LastModified))
            {
                Width = 130,
                AspectToStringConverter = v => ((DateTime)v).ToString("yyyy-MM-dd HH:mm")
            };
            var colFolder = new OLVColumn("Archive Store Path", nameof(WerReportArchiveItem.ReportFolder)) { Width = 300, FillsFreeSpace = true };

            _folvReports.AllColumns.AddRange(new[] { colApp, colType, colSize, colDate, colFolder });
            _folvReports.RebuildColumns();

            mainLayout.Controls.Add(_folvReports, 0, 0);

            // Summary
            _lblSummary = new Label
            {
                Text = "Scanning Windows Error Reporting (WER) ReportArchive and ReportQueue stores...",
                AutoSize = true,
                Margin = new Padding(0, 8, 0, 8),
                Font = new Font(Font, FontStyle.Bold)
            };
            mainLayout.Controls.Add(_lblSummary, 0, 1);

            // Buttons
            var btnPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                AutoSize = true
            };

            _btnClose = new Button { Text = "Close", DialogResult = DialogResult.OK, AutoSize = true };
            _btnRefresh = new Button { Text = "Refresh Scan", AutoSize = true };
            _btnRefresh.Click += (s, e) => ScanReports();

            _btnClean = new Button
            {
                Text = "Purge Selected WER Reports",
                AutoSize = true,
                Font = new Font(Font, FontStyle.Bold),
                ForeColor = Color.DarkSlateBlue
            };
            _btnClean.Click += OnCleanClick;

            btnPanel.Controls.Add(_btnClose);
            btnPanel.Controls.Add(_btnRefresh);
            btnPanel.Controls.Add(_btnClean);
            mainLayout.Controls.Add(btnPanel, 0, 2);

            Controls.Add(mainLayout);
        }

        private void ScanReports()
        {
            _items = WerReportArchiveCleanerEngine.ScanWerArchives();
            _folvReports.SetObjects(_items);

            var totalBytes = _items.Sum(i => i.SizeBytes);

            if (_items.Count == 0)
            {
                _lblSummary.Text = "No WER ReportArchive or crash queue files found. Crash reports are clean.";
                _lblSummary.ForeColor = Color.DarkGreen;
                _btnClean.Enabled = false;
            }
            else
            {
                _lblSummary.Text = $"Detected {_items.Count} WER crash/hang report store(s) taking up {FormatBytes(totalBytes)}.";
                _lblSummary.ForeColor = Color.DarkOrange;
                _btnClean.Enabled = true;
            }
        }

        private void OnCleanClick(object sender, EventArgs e)
        {
            var selected = _folvReports.SelectedObjects.Count > 0 ? _folvReports.SelectedObjects.Cast<WerReportArchiveItem>().ToList() : _items;
            if (selected.Count == 0) return;

            var confirm = MessageBox.Show(
                $"Delete {selected.Count} WER crash report folder(s)?",
                "Confirm Deletion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                var (folders, bytes) = WerReportArchiveCleanerEngine.PurgeWerArchives(selected);
                MessageBox.Show($"Purged {folders} WER report folder(s) (Freed {FormatBytes(bytes)}).", "Cleanup Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ScanReports();
            }
        }

        private static string FormatBytes(long bytes)
        {
            if (bytes <= 0) return "0 B";
            if (bytes < 1024) return $"{bytes} B";
            if (bytes < 1024 * 1024) return $"{bytes / 1024.0:F1} KB";
            return $"{bytes / (1024.0 * 1024.0):F2} MB";
        }
    }
}
