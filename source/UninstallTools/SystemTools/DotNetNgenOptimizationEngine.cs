/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    .NET Framework & Native Image Generator (NGEN) Optimization Subsystem
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace UninstallTools.SystemTools
{
    public sealed class NgenQueueStatus
    {
        public string FrameworkVersion { get; set; } = string.Empty;
        public string Architecture { get; set; } = string.Empty;
        public string NgenExecutablePath { get; set; } = string.Empty;
        public bool Exists { get; set; }
    }

    public sealed class NgenOptimizationResult
    {
        public int QueuesProcessed { get; set; }
        public List<string> Messages { get; set; } = new List<string>();
        public bool Success { get; set; }
    }

    public sealed class DotNetNgenOptimizationEngine
    {
        public static List<NgenQueueStatus> DiscoverNgenRuntimes()
        {
            var list = new List<NgenQueueStatus>();
            var winDir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
            var netDir = Path.Combine(winDir, "Microsoft.NET");

            if (Directory.Exists(netDir))
            {
                var paths = new[]
                {
                    Path.Combine(netDir, "Framework64", "v4.0.30319", "ngen.exe"),
                    Path.Combine(netDir, "Framework", "v4.0.30319", "ngen.exe")
                };

                foreach (var p in paths)
                {
                    var is64 = p.Contains("Framework64");
                    list.Add(new NgenQueueStatus
                    {
                        FrameworkVersion = "v4.0.30319",
                        Architecture = is64 ? "64-bit (x64)" : "32-bit (x86)",
                        NgenExecutablePath = p,
                        Exists = File.Exists(p)
                    });
                }
            }

            return list;
        }

        public static NgenOptimizationResult ExecuteQueuedItems()
        {
            var res = new NgenOptimizationResult();
            var targets = DiscoverNgenRuntimes();

            foreach (var t in targets)
            {
                if (t.Exists)
                {
                    try
                    {
                        var psi = new ProcessStartInfo
                        {
                            FileName = t.NgenExecutablePath,
                            Arguments = "executeQueuedItems",
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            RedirectStandardOutput = true
                        };

                        using (var p = Process.Start(psi))
                        {
                            if (p != null)
                            {
                                p.WaitForExit(10000);
                                res.QueuesProcessed++;
                                res.Messages.Add($"Executed NGEN queued items for {t.Architecture}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        res.Messages.Add($"NGEN {t.Architecture}: {ex.Message}");
                    }
                }
            }

            res.Success = true;
            return res;
        }
    }
}
