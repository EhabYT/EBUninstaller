/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Windows Update Rollback & Servicing Health Dialog
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Drawing;
using System.Windows.Forms;
using UninstallTools.SystemTools;

namespace BulkCrapUninstaller.Forms
{
    public sealed class WindowsUpdateRollbackHealthWindow : Form
    {
        private TextBox _txtReport;
        private Button _btnAnalyze;
        private Button _btnClose;
        private Label _lblStatus;

        public WindowsUpdateRollbackHealthWindow()
        {
            InitializeComponent();
            PerformAnalysis();
        }

        private void InitializeComponent()
        {
            Text = "Windows Update Rollback & Servicing Health - EBUninstaller Pro";
            Size = new Size(680, 420);
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            var panelTop = new Panel { Dock = DockStyle.Top, Height = 55, Padding = new Padding(12) };
            var lblTitle = new Label
            {
                Text = "Windows Servicing & Update Rollback Subsystem",
                Font = new Font(Font, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(12, 10)
            };
            var lblSub = new Label
            {
                Text = "Analyzes WinSxS component store health, superseded update packages, and rollback status.",
                ForeColor = Color.DimGray,
                AutoSize = true,
                Location = new Point(12, 30)
            };
            panelTop.Controls.AddRange(new Control[] { lblTitle, lblSub });

            _txtReport = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Consolas", 9.5F, FontStyle.Regular, GraphicsUnit.Point),
                BackColor = Color.FromArgb(250, 250, 250)
            };

            var panelBottom = new Panel { Dock = DockStyle.Bottom, Height = 50, Padding = new Padding(10) };
            _lblStatus = new Label { Text = "Ready", AutoSize = true, Location = new Point(10, 16) };
            _btnAnalyze = new Button { Text = "Analyze Servicing Health", Location = new Point(440, 10), Size = new Size(160, 28) };
            _btnAnalyze.Click += (s, e) => PerformAnalysis();

            _btnClose = new Button { Text = "Close", Location = new Point(605, 10), Size = new Size(60, 28) };
            _btnClose.Click += (s, e) => Close();

            panelBottom.Controls.AddRange(new Control[] { _lblStatus, _btnAnalyze, _btnClose });

            Controls.Add(_txtReport);
            Controls.Add(panelBottom);
            Controls.Add(panelTop);
        }

        private void PerformAnalysis()
        {
            _lblStatus.Text = "Analyzing servicing component store...";
            Application.DoEvents();

            var rep = WindowsUpdateRollbackHealthEngine.AnalyzeServicingHealth();
            _txtReport.Text = rep.RawDismOutput;
            _lblStatus.Text = rep.ComponentStoreCleanupRecommended ? "Component store cleanup recommended." : "Servicing state analyzed.";
        }
    }
}
