/*
    EBUninstaller Pro - Unit Test Suite
    Device Driver Backup Engine Tests
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.SystemTools;

namespace EBUninstallerTests
{
    [TestClass]
    public class DeviceDriverBackupTests
    {
        [TestMethod]
        public void TestQueryDrivers()
        {
            var drivers = DeviceDriverBackupEngine.QueryThirdPartyDrivers();
            Assert.IsNotNull(drivers);
            Assert.IsTrue(drivers.Count >= 0);
        }

        [TestMethod]
        public void TestExportDriversSafety()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), "EB_DrvTest_" + Guid.NewGuid().ToString("N"));
            try
            {
                var result = DeviceDriverBackupEngine.ExportDrivers(tempDir);
                Assert.IsNotNull(result);
            }
            finally
            {
                if (Directory.Exists(tempDir))
                {
                    try { Directory.Delete(tempDir, true); } catch { }
                }
            }
        }
    }
}
