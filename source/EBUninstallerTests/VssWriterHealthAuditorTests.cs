/*
    EBUninstaller Pro - Unit Test Suite
    VSS Writer Health Auditor Tests
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.Backup;

namespace EBUninstallerTests
{
    [TestClass]
    public class VssWriterHealthAuditorTests
    {
        [TestMethod]
        public void TestAuditWriters()
        {
            var res = VssWriterHealthAuditorEngine.AuditWriters();
            Assert.IsNotNull(res);
            Assert.IsTrue(res.Success);
            Assert.IsTrue(res.TotalWriters > 0);
        }
    }
}
