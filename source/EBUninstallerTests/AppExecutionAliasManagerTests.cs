/*
    EBUninstaller Pro - Unit Test Suite
    App Execution Alias Manager Tests
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.SystemTools;

namespace EBUninstallerTests
{
    [TestClass]
    public class AppExecutionAliasManagerTests
    {
        [TestMethod]
        public void TestScanExecutionAliases()
        {
            var res = AppExecutionAliasManagerEngine.ScanExecutionAliases();
            Assert.IsNotNull(res);
            Assert.IsTrue(res.Success);
            Assert.IsTrue(res.TotalAliasesFound >= 0);
        }
    }
}
