/*
    EBUninstaller Pro - Unit Test Suite
    Component Store Staging Cleaner Tests
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
    public class ComponentStoreStagingCleanerTests
    {
        [TestMethod]
        public void TestScanStagingPackagesSafety()
        {
            var pkgs = ComponentStoreStagingCleanerEngine.ScanStagingPackages();
            Assert.IsNotNull(pkgs);
        }

        [TestMethod]
        public void TestStagingPackageItemModel()
        {
            var item = new StagingPackageItem
            {
                FileName = "windows10.0-kb5034441-x64.cab",
                PackageType = "Windows Update Staging Payload",
                FilePath = @"C:\Windows\SoftwareDistribution\Download\windows10.0-kb5034441-x64.cab",
                SizeBytes = 268435456,
                CreationTime = DateTime.UtcNow
            };

            Assert.AreEqual("windows10.0-kb5034441-x64.cab", item.FileName);
            Assert.AreEqual(268435456, item.SizeBytes);
        }
    }
}
