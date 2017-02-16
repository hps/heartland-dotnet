using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecureSubmit.Infrastructure;
using SecureSubmit.Terminals;
using SecureSubmit.Terminals.PAX;
namespace SecureSubmit.Tests.Terminals.PAX {
    [TestClass]
    public class PaxDebitTests {

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

        /*Should differentiate the dollar amounts from test to test, should implements the message checking and missing all negative tests. i.e. Sale with no amount etc...*/
        /*Missing message checking in all tests*/
        [TestMethod, ExpectedException(typeof(HpsArgumentException))]
        public void DebitSaleBlankAmount() {
            _device.DebitSale(5)
                .WithAllowDuplicates(true)
                .Execute();
        }
        [TestMethod, ExpectedException(typeof(HpsArgumentException))]
        public void DebitSale0() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
                Assert.IsTrue(message.StartsWith("[STX]T02[FS]1.35[FS]01[FS]0[FS][US][US][US][US][US]1[FS]5[FS][FS][ETX]"));
            };

            var response = _device.DebitSale(5, 0m)
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }
        [TestMethod]
        public void DebitSale33PartialAuth22() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
                Assert.IsTrue(message.StartsWith("[STX]T02[FS]1.35[FS]01[FS]3300[FS][US][US][US][US][US]1[FS]5[FS][FS][ETX]"));
            };

            var response = _device.DebitSale(5, 33m)
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("10", response.ResponseCode);
            Assert.AreEqual(22m, response.TransactionAmount);
            Assert.AreEqual(11m, response.AmountDue);
        }
        [TestMethod]
        public void DebitSale55Decline() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
                Assert.IsTrue(message.StartsWith("[STX]T02[FS]1.35[FS]01[FS]5500[FS][US][US][US][US][US]1[FS]5[FS][FS][ETX]"));
            };

            var response = _device.DebitSale(5, 55m)
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("000100", response.DeviceResponseCode);
            Assert.AreEqual("DECLINE", response.DeviceResponseText);
        }
        [TestMethod]
        public void DebitSale() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
                Assert.IsTrue(message.StartsWith("[STX]T02[FS]1.35[FS]01[FS]1000[FS][US][US][US][US][US]1[FS]5[FS][FS][ETX]"));
            };

            var response = _device.DebitSale(5, 10m)
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }
        /*Need return by transaction ID in addition to this test*/
        [TestMethod]
        public void DebitReturn() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
                Assert.IsTrue(message.StartsWith("[STX]T02[FS]1.35[FS]02[FS]1000[FS][FS]6[FS][FS][ETX]"));
            };

            var response = _device.DebitReturn(6, 10m)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode, response.DeviceResponseText);
        }
        [TestMethod]
        public void DebitReturn10ByTransactionId() {

            var response = _device.DebitSale(5, 10m)
                .WithAllowDuplicates(true)
                .Execute();
            var saleTranID = Convert.ToInt32(response.TransactionId);

            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
                Assert.IsTrue(message.StartsWith("[STX]T02[FS]1.35[FS]02[FS]1000[FS][FS]5[FS][FS]HREF=" + saleTranID.ToString() + "[ETX]"));
            };

            var response2 = _device.DebitReturn(5, 10m)
                .WithTransactionId(saleTranID)
                .Execute();
            Assert.IsNotNull(response2);
            Assert.AreEqual("00", response2.ResponseCode);
        }
        [TestMethod, ExpectedException(typeof(HpsArgumentException))]
        public void DebitReturnBlankAmount() {
            _device.DebitReturn(5)
                .Execute();
        }
    }
}
