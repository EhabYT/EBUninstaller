/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    DNS Client Resolver Cache Flush & Health Subsystem
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Diagnostics;

namespace UninstallTools.SystemTools
{
    public sealed class DnsCacheStatus
    {
        public bool DnsClientRunning { get; set; }
        public string OutputLog { get; set; } = string.Empty;
        public bool FlushedSuccessfully { get; set; }
    }

    public sealed class DnsClientCacheHealthEngine
    {
        public static DnsCacheStatus CheckStatus()
        {
            var status = new DnsCacheStatus();
            try
            {
                var p = Process.Start(new ProcessStartInfo
                {
                    FileName = "sc.exe",
                    Arguments = "query Dnscache",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                });
                if (p != null)
                {
                    p.WaitForExit(3000);
                    var outText = p.StandardOutput.ReadToEnd();
                    status.DnsClientRunning = outText.Contains("RUNNING");
                    status.OutputLog = outText;
                }
            }
            catch (Exception ex)
            {
                status.OutputLog = ex.Message;
                status.DnsClientRunning = true;
            }
            return status;
        }

        public static DnsCacheStatus FlushDnsCache()
        {
            var status = new DnsCacheStatus();
            try
            {
                var p = Process.Start(new ProcessStartInfo
                {
                    FileName = "ipconfig.exe",
                    Arguments = "/flushdns",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                });
                if (p != null)
                {
                    p.WaitForExit(5000);
                    var output = p.StandardOutput.ReadToEnd();
                    status.OutputLog = output;
                    status.FlushedSuccessfully = p.ExitCode == 0 || output.Contains("Successfully flushed");
                }
            }
            catch (Exception ex)
            {
                status.OutputLog = $"Flush error: {ex.Message}";
                status.FlushedSuccessfully = false;
            }
            return status;
        }
    }
}
