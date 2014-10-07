// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VisaTests.cs" company="Heartland Payment Systems">
//   Copyright (c) Heartland Payment Systems. All rights reserved.
// </copyright>
// <summary>
//   VISA unit tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Hps.Exchange.PosGateway.Client;
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

    /// <summary>VISA unit tests.</summary>
    [TestClass]
    public class VisaTests
    {
        /// <summary>The Visa is ok test method.</summary>
        [TestMethod]
        public void Visa_WhenCardIsOk_ShouldReturnValidResult()
        {
            var response = ChargeValidVisa(50);
            StringAssert.Matches(response.ResponseCode, new Regex("00"));
        }

        /// <summary>The Visa test transaction details.</summary>
        [TestMethod]
        public void Visa_WhenCardIsOkAndIncludesDetails_ShouldReturnValidResult()
        {
            var service = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var charge = service.Charge(50, "usd", TestCreditCard.ValidVisa,
                TestCardHolder.ValidCardHolder, false, "descriptor", false, new HpsTransactionDetails
                {
                    Memo = "memo",
                    InvoiceNumber = "1234",
                    CustomerId = "customerID"
                });

            Assert.IsNotNull(charge);
            StringAssert.Matches(charge.ResponseCode, new Regex("00"));

            var transaction = service.Get(charge.TransactionId);
            Assert.IsNotNull(transaction);
            StringAssert.Matches(transaction.Memo, new Regex("memo"));
            StringAssert.Matches(transaction.InvoiceNumber, new Regex("1234"));
            StringAssert.Matches(transaction.CustomerId, new Regex("customerID"));
        }

        #region AVS Tests

        /// <summary>AVS result code should be "B" test method.</summary>
        [TestMethod]
        public void Visa_AvsResultCode_ShouldEqualB()
        {
            var response = ChargeValidVisa(91.01M);
            StringAssert.Matches(response.AvsResultCode, new Regex("^B$"));
        }

        /// <summary>AVS result code should be "C" test method.</summary>
        [TestMethod]
        public void Visa_AvsResultCode_ShouldEqualC()
        {
            var response = ChargeValidVisa(91.02M);
            StringAssert.Matches(response.AvsResultCode, new Regex("^C$"));
        }

        /// <summary>AVS result code should be "D" test method.</summary>
        [TestMethod]
        public void Visa_AvsResultCode_ShouldEqualD()
        {
            var response = ChargeValidVisa(91.03M);
            StringAssert.Matches(response.AvsResultCode, new Regex("^D$"));
        }

        /// <summary>AVS result code should be "I" test method.</summary>
        [TestMethod]
        public void Visa_AvsResultCode_ShouldEqualI()
        {
            var response = ChargeValidVisa(91.05M);
            StringAssert.Matches(response.AvsResultCode, new Regex("^I$"));
        }

        /// <summary>AVS result code should be "M" test method.</summary>
        [TestMethod]
        public void Visa_AvsResultCode_ShouldEqualM()
        {
            var response = ChargeValidVisa(91.06M);
            StringAssert.Matches(response.AvsResultCode, new Regex("^M$"));
        }

        /// <summary>AVS result code should be "P" test method.</summary>
        [TestMethod]
        public void Visa_AvsResultCode_ShouldEqualP()
        {
            var response = ChargeValidVisa(91.07M);
            StringAssert.Matches(response.AvsResultCode, new Regex("^P$"));
        }

        #endregion

        #region CVV Tests

        /// <summary>CVV result code should be "M" test method.</summary>
        [TestMethod]
        public void Visa_CvvResultCode_ShouldEqualM()
        {
            var response = ChargeValidVisa(96.01M);
            StringAssert.Matches(response.CvvResultCode, new Regex("^M$"));
        }

        /// <summary>CVV result code should be "N" test method.</summary>
        [TestMethod]
        public void Visa_CvvResultCode_ShouldEqualN()
        {
            var response = ChargeValidVisa(96.02M);
            StringAssert.Matches(response.CvvResultCode, new Regex("^N$"));
        }

        /// <summary>CVV result code should be "P" test method.</summary>
        [TestMethod]
        public void Visa_CvvResultCode_ShouldEqualP()
        {
            var response = ChargeValidVisa(96.03M);
            StringAssert.Matches(response.CvvResultCode, new Regex("^P$"));
        }

        /// <summary>CVV result code should be "S" test method.</summary>
        [TestMethod]
        public void Visa_CvvResultCode_ShouldEqualS()
        {
            var response = ChargeValidVisa(96.04M);
            StringAssert.Matches(response.CvvResultCode, new Regex("^S$"));
        }

        /// <summary>CVV result code should be "U" test method.</summary>
        [TestMethod]
        public void Visa_CvvResultCode_ShouldEqualU()
        {
            var response = ChargeValidVisa(96.05M);
            StringAssert.Matches(response.CvvResultCode, new Regex("^U$"));
        }

        #endregion

        #region Visa to Visa 2nd

        /// <summary>Transaction response code should indicate refer card issuer (ResponseText: 'CALLS', ResponseCode: '02').</summary>
        [TestMethod]
        public void Visa_ResponseCode_ShouldIndicateReferCardIssuer()
        {
            try
            {
                ChargeValidVisa(10.34M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.CardDeclined, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate invalid merchant (ResponseText: 'TERM ID ERROR', ResponseCode: '03').</summary>
        [TestMethod]
        public void Visa_ResponseCode_ShouldIndicateInvalidMerchant()
        {
            try
            {
                ChargeValidVisa(10.22M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.CardDeclined, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate pick up card (ResponseText: 'HOLD-CALLS', ResponseCode: '44').</summary>
        [TestMethod]
        public void Visa_ResponseCode_ShouldIndicatePickUpCard()
        {
            try
            {
                ChargeValidVisa(10.04M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.CardDeclined, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate do not honor (ResponseText: 'DECLINE', ResponseCode: '05').</summary>
        [TestMethod]
        public void Visa_ResponseCode_ShouldIndicateDoNotHonor()
        {
            try
            {
                ChargeValidVisa(10.25M);
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
        public void Visa_ResponseCode_ShouldIndicateInvalidTransaction()
        {
            try
            {
                ChargeValidVisa(10.26M);
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
        public void Visa_ResponseCode_ShouldIndicateInvalidAmount()
        {
            try
            {
                ChargeValidVisa(10.27M);
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
        public void Visa_ResponseCode_ShouldIndicateInvalidCard()
        {
            try
            {
                ChargeValidVisa(10.28M);
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
        public void Visa_ResponseCode_ShouldIndicateInvalidIssuer()
        {
            try
            {
                ChargeValidVisa(10.18M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.ProcessingError, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate system error re-enter (ResponseText: 'RE ENTER', ResponseCode: '19').</summary>
        [TestMethod]
        public void Visa_ResponseCode_ShouldIndicateSystemErrorReenter()
        {
            try
            {
                ChargeValidVisa(10.29M);
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
        public void Visa_ResponseCode_ShouldIndicateLostCard()
        {
            try
            {
                ChargeValidVisa(10.31M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.CardDeclined, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate hot card pick-up (ResponseText: 'HOLD-CALL', ResponseCode: '43').</summary>
        [TestMethod]
        public void Visa_ResponseCode_ShouldIndicateHotCardPickUp()
        {
            try
            {
                ChargeValidVisa(10.03M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.CardDeclined, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate insufficient funds (ResponseText: 'DECLINE', ResponseCode: '05').</summary>
        [TestMethod]
        public void Visa_ResponseCode_ShouldIndicateInsufficientFunds()
        {
            try
            {
                ChargeValidVisa(10.08M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.CardDeclined, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate no checking account (ResponseText: 'NO CHECK ACCOUNT', ResponseCode: '52').</summary>
        [TestMethod]
        public void Visa_ResponseCode_ShouldIndicateNoCheckAccount()
        {
            try
            {
                ChargeValidVisa(10.16M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.ProcessingError, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate no saving account (ResponseText: 'NO SAVE ACCOUNT', ResponseCode: '53').</summary>
        [TestMethod]
        public void Visa_ResponseCode_ShouldIndicateNoSavingAccount()
        {
            try
            {
                ChargeValidVisa(10.17M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.ProcessingError, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate expired card (ResponseText: 'EXPIRED CARD', ResponseCode: '54').</summary>
        [TestMethod]
        public void Visa_ResponseCode_ShouldIndicateExpiredCard()
        {
            try
            {
                ChargeValidVisa(10.32M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.ExpiredCard, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate transaction not permitted on card (ResponseText: 'SERV NOT ALLOWED', ResponseCode: '57').</summary>
        //[TestMethod]
        //public void Visa_ResponseCode_ShouldIndicateTxnNotPermittedOnCard()
        //{
        //    try
        //    {
        //        ChargeValidVisa(10.20M);
        //    }
        //    catch (HpsCreditException e)
        //    {
        //        Assert.AreEqual(HpsExceptionCodes.ProcessingError, e.Code);
        //        return;
        //    }

        //    Assert.Fail("No exception was thrown.");
        //}

        /// <summary>Transaction response code should indicate invalid acquirer (ResponseText: 'SERV NOT ALLOWED', ResponseCode: '58').</summary>
        [TestMethod]
        public void Visa_ResponseCode_ShouldIndicateInvalidAcquirer()
        {
            try
            {
                ChargeValidVisa(10.30M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.ProcessingError, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate exceeds limit (ResponseText: 'DECLINE', ResponseCode: '61').</summary>
        [TestMethod]
        public void Visa_ResponseCode_ShouldIndicateExceedsLimit()
        {
            try
            {
                ChargeValidVisa(10.09M);
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
        public void Visa_ResponseCode_ShouldIndicateRestrictedCard()
        {
            try
            {
                ChargeValidVisa(10.10M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.CardDeclined, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate exceeds freq limit (ResponseText: 'DECLINE', ResponseCode: '65').</summary>
        [TestMethod]
        public void Visa_ResponseCode_ShouldIndicateSecurityViolation()
        {
            try
            {
                ChargeValidVisa(10.11M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.CardDeclined, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate invalid CVV2 (ResponseText: 'CHECK DIGIT ERR', ResponseCode: 'EB').</summary>
        [TestMethod]
        public void Visa_ResponseCode_ShouldIndicateCheckDigitErr()
        {
            try
            {
                ChargeValidVisa(10.05M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.IncorrectCvc, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate switch not available (ResponseText: 'NO REPLY', ResponseCode: '14').</summary>
        [TestMethod]
        public void Visa_ResponseCode_ShouldIndicateSwitchNotAvailable()
        {
            try
            {
                ChargeValidVisa(10.33M);
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
        public void Visa_ResponseCode_ShouldIndicateSystemError()
        {
            try
            {
                ChargeValidVisa(10.21M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.ProcessingError, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>Transaction response code should indicate CVV2 mismatch (ResponseText: 'CVV2 MISMATCH', ResponseCode: 'N7').</summary>
        [TestMethod]
        public void Visa_ResponseCode_ShouldIndicateCvv2Mismatch()
        {
            try
            {
                ChargeValidVisa(10.23M);
            }
            catch (HpsCreditException e)
            {
                Assert.AreEqual(HpsExceptionCodes.IncorrectCvc, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        #endregion

        #region Verify, Authorize & Capture

        /// <summary>Visa verify should return response code '85'.</summary>
        [TestMethod]
        public void Visa_Verify_ShouldReturnOk()
        {
            var creditSvc = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var response = creditSvc.Verify(TestCreditCard.ValidVisa, TestCardHolder.ValidCardHolder);
            Assert.AreEqual("85", response.ResponseCode);
        }

        /// <summary>Visa authorize should return response code '00'.</summary>
        [TestMethod]
        public void Visa_Authorize_ShouldReturnOk()
        {
            var creditSvc = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var response = creditSvc.Authorize(50, "usd", TestCreditCard.ValidVisa, TestCardHolder.ValidCardHolder);
            Assert.AreEqual("00", response.ResponseCode);
        }

        /// <summary>Visa authorize should return response code '00'.</summary>
        [TestMethod]
        public void Visa_AuthorizeAndRequestToken_ShouldGetTokenAndReturnOk()
        {
            var creditSvc = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var response = creditSvc.Authorize(50, "usd", TestCreditCard.ValidVisa, TestCardHolder.ValidCardHolder, true);
            Assert.AreEqual(0, response.TokenData.TokenRspCode);
            Assert.AreEqual("00", response.ResponseCode);
        }

        /// <summary>Visa authorize should return response code '00'.</summary>
        [TestMethod]
        public void Visa_Capture_ShouldReturnOk()
        {
            // Authorize the card.
            var creditSvc = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var authResponse = creditSvc.Authorize(50, "usd", TestCreditCard.ValidVisa, TestCardHolder.ValidCardHolder);
            Assert.AreEqual("00", authResponse.ResponseCode);

            // Capture the authorization.
            var captureResponse = creditSvc.Capture(authResponse.TransactionId);
            Assert.AreEqual("00", captureResponse.ResponseCode);
        }

        /// <summary>Visa void test.</summary>
        [TestMethod]
        public void Visa_ShouldVoid_Ok()
        {
            var creditSvc = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var creditResponse = creditSvc.Charge(25.00m, "usd", TestCreditCard.ValidVisa, TestCardHolder.CertCardHolderShortZipNoStreet);
            var voidResponse = creditSvc.Void(creditResponse.TransactionId);
            StringAssert.Matches(voidResponse.ResponseCode, new Regex("^00$"));
        }

        #endregion

        #region Card Present

        [TestMethod]
        public void Visa_WhenValidTrackData_ShouldReturnValidResult()
        {
            var service = new HpsCreditService(TestServicesConfig.ValidServicesConfig());
            var charge = service.Charge(50, "usd", new HpsTrackData
            {
                Value = "%B4012002000060016^VI TEST CREDIT^251210118039000000000396?;4012002000060016=25121011803939600000?",
                Mehod = HpsTrackDataMethod.Swipe
            });

            Assert.IsNotNull(charge);
            StringAssert.Matches(charge.ResponseCode, new Regex("00"));
        }

        #endregion

        #region Debit

        [TestMethod]
        public void Visa_Debit_WhenValidTrackData_ShouldChargeOk()
        {
            var service = new HpsDebitService(TestServicesConfig.ValidServicesConfig());
            var charge = service.Charge(50, "usd", "%B4012002000060016^VI TEST CREDIT^251210118039000000000396?;4012002000060016=25121011803939600000?", "32539F50C245A6A93D123412324000AA", null, true);
            Assert.IsNotNull(charge);
            StringAssert.Matches(charge.ResponseCode, new Regex("00"));
        }

        [TestMethod]
        public void Visa_Debit_WhenValidE3Data_ShouldChargeOk()
        {
            const string e3 = "&lt;E1050711%B4012001000000016^VI TEST CREDIT^251200000000000000000000?|LO04K0WFOmdkDz0um+GwUkILL8ZZOP6Z" +
                              "c4rCpZ9+kg2T3JBT4AEOilWTI|+++++++Dbbn04ekG|11;4012001000000016=25120000000000000000?|1u2F/aEhbdoPixyAPGy" +
                              "IDv3gBfF|+++++++Dbbn04ekG|00|||/wECAQECAoFGAgEH2wYcShV78RZwb3NAc2VjdXJlZXhjaGFuZ2UubmV0PX50qfj4dt0lu9oFB" +
                              "ESQQNkpoxEVpCW3ZKmoIV3T93zphPS3XKP4+DiVlM8VIOOmAuRrpzxNi0TN/DWXWSjUC8m/PI2dACGdl/hVJ/imfqIs68wYDnp8j0Zfg" +
                              "vM26MlnDbTVRrSx68Nzj2QAgpBCHcaBb/FZm9T7pfMr2Mlh2YcAt6gGG1i2bJgiEJn8IiSDX5M2ybzqRT86PCbKle/XCTwFFe1X|&gt;";
            var service = new HpsDebitService(TestServicesConfig.ValidServicesConfig());
            var charge = service.Charge(50, "usd", e3, "32539F50C245A6A93D123412324000AA", new HpsEncryptionData
            {
                Version = "01"
            }, true);
            Assert.IsNotNull(charge);
            StringAssert.Matches(charge.ResponseCode, new Regex("00"));
        }

        [TestMethod]
        public void Visa_Debit_WhenValidTrackData_ShouldReturnOk()
        {
            var service = new HpsDebitService(TestServicesConfig.ValidServicesConfig());
            var charge = service.Charge(50, "usd", "%B4012002000060016^VI TEST CREDIT^251210118039000000000396?;4012002000060016=25121011803939600000?", "32539F50C245A6A93D123412324000AA", null, true);
            Assert.IsNotNull(charge);
            StringAssert.Matches(charge.ResponseCode, new Regex("00"));

            var debitReturn = service.Return(charge.TransactionId, 50, "%B4012002000060016^VI TEST CREDIT^251210118039000000000396?;4012002000060016=25121011803939600000?", "32539F50C245A6A93D123412324000AA", true);
            Assert.IsNotNull(debitReturn);
            StringAssert.Matches(debitReturn.ResponseCode, new Regex("00"));
        }

        [TestMethod]
        public void Visa_Debit_WhenValidTrackData_ShouldReverseOk()
        {
            var service = new HpsDebitService(TestServicesConfig.ValidServicesConfig());
            var charge = service.Charge(50, "usd", "%B4012002000060016^VI TEST CREDIT^251210118039000000000396?;4012002000060016=25121011803939600000?", "32539F50C245A6A93D123412324000AA", null, true);
            Assert.IsNotNull(charge);
            StringAssert.Matches(charge.ResponseCode, new Regex("00"));

            var reverse = service.Reverse(charge.TransactionId, 50, "%B4012002000060016^VI TEST CREDIT^251210118039000000000396?;4012002000060016=25121011803939600000?");
            Assert.IsNotNull(reverse);
            StringAssert.Matches(reverse.ResponseCode, new Regex("00"));
        }

        #endregion

        /// <summary>Charge a Visa with a valid config and valid Visa info.</summary>
        /// <param name="amt">Amount to charge</param>
        /// <returns>The HPS Charge.</returns>
        private static HpsCharge ChargeValidVisa(decimal amt)
        {
            var creditSvc = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            var response = creditSvc.Charge(amt, "usd", TestCreditCard.ValidVisa, TestCardHolder.ValidCardHolder);
            if (response == null)
            {
                Assert.Fail("Response is null.");
            }

            return response;
        }
    }
}
// ReSharper restore InconsistentNaming