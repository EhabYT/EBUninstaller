/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    GPU Vendor Shader & Pipeline Cache Cleaner Subsystem
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.IO;

namespace UninstallTools.JunkCleaner
{
    public sealed class GpuCacheItem
    {
        public string FilePath { get; set; } = string.Empty;
        public string Vendor { get; set; } = string.Empty; // NVIDIA, AMD, Intel
        public long FileSizeBytes { get; set; }
        public DateTime LastModified { get; set; }
    }

    public sealed class GpuCacheCleanResult
    {
        public int TotalFilesFound { get; set; }
        public int FilesDeleted { get; set; }
        public long BytesCleaned { get; set; }
        public List<string> Messages { get; set; } = new List<string>();
        public bool Success { get; set; }
    }

    public sealed class GpuDriverCacheCleanerEngine
    {
        public static List<GpuCacheItem> ScanGpuCaches()
        {
            var results = new List<GpuCacheItem>();

            var localApp = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            if (!string.IsNullOrEmpty(localApp))
            {
                // 1. NVIDIA Shader Caches
                ScanVendorDir(Path.Combine(localApp, "NVIDIA", "DXCache"), "NVIDIA DXCache", results);
                ScanVendorDir(Path.Combine(localApp, "NVIDIA", "GLCache"), "NVIDIA GLCache", results);
                ScanVendorDir(Path.Combine(localApp, "NVIDIA Corporation", "NV_Cache"), "NVIDIA NV_Cache", results);

                // 2. AMD Shader Caches
                ScanVendorDir(Path.Combine(localApp, "AMD", "DxCache"), "AMD Radeon DxCache", results);
                ScanVendorDir(Path.Combine(localApp, "AMD", "VkCache"), "AMD Vulkan Cache", results);

                // 3. Intel Graphics Caches
                ScanVendorDir(Path.Combine(localApp, "Intel", "ShaderCache"), "Intel Graphics ShaderCache", results);
                ScanVendorDir(Path.Combine(localApp, "Intel", "IGC"), "Intel Graphics Compiler Cache", results);
            }

            return results;
        }

        private static void ScanVendorDir(string dir, string vendor, List<GpuCacheItem> list)
        {
            if (Directory.Exists(dir))
            {
                try
                {
                    var files = Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories);
                    foreach (var f in files)
                    {
                        try
                        {
                            var fi = new FileInfo(f);
                            list.Add(new GpuCacheItem
                            {
                                FilePath = f,
                                Vendor = vendor,
                                FileSizeBytes = fi.Length,
                                LastModified = fi.LastWriteTime
                            });
                        }
                        catch { }
                    }
                }
                catch { }
            }
        }

        public static GpuCacheCleanResult PurgeGpuCaches()
        {
            var res = new GpuCacheCleanResult();
            var items = ScanGpuCaches();
            res.TotalFilesFound = items.Count;

            foreach (var item in items)
            {
                if (File.Exists(item.FilePath))
                {
                    try
                    {
                        var size = item.FileSizeBytes;
                        File.Delete(item.FilePath);
                        res.FilesDeleted++;
                        res.BytesCleaned += size;
                        res.Messages.Add($"[Purged] {item.FilePath} ({size} bytes)");
                    }
                    catch (Exception ex)
                    {
                        res.Messages.Add($"[Skip] {item.FilePath}: {ex.Message}");
                    }
                }
            }

            res.Success = true;
            return res;
        }
    }
}
