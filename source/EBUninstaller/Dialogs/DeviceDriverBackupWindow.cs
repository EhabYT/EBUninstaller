/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Device Driver Discovery & Export Backup Dialog
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using UninstallTools.SystemTools;

namespace BulkCrapUninstaller.Forms
{
    public sealed class DeviceDriverBackupWindow : Form
    {
        private ListView _listView;
        private Button _btnRefresh;
        private Button _btnExport;
        private Button _btnClose;
        private Label _lblStatus;

        public DeviceDriverBackupWindow()
        {
            InitializeComponent();
            RefreshDrivers();
        }

        private void InitializeComponent()
        {
            Text = "OEM & Device Driver Backup Manager - EBUninstaller Pro";
            Size = new Size(740, 450);
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            var panelTop = new Panel { Dock = DockStyle.Top, Height = 55, Padding = new Padding(12) };
            var lblTitle = new Label
            {
                Text = "Installed OEM Device Driver Backup",
                Font = new Font(Font, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(12, 10)
            };
            var lblSub = new Label
            {
                Text = "Discovers all third-party OEM hardware drivers and creates portable INF/SYS driver backups prior to system changes.",
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
            _listView.Columns.Add("Driver INF", 110);
            _listView.Columns.Add("Provider / Vendor", 170);
            _listView.Columns.Add("Device Class", 150);
            _listView.Columns.Add("Version / Date", 250);

            var panelBottom = new Panel { Dock = DockStyle.Bottom, Height = 50, Padding = new Padding(10) };
            _lblStatus = new Label { Text = "Ready", AutoSize = true, Location = new Point(10, 16) };
            _btnRefresh = new Button { Text = "Refresh", Location = new Point(410, 10), Size = new Size(80, 28) };
            _btnRefresh.Click += (s, e) => RefreshDrivers();

            _btnExport = new Button { Text = "Export All Drivers...", Location = new Point(495, 10), Size = new Size(145, 28) };
            _btnExport.Click += (s, e) => ExportDrivers();

            _btnClose = new Button { Text = "Close", Location = new Point(645, 10), Size = new Size(70, 28) };
            _btnClose.Click += (s, e) => Close();

            panelBottom.Controls.AddRange(new Control[] { _lblStatus, _btnRefresh, _btnExport, _btnClose });

            Controls.Add(_listView);
            Controls.Add(panelBottom);
            Controls.Add(panelTop);
        }

        private void RefreshDrivers()
        {
            _listView.Items.Clear();
            var drivers = DeviceDriverBackupEngine.QueryThirdPartyDrivers();
            foreach (var drv in drivers)
            {
                var lvi = new ListViewItem(drv.DriverInfName);
                lvi.SubItems.Add(drv.ProviderName);
                lvi.SubItems.Add(drv.ClassName);
                lvi.SubItems.Add(drv.DriverVersion);
                _listView.Items.Add(lvi);
            }
            _lblStatus.Text = $"Detected {drivers.Count} third-party OEM driver package(s).";
        }

        private void ExportDrivers()
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Select a folder to export driver backup files:";
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    _lblStatus.Text = "Exporting drivers, please wait...";
                    Application.DoEvents();

                    var res = DeviceDriverBackupEngine.ExportDrivers(fbd.SelectedPath);
                    if (res.Success || res.ExportedDriversCount > 0)
                    {
                        MessageBox.Show($"Successfully exported {res.ExportedDriversCount} driver(s) to:\n{fbd.SelectedPath}", "Driver Backup Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Export finished. Some drivers may require administrative elevation.", "Export Status", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    _lblStatus.Text = "Driver export complete.";
                }
            }
        }
    }
}
