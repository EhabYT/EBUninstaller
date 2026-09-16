/*
    EBUninstaller Pro - Unit Test Suite
    System Timer Resolution Optimizer Tests
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.SystemTools;

namespace EBUninstallerTests
{
    [TestClass]
    public class SystemTimerResolutionOptimizerTests
    {
        [TestMethod]
        public void TestAuditTimerResolution()
        {
            var res = SystemTimerResolutionOptimizerEngine.AuditTimerResolution();
            Assert.IsNotNull(res);
            Assert.IsTrue(res.Success);
            Assert.IsTrue(res.CurrentResolutionMs > 0);
        }
    }
}
