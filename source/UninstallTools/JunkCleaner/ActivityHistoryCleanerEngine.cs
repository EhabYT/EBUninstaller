/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Connected Devices Platform (CDP) Activity History & Timeline Cleaner Subsystem
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UninstallTools.Core;

namespace UninstallTools.JunkCleaner
{
    public sealed class ActivityHistoryStats
    {
        public int DatabaseCount { get; set; }
        public long TotalSizeBytes { get; set; }
        public List<string> DatabasePaths { get; set; } = new();
    }

    public static class ActivityHistoryCleanerEngine
    {
        public static ActivityHistoryStats ScanActivityHistory()
        {
            var stats = new ActivityHistoryStats();
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var cdpDir = Path.Combine(localAppData, "ConnectedDevicesPlatform");

            try
            {
                if (Directory.Exists(cdpDir))
                {
                    var files = Directory.GetFiles(cdpDir, "ActivitiesCache.db*", SearchOption.AllDirectories);
                    foreach (var f in files)
                    {
                        try
                        {
                            var fi = new FileInfo(f);
                            stats.DatabaseCount++;
                            stats.TotalSizeBytes += fi.Length;
                            stats.DatabasePaths.Add(f);
                        }
                        catch { }
                    }
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Warning(LogCategory.General, "Failed scanning ConnectedDevicesPlatform activity cache", ex.Message);
            }

            return stats;
        }

        public static (int DeletedFiles, long FreedBytes) PurgeActivityHistory()
        {
            int files = 0;
            long bytes = 0;
            var stats = ScanActivityHistory();

            foreach (var path in stats.DatabasePaths)
            {
                try
                {
                    if (File.Exists(path))
                    {
                        var len = new FileInfo(path).Length;
                        File.Delete(path);
                        files++;
                        bytes += len;
                    }
                }
                catch { }
            }

            return (files, bytes);
        }
    }
}
