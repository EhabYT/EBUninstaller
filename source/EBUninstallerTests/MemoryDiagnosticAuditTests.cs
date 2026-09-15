/*
    EBUninstaller Pro - Unit Test Suite
    Memory Diagnostics & RAM Allocation Tests
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.SystemTools;

namespace EBUninstallerTests
{
    [TestClass]
    public class MemoryDiagnosticAuditTests
    {
        [TestMethod]
        public void TestQueryMemoryDiagnosticStateSafety()
        {
            var info = MemoryDiagnosticAuditEngine.QueryMemoryDiagnosticState();
            Assert.IsNotNull(info);
        }

        [TestMethod]
        public void TestMemoryDiagnosticInfoModel()
        {
            var info = new MemoryDiagnosticInfo
            {
                TotalPhysicalMemoryBytes = 17179869184,
                FreePhysicalMemoryBytes = 8589934592,
                MemoryStickCount = 2,
                MemorySpeedAndType = "3200 MHz DDR4"
            };

            Assert.AreEqual((ulong)17179869184, info.TotalPhysicalMemoryBytes);
            Assert.AreEqual(2, info.MemoryStickCount);
            Assert.AreEqual("3200 MHz DDR4", info.MemorySpeedAndType);
        }
    }
}
