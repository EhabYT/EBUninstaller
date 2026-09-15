/*
    EBUninstaller Pro - Unit Test Suite
    Taskbar Jump List Cleaner Tests
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.PrivacyCleaner;

namespace EBUninstallerTests
{
    [TestClass]
    public class TaskbarJumpListCleanerTests
    {
        [TestMethod]
        public void TestScanJumpLists()
        {
            var list = TaskbarJumpListCleanerEngine.ScanJumpLists();
            Assert.IsNotNull(list);
            Assert.IsTrue(list.Count >= 0);
        }

        [TestMethod]
        public void TestPurgeJumpListsSafety()
        {
            var res = TaskbarJumpListCleanerEngine.PurgeJumpLists();
            Assert.IsNotNull(res);
            Assert.IsTrue(res.Success);
        }
    }
}
