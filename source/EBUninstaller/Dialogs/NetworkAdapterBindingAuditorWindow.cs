/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Network Adapter NDIS Filter & Protocol Binding Auditor Dialog
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Drawing;
using System.Windows.Forms;
using UninstallTools.SystemTools;

namespace BulkCrapUninstaller.Forms
{
    public sealed class NetworkAdapterBindingAuditorWindow : Form
    {
        private ListView _listView;
        private Button _btnScan;
        private Button _btnClose;
        private Label _lblStatus;

        public NetworkAdapterBindingAuditorWindow()
        {
            InitializeComponent();
            PerformScan();
        }

        private void InitializeComponent()
        {
            Text = "Network NDIS Protocol & Filter Driver Auditor - EBUninstaller Pro";
            Size = new Size(760, 440);
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            var panelTop = new Panel { Dock = DockStyle.Top, Height = 55, Padding = new Padding(12) };
            var lblTitle = new Label
            {
                Text = "Network Adapter NDIS Filter Drivers & Protocol Bindings",
                Font = new Font(Font, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(12, 10)
            };
            var lblSub = new Label
            {
                Text = "Audits third-party VPN, packet capture, and virtualization filter drivers bound to physical network adapters.",
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
            _listView.Columns.Add("Driver / Service Description", 280);
            _listView.Columns.Add("Component ID", 180);
            _listView.Columns.Add("Binding Class", 150);
            _listView.Columns.Add("INF Source", 120);

            var panelBottom = new Panel { Dock = DockStyle.Bottom, Height = 50, Padding = new Padding(10) };
            _lblStatus = new Label { Text = "Ready", AutoSize = true, Location = new Point(10, 16) };
            _btnScan = new Button { Text = "Scan Bindings", Location = new Point(560, 10), Size = new Size(110, 28) };
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
            var res = NetworkAdapterBindingAuditorEngine.ScanNetworkBindings();
            foreach (var itm in res.Items)
            {
                var lvi = new ListViewItem(itm.Description);
                lvi.SubItems.Add(itm.ComponentId);
                lvi.SubItems.Add(itm.ServiceType);
                lvi.SubItems.Add(itm.InfPath);
                if (itm.IsOrphanedOrSuspicious)
                {
                    lvi.ForeColor = Color.DarkOrange;
                }
                _listView.Items.Add(lvi);
            }
            _lblStatus.Text = $"Found {res.TotalBindingsFound} protocol bindings ({res.FilterDriversCount} NDIS filter drivers).";
        }
    }
}
