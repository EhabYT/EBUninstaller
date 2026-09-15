/*
    EBUninstaller Pro - Unit Test Suite
    Windows Telemetry DiagTrack Hardening Tests
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.SecurityHardening;

namespace EBUninstallerTests
{
    [TestClass]
    public class WindowsTelemetryDiagTrackTests
    {
        [TestMethod]
        public void TestGetTelemetryServices()
        {
            var services = WindowsTelemetryDiagTrackEngine.GetTelemetryServices();
            Assert.IsNotNull(services);
            Assert.IsTrue(services.Count >= 2);
        }

        [TestMethod]
        public void TestApplyPrivacyHardeningSafety()
        {
            var res = WindowsTelemetryDiagTrackEngine.ApplyPrivacyHardening(false);
            Assert.IsNotNull(res);
            Assert.IsTrue(res.Success);
        }
    }
}
