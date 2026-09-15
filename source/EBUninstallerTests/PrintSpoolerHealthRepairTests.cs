/*
    EBUninstaller Pro - Unit Test Suite
    Print Spooler Health Repair Tests
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.SystemTools;

namespace EBUninstallerTests
{
    [TestClass]
    public class PrintSpoolerHealthRepairTests
    {
        [TestMethod]
        public void TestScanStuckJobs()
        {
            var jobs = PrintSpoolerHealthRepairEngine.ScanStuckJobs();
            Assert.IsNotNull(jobs);
            Assert.IsTrue(jobs.Count >= 0);
        }

        [TestMethod]
        public void TestRepairSpoolerSafety()
        {
            var res = PrintSpoolerHealthRepairEngine.RepairAndPurgeSpooler();
            Assert.IsNotNull(res);
            Assert.IsTrue(res.Success);
        }
    }
}
