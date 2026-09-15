/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    NetBIOS & WINS Name Cache Flusher Window
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
    public sealed class NetBiosCacheFlusherWindow : Form
    {
        private FastObjectListView _folvNames;
        private Label _lblSummary;
        private Button _btnFlush;
        private Button _btnRefresh;
        private Button _btnClose;
        private List<NetBiosNameEntry> _items = new();

        public NetBiosCacheFlusherWindow()
        {
            InitializeComponent();
            ScanNetBios();
        }

        private void InitializeComponent()
        {
            Text = "NetBIOS & WINS Name Resolution Cache Flusher - EBUninstaller Pro";
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

            _folvNames = new FastObjectListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = false,
                GridLines = true
            };

            var colName = new OLVColumn("NetBIOS Host Name", nameof(NetBiosNameEntry.NetBiosName)) { Width = 230 };
            var colIp = new OLVColumn("Cached IP Address", nameof(NetBiosNameEntry.IpAddress)) { Width = 180 };
            var colType = new OLVColumn("Entry Record Type", nameof(NetBiosNameEntry.EntryType)) { Width = 160 };
            var colScope = new OLVColumn("Scope", nameof(NetBiosNameEntry.Scope)) { Width = 230, FillsFreeSpace = true };

            _folvNames.AllColumns.AddRange(new[] { colName, colIp, colType, colScope });
            _folvNames.RebuildColumns();

            mainLayout.Controls.Add(_folvNames, 0, 0);

            // Summary
            _lblSummary = new Label
            {
                Text = "Analyzing Windows NetBT NetBIOS & WINS name resolution cache table...",
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
            _btnRefresh = new Button { Text = "Refresh Table", AutoSize = true };
            _btnRefresh.Click += (s, e) => ScanNetBios();

            _btnFlush = new Button
            {
                Text = "Purge & Flush NetBIOS Cache",
                AutoSize = true,
                Font = new Font(Font, FontStyle.Bold),
                ForeColor = Color.DarkSlateBlue
            };
            _btnFlush.Click += OnFlushClick;

            btnPanel.Controls.Add(_btnClose);
            btnPanel.Controls.Add(_btnRefresh);
            btnPanel.Controls.Add(_btnFlush);
            mainLayout.Controls.Add(btnPanel, 0, 2);

            Controls.Add(mainLayout);
        }

        private void ScanNetBios()
        {
            _items = NetBiosCacheFlusherEngine.QueryNetBiosCache();
            _folvNames.SetObjects(_items);

            _lblSummary.Text = $"Detected {_items.Count} cached NetBIOS/WINS name entry/entries in local network resolver table.";
            _lblSummary.ForeColor = Color.DarkSlateBlue;
        }

        private void OnFlushClick(object sender, EventArgs e)
        {
            var success = NetBiosCacheFlusherEngine.FlushAndReloadNetBios();
            if (success)
            {
                MessageBox.Show("NetBIOS and WINS remote cache purged and local registrations refreshed.", "Cache Flushed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ScanNetBios();
            }
            else
            {
                MessageBox.Show("Failed flushing NetBIOS cache.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
