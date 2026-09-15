/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    NetBIOS & WINS Name Cache Flusher Subsystem
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UninstallTools.Core;

namespace UninstallTools.SystemTools
{
    public sealed class NetBiosNameEntry
    {
        public string NetBiosName { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public string EntryType { get; set; } = "Unique / Host";
        public string Scope { get; set; } = "Remote Cache";
    }

    public static class NetBiosCacheFlusherEngine
    {
        public static List<NetBiosNameEntry> QueryNetBiosCache()
        {
            var results = new List<NetBiosNameEntry>();

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "nbtstat.exe",
                    Arguments = "-c",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };

                using var proc = Process.Start(psi);
                if (proc != null)
                {
                    var output = proc.StandardOutput.ReadToEnd();
                    proc.WaitForExit(5000);

                    var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var line in lines)
                    {
                        var trimmed = line.Trim();
                        if (trimmed.Contains("<") && trimmed.Contains(">"))
                        {
                            var parts = trimmed.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            if (parts.Length >= 3)
                            {
                                results.Add(new NetBiosNameEntry
                                {
                                    NetBiosName = parts[0],
                                    EntryType = parts[1],
                                    IpAddress = parts[parts.Length - 1]
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Warning(LogCategory.General, "Failed querying NetBIOS cache", ex.Message);
            }

            return results;
        }

        public static bool FlushAndReloadNetBios()
        {
            try
            {
                // Run nbtstat -R (Purge and reload remote cache table)
                var psi = new ProcessStartInfo
                {
                    FileName = "nbtstat.exe",
                    Arguments = "-R",
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                using var proc = Process.Start(psi);
                proc?.WaitForExit(5000);

                // Run nbtstat -RR (Release and refresh registered names)
                var psi2 = new ProcessStartInfo
                {
                    FileName = "nbtstat.exe",
                    Arguments = "-RR",
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                using var proc2 = Process.Start(psi2);
                proc2?.WaitForExit(5000);

                StructuredLogger.Info(LogCategory.General, "Flushed and reloaded Windows NetBIOS & WINS name resolution cache");
                return true;
            }
            catch (Exception ex)
            {
                StructuredLogger.Error(LogCategory.General, "Failed flushing NetBIOS cache", ex.Message);
                return false;
            }
        }
    }
}
