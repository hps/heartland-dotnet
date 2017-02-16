using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecureSubmit.Terminals;
using SecureSubmit.Terminals.PAX;

namespace SecureSubmit.Tests.Terminals.PAX {
    [TestClass]
    public class PaxBatchMessageTests {
        PaxDevice _device;

        [TestInitialize]
        public void Setup() {
            _device = new PaxDevice(new ConnectionConfig {
                ConnectionMode = ConnectionModes.HTTP,
                IpAddress = "10.12.220.172",
                Port = "10009"
            });
        }

        [TestCleanup]
        public void TearDown() {
            _device.Dispose();
        }

        [TestMethod]
        public void BatchClose() {
            _device.OnMessageSent += (message) => {
            };

            var response = _device.BatchClose();
            Assert.IsNotNull(response);
            Assert.AreEqual("OK", response.DeviceResponseText);
        }
    }
}
