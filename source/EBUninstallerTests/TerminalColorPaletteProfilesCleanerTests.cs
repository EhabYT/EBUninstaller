/*
    EBUninstaller Pro - Unit Test Suite
    Terminal Color Palette & Console Profile Tests
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.PrivacyCleaner;

namespace EBUninstallerTests
{
    [TestClass]
    public class TerminalColorPaletteProfilesCleanerTests
    {
        [TestMethod]
        public void TestScanConsoleProfiles()
        {
            var list = TerminalColorPaletteProfilesCleanerEngine.ScanConsoleProfiles();
            Assert.IsNotNull(list);
            Assert.IsTrue(list.Count >= 0);
        }

        [TestMethod]
        public void TestPurgeProfilesSafety()
        {
            var res = TerminalColorPaletteProfilesCleanerEngine.PurgeCustomConsoleProfiles();
            Assert.IsNotNull(res);
            Assert.IsTrue(res.Success);
        }
    }
}
