/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Network Adapter NDIS Filter & Protocol Binding Auditor
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using Microsoft.Win32;

namespace UninstallTools.SystemTools
{
    public sealed class NetworkBindingItem
    {
        public string ComponentId { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string InfPath { get; set; } = string.Empty;
        public string ServiceType { get; set; } = string.Empty;
        public bool IsOrphanedOrSuspicious { get; set; }
    }

    public sealed class NetworkBindingAuditResult
    {
        public int TotalBindingsFound { get; set; }
        public int FilterDriversCount { get; set; }
        public List<NetworkBindingItem> Items { get; set; } = new List<NetworkBindingItem>();
        public bool Success { get; set; }
    }

    public sealed class NetworkAdapterBindingAuditorEngine
    {
        public static NetworkBindingAuditResult ScanNetworkBindings()
        {
            var res = new NetworkBindingAuditResult { Success = true };

            try
            {
                // Inspect HKLM\SYSTEM\CurrentControlSet\Control\Network\{4d36e975-e325-11ce-bfc1-08002be10318} (NetService / NetTrans / NetClient)
                var netClassRoot = @"SYSTEM\CurrentControlSet\Control\Class\{4d36e975-e325-11ce-bfc1-08002be10318}";
                using (var key = Registry.LocalMachine.OpenSubKey(netClassRoot))
                {
                    if (key != null)
                    {
                        var subKeyNames = key.GetSubKeyNames();
                        foreach (var sub in subKeyNames)
                        {
                            if (sub.StartsWith("Properties", StringComparison.OrdinalIgnoreCase))
                                continue;

                            using (var itemKey = key.OpenSubKey(sub))
                            {
                                if (itemKey != null)
                                {
                                    var desc = itemKey.GetValue("DriverDesc")?.ToString() ?? string.Empty;
                                    var compId = itemKey.GetValue("ComponentId")?.ToString() ?? string.Empty;
                                    var inf = itemKey.GetValue("InfPath")?.ToString() ?? string.Empty;

                                    if (!string.IsNullOrEmpty(desc) || !string.IsNullOrEmpty(compId))
                                    {
                                        var isFilter = desc.Contains("Filter") || desc.Contains("NDIS") || desc.Contains("Virtual") || desc.Contains("VPN");
                                        var item = new NetworkBindingItem
                                        {
                                            ComponentId = compId,
                                            Description = string.IsNullOrEmpty(desc) ? compId : desc,
                                            InfPath = inf,
                                            ServiceType = isFilter ? "NDIS Filter / Protocol" : "Network Client/Protocol",
                                            IsOrphanedOrSuspicious = isFilter && (desc.Contains("Tap") || desc.Contains("VirtualBox") || desc.Contains("Npcap") || desc.Contains("Cisco"))
                                        };

                                        res.Items.Add(item);
                                        if (isFilter) res.FilterDriversCount++;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch { }

            res.TotalBindingsFound = res.Items.Count;
            return res;
        }
    }
}
