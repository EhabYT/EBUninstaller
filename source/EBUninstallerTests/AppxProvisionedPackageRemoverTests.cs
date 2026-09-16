/*
    EBUninstaller Pro - Unit Test Suite
    Provisioned AppX Remover Tests
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.StoreApps;

namespace EBUninstallerTests
{
    [TestClass]
    public class AppxProvisionedPackageRemoverTests
    {
        [TestMethod]
        public void TestQueryProvisionedPackages()
        {
            var res = AppxProvisionedPackageRemoverEngine.QueryProvisionedPackages();
            Assert.IsNotNull(res);
            Assert.IsTrue(res.Success);
            Assert.IsTrue(res.TotalPackagesFound > 0);
        }

        [TestMethod]
        public void TestRemovePackageSafety()
        {
            var ok = AppxProvisionedPackageRemoverEngine.RemoveProvisionedPackage("NonExistent_Test_Package");
            Assert.IsFalse(ok);
        }
    }
}
