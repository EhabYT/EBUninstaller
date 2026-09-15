/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Storage Sense Policy Auditor Window
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Drawing;
using System.Windows.Forms;
using UninstallTools.SystemTools;

namespace BulkCrapUninstaller.Forms.Windows
{
    public sealed class StorageSensePolicyAuditorWindow : Form
    {
        private CheckBox _cbEnable;
        private ComboBox _cmbRecycle;
        private ComboBox _cmbDownloads;
        private Label _lblSummary;
        private Button _btnApply;
        private Button _btnRefresh;
        private Button _btnClose;
        private StorageSensePolicyInfo _policy;

        public StorageSensePolicyAuditorWindow()
        {
            InitializeComponent();
            RefreshPolicy();
        }

        private void InitializeComponent()
        {
            Text = "Windows Storage Sense & Downloads Folder Policy - EBUninstaller Pro";
            Size = new Size(800, 360);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(16) };

            var lblHeader = new Label
            {
                Text = "Windows Storage Sense & User Downloads Folder Retention",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Location = new Point(16, 16),
                AutoSize = true
            };

            var lblDesc = new Label
            {
                Text = "Windows Storage Sense automatically cleans temporary files. However, misconfigured policies\ncan silently delete installers and documents in your Downloads folder.\nConfigure retention thresholds safely here.",
                Location = new Point(16, 46),
                Size = new Size(740, 50)
            };

            var group = new GroupBox
            {
                Text = "Storage Sense Retention Policy",
                Location = new Point(16, 105),
                Size = new Size(740, 110)
            };

            _cbEnable = new CheckBox
            {
                Text = "Enable Windows Storage Sense automated background maintenance",
                Location = new Point(16, 26),
                AutoSize = true
            };

            var lblRec = new Label { Text = "Recycle Bin Retention:", Location = new Point(16, 62), AutoSize = true };
            _cmbRecycle = new ComboBox { Location = new Point(160, 58), Width = 160, DropDownStyle = ComboBoxStyle.DropDownList };
            _cmbRecycle.Items.AddRange(new object[] { "Never delete", "1 day", "14 days", "30 days (Default)", "60 days" });

            var lblDl = new Label { Text = "Downloads Folder Retention:", Location = new Point(350, 62), AutoSize = true };
            _cmbDownloads = new ComboBox { Location = new Point(530, 58), Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };
            _cmbDownloads.Items.AddRange(new object[] { "Never delete (Recommended)", "1 day", "14 days", "30 days", "60 days" });

            group.Controls.AddRange(new Control[] { _cbEnable, lblRec, _cmbRecycle, lblDl, _cmbDownloads });

            _lblSummary = new Label
            {
                Location = new Point(16, 230),
                AutoSize = true,
                Font = new Font(Font, FontStyle.Bold),
                Text = "Policy Status: Loading..."
            };

            _btnApply = new Button
            {
                Text = "Apply Policy",
                Font = new Font(Font, FontStyle.Bold),
                ForeColor = Color.DarkSlateBlue,
                Location = new Point(480, 265),
                Size = new Size(130, 32)
            };
            _btnApply.Click += OnApplyClick;

            _btnRefresh = new Button
            {
                Text = "Refresh",
                Location = new Point(620, 265),
                Size = new Size(80, 32)
            };
            _btnRefresh.Click += (s, e) => RefreshPolicy();

            _btnClose = new Button
            {
                Text = "Close",
                DialogResult = DialogResult.OK,
                Location = new Point(710, 265),
                Size = new Size(75, 32)
            };

            panel.Controls.AddRange(new Control[] { lblHeader, lblDesc, group, _lblSummary, _btnApply, _btnRefresh, _btnClose });
            Controls.Add(panel);
        }

        private void RefreshPolicy()
        {
            _policy = StorageSensePolicyAuditorEngine.QueryStorageSensePolicy();
            _cbEnable.Checked = _policy.IsStorageSenseEnabled;

            _cmbRecycle.SelectedIndex = _policy.PurgeRecycleBinDays switch
            {
                0 => 0,
                1 => 1,
                14 => 2,
                60 => 4,
                _ => 3
            };

            _cmbDownloads.SelectedIndex = _policy.PurgeDownloadsDays switch
            {
                1 => 1,
                14 => 2,
                30 => 3,
                60 => 4,
                _ => 0
            };

            _lblSummary.Text = $"Downloads Policy: {_policy.DownloadsPolicyDescription}";
            _lblSummary.ForeColor = _policy.PurgeDownloadsDays > 0 ? Color.DarkOrange : Color.DarkGreen;
        }

        private void OnApplyClick(object sender, EventArgs e)
        {
            int recDays = _cmbRecycle.SelectedIndex switch
            {
                0 => 0,
                1 => 1,
                2 => 14,
                4 => 60,
                _ => 30
            };

            int dlDays = _cmbDownloads.SelectedIndex switch
            {
                1 => 1,
                2 => 14,
                3 => 30,
                4 => 60,
                _ => 0
            };

            var success = StorageSensePolicyAuditorEngine.ApplyStorageSensePolicy(_cbEnable.Checked, recDays, dlDays);
            if (success)
            {
                MessageBox.Show("Storage Sense policy updated successfully.", "Policy Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshPolicy();
            }
            else
            {
                MessageBox.Show("Failed applying Storage Sense policy.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
