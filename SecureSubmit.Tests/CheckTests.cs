// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CheckTests.cs" company="Heartland Payment Systems">
//   Copyright (c) Heartland Payment Systems. All rights reserved.
// </copyright>
// <summary>
//   Check unit tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
// ReSharper disable InconsistentNaming

using Hps.Exchange.PosGateway.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecureSubmit.Infrastructure;
using SecureSubmit.Services.Check;
using SecureSubmit.Tests.TestData;
using System.Text.RegularExpressions;

namespace SecureSubmit.Tests
{
    [TestClass]
    public class CheckTests
    {
        /// <summary>The check sale method.</summary>
        [TestMethod]
        public void Check_ShouldSale()
        {
            var checkSvc = new HpsCheckService(TestServicesConfig.ValidSecretKeyConfig());
            var response = checkSvc.Sale(checkActionType.SALE, TestCheck.Approve, 5.00m );
            if (response == null)
            {
                Assert.Fail("Response is null.");
            }

            StringAssert.Matches(response.ResponseCode, new Regex("^0$"));
        }

        /// <summary>The check sale decline method.</summary>
        [TestMethod]
        public void Check_ShouldDecline()
        {
            try
            {
                var checkSvc = new HpsCheckService(TestServicesConfig.ValidSecretKeyConfig());
                checkSvc.Sale(checkActionType.SALE, TestCheck.Decline, 5.00m);
                Assert.Fail("The transaction should have thrown an HpsCheckException.");
            }
            catch (HpsCheckException ex)
            {
                Assert.AreEqual(ex.Code, 1);
            }
        }

        /// <summary>The check sale exception method.</summary>
        [TestMethod]
        public void Check_ShouldThrowHpsCheckException()
        {
            try
            {
                var checkSvc = new HpsCheckService(TestServicesConfig.ValidSecretKeyConfig());
                checkSvc.Sale(checkActionType.SALE, TestCheck.InvalidCheckHolder, 5.00m);
                Assert.Fail("The transaction should have thrown an HpsCheckException.");
            }
            catch (HpsCheckException ex)
            {
                Assert.AreEqual(ex.Code, 1);
            }
        }

        /// <summary>The check void method.</summary>
        [TestMethod]
        public void Check_ShouldVoid()
        {
            var checkSvc = new HpsCheckService(TestServicesConfig.ValidSecretKeyConfig());
            var saleResponse = checkSvc.Sale(checkActionType.SALE, TestCheck.Approve, 5.00m);
            var voidResponse = checkSvc.Void(saleResponse.TransactionId);
            if (voidResponse == null)
            {
                Assert.Fail("Response is null.");
            }

            StringAssert.Matches(voidResponse.ResponseCode, new Regex("^0$"));
        }
    }
}
