/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Taskbar Jump List & Recent Document Cleaner
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.IO;

namespace UninstallTools.PrivacyCleaner
{
    public sealed class JumpListItem
    {
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string JumpListType { get; set; } = string.Empty; // AutomaticDestinations or CustomDestinations
        public long FileSizeBytes { get; set; }
        public DateTime LastModified { get; set; }
    }

    public sealed class JumpListCleanResult
    {
        public int TotalItemsFound { get; set; }
        public int TotalItemsDeleted { get; set; }
        public long BytesCleaned { get; set; }
        public List<string> Messages { get; set; } = new List<string>();
        public bool Success { get; set; }
    }

    public sealed class TaskbarJumpListCleanerEngine
    {
        public static List<JumpListItem> ScanJumpLists()
        {
            var results = new List<JumpListItem>();

            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            if (!string.IsNullOrEmpty(appData))
            {
                var recentDir = Path.Combine(appData, "Microsoft", "Windows", "Recent");

                ScanDirectory(Path.Combine(recentDir, "AutomaticDestinations"), "Automatic Jump List", results);
                ScanDirectory(Path.Combine(recentDir, "CustomDestinations"), "Custom Jump List", results);
                ScanDirectory(recentDir, "Recent Document Shortcut", results, false);
            }

            return results;
        }

        private static void ScanDirectory(string dir, string type, List<JumpListItem> list, bool recursive = true)
        {
            if (Directory.Exists(dir))
            {
                try
                {
                    var opt = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                    var files = Directory.GetFiles(dir, "*.*", opt);
                    foreach (var f in files)
                    {
                        try
                        {
                            var fi = new FileInfo(f);
                            list.Add(new JumpListItem
                            {
                                FilePath = f,
                                FileName = Path.GetFileName(f),
                                JumpListType = type,
                                FileSizeBytes = fi.Length,
                                LastModified = fi.LastWriteTime
                            });
                        }
                        catch { }
                    }
                }
                catch { }
            }
        }

        public static JumpListCleanResult PurgeJumpLists()
        {
            var result = new JumpListCleanResult();
            var items = ScanJumpLists();
            result.TotalItemsFound = items.Count;

            foreach (var item in items)
            {
                if (File.Exists(item.FilePath))
                {
                    try
                    {
                        var size = item.FileSizeBytes;
                        File.Delete(item.FilePath);
                        result.TotalItemsDeleted++;
                        result.BytesCleaned += size;
                        result.Messages.Add($"[Purged] {item.FileName} ({size} bytes)");
                    }
                    catch (Exception ex)
                    {
                        result.Messages.Add($"[Skip] {item.FileName}: {ex.Message}");
                    }
                }
            }

            result.Success = true;
            return result;
        }
    }
}
