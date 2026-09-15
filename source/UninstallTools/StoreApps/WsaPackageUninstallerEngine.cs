/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Windows Subsystem for Android (WSA) & APK Package Uninstaller Subsystem
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using UninstallTools.Core;

namespace UninstallTools.StoreApps
{
    public sealed class WsaPackageItem
    {
        public string PackageId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Publisher { get; set; } = "Android App";
        public string InstallLocation { get; set; } = string.Empty;
        public bool IsOrphaned { get; set; }
        public string RegistryPath { get; set; } = string.Empty;
    }

    public static class WsaPackageUninstallerEngine
    {
        private static readonly string WsaRegistryPath = @"Software\Classes\Local Settings\Software\Microsoft\Windows\CurrentVersion\AppModel\Repository\Packages";

        public static List<WsaPackageItem> ScanWsaPackages()
        {
            var results = new List<WsaPackageItem>();

            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(WsaRegistryPath);
                if (key != null)
                {
                    foreach (var pkgName in key.GetSubKeyNames())
                    {
                        if (pkgName.Contains("WsaPackage", StringComparison.OrdinalIgnoreCase) ||
                            pkgName.Contains("Android", StringComparison.OrdinalIgnoreCase) ||
                            pkgName.Contains("AmazonAppstore", StringComparison.OrdinalIgnoreCase))
                        {
                            try
                            {
                                using var sub = key.OpenSubKey(pkgName);
                                if (sub == null) continue;

                                var displayName = sub.GetValue("DisplayName") as string ?? pkgName;
                                var publisher = sub.GetValue("Publisher") as string ?? "Android Developer";
                                var installPath = sub.GetValue("PackageRootFolder") as string ?? string.Empty;

                                var isMissing = !string.IsNullOrEmpty(installPath) && !Directory.Exists(installPath);

                                results.Add(new WsaPackageItem
                                {
                                    PackageId = pkgName,
                                    DisplayName = displayName,
                                    Publisher = publisher,
                                    InstallLocation = installPath,
                                    IsOrphaned = isMissing,
                                    RegistryPath = $@"{WsaRegistryPath}\{pkgName}"
                                });
                            }
                            catch { }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Warning(LogCategory.StoreApp, "Failed querying WSA Android packages", ex.Message);
            }

            return results.OrderBy(p => p.DisplayName).ToList();
        }

        public static int CleanWsaPackages(IEnumerable<WsaPackageItem> packages)
        {
            int count = 0;
            if (packages == null) return 0;

            try
            {
                using var baseKey = Registry.CurrentUser.OpenSubKey(WsaRegistryPath, true);
                if (baseKey != null)
                {
                    foreach (var p in packages)
                    {
                        try
                        {
                            baseKey.DeleteSubKeyTree(p.PackageId, false);
                            count++;
                            StructuredLogger.Info(LogCategory.StoreApp, $"Removed WSA package: {p.DisplayName} ({p.PackageId})");
                        }
                        catch { }
                    }
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Error(LogCategory.StoreApp, "Failed deleting WSA packages", ex.Message);
            }

            return count;
        }
    }
}
