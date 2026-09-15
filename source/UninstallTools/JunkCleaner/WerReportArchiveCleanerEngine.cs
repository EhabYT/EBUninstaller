/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Windows Error Reporting (WER) ReportArchive & Hang Diagnostics Cleaner Subsystem
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UninstallTools.Core;

namespace UninstallTools.JunkCleaner
{
    public sealed class WerReportArchiveItem
    {
        public string ReportFolder { get; set; } = string.Empty;
        public string ApplicationName { get; set; } = string.Empty;
        public string EventType { get; set; } = "AppCrash";
        public long SizeBytes { get; set; }
        public DateTime LastModified { get; set; }
    }

    public static class WerReportArchiveCleanerEngine
    {
        public static List<WerReportArchiveItem> ScanWerArchives()
        {
            var results = new List<WerReportArchiveItem>();
            var progData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            var archiveDirs = new[]
            {
                Path.Combine(progData, @"Microsoft\Windows\WER\ReportArchive"),
                Path.Combine(progData, @"Microsoft\Windows\WER\ReportQueue"),
                Path.Combine(localAppData, @"Microsoft\Windows\WER\ReportArchive"),
                Path.Combine(localAppData, @"Microsoft\Windows\WER\ReportQueue")
            };

            foreach (var dir in archiveDirs)
            {
                try
                {
                    if (Directory.Exists(dir))
                    {
                        var subDirs = Directory.GetDirectories(dir);
                        foreach (var sub in subDirs)
                        {
                            try
                            {
                                var di = new DirectoryInfo(sub);
                                var files = di.GetFiles("*", SearchOption.AllDirectories);
                                var size = files.Sum(f => f.Length);
                                var name = di.Name;

                                var parts = name.Split('_');
                                var appName = parts.Length > 1 ? parts[1] : name;
                                var eventType = parts.Length > 0 ? parts[0] : "Crash";

                                results.Add(new WerReportArchiveItem
                                {
                                    ReportFolder = sub,
                                    ApplicationName = appName,
                                    EventType = eventType,
                                    SizeBytes = size,
                                    LastModified = di.LastWriteTime
                                });
                            }
                            catch { }
                        }
                    }
                }
                catch (Exception ex)
                {
                    StructuredLogger.Warning(LogCategory.General, $"Failed scanning WER archive {dir}", ex.Message);
                }
            }

            return results.OrderByDescending(r => r.SizeBytes).ToList();
        }

        public static (int DeletedFolders, long FreedBytes) PurgeWerArchives(IEnumerable<WerReportArchiveItem> items)
        {
            int count = 0;
            long freed = 0;
            if (items == null) return (0, 0);

            foreach (var item in items)
            {
                try
                {
                    if (Directory.Exists(item.ReportFolder))
                    {
                        var s = item.SizeBytes;
                        Directory.Delete(item.ReportFolder, true);
                        count++;
                        freed += s;
                    }
                }
                catch { }
            }

            return (count, freed);
        }
    }
}
