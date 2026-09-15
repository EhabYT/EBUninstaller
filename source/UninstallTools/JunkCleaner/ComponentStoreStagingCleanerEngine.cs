/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Component Store (SxS) Staging & Servicing Package Cache Cleaner Subsystem
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UninstallTools.Core;

namespace UninstallTools.JunkCleaner
{
    public sealed class StagingPackageItem
    {
        public string FileName { get; set; } = string.Empty;
        public string PackageType { get; set; } = "MUM Package Manifest";
        public string FilePath { get; set; } = string.Empty;
        public long SizeBytes { get; set; }
        public DateTime CreationTime { get; set; }
    }

    public static class ComponentStoreStagingCleanerEngine
    {
        public static List<StagingPackageItem> ScanStagingPackages()
        {
            var results = new List<StagingPackageItem>();
            var winDir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);

            var targets = new (string Path, string Pattern, string Type)[]
            {
                (Path.Combine(winDir, @"SoftwareDistribution\Download"), "*.*", "Windows Update Staging Payload"),
                (Path.Combine(winDir, @"Logs\CBS"), "*.log", "CBS Servicing Log File"),
                (Path.Combine(winDir, @"Logs\DISM"), "*.log", "DISM Servicing Log File")
            };

            foreach (var (path, pattern, type) in targets)
            {
                try
                {
                    if (Directory.Exists(path))
                    {
                        var files = Directory.GetFiles(path, pattern, SearchOption.AllDirectories);
                        foreach (var f in files)
                        {
                            try
                            {
                                var fi = new FileInfo(f);
                                results.Add(new StagingPackageItem
                                {
                                    FileName = fi.Name,
                                    PackageType = type,
                                    FilePath = f,
                                    SizeBytes = fi.Length,
                                    CreationTime = fi.CreationTime
                                });
                            }
                            catch { }
                        }
                    }
                }
                catch (Exception ex)
                {
                    StructuredLogger.Warning(LogCategory.General, $"Failed scanning staging packages in {path}", ex.Message);
                }
            }

            return results.OrderByDescending(s => s.SizeBytes).ToList();
        }

        public static (int CleanedFiles, long FreedBytes) PurgeStagingPackages(IEnumerable<StagingPackageItem> items)
        {
            int count = 0;
            long bytes = 0;
            if (items == null) return (0, 0);

            foreach (var item in items)
            {
                try
                {
                    if (File.Exists(item.FilePath))
                    {
                        var len = item.SizeBytes;
                        File.Delete(item.FilePath);
                        count++;
                        bytes += len;
                    }
                }
                catch { }
            }

            return (count, bytes);
        }
    }
}
