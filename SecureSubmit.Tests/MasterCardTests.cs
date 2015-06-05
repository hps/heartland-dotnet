// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MasterCardTests.cs" company="Heartland Payment Systems">
//   Copyright (c) Heartland Payment Systems. All rights reserved.
// </copyright>
// <summary>
//   MasterCard unit tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using SecureSubmit.Services.Credit;
using SecureSubmit.Services.Debit;

// ReSharper disable InconsistentNaming

namespace SecureSubmit.Tests
{
    using Entities;
    using Infrastructure;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Text.RegularExpressions;
    using TestData;

    /// <summary>MasterCard unit tests.</summary>
    [TestClass]
    public class MasterCardTests
    {
        /// <summary>The MasterCard is ok test method.</summary>
        [TestMethod]
        public void MasterCard_WhenCardIsOk_ShouldReturnValidResult()
        {
            var response = ChargeValidMasterCard(50);
            StringAssert.Matches(response.ResponseCode, new Regex("00"));
        }

        /// <summary>The MasterCard test transaction details.</summary>
        [TestMethod]
        public void MasterCard_WhenCardIsOkAndIncludesDetails_ShouldReturnValidResult()
        {
            var service = new HpsCreditService(TestServicesConfig.ValidServicesConfig());
            var charge = service.Charge(50, "usd", TestCreditCard.ValidMasterCard,
                TestCardHolder.ValidCardHolder, false, "descriptor", true, new HpsTransactionDetails
                {
                    Memo = "memo",
                    InvoiceNumber = "1234",
                    CustomerId = "customerID",
                    ClientTransactionId = 12345678
                });

            Assert.IsNotNull(charge);
            StringAssert.Matches(charge.ResponseCode, new Regex("00"));

            var transaction = service.Get(charge.TransactionId);
            Assert.IsNotNull(transaction);
            StringAssert.Matches(transaction.Memo, new Regex("memo"));
            StringAssert.Matches(transaction.InvoiceNumber, new Regex("1234"));
            StringAssert.Matches(transaction.CustomerId, new Regex("customerID"));
            Assert.AreEqual(charge.ClientTransactionId, 12345678);
        }

        #region AVS Tests

        /// <summary>AVS result code should be "A" test method.</summary>
        [TestMethod]
        public void MasterCard_AvsResultCode_ShouldEqualA()
        {
            var response = ChargeValidMasterCard(90.01M);
            StringAssert.Matches(response.AvsResultCode, new Regex("^A$"));
        }

        /// <summary>AVS result code should be "N" test method.</summary>
        [TestMethod]
        public void MasterCard_AvsResultCode_ShouldEqualN()
        {
            var response = ChargeValidMasterCard(90.02M);
            StringAssert.Matches(response.AvsResultCode, new Regex("^N$"));
        }

        /// <summary>AVS result code should be "R" test method.</summary>
        [TestMethod]
        public void MasterCard_AvsResultCode_ShouldEqualR()
        {
            var response = ChargeValidMasterCard(90.03M);
            StringAssert.Matches(response.AvsResultCode, new Regex("^R$"));
        }

        /// <summary>AVS result code should be "S" test method.</summary>
        [TestMethod]
        public void MasterCard_AvsResultCode_ShouldEqualS()
        {
            var response = ChargeValidMasterCard(90.04M);
            StringAssert.Matches(response.AvsResultCode, new Regex("^S$"));
        }

        /// <summary>AVS result code should be "U" test method.</summary>
        [TestMethod]
        public void MasterCard_AvsResultCode_ShouldEqualU()
        {
            var response = ChargeValidMasterCard(90.05M);
            StringAssert.Matches(response.AvsResultCode, new Regex("^U$"));
        }

        /// <summary>AVS result code should be "W" test method.</summary>
        [TestMethod]
        public void MasterCard_AvsResultCode_ShouldEqualW()
        {
            var response = ChargeValidMasterCard(90.06M);
            StringAssert.Matches(response.AvsResultCode, new Regex("^W$"));
        }

        /// <summary>AVS result code should be "X" test method.</summary>
        [TestMethod]
        public void MasterCard_AvsResultCode_ShouldEqualX()
        {
            var response = ChargeValidMasterCard(90.07M);
            StringAssert.Matches(response.AvsResultCode, new Regex("^X$"));
        }

        /// <summary>AVS result code should be "Y" test method.</summary>
        [TestMethod]
        public void MasterCard_AvsResultCode_ShouldEqualY()
        {
            var response = ChargeValidMasterCard(90.08M);
            StringAssert.Matches(response.AvsResultCode, new Regex("^Y$"));
        }

        /// <summary>AVS result code should be "Z" test method.</summary>
        [TestMethod]
        public void MasterCard_AvsResultCode_ShouldEqualZ()
        {
            var response = ChargeValidMasterCard(90.09M);
            StringAssert.Matches(response.AvsResultCode, new Regex("^Z$"));
        }

        #endregion

        #region CVV Tests

        /// <summary>CVV result code should be "M" test method.</summary>
        [TestMethod]
        public void MasterCard_CvvResultCode_ShouldEqualM()
        {
            var response = ChargeValidMasterCard(95.01M);
            StringAssert.Matches(response.CvvResultCode, new Regex("^M$"));
        }

        /// <summary>CVV result code should be "N" test method.</summary>
        [TestMethod]
        public void MasterCard_CvvResultCode_ShouldEqualN()
        {
            var response = ChargeValidMasterCard(95.02M);
            StringAssert.Matches(response.CvvResultCode, new Regex("^N$"));
        }

        /// <summary>CVV result code should be "P" test method.</summary>
        [TestMethod]
        public void MasterCard_CvvResultCode_ShouldEqualP()
        {
            var response = ChargeValidMasterCard(95.03M);
            StringAssert.Matches(response.CvvResultCode, new Regex("^P$"));
        }

        /// <summary>CVV result code should be "U" test method.</summary>
        [TestMethod]
        public void MasterCard_CvvResultCode_ShouldEqualU()
        {
            var response = ChargeValidMasterCard(95.04M);
            StringAssert.Matches(response.CvvResultCode, new Regex("^U$"));
        }

        #endregion

        #region MasterCard to 8583

        /// <summary>Transaction response code should indicate refer card issuer (ResponseText: 'CALLS', ResponseCode: '02').</summary>
        [TestMethod]
        public void Mastercard_ResponseCode_ShouldIndicateReferCardIssuer()
        {
            try
            {
                ChargeValidMasterCard(10.34M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.CardDeclined, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate term ID error (ResponseText: 'TERM ID ERROR', ResponseCode: '03').</summary>
        [TestMethod]
        public void Mastercard_ResponseCode_ShouldIndicateTermIdError()
        {
            try
            {
                ChargeValidMasterCard(10.22M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.CardDeclined, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate invalid merchant (ResponseText: 'HOLD-CALL', ResponseCode: '04').</summary>
        [TestMethod]
        public void Mastercard_ResponseCode_ShouldIndicateInvalidMerchant()
        {
            try
            {
                ChargeValidMasterCard(10.01M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.CardDeclined, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate decline (ResponseText: 'DECLINE', ResponseCode: '05').</summary>
        [TestMethod]
        public void Mastercard_ResponseCode_ShouldIndicateDoNotHonor()
        {
            try
            {
                ChargeValidMasterCard(10.25M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.CardDeclined, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate invalid transaction (ResponseText: 'INVALID TRANS', ResponseCode: '12').</summary>
        [TestMethod]
        public void Mastercard_ResponseCode_ShouldIndicateInvalidTransaction()
        {
            try
            {
                ChargeValidMasterCard(10.26M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.ProcessingError, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate invalid amount (ResponseText: 'AMOUNT ERROR', ResponseCode: '13').</summary>
        [TestMethod]
        public void Mastercard_ResponseCode_ShouldIndicateInvalidAmount()
        {
            try
            {
                ChargeValidMasterCard(10.27M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.InvalidAmount, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate invalid card (ResponseText: 'CARD NO. ERROR', ResponseCode: '14').</summary>
        [TestMethod]
        public void Mastercard_ResponseCode_ShouldIndicateInvalidCard()
        {
            try
            {
                ChargeValidMasterCard(10.28M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.IncorrectNumber, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate invalid issuer (ResponseText: 'NO SUCH ISSUER', ResponseCode: '15').</summary>
        [TestMethod]
        public void Mastercard_ResponseCode_ShouldIndicateInvalidIssuer()
        {
            try
            {
                ChargeValidMasterCard(10.18M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.ProcessingError, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate lost card (ResponseText: 'HOLD-CALL', ResponseCode: '41').</summary>
        [TestMethod]
        public void Mastercard_ResponseCode_ShouldIndicateLostCard()
        {
            try
            {
                ChargeValidMasterCard(10.31M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.CardDeclined, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate hold-call (ResponseText: 'HOLD-CALL', ResponseCode: '43').</summary>
        [TestMethod]
        public void Mastercard_ResponseCode_ShouldIndicateHoldCall()
        {
            try
            {
                ChargeValidMasterCard(10.03M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.CardDeclined, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate decline (ResponseText: 'DECLINE', ResponseCode: '51').</summary>
        [TestMethod]
        public void Mastercard_ResponseCode_ShouldIndicateDecline()
        {
            try
            {
                ChargeValidMasterCard(10.08M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.CardDeclined, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate expired card (ResponseText: 'EXPIRED CARD', ResponseCode: '54').</summary>
        [TestMethod]
        public void Mastercard_ResponseCode_ShouldIndicateExpiredCard()
        {
            try
            {
                ChargeValidMasterCard(10.32M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.ExpiredCard, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate exceeds limit (ResponseText: 'DECLINE', ResponseCode: '61').</summary>
        [TestMethod]
        public void Mastercard_ResponseCode_ShouldIndicateExceedsLimit()
        {
            try
            {
                ChargeValidMasterCard(10.09M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.CardDeclined, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate restricted card (ResponseText: 'DECLINE', ResponseCode: '62').</summary>
        [TestMethod]
        public void Mastercard_ResponseCode_ShouldIndicateRestrictedCard()
        {
            try
            {
                ChargeValidMasterCard(10.10M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.CardDeclined, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate security violation (ResponseText: 'SEC VIOLATION', ResponseCode: '63').</summary>
        [TestMethod]
        public void Mastercard_ResponseCode_ShouldIndicateSecurityViolation()
        {
            try
            {
                ChargeValidMasterCard(10.19M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.CardDeclined, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate exceeds freq limit (ResponseText: 'DECLINE$', ResponseCode: '65').</summary>
        [TestMethod]
        public void Mastercard_ResponseCode_ShouldIndicateExceedsFreqLimit()
        {
            try
            {
                ChargeValidMasterCard(10.11M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.CardDeclined, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate invalid account (ResponseText: 'CARD NO. ERROR', ResponseCode: '14').</summary>
        [TestMethod]
        public void Mastercard_ResponseCode_ShouldIndicateCardNoError()
        {
            try
            {
                ChargeValidMasterCard(10.14M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.IncorrectNumber, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate format error (ResponseText: 'CID FORMAT ERROR', ResponseCode: '79').</summary>
        [TestMethod]
        public void Mastercard_ResponseCode_ShouldIndicateInvalidAccount()
        {
            try
            {
                ChargeValidMasterCard(10.06M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.ProcessingError, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate switch not available (ResponseText: 'NO REPLY', ResponseCode: '14').</summary>
        [TestMethod]
        public void Mastercard_ResponseCode_ShouldIndicateSwitchNotAvailable()
        {
            try
            {
                ChargeValidMasterCard(10.33M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.IssuerTimeout, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate system error (ResponseText: 'SYSTEM ERROR', ResponseCode: '96').</summary>
        [TestMethod]
        public void Mastercard_ResponseCode_ShouldIndicateSystemError()
        {
            try
            {
                ChargeValidMasterCard(10.21M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.ProcessingError, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        #endregion

        #region Verify, Authorize, Refund, Capture, Void, Recurring

        /// <summary>Mastercard verify should return response code '85'.</summary>
        [TestMethod]
        public void Mastercard_Verify_ShouldReturnOk()
        {
            var creditSvc = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var response = creditSvc.Verify(TestCreditCard.ValidMasterCard, TestCardHolder.ValidCardHolder);
            Assert.AreEqual("85", response.ResponseCode);
        }

        /// <summary>Mastercard authorize should return response code '00'.</summary>
        [TestMethod]
        public void Mastercard_Authorize_ShouldReturnOk()
        {
            var creditSvc = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var response = creditSvc.Authorize(50, "usd", TestCreditCard.ValidMasterCard, TestCardHolder.ValidCardHolder);
            Assert.AreEqual("00", response.ResponseCode);
        }

        /// <summary>Mastercard authorize should return response code '00'.</summary>
        [TestMethod]
        public void Mastercard_AuthorizeAndRequestToken_ShouldGetTokenAndReturnOk()
        {
            var creditSvc = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var response = creditSvc.Authorize(50, "usd", TestCreditCard.ValidMasterCard, TestCardHolder.ValidCardHolder, true);
            Assert.AreEqual(0, response.TokenData.TokenRspCode);
            Assert.AreEqual("00", response.ResponseCode);
        }

        /// <summary>MasterCard refund test.</summary>
        [TestMethod]
        public void MasterCard_ShouldRefund_Ok()
        {
            var creditSvc = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var creditResponse = creditSvc.Charge(25.00m, "usd", TestCreditCard.ValidMasterCard, TestCardHolder.CertCardHolderShortZipNoStreet);
            var refundResponse = creditSvc.Refund(25.00m, "usd", creditResponse.TransactionId);
            StringAssert.Matches(refundResponse.ResponseCode, new Regex("^00$"));
        }

        /// <summary>Mastercard authorize should return response code '00'.</summary>
        [TestMethod]
        public void Mastercard_Capture_ShouldReturnOk()
        {
            // Authorize the card.
            var creditSvc = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var authResponse = creditSvc.Authorize(50, "usd", TestCreditCard.ValidMasterCard, TestCardHolder.ValidCardHolder);
            Assert.AreEqual("00", authResponse.ResponseCode);

            // Capture the authorization.
            var captureResponse = creditSvc.Capture(authResponse.TransactionId, null);
            Assert.AreEqual("00", captureResponse.ResponseCode);
        }

        /// <summary>MasterCard void test.</summary>
        [TestMethod]
        public void MasterCard_ShouldVoid_Ok()
        {
            var creditSvc = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var creditResponse = creditSvc.Charge(25.00m, "usd", TestCreditCard.ValidMasterCard, TestCardHolder.CertCardHolderShortZipNoStreet);
            var voidResponse = creditSvc.Void(creditResponse.TransactionId).Execute();
            StringAssert.Matches(voidResponse.ResponseCode, new Regex("^00$"));
        }

        #endregion

        #region Card Present

        [TestMethod]
        public void Mastercard_WhenValidTrackData_ShouldReturnValidResult()
        {
            var r = new Random();
            var service = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var charge = service.Charge(r.Next(1, 50), "usd", new HpsTrackData
            {
                Value = "%B5473500000000014^MC TEST CARD^251210199998888777766665555444433332?;5473500000000014=25121019999888877776?",
                Method = HpsTrackDataMethod.Swipe
            });

            Assert.IsNotNull(charge);
            StringAssert.Matches(charge.ResponseCode, new Regex("00"));
        }

        #endregion

        #region Debit

        [TestMethod]
        public void Mastercard_Debit_WhenValidTrackData_ShouldChargeOk()
        {
            var service = new HpsDebitService(TestServicesConfig.ValidServicesConfig());
            var charge = service.Charge(50, "usd", "%B5473500000000014^MC TEST CARD^251210199998888777766665555444433332?;5473500000000014=25121019999888877776?", "F505AD81659AA42A3D123412324000AB", null, true);
            Assert.IsNotNull(charge);
            StringAssert.Matches(charge.ResponseCode, new Regex("00"));
        }

        [TestMethod]
        public void Mastercard_Debit_WhenValidE3Data_ShouldChargeOk()
        {
            const string e3 = "&lt;E1052711%B5473501000000014^MC TEST CARD^251200000000000000000000000000000000?|GVEY/MKaKXuqqjKRRueIdCHP" +
                "Poj1gMccgNOtHC41ymz7bIvyJJVdD3LW8BbwvwoenI+|+++++++C4cI2zjMp|11;5473501000000014=25120000000000000000?|8XqYkQGMdGeiIsgM0" +
                "pzdCbEGUDP|+++++++C4cI2zjMp|00|||/wECAQECAoFGAgEH2wYcShV78RZwb3NAc2VjdXJlZXhjaGFuZ2UubmV0PX50qfj4dt0lu9oFBESQQNkpoxEVpCW" +
                "3ZKmoIV3T93zphPS3XKP4+DiVlM8VIOOmAuRrpzxNi0TN/DWXWSjUC8m/PI2dACGdl/hVJ/imfqIs68wYDnp8j0ZfgvM26MlnDbTVRrSx68Nzj2QAgpBCHca" +
                "Bb/FZm9T7pfMr2Mlh2YcAt6gGG1i2bJgiEJn8IiSDX5M2ybzqRT86PCbKle/XCTwFFe1X|&gt;";
            var service = new HpsDebitService(TestServicesConfig.ValidServicesConfig());
            var charge = service.Charge(50, "usd", e3, "F505AD81659AA42A3D123412324000AB", new HpsEncryptionData
            {
                Version = "01"
            }, true);
            Assert.IsNotNull(charge);
            StringAssert.Matches(charge.ResponseCode, new Regex("00"));
        }

        [TestMethod]
        public void Mastercard_Debit_WhenValidTrackData_ShouldReturnOk()
        {
            var service = new HpsDebitService(TestServicesConfig.ValidServicesConfig());
            var charge = service.Charge(50, "usd", "%B5473500000000014^MC TEST CARD^251210199998888777766665555444433332?;5473500000000014=25121019999888877776?", "F505AD81659AA42A3D123412324000AB", null, true);
            Assert.IsNotNull(charge);
            StringAssert.Matches(charge.ResponseCode, new Regex("00"));

            var debitReturn = service.Return(charge.TransactionId, 50, "%B5473500000000014^MC TEST CARD^251210199998888777766665555444433332?;5473500000000014=25121019999888877776?", "F505AD81659AA42A3D123412324000AB", true);
            Assert.IsNotNull(debitReturn);
            StringAssert.Matches(debitReturn.ResponseCode, new Regex("00"));
        }

        [TestMethod]
        public void Mastercard_Debit_WhenValidTrackData_ShouldReverseOk()
        {
            var service = new HpsDebitService(TestServicesConfig.ValidServicesConfig());
            var charge = service.Charge(50, "usd", "%B5473500000000014^MC TEST CARD^251210199998888777766665555444433332?;5473500000000014=25121019999888877776?", "F505AD81659AA42A3D123412324000AB", null, true);
            Assert.IsNotNull(charge);
            StringAssert.Matches(charge.ResponseCode, new Regex("00"));

            var reverse = service.Reverse(charge.TransactionId, 50, "%B5473500000000014^MC TEST CARD^251210199998888777766665555444433332?;5473500000000014=25121019999888877776?");
            Assert.IsNotNull(reverse);
            StringAssert.Matches(reverse.ResponseCode, new Regex("00"));
        }

        #endregion

        /// <summary>Charge a MC with a valid config and valid MC info.</summary>
        /// <param name="amt">Amount to charge</param>
        /// <returns>The HPS Charge.</returns>
        private HpsCharge ChargeValidMasterCard(decimal amt)
        {
            var creditSvc = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var response = creditSvc.Charge(amt, "usd", TestCreditCard.ValidMasterCard, TestCardHolder.ValidCardHolder);
            if (response == null)
            {
                Assert.Fail("Response is null.");
            }

            return response;
        }
    }
}
// ReSharper restore InconsistentNaming