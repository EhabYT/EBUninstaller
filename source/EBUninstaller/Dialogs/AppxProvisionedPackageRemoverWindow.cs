/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Provisioned AppX Packages Auditor & Remover Dialog
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Drawing;
using System.Windows.Forms;
using UninstallTools.StoreApps;

namespace BulkCrapUninstaller.Forms
{
    public sealed class AppxProvisionedPackageRemoverWindow : Form
    {
        private ListView _listView;
        private Button _btnRefresh;
        private Button _btnRemove;
        private Button _btnClose;
        private Label _lblStatus;

        public AppxProvisionedPackageRemoverWindow()
        {
            InitializeComponent();
            RefreshPackages();
        }

        private void InitializeComponent()
        {
            Text = "Provisioned AppX / MSIX System Packages Deprovisioner - EBUninstaller Pro";
            Size = new Size(780, 450);
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            var panelTop = new Panel { Dock = DockStyle.Top, Height = 55, Padding = new Padding(12) };
            var lblTitle = new Label
            {
                Text = "Provisioned AppX / MSIX Packages Deprovisioner",
                Font = new Font(Font, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(12, 10)
            };
            var lblSub = new Label
            {
                Text = "Deprovisions pre-installed Windows Store applications so they will not reinstall for new or existing user accounts.",
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
            _listView.Columns.Add("Display Name", 220);
            _listView.Columns.Add("Full Package Name", 320);
            _listView.Columns.Add("Version", 110);
            _listView.Columns.Add("Arch", 70);

            var panelBottom = new Panel { Dock = DockStyle.Bottom, Height = 50, Padding = new Padding(10) };
            _lblStatus = new Label { Text = "Ready", AutoSize = true, Location = new Point(10, 16) };
            _btnRefresh = new Button { Text = "Refresh", Location = new Point(410, 10), Size = new Size(80, 28) };
            _btnRefresh.Click += (s, e) => RefreshPackages();

            _btnRemove = new Button { Text = "Deprovision Selected", Location = new Point(495, 10), Size = new Size(160, 28) };
            _btnRemove.Click += (s, e) => RemoveSelected();

            _btnClose = new Button { Text = "Close", Location = new Point(665, 10), Size = new Size(65, 28) };
            _btnClose.Click += (s, e) => Close();

            panelBottom.Controls.AddRange(new Control[] { _lblStatus, _btnRefresh, _btnRemove, _btnClose });

            Controls.Add(_listView);
            Controls.Add(panelBottom);
            Controls.Add(panelTop);
        }

        private void RefreshPackages()
        {
            _listView.Items.Clear();
            var res = AppxProvisionedPackageRemoverEngine.QueryProvisionedPackages();
            foreach (var p in res.Packages)
            {
                var lvi = new ListViewItem(p.DisplayName);
                lvi.SubItems.Add(p.PackageName);
                lvi.SubItems.Add(p.Version);
                lvi.SubItems.Add(p.Architecture);
                _listView.Items.Add(lvi);
            }
            _lblStatus.Text = $"Detected {res.TotalPackagesFound} provisioned AppX package(s).";
        }

        private void RemoveSelected()
        {
            if (_listView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select a provisioned package to deprovision.", "Select Package", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var pkgName = _listView.SelectedItems[0].SubItems[1].Text;
            if (MessageBox.Show($"Are you sure you want to permanently deprovision:\n{pkgName}?", "Confirm Deprovisioning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                var ok = AppxProvisionedPackageRemoverEngine.RemoveProvisionedPackage(pkgName);
                MessageBox.Show(ok ? $"Successfully deprovisioned {pkgName}." : "Deprovisioning command executed (administrative privileges may be required).", "Deprovisioning Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshPackages();
            }
        }
    }
}
