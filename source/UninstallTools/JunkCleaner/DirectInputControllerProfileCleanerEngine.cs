/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    DirectInput & Game Controller Profile Cleaner Subsystem
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using Microsoft.Win32;

namespace UninstallTools.JunkCleaner
{
    public sealed class DirectInputDeviceItem
    {
        public string DeviceGuid { get; set; } = string.Empty;
        public string RegistryPath { get; set; } = string.Empty;
        public string JoystickName { get; set; } = string.Empty;
        public bool HasCalibrationData { get; set; }
    }

    public sealed class DirectInputCleanResult
    {
        public int TotalDevicesScanned { get; set; }
        public int ProfilesDeleted { get; set; }
        public List<string> Messages { get; set; } = new List<string>();
        public bool Success { get; set; }
    }

    public sealed class DirectInputControllerProfileCleanerEngine
    {
        public static List<DirectInputDeviceItem> ScanDirectInputProfiles()
        {
            var results = new List<DirectInputDeviceItem>();
            try
            {
                var basePath = @"System\CurrentControlSet\Control\MediaProperties\PrivateProperties\DirectInput";
                using (var key = Registry.CurrentUser.OpenSubKey(basePath))
                {
                    if (key != null)
                    {
                        var guids = key.GetSubKeyNames();
                        foreach (var g in guids)
                        {
                            using (var devKey = key.OpenSubKey(g))
                            {
                                var cal = devKey?.OpenSubKey("Calibration");
                                var name = devKey?.GetValue("OEMName")?.ToString() ?? "Game Controller Profile";
                                results.Add(new DirectInputDeviceItem
                                {
                                    DeviceGuid = g,
                                    RegistryPath = $@"HKCU\{basePath}\{g}",
                                    JoystickName = name,
                                    HasCalibrationData = cal != null
                                });
                            }
                        }
                    }
                }
            }
            catch { }

            return results;
        }

        public static DirectInputCleanResult PurgeDirectInputProfiles()
        {
            var res = new DirectInputCleanResult();
            var devices = ScanDirectInputProfiles();
            res.TotalDevicesScanned = devices.Count;

            var basePath = @"System\CurrentControlSet\Control\MediaProperties\PrivateProperties\DirectInput";
            foreach (var d in devices)
            {
                try
                {
                    using (var key = Registry.CurrentUser.OpenSubKey(basePath, true))
                    {
                        if (key != null)
                        {
                            key.DeleteSubKeyTree(d.DeviceGuid, false);
                            res.ProfilesDeleted++;
                            res.Messages.Add($"[Purged] {d.DeviceGuid}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    res.Messages.Add($"[Skip] {d.DeviceGuid}: {ex.Message}");
                }
            }

            res.Success = true;
            return res;
        }
    }
}
