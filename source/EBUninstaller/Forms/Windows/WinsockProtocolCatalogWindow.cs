/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Winsock Protocol Catalog Auditor Window
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BrightIdeasSoftware;
using UninstallTools.SystemTools;

namespace BulkCrapUninstaller.Forms.Windows
{
    public sealed class WinsockProtocolCatalogWindow : Form
    {
        private FastObjectListView _folvEntries;
        private Label _lblSummary;
        private Button _btnRefresh;
        private Button _btnClose;
        private List<WinsockProtocolEntry> _items = new();

        public WinsockProtocolCatalogWindow()
        {
            InitializeComponent();
            ScanCatalog();
        }

        private void InitializeComponent()
        {
            Text = "Winsock Protocol Catalog & Transport Provider Auditor - EBUninstaller Pro";
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

            _folvEntries = new FastObjectListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = false,
                GridLines = true
            };

            var colId = new OLVColumn("Entry ID", nameof(WinsockProtocolEntry.EntryId)) { Width = 110 };
            var colName = new OLVColumn("Protocol / Provider Name", nameof(WinsockProtocolEntry.ProtocolName)) { Width = 260 };
            var colStatus = new OLVColumn("Health Status", nameof(WinsockProtocolEntry.HealthStatus)) { Width = 190 };
            var colDll = new OLVColumn("Provider DLL / Binary Path", nameof(WinsockProtocolEntry.ProviderDllPath)) { Width = 380, FillsFreeSpace = true };

            _folvEntries.AllColumns.AddRange(new[] { colId, colName, colStatus, colDll });
            _folvEntries.RebuildColumns();

            mainLayout.Controls.Add(_folvEntries, 0, 0);

            // Summary
            _lblSummary = new Label
            {
                Text = "Analyzing Winsock2 Protocol Catalog transport and base service providers...",
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
            _btnRefresh = new Button { Text = "Refresh Catalog", AutoSize = true };
            _btnRefresh.Click += (s, e) => ScanCatalog();

            btnPanel.Controls.Add(_btnClose);
            btnPanel.Controls.Add(_btnRefresh);
            mainLayout.Controls.Add(btnPanel, 0, 2);

            Controls.Add(mainLayout);
        }

        private void ScanCatalog()
        {
            _items = WinsockProtocolCatalogAuditorEngine.ScanProtocolCatalog();
            _folvEntries.SetObjects(_items);

            var missingCount = _items.Count(i => i.IsDllMissing);
            var validCount = _items.Count(i => !i.IsDllMissing);

            if (missingCount == 0)
            {
                _lblSummary.Text = $"All {validCount} Winsock protocol catalog entries are intact. Transport providers verified.";
                _lblSummary.ForeColor = Color.DarkGreen;
            }
            else
            {
                _lblSummary.Text = $"Detected {missingCount} orphaned protocol entry/entries with missing DLLs (Potential network latency or reset issues).";
                _lblSummary.ForeColor = Color.DarkOrange;
            }
        }
    }
}
