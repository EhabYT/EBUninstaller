/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Windows Event Log Retention & Channel Health Subsystem
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Win32;

namespace UninstallTools.SystemTools
{
    public sealed class EventLogChannelInfo
    {
        public string ChannelName { get; set; } = string.Empty;
        public long MaxSizeKilobytes { get; set; }
        public int RetentionPolicy { get; set; }
        public bool IsEnabled { get; set; }
        public string RetentionDescription { get; set; } = string.Empty;
    }

    public sealed class EventLogAuditResult
    {
        public int TotalChannelsScanned { get; set; }
        public List<EventLogChannelInfo> Channels { get; set; } = new List<EventLogChannelInfo>();
        public bool Success { get; set; }
    }

    public sealed class WindowsEventLogRetentionPolicyEngine
    {
        private static readonly string[] CoreChannels = new[]
        {
            "Application",
            "System",
            "Security",
            "Setup",
            "Microsoft-Windows-Diagnostics-Performance/Operational",
            "Microsoft-Windows-TaskScheduler/Operational"
        };

        public static EventLogAuditResult AuditEventLogChannels()
        {
            var res = new EventLogAuditResult { Success = true };

            foreach (var ch in CoreChannels)
            {
                var info = new EventLogChannelInfo
                {
                    ChannelName = ch,
                    MaxSizeKilobytes = 20480, // Default 20MB
                    IsEnabled = true,
                    RetentionDescription = "Overwrite As Needed"
                };

                try
                {
                    using (var key = Registry.LocalMachine.OpenSubKey($@"SYSTEM\CurrentControlSet\Services\EventLog\{ch}"))
                    {
                        if (key != null)
                        {
                            var max = key.GetValue("MaxSize");
                            if (max is int intMax)
                                info.MaxSizeKilobytes = intMax / 1024;
                            else if (max is long longMax)
                                info.MaxSizeKilobytes = longMax / 1024;

                            var ret = key.GetValue("Retention");
                            if (ret is int retVal)
                            {
                                info.RetentionPolicy = retVal;
                                info.RetentionDescription = retVal == 0 ? "Overwrite As Needed" : (retVal == -1 ? "Do Not Overwrite" : $"Retain {retVal} Days");
                            }
                        }
                    }
                }
                catch { }

                res.Channels.Add(info);
            }

            res.TotalChannelsScanned = res.Channels.Count;
            return res;
        }

        public static bool ClearEventLogChannel(string channelName)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "wevtutil.exe",
                    Arguments = $"cl \"{channelName}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                using (var p = Process.Start(psi))
                {
                    p?.WaitForExit(5000);
                    return p != null && p.ExitCode == 0;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
