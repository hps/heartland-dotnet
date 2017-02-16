using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecureSubmit.Entities;
using SecureSubmit.Infrastructure;
using SecureSubmit.Terminals;
using SecureSubmit.Terminals.PAX;
using Hps.Exchange.PosGateway.Client;

namespace SecureSubmit.Tests.Terminals.PAX
{

    [TestClass]
    public class PaxGiftTests
    {
        PaxDevice _device;

        #region Setup/Teardown
        [TestInitialize]
        public void Setup()
        {
            _device = new PaxDevice(new ConnectionConfig
            {
                ConnectionMode = ConnectionModes.HTTP,
                IpAddress = "10.12.220.172",
                Port = "10009"
            });
        }

        [TestCleanup]
        public void TearDown()
        {
            _device.Dispose();
        }
        #endregion

        #region GiftSale
        [TestMethod]
        public void Sale()
        {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
                Assert.IsTrue(message.StartsWith("[STX]T06[FS]1.35[FS]01[FS]1000[FS][FS]1[FS][FS][ETX]"));
            };

            var response = _device.GiftSale(1, 10m).Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void SaleManual()
        {
            var card = new HpsGiftCard() { Value = "5022440000000000098" };

            _device.OnMessageSent += (message) =>
            {
                Assert.IsNotNull(message);
                Assert.IsTrue(message.StartsWith("[STX]T06[FS]1.35[FS]01[FS]1000[FS]5022440000000000098[FS]2[FS][FS][ETX]"));
            };

            var response = _device.GiftSale(2, 10m)
                .WithCard(card)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod, Ignore]
        public void SaleWithGratuity()
        {
            var card = new HpsGiftCard() { Value = "5022440000000000098" };

            _device.OnMessageSent += (message) =>
            {
                Assert.IsNotNull(message);
                Assert.IsTrue(message.StartsWith("[STX]T06[FS]1.35[FS]01[FS]800[US]200[FS]5022440000000000098[FS]3[FS][FS][ETX]"));
            };

            var response = _device.GiftSale(3, 8m)
                .WithCard(card)
                .WithGratuity(2m)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void SaleWithInvoiceNumber()
        {
            var card = new HpsGiftCard() { Value = "5022440000000000098" };

            _device.OnMessageSent += (message) =>
            {
                Assert.IsNotNull(message);
                Assert.IsTrue(message.StartsWith("[STX]T06[FS]1.35[FS]01[FS]800[FS]5022440000000000098[FS]4[US]1234[FS][FS][ETX]"));
            };

            var details = new HpsTransactionDetails { InvoiceNumber = "1234" };

            var response = _device.GiftSale(4, 8m)
                .WithCard(card)
                .WithDetails(details)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);

        }

        [TestMethod]
        public void SaleLoyaltyManual()
        {
            var card = new HpsGiftCard() { Value = "5022440000000000098" };

            _device.OnMessageSent += (message) =>
            {
                Assert.IsNotNull(message);
                Assert.IsTrue(message.StartsWith("[STX]T08[FS]1.35[FS]01[FS]1000[FS]5022440000000000098[FS]5[FS][FS][ETX]"));
            };

            var response = _device.GiftSale(5, 10m)
                .WithCurrency(currencyType.POINTS)
                .WithCard(card)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }        

        [TestMethod, ExpectedException(typeof(HpsArgumentException))]
        public void SaleNoAmount()
        {
            _device.GiftSale(6).Execute();
        }

        [TestMethod, ExpectedException(typeof(HpsArgumentException))]
        public void SaleNoCurrency()
        {
            _device.GiftSale(7, 10m).WithCurrency(null).Execute();
        }
        #endregion

        #region GiftAddValue
        [TestMethod]
        public void AddValueManual()
        {
            var card = new HpsGiftCard() { Value = "5022440000000000098" };

            _device.OnMessageSent += (message) =>
            {
                Assert.IsNotNull(message);
                Assert.IsTrue(message.StartsWith("[STX]T06[FS]1.35[FS]10[FS]1000[FS]5022440000000000098[FS]8[FS][FS][ETX]"));
            };

            var response = _device.GiftAddValue(8)
                .WithCard(card)
                .WithAmount(10m)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void AddValue()
        {
            _device.OnMessageSent += (message) =>
            {
                Assert.IsNotNull(message);
                Assert.IsTrue(message.StartsWith("[STX]T06[FS]1.35[FS]10[FS]1000[FS][FS]9[FS][FS][ETX]"));
            };

            var response = _device.GiftAddValue(9)
                .WithAmount(10m)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void AddValueManualLoyalty()
        {
            var card = new HpsGiftCard() { Value = "5022440000000000098" };

            _device.OnMessageSent += (message) =>
            {
                Assert.IsNotNull(message);
                Assert.IsTrue(message.StartsWith("[STX]T08[FS]1.35[FS]10[FS]800[FS]5022440000000000098[FS]10[FS][FS][ETX]"));
            };

            var response = _device.GiftAddValue(10)
                .WithCard(card)
                .WithCurrency(currencyType.POINTS)
                .WithAmount(8m)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(HpsArgumentException))]
        public void AddValueWithoutAmountThrowsException()
        {
            _device.GiftAddValue(11)
                .Execute();
        }

        [TestMethod, ExpectedException(typeof(HpsArgumentException))]
        public void AddWithoutCurrencyThrowsException()
        {
            _device.GiftAddValue(12).WithCurrency(null).Execute();
        }
        #endregion

        #region GiftVoid
        [TestMethod]
        public void VoidManual()
        {
            var card = new HpsGiftCard() { Value = "5022440000000000098" };
            var saleResponse = _device.GiftSale(13).WithAmount(10m).WithCard(card).Execute();

            _device.OnMessageSent += (message) =>
            {
                Assert.IsNotNull(message);
                Assert.IsTrue(message.StartsWith("[STX]T06[FS]1.35[FS]16[FS][FS][FS]13[FS][FS]HREF="+saleResponse.TransactionId+"[ETX]"));
            };

            var voidResponse = _device.GiftVoid(13)
                .WithTransactionId(saleResponse.TransactionId)
                .Execute();

            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("0", voidResponse.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(HpsArgumentException))]
        public void voidNoCurrency()
        {
            _device.GiftVoid(14)
                .WithCurrency(null)
                .WithTransactionId(1)
                .Execute();
        }

        [TestMethod, ExpectedException(typeof(HpsArgumentException))]
        public void voidNoTransactionId()
        {
            _device.GiftVoid(15)
                .WithTransactionId(0)
                .Execute();
        }
        #endregion

        #region GiftBalance
        [TestMethod]
        public void Balance()
        {
            _device.OnMessageSent += (message) =>
            {
                Assert.IsNotNull(message);
                Assert.IsTrue(message.StartsWith("[STX]T06[FS]1.35[FS]23[FS][FS][FS]16[FS][FS][ETX]"));
            };

            var response = _device.GiftBalance(16)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void BalanceManual()
        {
            var card = new HpsGiftCard() { Value = "5022440000000000098" };

            _device.OnMessageSent += (message) =>
            {
                Assert.IsNotNull(message);
                Assert.IsTrue(message.StartsWith("[STX]T06[FS]1.35[FS]23[FS][FS]5022440000000000098[FS]17[FS][FS][ETX]"));
            };

            var response = _device.GiftBalance(17)
                .WithCard(card)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void BalanceLoyaltyManual()
        {
            var card = new HpsGiftCard() { Value = "5022440000000000098" };

            _device.OnMessageSent += (message) =>
            {
                Assert.IsNotNull(message);
                Assert.IsTrue(message.StartsWith("[STX]T08[FS]1.35[FS]23[FS][FS]5022440000000000098[FS]18[FS][FS][ETX]"));
            };

            var response = _device.GiftBalance(18)
                .WithCard(card)
                .WithCurrency(currencyType.POINTS)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(HpsArgumentException))]
        public void BalanceNoCurrency()
        {
            _device.GiftBalance(19).WithCurrency(null).Execute();
        }
        #endregion

        #region Certification
        [TestMethod]
        public void test_case_15a() {
            var response = _device.GiftBalance(1).Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
            Assert.AreEqual(10m, response.BalanceAmount);
        }

        [TestMethod]
        public void test_case_15b() {
            var response = _device.GiftAddValue(2).WithAmount(8m).Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void test_case_15c() {
            var response = _device.GiftSale(3, 1m).Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        //public void test_case_15d() {
        //    var response = _device
        //    Assert.IsNotNull(response);
        //    Assert.AreEqual("00", response.ResponseCode);
        //}
        #endregion
    }
}
