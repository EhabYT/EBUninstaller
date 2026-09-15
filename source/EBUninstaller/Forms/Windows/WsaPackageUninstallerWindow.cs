/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Windows Subsystem for Android (WSA) Package Uninstaller Window
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BrightIdeasSoftware;
using UninstallTools.StoreApps;

namespace BulkCrapUninstaller.Forms.Windows
{
    public sealed class WsaPackageUninstallerWindow : Form
    {
        private FastObjectListView _folvPackages;
        private Label _lblSummary;
        private Button _btnClean;
        private Button _btnRefresh;
        private Button _btnClose;
        private List<WsaPackageItem> _items = new();

        public WsaPackageUninstallerWindow()
        {
            InitializeComponent();
            ScanPackages();
        }

        private void InitializeComponent()
        {
            Text = "Windows Subsystem for Android (WSA) Package Manager - EBUninstaller Pro";
            Size = new Size(1000, 500);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(780, 400);
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

            var colName = new OLVColumn("Android Package Name", nameof(WsaPackageItem.DisplayName)) { Width = 260 };
            var colPub = new OLVColumn("Developer / Publisher", nameof(WsaPackageItem.Publisher)) { Width = 180 };
            var colStatus = new OLVColumn("Status", nameof(WsaPackageItem.IsOrphaned))
            {
                Width = 130,
                AspectToStringConverter = v => (bool)v ? "Orphaned (Missing APK)" : "Installed Package"
            };
            var colId = new OLVColumn("Package Identity", nameof(WsaPackageItem.PackageId)) { Width = 380, FillsFreeSpace = true };

            _folvPackages.AllColumns.AddRange(new[] { colName, colPub, colStatus, colId });
            _folvPackages.RebuildColumns();

            mainLayout.Controls.Add(_folvPackages, 0, 0);

            // Summary
            _lblSummary = new Label
            {
                Text = "Analyzing Windows Subsystem for Android (WSA) package registrations...",
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
            _btnRefresh = new Button { Text = "Refresh Packages", AutoSize = true };
            _btnRefresh.Click += (s, e) => ScanPackages();

            _btnClean = new Button
            {
                Text = "Remove Selected Android Packages",
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

        private void ScanPackages()
        {
            _items = WsaPackageUninstallerEngine.ScanWsaPackages();
            _folvPackages.SetObjects(_items);

            var orphanCount = _items.Count(i => i.IsOrphaned);

            if (_items.Count == 0)
            {
                _lblSummary.Text = "No Windows Subsystem for Android (WSA) packages detected. Android environment is clean.";
                _lblSummary.ForeColor = Color.DarkGreen;
                _btnClean.Enabled = false;
            }
            else
            {
                _lblSummary.Text = $"Detected {_items.Count} Android package(s) registered with Windows ({orphanCount} orphaned package registrations).";
                _lblSummary.ForeColor = orphanCount > 0 ? Color.DarkOrange : Color.DarkGreen;
                _btnClean.Enabled = true;
            }
        }

        private void OnCleanClick(object sender, EventArgs e)
        {
            var selected = _folvPackages.SelectedObjects.Count > 0 ? _folvPackages.SelectedObjects.Cast<WsaPackageItem>().ToList() : _items;
            if (selected.Count == 0) return;

            var confirm = MessageBox.Show(
                $"Remove {selected.Count} selected WSA Android package registration(s)?",
                "Confirm Removal",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                var cleaned = WsaPackageUninstallerEngine.CleanWsaPackages(selected);
                MessageBox.Show($"Removed {cleaned} Android package registration(s).", "Packages Removed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ScanPackages();
            }
        }
    }
}
