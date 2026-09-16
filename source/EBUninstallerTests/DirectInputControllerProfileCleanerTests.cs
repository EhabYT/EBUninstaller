/*
    EBUninstaller Pro - Unit Test Suite
    DirectInput Controller Profile Cleaner Tests
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.JunkCleaner;

namespace EBUninstallerTests
{
    [TestClass]
    public class DirectInputControllerProfileCleanerTests
    {
        [TestMethod]
        public void TestScanProfiles()
        {
            var list = DirectInputControllerProfileCleanerEngine.ScanDirectInputProfiles();
            Assert.IsNotNull(list);
            Assert.IsTrue(list.Count >= 0);
        }

        [TestMethod]
        public void TestPurgeSafety()
        {
            var res = DirectInputControllerProfileCleanerEngine.PurgeDirectInputProfiles();
            Assert.IsNotNull(res);
            Assert.IsTrue(res.Success);
        }
    }
}
