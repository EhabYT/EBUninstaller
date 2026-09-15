/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Storage Sense Policy & Downloads Folder Auto-Purge Auditor Subsystem
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using UninstallTools.Core;

namespace UninstallTools.SystemTools
{
    public sealed class StorageSensePolicyInfo
    {
        public bool IsStorageSenseEnabled { get; set; }
        public int PurgeRecycleBinDays { get; set; } = 30;
        public int PurgeDownloadsDays { get; set; } // 0 = Never
        public int PurgeTempFilesDays { get; set; } = 1;
        public string DownloadsPolicyDescription => PurgeDownloadsDays switch
        {
            0 => "Disabled (Downloads folder is never automatically deleted)",
            1 => "Purge files older than 1 day",
            14 => "Purge files older than 14 days",
            30 => "Purge files older than 30 days",
            60 => "Purge files older than 60 days",
            _ => $"Purge files older than {PurgeDownloadsDays} days"
        };
    }

    public static class StorageSensePolicyAuditorEngine
    {
        private static readonly string StorageSenseKey = @"Software\Microsoft\Windows\CurrentVersion\StorageSense\Parameters\StoragePolicy";

        public static StorageSensePolicyInfo QueryStorageSensePolicy()
        {
            var policy = new StorageSensePolicyInfo();

            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(StorageSenseKey);
                if (key != null)
                {
                    policy.IsStorageSenseEnabled = ((int?)key.GetValue("01") ?? 0) == 1;
                    policy.PurgeRecycleBinDays = (int?)key.GetValue("04") ?? 30;
                    policy.PurgeDownloadsDays = (int?)key.GetValue("32") ?? 0;
                    policy.PurgeTempFilesDays = (int?)key.GetValue("08") ?? 1;
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Warning(LogCategory.Registry, "Failed querying Storage Sense policy", ex.Message);
            }

            return policy;
        }

        public static bool ApplyStorageSensePolicy(bool enable, int recycleBinDays, int downloadsDays)
        {
            try
            {
                using var key = Registry.CurrentUser.CreateSubKey(StorageSenseKey, true);
                if (key != null)
                {
                    key.SetValue("01", enable ? 1 : 0, RegistryValueKind.DWord);
                    key.SetValue("04", recycleBinDays, RegistryValueKind.DWord);
                    key.SetValue("32", downloadsDays, RegistryValueKind.DWord);
                }
                StructuredLogger.Info(LogCategory.Registry, $"Updated Storage Sense policy: Enabled={enable}, DownloadsDays={downloadsDays}");
                return true;
            }
            catch (Exception ex)
            {
                StructuredLogger.Error(LogCategory.Registry, "Failed applying Storage Sense policy", ex.Message);
                return false;
            }
        }
    }
}
