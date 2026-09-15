/*
    EBUninstaller Pro - Unit Test Suite
    Storage Sense Policy Auditor Tests
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.SystemTools;

namespace EBUninstallerTests
{
    [TestClass]
    public class StorageSensePolicyAuditorTests
    {
        [TestMethod]
        public void TestQueryStorageSensePolicySafety()
        {
            var policy = StorageSensePolicyAuditorEngine.QueryStorageSensePolicy();
            Assert.IsNotNull(policy);
        }

        [TestMethod]
        public void TestStorageSensePolicyInfoModel()
        {
            var policy = new StorageSensePolicyInfo
            {
                IsStorageSenseEnabled = true,
                PurgeRecycleBinDays = 30,
                PurgeDownloadsDays = 0,
                PurgeTempFilesDays = 1
            };

            Assert.IsTrue(policy.IsStorageSenseEnabled);
            Assert.AreEqual(0, policy.PurgeDownloadsDays);
            Assert.IsTrue(policy.DownloadsPolicyDescription.Contains("Disabled"));
        }
    }
}
