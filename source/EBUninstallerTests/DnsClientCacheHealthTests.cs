/*
    EBUninstaller Pro - Unit Test Suite
    DNS Client Resolver Cache Health Tests
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.SystemTools;

namespace EBUninstallerTests
{
    [TestClass]
    public class DnsClientCacheHealthTests
    {
        [TestMethod]
        public void TestCheckStatus()
        {
            var st = DnsClientCacheHealthEngine.CheckStatus();
            Assert.IsNotNull(st);
        }

        [TestMethod]
        public void TestFlushDnsSafety()
        {
            var res = DnsClientCacheHealthEngine.FlushDnsCache();
            Assert.IsNotNull(res);
        }
    }
}
