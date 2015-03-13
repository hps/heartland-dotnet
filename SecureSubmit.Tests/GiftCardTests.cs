// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GiftCardTests.cs" company="Heartland Payment Systems">
//   Copyright (c) Heartland Payment Systems. All rights reserved.
// </copyright>
// <summary>
//   Gift card unit tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable InconsistentNaming
namespace SecureSubmit.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Infrastructure;
    using Services.GiftCard;
    using TestData;
    using System.Text.RegularExpressions;
    using SecureSubmit.Services;
    using SecureSubmit.Entities;

    /// <summary>Gift card unit tests.</summary>
    [TestClass]
    public class GiftCardTests
    {        
        /// <summary>The gift card activates method.</summary>
        [TestMethod]
        public void GiftCard_ManualCard_ShouldActivate()
        {
            var giftCardSvc = new HpsGiftCardService(TestServicesConfig.ValidSecretKeyConfig());
            var response = giftCardSvc.Activate(100.00M, "usd", TestGiftCard.Manual.validGiftCardNotEncrypted);
            if (response == null)
            {
                Assert.Fail("Response is null.");
            }

            StringAssert.Matches(response.ResponseCode, new Regex("^0$"));
        }

        /// <summary>The gift card activates method.</summary>
        [TestMethod]
        public void GiftCard_ManualCard_ShouldAddValue()
        {
            var giftCardSvc = new HpsGiftCardService(TestServicesConfig.ValidSecretKeyConfig());
            var response = giftCardSvc.AddValue(10.00M, "usd", TestGiftCard.Manual.validGiftCardNotEncrypted);
            if (response == null)
            {
                Assert.Fail("Response is null.");
            }

            StringAssert.Matches(response.ResponseCode, new Regex("^0$"));
        }

        /// <summary>The gift card alias method.</summary>
        [TestMethod]
        public void GiftCard_ManualCard_ShouldAlias()
        {
            var giftCardSvc = new HpsGiftCardService(TestServicesConfig.ValidSecretKeyConfig());
            try
            {
                var response = giftCardSvc.Alias(HpsGiftCardAliasAction.Add,
                    TestGiftCard.Manual.validGiftCardNotEncrypted, "1234567890");
                if (response == null)
                {
                    Assert.Fail("Response is null.");
                }

                StringAssert.Matches(response.ResponseCode, new Regex("^0$"));
            }
            catch (HpsCreditException ex)
            {
                if (ex.Details.IssuerResponseCode != "6")
                {
                    throw;
                }
            }
        }

        /// <summary>The gift card balance method.</summary>
        [TestMethod]
        public void GiftCard_ManualCard_ShouldBalance()
        {
            var giftCardSvc = new HpsGiftCardService(TestServicesConfig.ValidSecretKeyConfig());
            var response = giftCardSvc.Balance(TestGiftCard.Manual.validGiftCardNotEncrypted);
            if (response == null)
            {
                Assert.Fail("Response is null.");
            }

            StringAssert.Matches(response.ResponseCode, new Regex("^0$"));
        }

        /// <summary>The gift card deactivate method.</summary>
        [TestMethod]
        public void GiftCard_ManualCard_ShouldDeactivate()
        {
            var giftCardSvc = new HpsGiftCardService(TestServicesConfig.ValidSecretKeyConfig());
            var response = giftCardSvc.Deactivate(TestGiftCard.Manual.validGiftCardNotEncrypted);
            if (response == null)
            {
                Assert.Fail("Response is null.");
            }

            StringAssert.Matches(response.ResponseCode, new Regex("^0$"));
        }

        /// <summary>The gift card replace method.</summary>
        [TestMethod]
        public void GiftCard_ManualCard_ShouldReplace()
        {
            var giftCardSvc = new HpsGiftCardService(TestServicesConfig.ValidSecretKeyConfig());
            var response = giftCardSvc.Replace(TestGiftCard.Manual.validGiftCardNotEncrypted, TestGiftCard.Manual.validGiftCardNotEncrypted2);
            if (response == null)
            {
                Assert.Fail("Response is null.");
            }

            StringAssert.Matches(response.ResponseCode, new Regex("^0$"));
        }

        /// <summary>The gift card reward method.</summary>
        [TestMethod]
        public void GiftCard_ManualCard_ShouldReward()
        {
            var giftCardSvc = new HpsGiftCardService(TestServicesConfig.ValidSecretKeyConfig());
            var response = giftCardSvc.Reward(TestGiftCard.Manual.validGiftCardNotEncrypted, 10.00m);
            if (response == null)
            {
                Assert.Fail("Response is null.");
            }

            StringAssert.Matches(response.ResponseCode, new Regex("^0$"));
        }

        /// <summary>The gift card sale method.</summary>
        [TestMethod]
        public void GiftCard_ManualCard_ShouldSale()
        {
            var giftCardSvc = new HpsGiftCardService(TestServicesConfig.ValidSecretKeyConfig());
            var response = giftCardSvc.Sale(TestGiftCard.Manual.validGiftCardNotEncrypted, 10.00m);
            if (response == null)
            {
                Assert.Fail("Response is null.");
            }

            StringAssert.Matches(response.ResponseCode, new Regex("^0$"));
        }

        /// <summary>The gift card void method.</summary>
        [TestMethod]
        public void GiftCard_ManualCard_ShouldVoid()
        {
            var giftCardSvc = new HpsGiftCardService(TestServicesConfig.ValidSecretKeyConfig());
            var saleResponse = giftCardSvc.Sale(TestGiftCard.Manual.validGiftCardNotEncrypted, 10.00m);
            StringAssert.Matches(saleResponse.ResponseCode, new Regex("^0$"));
            var voidResponse = giftCardSvc.Void(saleResponse.TransactionId);
            StringAssert.Matches(voidResponse.ResponseCode, new Regex("^0$"));
        }

        /// <summary>The gift card reverse method.</summary>
        [TestMethod]
        public void GiftCard_ManualCard_ShouldReverseUsingTxnID()
        {
            var giftCardSvc = new HpsGiftCardService(TestServicesConfig.ValidSecretKeyConfig());
            var saleResponse = giftCardSvc.Sale(TestGiftCard.Manual.validGiftCardNotEncrypted, 10.00m);
            StringAssert.Matches(saleResponse.ResponseCode, new Regex("^0$"));
            var reverseResponse = giftCardSvc.Reverse(saleResponse.TransactionId, 10.00m);
            StringAssert.Matches(reverseResponse.ResponseCode, new Regex("^0$"));
        }

        /// <summary>The gift card reverse method.</summary>
        [TestMethod]
        public void GiftCard_ManualCard_ShouldReverseUsingGiftCard()
        {
            var giftCardSvc = new HpsGiftCardService(TestServicesConfig.ValidSecretKeyConfig());
            var saleResponse = giftCardSvc.Sale(TestGiftCard.Manual.validGiftCardNotEncrypted, 10.00m);
            StringAssert.Matches(saleResponse.ResponseCode, new Regex("^0$"));
            var reverseResponse = giftCardSvc.Reverse(TestGiftCard.Manual.validGiftCardNotEncrypted, 10.00m);
            StringAssert.Matches(reverseResponse.ResponseCode, new Regex("^0$"));
        }
    }
}
