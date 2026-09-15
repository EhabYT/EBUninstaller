/*
    EBUninstaller Pro - Unit Test Suite
    Modern App Details Card & UI Component Tests
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BulkCrapUninstaller.Controls;

namespace EBUninstallerTests
{
    [TestClass]
    public class ModernAppDetailsCardTests
    {
        [TestMethod]
        public void TestCardInstantiation()
        {
            var card = new ModernAppDetailsCard();
            Assert.IsNotNull(card);
            Assert.AreEqual(110, card.Height);
        }

        [TestMethod]
        public void TestEmptyState()
        {
            var card = new ModernAppDetailsCard();
            card.SetEmptyState();
            Assert.IsNotNull(card);
        }
    }
}
