/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Terminal & Console Profile Registry Trace Cleaner
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using Microsoft.Win32;

namespace UninstallTools.PrivacyCleaner
{
    public sealed class ConsoleProfileItem
    {
        public string SubKeyName { get; set; } = string.Empty;
        public string RegistryPath { get; set; } = string.Empty;
        public string TitlePreview { get; set; } = string.Empty;
        public bool IsCustomAppProfile { get; set; }
    }

    public sealed class ConsoleProfileCleanResult
    {
        public int ProfilesScanned { get; set; }
        public int ProfilesCleaned { get; set; }
        public List<string> Messages { get; set; } = new List<string>();
        public bool Success { get; set; }
    }

    public sealed class TerminalColorPaletteProfilesCleanerEngine
    {
        public static List<ConsoleProfileItem> ScanConsoleProfiles()
        {
            var results = new List<ConsoleProfileItem>();
            try
            {
                using (var consoleKey = Registry.CurrentUser.OpenSubKey(@"Console"))
                {
                    if (consoleKey != null)
                    {
                        var names = consoleKey.GetSubKeyNames();
                        foreach (var name in names)
                        {
                            var fullPath = $@"HKCU\Console\{name}";
                            var isCustom = name.Contains("%") || name.Contains(":") || name.Contains("Git") || name.Contains("Node") || name.Contains("Cygwin");
                            results.Add(new ConsoleProfileItem
                            {
                                SubKeyName = name,
                                RegistryPath = fullPath,
                                TitlePreview = name.Replace("%SystemRoot%", "C:\\Windows"),
                                IsCustomAppProfile = isCustom
                            });
                        }
                    }
                }
            }
            catch { }

            return results;
        }

        public static ConsoleProfileCleanResult PurgeCustomConsoleProfiles()
        {
            var res = new ConsoleProfileCleanResult();
            var profiles = ScanConsoleProfiles();
            res.ProfilesScanned = profiles.Count;

            foreach (var p in profiles)
            {
                if (p.IsCustomAppProfile)
                {
                    try
                    {
                        using (var consoleKey = Registry.CurrentUser.OpenSubKey(@"Console", true))
                        {
                            if (consoleKey != null)
                            {
                                consoleKey.DeleteSubKeyTree(p.SubKeyName, false);
                                res.ProfilesCleaned++;
                                res.Messages.Add($"[Purged] {p.RegistryPath}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        res.Messages.Add($"[Notice] {p.RegistryPath}: {ex.Message}");
                    }
                }
            }

            res.Success = true;
            return res;
        }
    }
}
