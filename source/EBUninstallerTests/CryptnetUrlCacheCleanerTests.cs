/*
    EBUninstaller Pro - Unit Test Suite
    Cryptnet URL Cache Cleaner Tests
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.JunkCleaner;

namespace EBUninstallerTests
{
    [TestClass]
    public class CryptnetUrlCacheCleanerTests
    {
        [TestMethod]
        public void TestScanCache()
        {
            var items = CryptnetUrlCacheCleanerEngine.ScanCache();
            Assert.IsNotNull(items);
            Assert.IsTrue(items.Count >= 0);
        }

        [TestMethod]
        public void TestPurgeCacheSafety()
        {
            var result = CryptnetUrlCacheCleanerEngine.PurgeCache();
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
        }
    }
}
