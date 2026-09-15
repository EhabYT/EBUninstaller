/*
    EBUninstaller Pro - Unit Test Suite
    WER ReportArchive Cleaner Tests
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
    public class WerReportArchiveCleanerTests
    {
        [TestMethod]
        public void TestScanWerArchivesSafety()
        {
            var archives = WerReportArchiveCleanerEngine.ScanWerArchives();
            Assert.IsNotNull(archives);
        }

        [TestMethod]
        public void TestWerReportArchiveItemModel()
        {
            var item = new WerReportArchiveItem
            {
                ReportFolder = @"C:\ProgramData\Microsoft\Windows\WER\ReportArchive\AppCrash_photoviewer.exe_12345",
                ApplicationName = "photoviewer.exe",
                EventType = "AppCrash",
                SizeBytes = 5242880,
                LastModified = DateTime.UtcNow
            };

            Assert.AreEqual("photoviewer.exe", item.ApplicationName);
            Assert.AreEqual("AppCrash", item.EventType);
            Assert.AreEqual(5242880, item.SizeBytes);
        }
    }
}
