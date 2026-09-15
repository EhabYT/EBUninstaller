/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Modern Application Details Card Control
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using BulkCrapUninstaller.Functions;
using UninstallTools;

namespace BulkCrapUninstaller.Controls
{
    public sealed class ModernAppDetailsCard : UserControl
    {
        private PictureBox _pbIcon;
        private Label _lblTitle;
        private Label _lblPublisherVersion;
        private Label _lblSize;
        private Label _lblArchBadge;
        private Label _lblTypeBadge;
        private Label _lblCertBadge;

        private Button _btnUninstall;
        private Button _btnForced;
        private Button _btnLeftovers;
        private Button _btnOpenDir;

        public event EventHandler<ApplicationUninstallerEntry> RequestUninstall;
        public event EventHandler<ApplicationUninstallerEntry> RequestForcedRemoval;
        public event EventHandler<ApplicationUninstallerEntry> RequestScanLeftovers;
        public event EventHandler<ApplicationUninstallerEntry> RequestOpenFolder;

        private ApplicationUninstallerEntry _currentEntry;

        public ModernAppDetailsCard()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Dock = DockStyle.Bottom;
            Height = 110;
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            Padding = new Padding(12, 8, 12, 8);
            BackColor = Color.FromArgb(248, 249, 250);

            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 1
            };
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 56)); // Icon
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60)); // Info
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40)); // Badges
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // Actions

            // 1. App Icon
            _pbIcon = new PictureBox
            {
                Size = new Size(48, 48),
                SizeMode = PictureBoxSizeMode.CenterImage,
                Anchor = AnchorStyles.Left
            };
            mainLayout.Controls.Add(_pbIcon, 0, 0);

            // 2. Info Panel
            var infoPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = true
            };

            _lblTitle = new Label
            {
                Text = "Select an application to view details",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                AutoSize = true
            };

            _lblPublisherVersion = new Label
            {
                Text = "Publisher: - | Version: - | Installed: -",
                ForeColor = Color.DimGray,
                AutoSize = true
            };

            _lblSize = new Label
            {
                Text = "Estimated Size: - | Install Location: -",
                ForeColor = Color.DimGray,
                AutoSize = true
            };

            infoPanel.Controls.AddRange(new Control[] { _lblTitle, _lblPublisherVersion, _lblSize });
            mainLayout.Controls.Add(infoPanel, 1, 0);

            // 3. Badges Panel
            var badgesPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                AutoSize = true
            };

            _lblArchBadge = CreateBadge("64-bit", Color.FromArgb(0, 120, 215));
            _lblTypeBadge = CreateBadge("Standard Installer", Color.FromArgb(108, 117, 125));
            _lblCertBadge = CreateBadge("Authenticode Signed", Color.FromArgb(40, 167, 69));

            badgesPanel.Controls.AddRange(new Control[] { _lblArchBadge, _lblTypeBadge, _lblCertBadge });
            mainLayout.Controls.Add(badgesPanel, 2, 0);

            // 4. Action Buttons
            var actionPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false,
                AutoSize = true
            };

            _btnUninstall = new Button
            {
                Text = "⚡ Uninstall",
                Font = new Font(Font, FontStyle.Bold),
                ForeColor = Color.DarkSlateBlue,
                FlatStyle = FlatStyle.Flat,
                Height = 32,
                AutoSize = true
            };
            _btnUninstall.Click += (s, e) => { if (_currentEntry != null) RequestUninstall?.Invoke(this, _currentEntry); };

            _btnForced = new Button
            {
                Text = "🛑 Forced Removal",
                FlatStyle = FlatStyle.Flat,
                Height = 32,
                AutoSize = true
            };
            _btnForced.Click += (s, e) => { if (_currentEntry != null) RequestForcedRemoval?.Invoke(this, _currentEntry); };

            _btnLeftovers = new Button
            {
                Text = "🔍 Leftovers",
                FlatStyle = FlatStyle.Flat,
                Height = 32,
                AutoSize = true
            };
            _btnLeftovers.Click += (s, e) => { if (_currentEntry != null) RequestScanLeftovers?.Invoke(this, _currentEntry); };

            _btnOpenDir = new Button
            {
                Text = "📁 Open Folder",
                FlatStyle = FlatStyle.Flat,
                Height = 32,
                AutoSize = true
            };
            _btnOpenDir.Click += (s, e) => { if (_currentEntry != null) RequestOpenFolder?.Invoke(this, _currentEntry); };

            actionPanel.Controls.AddRange(new Control[] { _btnUninstall, _btnForced, _btnLeftovers, _btnOpenDir });
            mainLayout.Controls.Add(actionPanel, 3, 0);

            Controls.Add(mainLayout);
            SetEmptyState();
        }

        private static Label CreateBadge(string text, Color backColor)
        {
            return new Label
            {
                Text = text,
                BackColor = backColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                Padding = new Padding(6, 2, 6, 2),
                Margin = new Padding(2, 4, 2, 4),
                AutoSize = true
            };
        }

        public void SetApplication(ApplicationUninstallerEntry entry)
        {
            _currentEntry = entry;
            if (entry == null)
            {
                SetEmptyState();
                return;
            }

            _lblTitle.Text = entry.DisplayName ?? "Unknown Application";
            _lblPublisherVersion.Text = $"Publisher: {entry.PublisherTrimmed ?? "Unknown"} | Version: {entry.DisplayVersion ?? "N/A"} | Installed: {(entry.InstallDate.Year > 2000 ? entry.InstallDate.ToString("yyyy-MM-dd") : "Unknown")}";
            _lblSize.Text = $"Estimated Size: {entry.EstimatedSize} | Location: {(string.IsNullOrEmpty(entry.InstallLocation) ? "Registry / System Root" : entry.InstallLocation)}";

            _lblArchBadge.Text = entry.Is64Bit ? "64-bit (x64)" : "32-bit (x86)";
            _lblArchBadge.BackColor = entry.Is64Bit ? Color.FromArgb(0, 120, 215) : Color.FromArgb(108, 117, 125);

            _lblTypeBadge.Text = entry.UninstallerKind.ToString();
            _lblCertBadge.Text = entry.IsCertificateValid ? "Authenticode Verified" : "Unsigned / Unknown Cert";
            _lblCertBadge.BackColor = entry.IsCertificateValid ? Color.FromArgb(40, 167, 69) : Color.FromArgb(230, 81, 0);

            _btnUninstall.Enabled = true;
            _btnForced.Enabled = true;
            _btnLeftovers.Enabled = true;
            _btnOpenDir.Enabled = !string.IsNullOrEmpty(entry.InstallLocation) && Directory.Exists(entry.InstallLocation);
        }

        public void SetEmptyState()
        {
            _currentEntry = null;
            _lblTitle.Text = "No application selected";
            _lblPublisherVersion.Text = "Select an application from the list to view detailed properties and actions.";
            _lblSize.Text = "EBUninstaller Pro Discovery Engine";
            _lblArchBadge.Text = "x64/x86";
            _lblTypeBadge.Text = "All Types";
            _lblCertBadge.Text = "Signature";

            _btnUninstall.Enabled = false;
            _btnForced.Enabled = false;
            _btnLeftovers.Enabled = false;
            _btnOpenDir.Enabled = false;
        }
    }
}
