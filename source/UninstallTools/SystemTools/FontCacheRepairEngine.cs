/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Font Cache Repair & Integrity Subsystem
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.IO;

namespace UninstallTools.SystemTools
{
    public sealed class FontCacheAuditItem
    {
        public string TargetPath { get; set; } = string.Empty;
        public string ServiceName { get; set; } = string.Empty;
        public long FileSizeBytes { get; set; }
        public bool Exists { get; set; }
        public string StatusDescription { get; set; } = string.Empty;
    }

    public sealed class FontCacheRepairResult
    {
        public int FilesChecked { get; set; }
        public int FilesPurged { get; set; }
        public long BytesReclaimed { get; set; }
        public List<string> Messages { get; set; } = new List<string>();
        public bool Success { get; set; }
    }

    public sealed class FontCacheRepairEngine
    {
        public static List<FontCacheAuditItem> ScanFontCaches()
        {
            var results = new List<FontCacheAuditItem>();

            var localApp = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            if (!string.IsNullOrEmpty(localApp))
            {
                var wpfCache = Path.Combine(localApp, "FontCache3.0.0.0.dat");
                var item = new FontCacheAuditItem
                {
                    TargetPath = wpfCache,
                    ServiceName = "FontCache3.0.0.0",
                    Exists = File.Exists(wpfCache)
                };
                if (item.Exists)
                {
                    try { item.FileSizeBytes = new FileInfo(wpfCache).Length; } catch { }
                    item.StatusDescription = "WPF User Font Cache present";
                }
                else
                {
                    item.StatusDescription = "Clean / Not generated";
                }
                results.Add(item);
            }

            var winDir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
            if (!string.IsNullOrEmpty(winDir))
            {
                var svcDir = Path.Combine(winDir, "ServiceProfiles", "LocalService", "AppData", "Local", "FontCache");
                if (Directory.Exists(svcDir))
                {
                    try
                    {
                        var files = Directory.GetFiles(svcDir, "*.dat");
                        foreach (var f in files)
                        {
                            long sz = 0;
                            try { sz = new FileInfo(f).Length; } catch { }
                            results.Add(new FontCacheAuditItem
                            {
                                TargetPath = f,
                                ServiceName = "FontCache",
                                Exists = true,
                                FileSizeBytes = sz,
                                StatusDescription = "System Font Cache data file"
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        results.Add(new FontCacheAuditItem
                        {
                            TargetPath = svcDir,
                            ServiceName = "FontCache",
                            Exists = true,
                            StatusDescription = $"Access restricted: {ex.Message}"
                        });
                    }
                }
            }

            return results;
        }

        public static FontCacheRepairResult RepairAndPurge()
        {
            var res = new FontCacheRepairResult();
            var items = ScanFontCaches();
            res.FilesChecked = items.Count;

            foreach (var itm in items)
            {
                if (itm.Exists && File.Exists(itm.TargetPath))
                {
                    try
                    {
                        var sz = itm.FileSizeBytes;
                        File.Delete(itm.TargetPath);
                        res.FilesPurged++;
                        res.BytesReclaimed += sz;
                        res.Messages.Add($"[Purged] {itm.TargetPath} ({sz} bytes)");
                    }
                    catch (Exception ex)
                    {
                        res.Messages.Add($"[Notice] Could not delete {itm.TargetPath}: {ex.Message}");
                    }
                }
            }

            res.Success = true;
            return res;
        }
    }
}
