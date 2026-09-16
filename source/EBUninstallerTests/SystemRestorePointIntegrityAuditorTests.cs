/*
    EBUninstaller Pro - Unit Test Suite
    System Restore Point Integrity Tests
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.Backup;

namespace EBUninstallerTests
{
    [TestClass]
    public class SystemRestorePointIntegrityAuditorTests
    {
        [TestMethod]
        public void TestAuditRestorePoints()
        {
            var res = SystemRestorePointIntegrityAuditorEngine.AuditRestorePoints();
            Assert.IsNotNull(res);
            Assert.IsTrue(res.Success);
            Assert.IsTrue(res.TotalRestorePoints >= 0);
        }
    }
}
