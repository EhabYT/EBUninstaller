/*
    EBUninstaller Pro - Unit Test Suite
    Winsock Protocol Catalog Auditor Tests
    Copyright (c) 2026 EhabYT. All rights reserved.
*/

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UninstallTools.SystemTools;

namespace EBUninstallerTests
{
    [TestClass]
    public class WinsockProtocolCatalogAuditorTests
    {
        [TestMethod]
        public void TestScanProtocolCatalogSafety()
        {
            var entries = WinsockProtocolCatalogAuditorEngine.ScanProtocolCatalog();
            Assert.IsNotNull(entries);
        }

        [TestMethod]
        public void TestWinsockProtocolEntryModel()
        {
            var entry = new WinsockProtocolEntry
            {
                EntryId = "000000000001",
                ProtocolName = "MSAFD Tcpip [TCP/IP]",
                ProviderDllPath = @"%SystemRoot%\system32\mswsock.dll",
                IsDllMissing = false
            };

            Assert.AreEqual("MSAFD Tcpip [TCP/IP]", entry.ProtocolName);
            Assert.IsFalse(entry.IsDllMissing);
            Assert.AreEqual("Valid Protocol Provider", entry.HealthStatus);
        }
    }
}
