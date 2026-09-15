/*
    EBUninstaller Pro - Unit Test Suite
    NetBIOS Cache Flusher Tests
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.SystemTools;

namespace EBUninstallerTests
{
    [TestClass]
    public class NetBiosCacheFlusherTests
    {
        [TestMethod]
        public void TestQueryNetBiosCacheSafety()
        {
            var results = NetBiosCacheFlusherEngine.QueryNetBiosCache();
            Assert.IsNotNull(results);
        }

        [TestMethod]
        public void TestNetBiosNameEntryModel()
        {
            var entry = new NetBiosNameEntry
            {
                NetBiosName = "NAS-SERVER",
                IpAddress = "192.168.1.100",
                EntryType = "<00> UNIQUE",
                Scope = "Remote Cache"
            };

            Assert.AreEqual("NAS-SERVER", entry.NetBiosName);
            Assert.AreEqual("192.168.1.100", entry.IpAddress);
        }
    }
}
