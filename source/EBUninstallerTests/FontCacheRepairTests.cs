/*
    EBUninstaller Pro - Unit Test Suite
    Font Cache Repair & Integrity Tests
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.SystemTools;

namespace EBUninstallerTests
{
    [TestClass]
    public class FontCacheRepairTests
    {
        [TestMethod]
        public void TestScanFontCaches()
        {
            var results = FontCacheRepairEngine.ScanFontCaches();
            Assert.IsNotNull(results);
            Assert.IsTrue(results.Count >= 0);
        }

        [TestMethod]
        public void TestRepairFontCaches()
        {
            var res = FontCacheRepairEngine.RepairAndPurge();
            Assert.IsNotNull(res);
            Assert.IsTrue(res.Success);
        }
    }
}
