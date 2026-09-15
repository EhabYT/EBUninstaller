/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Most Recently Used (MRU) History & Explorer Trace Cleaner
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using Microsoft.Win32;

namespace UninstallTools.PrivacyCleaner
{
    public sealed class MruHistoryItem
    {
        public string RegistryPath { get; set; } = string.Empty;
        public string ValueName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string DataPreview { get; set; } = string.Empty;
    }

    public sealed class MruHistoryCleanResult
    {
        public int KeysScanned { get; set; }
        public int EntriesCleaned { get; set; }
        public List<string> Messages { get; set; } = new List<string>();
        public bool Success { get; set; }
    }

    public sealed class MruRecentHistoryCleanerEngine
    {
        private static readonly string[] MruKeyPaths = new[]
        {
            @"Software\Microsoft\Windows\CurrentVersion\Explorer\RunMRU",
            @"Software\Microsoft\Windows\CurrentVersion\Explorer\ComDlg32\OpenSavePidlMRU",
            @"Software\Microsoft\Windows\CurrentVersion\Explorer\ComDlg32\LastVisitedPidlMRU",
            @"Software\Microsoft\Windows\CurrentVersion\Explorer\ComDlg32\FirstFolder",
            @"Software\Microsoft\Windows\CurrentVersion\Applets\Paint\Recent File List",
            @"Software\Microsoft\Windows\CurrentVersion\Applets\Wordpad\Recent File List",
            @"Software\Microsoft\Windows\CurrentVersion\Applets\Regedit\Favorites"
        };

        public static List<MruHistoryItem> ScanMruEntries()
        {
            var results = new List<MruHistoryItem>();

            foreach (var keyPath in MruKeyPaths)
            {
                try
                {
                    using (var key = Registry.CurrentUser.OpenSubKey(keyPath, false))
                    {
                        if (key != null)
                        {
                            var names = key.GetValueNames();
                            foreach (var name in names)
                            {
                                if (name == "MRUList" || name == "MRUListEx" || string.IsNullOrEmpty(name))
                                    continue;

                                var val = key.GetValue(name)?.ToString() ?? string.Empty;
                                results.Add(new MruHistoryItem
                                {
                                    RegistryPath = $@"HKCU\{keyPath}",
                                    ValueName = name,
                                    Description = GetCategory(keyPath),
                                    DataPreview = val.Length > 60 ? val.Substring(0, 57) + "..." : val
                                });
                            }
                        }
                    }
                }
                catch { }
            }

            return results;
        }

        private static string GetCategory(string keyPath)
        {
            if (keyPath.Contains("RunMRU")) return "Run Command History";
            if (keyPath.Contains("OpenSavePidlMRU")) return "Common Dialog Open/Save History";
            if (keyPath.Contains("LastVisitedPidlMRU")) return "Explorer Last Visited Folders";
            if (keyPath.Contains("Paint")) return "MS Paint Recent Files";
            if (keyPath.Contains("Wordpad")) return "WordPad Recent Files";
            return "Application MRU Trace";
        }

        public static MruHistoryCleanResult PurgeMruHistory()
        {
            var res = new MruHistoryCleanResult();
            var items = ScanMruEntries();
            res.KeysScanned = MruKeyPaths.Length;

            foreach (var keyPath in MruKeyPaths)
            {
                try
                {
                    using (var key = Registry.CurrentUser.OpenSubKey(keyPath, true))
                    {
                        if (key != null)
                        {
                            var names = key.GetValueNames();
                            foreach (var name in names)
                            {
                                try
                                {
                                    key.DeleteValue(name, false);
                                    res.EntriesCleaned++;
                                }
                                catch { }
                            }
                            res.Messages.Add($"[Cleaned] HKCU\\{keyPath}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    res.Messages.Add($"[Notice] HKCU\\{keyPath}: {ex.Message}");
                }
            }

            res.Success = true;
            return res;
        }
    }
}
