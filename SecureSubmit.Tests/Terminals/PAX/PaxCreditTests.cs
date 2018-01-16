using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecureSubmit.Entities;
using SecureSubmit.Infrastructure;
using SecureSubmit.Terminals;
using SecureSubmit.Terminals.PAX;

namespace SecureSubmit.Tests.Terminals.PAX {
    [TestClass]
    public class PaxCreditTests {
        PaxDevice _device;

        [TestInitialize]
        public void Setup() {
            _device = new PaxDevice(new ConnectionConfig {
                ConnectionMode = ConnectionModes.TCP_IP,
                IpAddress = "10.12.220.172",
                Port = "10009",
                TimeOut = 10000
            });
        }

        [TestCleanup]
        public void TearDown() {
            _device.Dispose();
        }

        [TestMethod]
        public void Sale() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var response = _device.CreditSale(1, 10m)
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void SaleManual() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var card = new HpsCreditCard {
                Number = "4005554444444460",
                ExpMonth = 12,
                ExpYear = 25,
                Cvv = "123"
            };

            var address = new HpsAddress {
                Address = "1 Heartland Way",
                Zip = "95124"
            };

            var response = _device.CreditSale(1, 11m)
                .WithAllowDuplicates(true)
                .WithCard(card)
                .WithAddress(address)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void SaleWithSignatureCapture() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var card = new HpsCreditCard {
                Number = "4005554444444460",
                ExpMonth = 12,
                ExpYear = 17,
                Cvv = "123"
            };

            var address = new HpsAddress {
                Address = "1 Heartland Way",
                Zip = "95124"
            };

            var response = _device.CreditSale(1, 12m)
                .WithAllowDuplicates(true)
                .WithCard(card)
                .WithAddress(address)
                .WithSignatureCapture(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(HpsArgumentException))]
        public void SaleNoAmount() {
            _device.CreditSale(1).Execute();
        }

        [TestMethod, ExpectedException(typeof(HpsArgumentException))]
        public void SaleWithMultiplePayments() {
            _device.CreditSale(1)
                .WithCard(new HpsCreditCard())
                .WithToken("1234567")
                .Execute();
        }

        [TestMethod]
        public void Auth_Capture() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var response = _device.CreditAuth(1, 12m)
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            var captureResponse = _device.CreditCapture(2, 12m)
                .WithTransactionId(response.TransactionId)
                .Execute();
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("00", captureResponse.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(HpsArgumentException))]
        public void AuthNoAmount() {
            _device.CreditAuth(1).Execute();
        }

        [TestMethod, ExpectedException(typeof(HpsArgumentException))]
        public void AuthWithMultiplePayments() {
            _device.CreditAuth(1)
                .WithCard(new HpsCreditCard())
                .WithToken("1234567")
                .Execute();
        }

        [TestMethod, ExpectedException(typeof(HpsArgumentException))]
        public void CaptureNoTransactionId() {
            _device.CreditCapture(1).Execute();
        }

        [TestMethod]
        public void ReturnByTransactionId() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var card = new HpsCreditCard {
                Number = "4005554444444460",
                ExpMonth = 12,
                ExpYear = 17,
                Cvv = "123"
            };

            var address = new HpsAddress {
                Address = "1 Heartland Way",
                Zip = "95124"
            };

            var saleResponse = _device.CreditSale(1, 16m)
                .WithCard(card)
                .WithAddress(address)
                .WithAllowDuplicates(true)
                .WithRequestMultiUseToken(true)
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);

            var returnResponse = _device.CreditReturn(2, 16m)
                .WithTransactionId(saleResponse.TransactionId)
                .WithAuthCode(saleResponse.AuthorizationCode)
                .Execute();
            Assert.IsNotNull(returnResponse);
            Assert.AreEqual("00", returnResponse.ResponseCode);
        }

        [TestMethod]
        public void ReturnByCard() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var card = new HpsCreditCard {
                Number = "4005554444444460",
                ExpMonth = 12,
                ExpYear = 17,
                Cvv = "123"
            };

            var returnResponse = _device.CreditReturn(2, 14m)
                .WithCard(card)
                .Execute();
            Assert.IsNotNull(returnResponse);
            Assert.AreEqual("00", returnResponse.ResponseCode);
        }

        [TestMethod]
        public void ReturnByToken() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var token = "EdAJfE08RX802896yr3G4460";
            var returnResponse = _device.CreditReturn(2, 15m)
                .WithToken(token)
                .Execute();
            Assert.IsNotNull(returnResponse);
            Assert.AreEqual("00", returnResponse.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(HpsArgumentException))]
        public void ReturnWithMultiplePayments() {
            _device.CreditReturn(1)
                .WithTransactionId(1234567)
                .WithToken("1234567")
                .Execute();
        }

        [TestMethod, ExpectedException(typeof(HpsArgumentException))]
        public void ReturnNoAmount() {
            _device.CreditReturn(1).Execute();
        }

        [TestMethod, ExpectedException(typeof(HpsArgumentException))]
        public void ReturnByTransactionIdNoAuthCode() {
            _device.CreditReturn(2, 13m)
                .WithTransactionId(1234567)
                .Execute();
        }

        [TestMethod]
        public void Verify() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var response = _device.CreditVerify(1).Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("85", response.ResponseCode);
        }

        [TestMethod]
        public void VerifyManual() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var card = new HpsCreditCard {
                Number = "4005554444444460",
                ExpMonth = 12,
                ExpYear = 17,
                Cvv = "123"
            };

            var address = new HpsAddress {
                Address = "1 Heartland Way",
                Zip = "95124"
            };

            var response = _device.CreditVerify(1)
                .WithCard(card)
                .WithAddress(address)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("85", response.ResponseCode);
        }

        [TestMethod]
        public void Tokenize() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var response = _device.CreditVerify(1)
                .WithRequestMultiUseToken(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("85", response.ResponseCode);
            Assert.IsNotNull(response.Token);
        }

        [TestMethod]
        public void Void() {
            _device.OnMessageSent += (message) => {
                Assert.IsNotNull(message);
            };

            var card = new HpsCreditCard {
                Number = "4005554444444460",
                ExpMonth = 12,
                ExpYear = 17,
                Cvv = "123"
            };

            var address = new HpsAddress {
                Address = "1 Heartland Way",
                Zip = "95124"
            };

            var saleResponse = _device.CreditSale(1, 16m)
                .WithCard(card)
                .WithAddress(address)
                .WithAllowDuplicates(true)
                .WithRequestMultiUseToken(true)
                .Execute();
            Assert.IsNotNull(saleResponse);
            Assert.AreEqual("00", saleResponse.ResponseCode);

            var voidResponse = _device.CreditVoid(1).WithTransactionId(saleResponse.TransactionId).Execute();
            Assert.IsNotNull(voidResponse);
            Assert.AreEqual("00", voidResponse.ResponseCode);
        }

        [TestMethod, ExpectedException(typeof(HpsArgumentException))]
        public void VoidNoTransactionId() {
            _device.CreditVoid(1).Execute();
        }

        [TestMethod, ExpectedException(typeof(HpsConfigurationException))]
        public void EditNoConfiguration() {
            _device.CreditEdit(10m).Execute();
        }

        #region Certification
        [TestMethod]
        public void test_case_14a() {
            _device = new PaxDevice(new ConnectionConfig {
                ConnectionMode = ConnectionModes.HTTP,
                IpAddress = "10.12.220.172",
                Port = "10009",
                DeviceId = 5569387,
                SiteId = 102311,
                LicenseId = 102308,
                UserName = "777700872100",
                Password = "$Test1234",
                Url = "https://cert.api2.heartlandportico.com/Hps.Exchange.PosGateway/PosGatewayService.asmx"
            });

            var response = _device.CreditAuth(1, 15.12m).Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            var capture = _device.CreditCapture(2, 18.12m)
                .WithGratuity(3m)
                .WithTransactionId(response.TransactionId)
                .Execute();
            Assert.IsNotNull(capture);
            Assert.AreEqual("00", capture.ResponseCode);
            Assert.IsNotNull(capture.SubTransaction);
            Assert.AreEqual("00", capture.SubTransaction.ResponseCode);
        }

        [TestMethod]
        public void test_case_14b() {
            _device = new PaxDevice(new ConnectionConfig {
                ConnectionMode = ConnectionModes.HTTP,
                IpAddress = "10.12.220.172",
                Port = "10009",
                DeviceId = 5569387,
                SiteId = 102311,
                LicenseId = 102308,
                UserName = "777700872100",
                Password = "$Test1234",
                Url = "https://cert.api2.heartlandportico.com/Hps.Exchange.PosGateway/PosGatewayService.asmx"
            });

            var response = _device.CreditSale(1, 15.12m)
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            var editResponse = _device.CreditEdit(18.12m)
                .WithGratuity(2m)
                .WithTransactionId(response.TransactionId)
                .Execute();
            Assert.IsNotNull(editResponse);
            Assert.AreEqual("00", editResponse.ResponseCode);
        }
        #endregion
    }
}
