/*
    EBUninstaller Pro - Unit Test Suite
    Windows Event Log Retention Policy Tests
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.SystemTools;

namespace EBUninstallerTests
{
    [TestClass]
    public class WindowsEventLogRetentionPolicyTests
    {
        [TestMethod]
        public void TestAuditEventLogChannels()
        {
            var res = WindowsEventLogRetentionPolicyEngine.AuditEventLogChannels();
            Assert.IsNotNull(res);
            Assert.IsTrue(res.Success);
            Assert.IsTrue(res.TotalChannelsScanned > 0);
        }

        [TestMethod]
        public void TestClearChannelSafety()
        {
            var dummy = WindowsEventLogRetentionPolicyEngine.ClearEventLogChannel("NonExistentDummyChannelForTest");
            Assert.IsFalse(dummy);
        }
    }
}
