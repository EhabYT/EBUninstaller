/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Winsock Protocol Catalog & Transport Provider Auditor Subsystem
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using UninstallTools.Core;

namespace UninstallTools.SystemTools
{
    public sealed class WinsockProtocolEntry
    {
        public string EntryId { get; set; } = string.Empty;
        public string ProtocolName { get; set; } = string.Empty;
        public string ProviderDllPath { get; set; } = string.Empty;
        public bool IsDllMissing { get; set; }
        public string RegistryPath { get; set; } = string.Empty;
        public string HealthStatus => IsDllMissing ? "Orphaned Provider (DLL Missing)" : "Valid Protocol Provider";
    }

    public static class WinsockProtocolCatalogAuditorEngine
    {
        private static readonly string ProtocolCatalogKey = @"SYSTEM\CurrentControlSet\Services\WinSock2\Parameters\Protocol_Catalog9\Catalog_Entries";

        public static List<WinsockProtocolEntry> ScanProtocolCatalog()
        {
            var results = new List<WinsockProtocolEntry>();
            var sysDir = Environment.GetFolderPath(Environment.SpecialFolder.System);

            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(ProtocolCatalogKey);
                if (key != null)
                {
                    foreach (var subName in key.GetSubKeyNames())
                    {
                        try
                        {
                            using var sub = key.OpenSubKey(subName);
                            if (sub == null) continue;

                            var name = sub.GetValue("ProtocolName") as string ?? sub.GetValue("PackedProtocolName") as string ?? subName;
                            var dll = sub.GetValue("PackedCatalogItem") as string ?? sub.GetValue("ProviderId") as string ?? string.Empty;

                            var isMissing = false;
                            if (!string.IsNullOrEmpty(dll) && dll.Contains("\\"))
                            {
                                var expanded = Environment.ExpandEnvironmentVariables(dll);
                                if (!File.Exists(expanded)) isMissing = true;
                            }

                            results.Add(new WinsockProtocolEntry
                            {
                                EntryId = subName,
                                ProtocolName = name,
                                ProviderDllPath = dll,
                                IsDllMissing = isMissing,
                                RegistryPath = $@"{ProtocolCatalogKey}\{subName}"
                            });
                        }
                        catch { }
                    }
                }
            }
            catch (Exception ex)
            {
                StructuredLogger.Warning(LogCategory.Registry, "Failed scanning Winsock Protocol Catalog", ex.Message);
            }

            return results.OrderByDescending(p => p.IsDllMissing).ThenBy(p => p.EntryId).ToList();
        }
    }
}
