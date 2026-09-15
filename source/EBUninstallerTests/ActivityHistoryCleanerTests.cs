/*
    EBUninstaller Pro - Unit Test Suite
    Activity History & Timeline Cleaner Tests
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.JunkCleaner;

namespace EBUninstallerTests
{
    [TestClass]
    public class ActivityHistoryCleanerTests
    {
        [TestMethod]
        public void TestScanActivityHistorySafety()
        {
            var stats = ActivityHistoryCleanerEngine.ScanActivityHistory();
            Assert.IsNotNull(stats);
        }

        [TestMethod]
        public void TestActivityHistoryStatsModel()
        {
            var stats = new ActivityHistoryStats
            {
                DatabaseCount = 3,
                TotalSizeBytes = 10485760
            };

            Assert.AreEqual(3, stats.DatabaseCount);
            Assert.AreEqual(10485760, stats.TotalSizeBytes);
        }
    }
}
