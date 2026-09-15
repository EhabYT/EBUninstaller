/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Print Spooler Health & Stuck Job Repair Subsystem
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace UninstallTools.SystemTools
{
    public sealed class SpoolerJobItem
    {
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string JobType { get; set; } = string.Empty; // SPL or SHD
        public long FileSizeBytes { get; set; }
        public DateTime CreatedTime { get; set; }
    }

    public sealed class SpoolerRepairResult
    {
        public int StuckJobsFound { get; set; }
        public int JobsPurged { get; set; }
        public bool SpoolerRestarted { get; set; }
        public List<string> Messages { get; set; } = new List<string>();
        public bool Success { get; set; }
    }

    public sealed class PrintSpoolerHealthRepairEngine
    {
        public static List<SpoolerJobItem> ScanStuckJobs()
        {
            var results = new List<SpoolerJobItem>();
            try
            {
                var winDir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
                var spoolDir = Path.Combine(winDir, "System32", "spool", "PRINTERS");
                if (Directory.Exists(spoolDir))
                {
                    var files = Directory.GetFiles(spoolDir, "*.*");
                    foreach (var file in files)
                    {
                        var ext = Path.GetExtension(file).ToLowerInvariant();
                        if (ext == ".spl" || ext == ".shd" || ext == ".tmp")
                        {
                            var fi = new FileInfo(file);
                            results.Add(new SpoolerJobItem
                            {
                                FilePath = file,
                                FileName = Path.GetFileName(file),
                                JobType = ext == ".spl" ? "Spool Print Data (.spl)" : (ext == ".shd" ? "Shadow Header (.shd)" : "Spool Temp"),
                                FileSizeBytes = fi.Length,
                                CreatedTime = fi.CreationTime
                            });
                        }
                    }
                }
            }
            catch { }

            return results;
        }

        public static SpoolerRepairResult RepairAndPurgeSpooler()
        {
            var res = new SpoolerRepairResult();
            var jobs = ScanStuckJobs();
            res.StuckJobsFound = jobs.Count;

            // Step 1: Attempt to stop spooler service
            try
            {
                var pStop = Process.Start(new ProcessStartInfo
                {
                    FileName = "net.exe",
                    Arguments = "stop spooler /y",
                    UseShellExecute = false,
                    CreateNoWindow = true
                });
                pStop?.WaitForExit(5000);
            }
            catch { }

            // Step 2: Delete stuck jobs
            foreach (var job in jobs)
            {
                if (File.Exists(job.FilePath))
                {
                    try
                    {
                        File.Delete(job.FilePath);
                        res.JobsPurged++;
                        res.Messages.Add($"[Purged] {job.FileName}");
                    }
                    catch (Exception ex)
                    {
                        res.Messages.Add($"[Skip] {job.FileName}: {ex.Message}");
                    }
                }
            }

            // Step 3: Restart spooler service
            try
            {
                var pStart = Process.Start(new ProcessStartInfo
                {
                    FileName = "net.exe",
                    Arguments = "start spooler",
                    UseShellExecute = false,
                    CreateNoWindow = true
                });
                pStart?.WaitForExit(5000);
                res.SpoolerRestarted = pStart != null && pStart.ExitCode == 0;
            }
            catch { }

            res.Success = true;
            return res;
        }
    }
}
