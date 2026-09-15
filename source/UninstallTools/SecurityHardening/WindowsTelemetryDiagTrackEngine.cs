/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Connected User Experiences & Telemetry (DiagTrack) Hardening Subsystem
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Win32;

namespace UninstallTools.SecurityHardening
{
    public sealed class TelemetryServiceItem
    {
        public string ServiceName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public sealed class TelemetryHardeningResult
    {
        public int ServicesAdjusted { get; set; }
        public int RegistryPoliciesSet { get; set; }
        public List<string> Messages { get; set; } = new List<string>();
        public bool Success { get; set; }
    }

    public sealed class WindowsTelemetryDiagTrackEngine
    {
        public static List<TelemetryServiceItem> GetTelemetryServices()
        {
            var list = new List<TelemetryServiceItem>
            {
                new TelemetryServiceItem
                {
                    ServiceName = "DiagTrack",
                    DisplayName = "Connected User Experiences and Telemetry",
                    Description = "Collects system diagnostics and usage data for Microsoft telemetry servers."
                },
                new TelemetryServiceItem
                {
                    ServiceName = "dmwappushservice",
                    DisplayName = "Device Management Wireless Application Protocol (WAP) Push",
                    Description = "Routing Service for telemetry notifications and push messages."
                },
                new TelemetryServiceItem
                {
                    ServiceName = "diagnosticshub.standardcollector.service",
                    DisplayName = "Microsoft (R) Diagnostics Hub Standard Collector Service",
                    Description = "Real-time diagnostic and profiling event collector service."
                }
            };

            foreach (var item in list)
            {
                try
                {
                    using (var scKey = Registry.LocalMachine.OpenSubKey($@"SYSTEM\CurrentControlSet\Services\{item.ServiceName}"))
                    {
                        if (scKey != null)
                        {
                            var startVal = scKey.GetValue("Start");
                            item.IsActive = startVal != null && (int)startVal != 4; // 4 = Disabled
                        }
                    }
                }
                catch { }
            }

            return list;
        }

        public static TelemetryHardeningResult ApplyPrivacyHardening(bool disableTelemetry)
        {
            var res = new TelemetryHardeningResult();

            // Set DataCollection AllowTelemetry Policy
            try
            {
                using (var key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows\DataCollection"))
                {
                    if (key != null)
                    {
                        // 0 = Security level (Enterprise/Edu), 1 = Basic
                        key.SetValue("AllowTelemetry", disableTelemetry ? 0 : 1, RegistryValueKind.DWord);
                        res.RegistryPoliciesSet++;
                        res.Messages.Add("Configured AllowTelemetry policy in HKLM");
                    }
                }
            }
            catch (Exception ex)
            {
                res.Messages.Add($"Policy error: {ex.Message}");
            }

            // Adjust DiagTrack service
            var services = new[] { "DiagTrack", "dmwappushservice" };
            foreach (var s in services)
            {
                try
                {
                    var mode = disableTelemetry ? "disabled" : "auto";
                    var p = Process.Start(new ProcessStartInfo
                    {
                        FileName = "sc.exe",
                        Arguments = $"config {s} start= {mode}",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    });
                    p?.WaitForExit(3000);

                    if (disableTelemetry)
                    {
                        var pStop = Process.Start(new ProcessStartInfo
                        {
                            FileName = "net.exe",
                            Arguments = $"stop {s} /y",
                            UseShellExecute = false,
                            CreateNoWindow = true
                        });
                        pStop?.WaitForExit(3000);
                    }

                    res.ServicesAdjusted++;
                    res.Messages.Add($"Configured service {s} to {mode}");
                }
                catch (Exception ex)
                {
                    res.Messages.Add($"Service error {s}: {ex.Message}");
                }
            }

            res.Success = true;
            return res;
        }
    }
}
