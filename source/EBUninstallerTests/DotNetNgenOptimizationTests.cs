/*
    EBUninstaller Pro - Unit Test Suite
    .NET NGEN Optimization Tests
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.SystemTools;

namespace EBUninstallerTests
{
    [TestClass]
    public class DotNetNgenOptimizationTests
    {
        [TestMethod]
        public void TestDiscoverNgenRuntimes()
        {
            var list = DotNetNgenOptimizationEngine.DiscoverNgenRuntimes();
            Assert.IsNotNull(list);
            Assert.IsTrue(list.Count >= 0);
        }

        [TestMethod]
        public void TestExecuteQueuedItemsSafety()
        {
            var res = DotNetNgenOptimizationEngine.ExecuteQueuedItems();
            Assert.IsNotNull(res);
            Assert.IsTrue(res.Success);
        }
    }
}
