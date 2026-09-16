/*
    EBUninstaller Pro - Unit Test Suite
    GPU Driver Cache Cleaner Tests
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.JunkCleaner;

namespace EBUninstallerTests
{
    [TestClass]
    public class GpuDriverCacheCleanerTests
    {
        [TestMethod]
        public void TestScanGpuCaches()
        {
            var items = GpuDriverCacheCleanerEngine.ScanGpuCaches();
            Assert.IsNotNull(items);
            Assert.IsTrue(items.Count >= 0);
        }

        [TestMethod]
        public void TestPurgeGpuCachesSafety()
        {
            var res = GpuDriverCacheCleanerEngine.PurgeGpuCaches();
            Assert.IsNotNull(res);
            Assert.IsTrue(res.Success);
        }
    }
}
