/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Device Driver Discovery & Backup Subsystem
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace UninstallTools.SystemTools
{
    public sealed class DeviceDriverItem
    {
        public string DriverInfName { get; set; } = string.Empty;
        public string ProviderName { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string DriverVersion { get; set; } = string.Empty;
        public string DateString { get; set; } = string.Empty;
        public string OriginalFileName { get; set; } = string.Empty;
    }

    public sealed class DeviceDriverBackupResult
    {
        public int TotalDriversFound { get; set; }
        public int ExportedDriversCount { get; set; }
        public string DestinationPath { get; set; } = string.Empty;
        public bool Success { get; set; }
        public List<string> Messages { get; set; } = new List<string>();
    }

    public sealed class DeviceDriverBackupEngine
    {
        public static List<DeviceDriverItem> QueryThirdPartyDrivers()
        {
            var list = new List<DeviceDriverItem>();
            try
            {
                var windir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
                var infDir = Path.Combine(windir, "INF");
                if (Directory.Exists(infDir))
                {
                    var oemInfs = Directory.GetFiles(infDir, "oem*.inf");
                    foreach (var inf in oemInfs)
                    {
                        var fileName = Path.GetFileName(inf);
                        var item = new DeviceDriverItem
                        {
                            DriverInfName = fileName,
                            OriginalFileName = inf,
                            ProviderName = "Third-Party OEM",
                            ClassName = "System Device",
                            DriverVersion = "Installed"
                        };

                        try
                        {
                            var lines = File.ReadLines(inf);
                            foreach (var line in lines)
                            {
                                var trim = line.Trim();
                                if (trim.StartsWith("Provider=", StringComparison.OrdinalIgnoreCase))
                                    item.ProviderName = trim.Substring("Provider=".Length).Trim('"', ' ', '%');
                                else if (trim.StartsWith("Class=", StringComparison.OrdinalIgnoreCase))
                                    item.ClassName = trim.Substring("Class=".Length).Trim('"', ' ');
                                else if (trim.StartsWith("DriverVer=", StringComparison.OrdinalIgnoreCase))
                                    item.DriverVersion = trim.Substring("DriverVer=".Length).Trim('"', ' ');
                            }
                        }
                        catch { }

                        list.Add(item);
                    }
                }
            }
            catch { }

            return list;
        }

        public static DeviceDriverBackupResult ExportDrivers(string destinationDirectory)
        {
            var result = new DeviceDriverBackupResult
            {
                DestinationPath = destinationDirectory
            };

            var drivers = QueryThirdPartyDrivers();
            result.TotalDriversFound = drivers.Count;

            try
            {
                if (!Directory.Exists(destinationDirectory))
                    Directory.CreateDirectory(destinationDirectory);

                // Export drivers using DISM command if available
                var psi = new ProcessStartInfo
                {
                    FileName = "dism.exe",
                    Arguments = $"/online /export-driver /destination:\"{destinationDirectory}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using (var proc = Process.Start(psi))
                {
                    if (proc != null)
                    {
                        proc.WaitForExit(30000);
                        var output = proc.StandardOutput.ReadToEnd();
                        result.Messages.Add(output);
                        result.Success = proc.ExitCode == 0;
                        result.ExportedDriversCount = drivers.Count;
                    }
                }
            }
            catch (Exception ex)
            {
                result.Messages.Add($"Export fallback triggered: {ex.Message}");
                // Fallback: Copy inf files
                int copied = 0;
                foreach (var drv in drivers)
                {
                    if (File.Exists(drv.OriginalFileName))
                    {
                        try
                        {
                            var dest = Path.Combine(destinationDirectory, drv.DriverInfName);
                            File.Copy(drv.OriginalFileName, dest, true);
                            copied++;
                        }
                        catch { }
                    }
                }
                result.ExportedDriversCount = copied;
                result.Success = copied > 0;
            }

            return result;
        }
    }
}
