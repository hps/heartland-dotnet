// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GeneralTests.cs" company="Heartland Payment Systems">
//   Copyright (c) Heartland Payment Systems. All rights reserved.
// </copyright>
// <summary>
//   Charge unit tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Text.RegularExpressions;
using SecureSubmit.Services.Credit;
// ReSharper disable InconsistentNaming

namespace SecureSubmit.Tests
{
    using Entities;
    using Infrastructure;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;
    using TestData;

    /// <summary>General charge unit tests (e.g. amounts, config, currency, etc.).</summary>
    [TestClass]
    public class GeneralTests
    {
        /// <summary>The less than zero amount test method.</summary>
        [TestMethod]
        public void Charge_WhenAmountIsLessThanZero_ShouldThrowArgumentOutOfRange()
        {
            const decimal ChargeAmount = -5;
            var chargeSvc = new HpsCreditService();

            try
            {
                chargeSvc.Charge(ChargeAmount, "usd", TestCreditCard.ValidVisa, TestCardHolder.ValidCardHolder);
            }
            catch (HpsInvalidRequestException e)
            {
                Assert.AreEqual(e.Code, HpsExceptionCodes.InvalidAmount);
                Assert.AreEqual(e.ParamName, "amount");
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>The empty currency test method.</summary>
        [TestMethod]
        public void Charge_WhenCurrencyIsEmpty_ShouldThrowArgumentNull()
        {
            const decimal ChargeAmount = 50;
            const string Currency = "";
            var chargeSvc = new HpsCreditService();

            try
            {
                chargeSvc.Charge(ChargeAmount, Currency, TestCreditCard.ValidVisa, TestCardHolder.ValidCardHolder);
            }
            catch (HpsInvalidRequestException e)
            {
                Assert.AreEqual(e.Code, HpsExceptionCodes.MissingCurrency);
                Assert.AreEqual(e.ParamName, "currency");
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>The invalid currency test method.</summary>
        [TestMethod]
        public void Charge_WhenCurrencyIsNotUsd_ShouldThrowArgumentException()
        {
            const decimal ChargeAmount = 50;
            const string Currency = "eur";
            var chargeSvc = new HpsCreditService();

            try
            {
                chargeSvc.Charge(ChargeAmount, Currency, TestCreditCard.ValidVisa, TestCardHolder.ValidCardHolder);
            }
            catch (HpsInvalidRequestException e)
            {
                Assert.AreEqual(e.Code, HpsExceptionCodes.InvalidCurrency);
                Assert.AreEqual(e.ParamName, "currency");
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>The invalid HPS config test method.</summary>
        [TestMethod]
        public void Charge_WhenHpsConfigIsInvalid_ShouldThrowHpsException()
        {
            const decimal ChargeAmount = 50;
            const string Currency = "usd";
            var chargeSvc = new HpsCreditService();

            try
            {
                chargeSvc.Charge(ChargeAmount, Currency, TestCreditCard.ValidVisa, TestCardHolder.ValidCardHolder);
            }
            catch (HpsAuthenticationException e)
            {
                StringAssert.Contains(e.Message, Resource.Exception_Message_InvalidConfig);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>The invalid HPS config test method.</summary>
        //[TestMethod]
        //public void Charge_WhenHpsLicenseIdIsInvalid_ShouldThrowHpsException()
        //{
        //    const decimal ChargeAmount = 50;
        //    const string Currency = "usd";
        //    var chargeSvc = new HpsCreditService(TestServicesConfig.BadLicenseId());

        //    try
        //    {
        //        chargeSvc.Charge(ChargeAmount, Currency, TestCreditCard.ValidVisa, TestCardHolder.ValidCardHolder);
        //    }
        //    catch (HpsAuthenticationException)
        //    {
        //        return;
        //    }

        //    Assert.Fail("No exception was thrown.");
        //}

        /// <summary>The invalid HPS config test method.</summary>
        [TestMethod]
        public void Charge_WhenCardNumberIsInvalid_ShouldThrowHpsException()
        {
            const decimal ChargeAmount = 50;
            const string Currency = "usd";
            var chargeSvc = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());

            try
            {
                chargeSvc.Charge(ChargeAmount, Currency, TestCreditCard.InvalidCard, TestCardHolder.ValidCardHolder);
            }
            catch (HpsGatewayException e)
            {
                Assert.AreEqual(HpsExceptionCodes.InvalidNumber, e.Code);
                return;
            }

            Assert.Fail("No exception was thrown.");
        }

        /// <summary>The list transactions test method.</summary>
        [TestMethod]
        public void List_WhenConfigValid_ShouldListTransactions()
        {
            var chargeSvc = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());

            List<HpsReportTransactionSummary> items = chargeSvc.List(DateTime.Today.AddDays(-10), DateTime.Today);
            Assert.IsNotNull(items);
        }

        /// <summary>The list charges test method.</summary>
        [TestMethod]
        public void List_WhenConfigValid_ShouldListCharges()
        {
            var chargeSvc = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());

            List<HpsReportTransactionSummary> items = chargeSvc.List(DateTime.UtcNow.AddDays(-10), DateTime.UtcNow, HpsTransactionType.Capture);
            Assert.IsNotNull(items);
        }

        /// <summary>The list charges test method (without txn type).</summary>
        [TestMethod]
        public void List_WhenConfigValidNoTxn_ShouldListCharges()
        {
            var chargeSvc = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());

            List<HpsReportTransactionSummary> items = chargeSvc.List(DateTime.UtcNow.AddDays(-10), DateTime.UtcNow);
            Assert.IsNotNull(items);
        }

        /// <summary>The get first charge test method.</summary>
        [TestMethod]
        public void GetFirst_WhenConfigValid_ShouldGetTheFirstCharge()
        {
            var chargeSvc = new HpsCreditService(TestServicesConfig.ValidSecretKeyConfig());
            List<HpsReportTransactionSummary> items = chargeSvc.List(DateTime.Today.AddDays(-10), DateTime.Today);

            if (items.Count > 0)
            {
                HpsReportTransactionDetails charge = chargeSvc.Get(items[0].TransactionId);
                Assert.IsNotNull(charge);
            }
        }

        /// <summary>Partial reversal (partial void) test.</summary>
        [TestMethod]
        public void Cc_ShouldPartialReverse_Ok()
        {
            var chargeSvc = new HpsCreditService(TestServicesConfig.ValidServicesConfig());
            var charge = chargeSvc.Charge(20, "usd", TestCreditCard.ValidVisa,
                TestCardHolder.CertCardHolderShortZip);

            var response = chargeSvc.Reverse(charge.TransactionId, 20, "usd", null, 10);
            Assert.IsNotNull(response);
            StringAssert.Matches(response.ResponseCode, new Regex("^00$"));
        }

        /// <summary>Offline authorize test.</summary>
        [TestMethod]
        public void Cc_ShouldOfflineCharge_Ok()
        {
            var chargeSvc = new HpsCreditService(TestServicesConfig.ValidServicesConfig());
            var charge = chargeSvc.OfflineCharge(20, "usd", TestCreditCard.ValidVisa, "654321", true);
            Assert.IsNotNull(charge);
            StringAssert.Matches(charge.ResponseCode, new Regex("^00$"));
        }

        /// <summary>Offline authorize test.</summary>
        [TestMethod]
        public void Cc_ShouldOPartialApprove_Ok()
        {
            var chargeSvc = new HpsCreditService(TestServicesConfig.ValidServicesConfig());
            var charge = chargeSvc.Authorize(20, "usd", TestCreditCard.ValidVisa, TestCardHolder.ValidCardHolder, false, null, true);
            Assert.IsNotNull(charge);
            StringAssert.Matches(charge.ResponseCode, new Regex("^00$"));
        }

        [TestMethod]
        public void Cc_ShouldTipEdit_Ok()
        {
            var creditSvc = new HpsCreditService(TestServicesConfig.ValidServicesConfig());
            var authResponse = creditSvc.Authorize(50, "usd", TestCreditCard.ValidVisa, TestCardHolder.ValidCardHolder);
            Assert.AreEqual("00", authResponse.ResponseCode);

            var captureResponse = creditSvc.Capture(authResponse.TransactionId, null, 5);
            Assert.AreEqual("00", captureResponse.ResponseCode);
        }
    }
}
// ReSharper restore InconsistentNaming