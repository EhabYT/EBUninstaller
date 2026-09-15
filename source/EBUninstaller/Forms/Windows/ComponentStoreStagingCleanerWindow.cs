/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Component Store Staging & Servicing Cache Cleaner Window
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
    public sealed class ComponentStoreStagingCleanerWindow : Form
    {
        private FastObjectListView _folvPackages;
        private Label _lblSummary;
        private Button _btnClean;
        private Button _btnRefresh;
        private Button _btnClose;
        private List<StagingPackageItem> _items = new();

        public ComponentStoreStagingCleanerWindow()
        {
            InitializeComponent();
            ScanStaging();
        }

        private void InitializeComponent()
        {
            Text = "Component Store Staging & CBS Servicing Cache Cleaner - EBUninstaller Pro";
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

            _folvPackages = new FastObjectListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = true,
                GridLines = true
            };

            var colFile = new OLVColumn("Staging File Name", nameof(StagingPackageItem.FileName)) { Width = 230 };
            var colType = new OLVColumn("Package Classification", nameof(StagingPackageItem.PackageType)) { Width = 190 };
            var colSize = new OLVColumn("File Size", nameof(StagingPackageItem.SizeBytes))
            {
                Width = 110,
                AspectToStringConverter = v => FormatBytes((long)v)
            };
            var colCreated = new OLVColumn("Creation Time", nameof(StagingPackageItem.CreationTime))
            {
                Width = 130,
                AspectToStringConverter = v => ((DateTime)v).ToString("yyyy-MM-dd HH:mm")
            };
            var colPath = new OLVColumn("Disk Location", nameof(StagingPackageItem.FilePath)) { Width = 270, FillsFreeSpace = true };

            _folvPackages.AllColumns.AddRange(new[] { colFile, colType, colSize, colCreated, colPath });
            _folvPackages.RebuildColumns();

            mainLayout.Controls.Add(_folvPackages, 0, 0);

            // Summary
            _lblSummary = new Label
            {
                Text = "Analyzing Windows Servicing and Update download staging cache...",
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
            _btnRefresh.Click += (s, e) => ScanStaging();

            _btnClean = new Button
            {
                Text = "Purge Selected Staging Caches",
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

        private void ScanStaging()
        {
            _items = ComponentStoreStagingCleanerEngine.ScanStagingPackages();
            _folvPackages.SetObjects(_items);

            var totalBytes = _items.Sum(i => i.SizeBytes);

            if (_items.Count == 0)
            {
                _lblSummary.Text = "No staging update payloads or CBS logs detected. Servicing cache is clear.";
                _lblSummary.ForeColor = Color.DarkGreen;
                _btnClean.Enabled = false;
            }
            else
            {
                _lblSummary.Text = $"Detected {_items.Count} staging update file(s) and servicing log(s) taking up {FormatBytes(totalBytes)}.";
                _lblSummary.ForeColor = Color.DarkOrange;
                _btnClean.Enabled = true;
            }
        }

        private void OnCleanClick(object sender, EventArgs e)
        {
            var selected = _folvPackages.SelectedObjects.Count > 0 ? _folvPackages.SelectedObjects.Cast<StagingPackageItem>().ToList() : _items;
            if (selected.Count == 0) return;

            var confirm = MessageBox.Show(
                $"Purge {selected.Count} staging cache file(s)?",
                "Confirm Staging Purge",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                var (files, bytes) = ComponentStoreStagingCleanerEngine.PurgeStagingPackages(selected);
                MessageBox.Show($"Purged {files} staging file(s) (Freed {FormatBytes(bytes)}).", "Staging Purge Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ScanStaging();
            }
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
