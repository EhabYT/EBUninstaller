/*
    EBUninstaller Pro - Unit Test Suite
    MRU History Cleaner Tests
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.PrivacyCleaner;

namespace EBUninstallerTests
{
    [TestClass]
    public class MruRecentHistoryCleanerTests
    {
        [TestMethod]
        public void TestScanMruEntries()
        {
            var entries = MruRecentHistoryCleanerEngine.ScanMruEntries();
            Assert.IsNotNull(entries);
            Assert.IsTrue(entries.Count >= 0);
        }

        [TestMethod]
        public void TestPurgeMruHistorySafety()
        {
            var res = MruRecentHistoryCleanerEngine.PurgeMruHistory();
            Assert.IsNotNull(res);
            Assert.IsTrue(res.Success);
        }
    }
}
