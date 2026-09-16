/*
    EBUninstaller Pro - Unit Test Suite
    Network Adapter NDIS Filter & Binding Auditor Tests
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.SystemTools;

namespace EBUninstallerTests
{
    [TestClass]
    public class NetworkAdapterBindingAuditorTests
    {
        [TestMethod]
        public void TestScanNetworkBindings()
        {
            var res = NetworkAdapterBindingAuditorEngine.ScanNetworkBindings();
            Assert.IsNotNull(res);
            Assert.IsTrue(res.Success);
            Assert.IsTrue(res.TotalBindingsFound >= 0);
        }
    }
}
