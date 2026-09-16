/*
    EBUninstaller Pro - Unit Test Suite
    Windows Update Rollback Health Tests
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.SystemTools;

namespace EBUninstallerTests
{
    [TestClass]
    public class WindowsUpdateRollbackHealthTests
    {
        [TestMethod]
        public void TestAnalyzeServicingHealth()
        {
            var rep = WindowsUpdateRollbackHealthEngine.AnalyzeServicingHealth();
            Assert.IsNotNull(rep);
            Assert.IsTrue(rep.Success);
            Assert.IsNotNull(rep.RawDismOutput);
        }
    }
}
