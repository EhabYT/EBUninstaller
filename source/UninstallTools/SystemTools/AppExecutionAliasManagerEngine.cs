/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    App Execution Alias & App Paths Residuals Manager
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Win32;

namespace UninstallTools.SystemTools
{
    public sealed class AppExecutionAliasItem
    {
        public string AliasName { get; set; } = string.Empty;
        public string TargetPath { get; set; } = string.Empty;
        public string SourceCategory { get; set; } = string.Empty; // App Paths or WindowsApps
        public bool TargetExists { get; set; }
    }

    public sealed class AppExecutionAliasResult
    {
        public int TotalAliasesFound { get; set; }
        public int BrokenAliasesCount { get; set; }
        public List<AppExecutionAliasItem> Items { get; set; } = new List<AppExecutionAliasItem>();
        public bool Success { get; set; }
    }

    public sealed class AppExecutionAliasManagerEngine
    {
        public static AppExecutionAliasResult ScanExecutionAliases()
        {
            var res = new AppExecutionAliasResult { Success = true };

            // 1. Scan App Paths in HKLM & HKCU
            ScanRegistryAppPaths(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\App Paths", "HKCU App Paths", res);
            ScanRegistryAppPaths(Registry.LocalMachine, @"Software\Microsoft\Windows\CurrentVersion\App Paths", "HKLM App Paths", res);

            // 2. Scan %LocalAppData%\Microsoft\WindowsApps
            var localApp = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            if (!string.IsNullOrEmpty(localApp))
            {
                var winAppsDir = Path.Combine(localApp, "Microsoft", "WindowsApps");
                if (Directory.Exists(winAppsDir))
                {
                    try
                    {
                        var files = Directory.GetFiles(winAppsDir, "*.exe");
                        foreach (var f in files)
                        {
                            var name = Path.GetFileName(f);
                            var item = new AppExecutionAliasItem
                            {
                                AliasName = name,
                                TargetPath = f,
                                SourceCategory = "WindowsApps Execution Alias",
                                TargetExists = true
                            };
                            res.Items.Add(item);
                        }
                    }
                    catch { }
                }
            }

            res.TotalAliasesFound = res.Items.Count;
            return res;
        }

        private static void ScanRegistryAppPaths(RegistryKey root, string keyPath, string category, AppExecutionAliasResult res)
        {
            try
            {
                using (var key = root.OpenSubKey(keyPath))
                {
                    if (key != null)
                    {
                        var subKeys = key.GetSubKeyNames();
                        foreach (var sub in subKeys)
                        {
                            using (var appKey = key.OpenSubKey(sub))
                            {
                                var path = appKey?.GetValue("")?.ToString() ?? string.Empty;
                                var exists = string.IsNullOrEmpty(path) || File.Exists(path.Trim('"', ' '));

                                var item = new AppExecutionAliasItem
                                {
                                    AliasName = sub,
                                    TargetPath = path,
                                    SourceCategory = category,
                                    TargetExists = exists
                                };
                                res.Items.Add(item);
                                if (!exists) res.BrokenAliasesCount++;
                            }
                        }
                    }
                }
            }
            catch { }
        }
    }
}
