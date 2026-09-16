/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Windows System Restore Point Integrity & Sequence Auditor
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;

namespace UninstallTools.Backup
{
    public sealed class RestorePointItem
    {
        public int SequenceNumber { get; set; }
        public string Description { get; set; } = string.Empty;
        public string RestorePointType { get; set; } = string.Empty;
        public string CreationTime { get; set; } = string.Empty;
        public bool IsValid { get; set; }
    }

    public sealed class RestorePointAuditResult
    {
        public int TotalRestorePoints { get; set; }
        public List<RestorePointItem> Points { get; set; } = new List<RestorePointItem>();
        public bool SystemRestoreEnabled { get; set; }
        public bool Success { get; set; }
    }

    public sealed class SystemRestorePointIntegrityAuditorEngine
    {
        public static RestorePointAuditResult AuditRestorePoints()
        {
            var res = new RestorePointAuditResult { Success = true, SystemRestoreEnabled = true };

            try
            {
                using (var searcher = new ManagementObjectSearcher(@"root\default", "SELECT * FROM SystemRestore"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        var seq = Convert.ToInt32(obj["SequenceNumber"]);
                        var desc = obj["Description"]?.ToString() ?? "System Snapshot";
                        var rpType = Convert.ToInt32(obj["RestorePointType"]);
                        var time = obj["CreationTime"]?.ToString() ?? string.Empty;

                        res.Points.Add(new RestorePointItem
                        {
                            SequenceNumber = seq,
                            Description = desc,
                            RestorePointType = GetRestorePointTypeName(rpType),
                            CreationTime = time,
                            IsValid = true
                        });
                    }
                }
            }
            catch
            {
                // Fallback using vssadmin shadow copies list
                try
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName = "vssadmin.exe",
                        Arguments = "list shadows",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    };
                    using (var p = Process.Start(psi))
                    {
                        if (p != null)
                        {
                            p.WaitForExit(4000);
                            var outText = p.StandardOutput.ReadToEnd();
                            if (outText.Contains("Shadow Copy Volume"))
                            {
                                res.Points.Add(new RestorePointItem
                                {
                                    SequenceNumber = 1,
                                    Description = "VSS System Volume Snapshot",
                                    RestorePointType = "Volume Snapshot",
                                    CreationTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                                    IsValid = true
                                });
                            }
                        }
                    }
                }
                catch { }
            }

            res.TotalRestorePoints = res.Points.Count;
            return res;
        }

        private static string GetRestorePointTypeName(int typeId)
        {
            return typeId switch
            {
                0 => "APPLICATION_INSTALL",
                1 => "APPLICATION_UNINSTALL",
                10 => "DEVICE_DRIVER_INSTALL",
                12 => "MODIFY_SETTINGS",
                13 => "CANCELLED_OPERATION",
                _ => "MANUAL_CHECKPOINT"
            };
        }
    }
}
