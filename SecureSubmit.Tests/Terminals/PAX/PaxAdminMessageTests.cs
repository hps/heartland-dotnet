using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecureSubmit.Infrastructure;
using SecureSubmit.Terminals;
using SecureSubmit.Terminals.PAX;

namespace SecureSubmit.Tests.Terminals.PAX {
    [TestClass]
    public class PaxAdminMessageTests {
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
        public void Initialize() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
                Assert.IsTrue(message.StartsWith("[STX]A00[FS]1.35[FS][ETX]"));
            };

            var response = _device.Initialize();
            Assert.IsNotNull(response);
            Assert.AreEqual("OK", response.DeviceResponseText);
            Assert.IsNotNull(response.SerialNumber);
        }

        [TestMethod, ExpectedException(typeof(HpsMessageException))]
        public void Cancel() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
                Assert.AreEqual("[STX]A14[FS]1.31[FS][ETX]_", message);
            };

            _device.Cancel();
        }

        [TestMethod]
        public void Reset() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
                Assert.IsTrue(message.StartsWith("[STX]A16[FS]1.35[FS][ETX]"));
            };

            var response = _device.Reset();
            Assert.IsNotNull(response);
            Assert.AreEqual("OK", response.DeviceResponseText);
        }

        [TestMethod, Ignore]
        public void Reboot() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
                Assert.AreEqual("[STX]A26[FS]1.31[FS][ETX][", message);
            };

            var response = _device.Reboot();
            Assert.IsNotNull(response);
            Assert.AreEqual("OK", response.DeviceResponseText);
        }
    }
}
