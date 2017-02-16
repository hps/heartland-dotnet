using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecureSubmit.Terminals;
using SecureSubmit.Terminals.PAX;

namespace SecureSubmit.Tests.Terminals.PAX {
    [TestClass]
    public class PaxConnectionTests {
        PaxDevice device;

        [TestInitialize]
        public void Setup() {
            
        }

        [TestCleanup]
        public void TearDown() {
            
        }

        [TestMethod, Ignore]
        public void PaxTcpConnection() {
            device = new PaxDevice(new ConnectionConfig {
                ConnectionMode = ConnectionModes.TCP_IP,
                IpAddress = "10.12.220.172",
                Port = "10009",
                TimeOut = 10000
            });

            var response = device.Initialize();
            Assert.IsNotNull(response);
            Assert.AreEqual("OK", response.DeviceResponseText);

            device.Dispose();
        }

        [TestMethod]
        public void PaxHttpConnection() {
            device = new PaxDevice(new ConnectionConfig {
                ConnectionMode = ConnectionModes.HTTP,
                IpAddress = "10.12.220.172",
                Port = "10009"
            });

            var response = device.Initialize();
            Assert.IsNotNull(response);
            Assert.AreEqual("OK", response.DeviceResponseText);

            device.Dispose();
        }
    }
}
