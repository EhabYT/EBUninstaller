/*
    EBUninstaller Pro - Unit Test Suite
    System Resource Metrics Bar Tests
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BulkCrapUninstaller.Controls;

namespace EBUninstallerTests
{
    [TestClass]
    public class ModernSystemResourceMetricsBarTests
    {
        [TestMethod]
        public void TestMetricsBarInstantiation()
        {
            var bar = new ModernSystemResourceMetricsBar();
            Assert.IsNotNull(bar);
            Assert.AreEqual(28, bar.Height);
        }

        [TestMethod]
        public void TestRefreshMetricsSafety()
        {
            var bar = new ModernSystemResourceMetricsBar();
            bar.RefreshMetrics();
            Assert.IsNotNull(bar);
        }
    }
}
