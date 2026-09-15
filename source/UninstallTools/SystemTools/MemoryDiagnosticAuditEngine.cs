/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Windows Memory Diagnostics & RAM Allocation Auditor Subsystem
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Diagnostics;
using System.IO;
using System.Management;
using UninstallTools.Core;

namespace UninstallTools.SystemTools
{
    public sealed class MemoryDiagnosticInfo
    {
        public ulong TotalPhysicalMemoryBytes { get; set; }
        public ulong FreePhysicalMemoryBytes { get; set; }
        public int MemoryStickCount { get; set; }
        public string MemorySpeedAndType { get; set; } = "DDR4 / DDR5";
        public bool IsDiagnosticScheduled { get; set; }
    }

    public static class MemoryDiagnosticAuditEngine
    {
        public static MemoryDiagnosticInfo QueryMemoryDiagnosticState()
        {
            var info = new MemoryDiagnosticInfo();

            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT TotalVisibleMemorySize, FreePhysicalMemory FROM Win32_OperatingSystem");
                foreach (var obj in searcher.Get())
                {
                    if (ulong.TryParse(obj["TotalVisibleMemorySize"]?.ToString(), out var totalKb))
                    {
                        info.TotalPhysicalMemoryBytes = totalKb * 1024;
                    }
                    if (ulong.TryParse(obj["FreePhysicalMemory"]?.ToString(), out var freeKb))
                    {
                        info.FreePhysicalMemoryBytes = freeKb * 1024;
                    }
                }

                using var stickSearcher = new ManagementObjectSearcher("SELECT Speed, MemoryType, SMBIOSMemoryType FROM Win32_PhysicalMemory");
                int sticks = 0;
                string speedStr = string.Empty;
                foreach (var stick in stickSearcher.Get())
                {
                    sticks++;
                    var sp = stick["Speed"]?.ToString();
                    if (!string.IsNullOrEmpty(sp)) speedStr = $"{sp} MHz";
                }
                info.MemoryStickCount = sticks > 0 ? sticks : 1;
                info.MemorySpeedAndType = string.IsNullOrEmpty(speedStr) ? "Standard RAM" : speedStr;
            }
            catch (Exception ex)
            {
                StructuredLogger.Warning(LogCategory.General, "Failed querying memory diagnostic state", ex.Message);
            }

            return info;
        }

        public static bool ScheduleMemoryTestOnNextReboot()
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "MdSched.exe"),
                    Arguments = "",
                    UseShellExecute = true
                };
                Process.Start(psi);
                StructuredLogger.Info(LogCategory.General, "Launched Windows Memory Diagnostic (MdSched.exe)");
                return true;
            }
            catch (Exception ex)
            {
                StructuredLogger.Error(LogCategory.General, "Failed launching MdSched", ex.Message);
                return false;
            }
        }
    }
}
