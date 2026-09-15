/*
    EBUninstaller Pro - Professional Windows Uninstaller & System Maintenance
    Cryptnet URL Cache (CryptoAPI OCSP/CRL) Cleaner Subsystem
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.IO;

namespace UninstallTools.JunkCleaner
{
    public sealed class CryptnetCacheItem
    {
        public string FilePath { get; set; } = string.Empty;
        public string CacheType { get; set; } = string.Empty; // Content or MetaData
        public long FileSizeBytes { get; set; }
        public DateTime LastAccessTime { get; set; }
    }

    public sealed class CryptnetUrlCacheCleanResult
    {
        public int TotalItemsFound { get; set; }
        public int TotalItemsDeleted { get; set; }
        public long BytesCleaned { get; set; }
        public List<string> Messages { get; set; } = new List<string>();
        public bool Success { get; set; }
    }

    public sealed class CryptnetUrlCacheCleanerEngine
    {
        public static List<CryptnetCacheItem> ScanCache()
        {
            var results = new List<CryptnetCacheItem>();

            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            if (!string.IsNullOrEmpty(userProfile))
            {
                var cryptnetPath = Path.Combine(userProfile, "AppData", "LocalLow", "Microsoft", "CryptnetUrlCache");
                if (Directory.Exists(cryptnetPath))
                {
                    ScanSubfolder(Path.Combine(cryptnetPath, "Content"), "CRL/OCSP Content", results);
                    ScanSubfolder(Path.Combine(cryptnetPath, "MetaData"), "Certificate Metadata", results);
                }
            }

            return results;
        }

        private static void ScanSubfolder(string dir, string type, List<CryptnetCacheItem> results)
        {
            if (Directory.Exists(dir))
            {
                try
                {
                    var files = Directory.GetFiles(dir, "*", SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                        try
                        {
                            var fi = new FileInfo(file);
                            results.Add(new CryptnetCacheItem
                            {
                                FilePath = file,
                                CacheType = type,
                                FileSizeBytes = fi.Length,
                                LastAccessTime = fi.LastAccessTime
                            });
                        }
                        catch { }
                    }
                }
                catch { }
            }
        }

        public static CryptnetUrlCacheCleanResult PurgeCache()
        {
            var result = new CryptnetUrlCacheCleanResult();
            var items = ScanCache();
            result.TotalItemsFound = items.Count;

            foreach (var item in items)
            {
                if (File.Exists(item.FilePath))
                {
                    try
                    {
                        var size = item.FileSizeBytes;
                        File.Delete(item.FilePath);
                        result.TotalItemsDeleted++;
                        result.BytesCleaned += size;
                        result.Messages.Add($"[Purged] {item.FilePath} ({size} bytes)");
                    }
                    catch (Exception ex)
                    {
                        result.Messages.Add($"[Skip] {item.FilePath}: {ex.Message}");
                    }
                }
            }

            result.Success = true;
            return result;
        }
    }
}
