/*
    EBUninstaller Pro - Unit Test Suite
    WSA Android Package Uninstaller Tests
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.StoreApps;

namespace EBUninstallerTests
{
    [TestClass]
    public class WsaPackageUninstallerTests
    {
        [TestMethod]
        public void TestScanWsaPackagesSafety()
        {
            var pkgs = WsaPackageUninstallerEngine.ScanWsaPackages();
            Assert.IsNotNull(pkgs);
        }

        [TestMethod]
        public void TestWsaPackageItemModel()
        {
            var item = new WsaPackageItem
            {
                PackageId = "com.spotify.music_z2779a370x5yp",
                DisplayName = "Spotify Music (Android)",
                Publisher = "Spotify Ltd.",
                InstallLocation = @"C:\Program Files\WindowsApps\com.spotify.music",
                IsOrphaned = false
            };

            Assert.AreEqual("Spotify Music (Android)", item.DisplayName);
            Assert.IsFalse(item.IsOrphaned);
            Assert.AreEqual("Spotify Ltd.", item.Publisher);
        }
    }
}
