using System;
using Hps.Exchange.PosGateway.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecureSubmit.Entities;
using SecureSubmit.Fluent;
using SecureSubmit.Fluent.Services;
using SecureSubmit.Infrastructure;
using SecureSubmit.Services;

namespace SecureSubmit.Tests.Certification {
    [TestClass]
    public class Retail {
        HpsBatchService batchService;
        HpsFluentCreditService creditService;
        HpsFluentDebitService debitService;
        HpsFluentEbtService ebtService;
        HpsFluentGiftCardService giftService;

        bool useTokens = false;

        private static String visaToken;
        private static String mastercardToken;
        private static String discoverToken;
        private static String amexToken;

        private static long test010TransactionId;
        private static long test014TransactionId;
        private static long test015TransactionId;
        private static long test017TransactionId;
        private static long test021TransactionId;
        private static long test023TransactionId;
        private static long test042TransactionId;
        private static long test066TransactionId;
        private static long test069TransactionId;
        private static long test105TransactionId;
        private static long test106TransactionId;

        public Retail() {
            HpsServicesConfig config = new HpsServicesConfig {
                SecretApiKey = "skapi_cert_MaePAQBr-1QAqjfckFC8FTbRTT120bVQUlfVOjgCBw"
            };

            batchService = new HpsBatchService(config);
            creditService = new HpsFluentCreditService(config, true);
            debitService = new HpsFluentDebitService(config, true);
            ebtService = new HpsFluentEbtService(config, true);
            giftService = new HpsFluentGiftCardService(config, true);
        }

        [TestMethod]
        public void retail_000_CloseBatch() {
            try {
                HpsBatch response = this.batchService.CloseBatch();
                if (response == null)
                    Assert.Fail("Response is null");
                Console.WriteLine(String.Format("Batch ID: {0}", response.Id));
                Console.WriteLine(String.Format("Sequence Number: {0}", response.SequenceNumber));
            }
            catch (HpsException exc) {
                if (exc.Message != "Transaction was rejected because it requires a batch to be open.")
                    Assert.Fail(exc.Message);
            }
        }

        /*
            CREDIT CARD FUNCTIONS
            CARD VERIFY
            ACCOUNT VERIFICATION
         */
        [TestMethod]
        public void retail_001_CardVerifyVisa() {
            HpsTrackData trackData = new HpsTrackData {
                Value = "%B4012002000060016^VI TEST CREDIT^251210118039000000000396?;4012002000060016=25121011803939600000?",
                Method = HpsTrackDataMethod.Swipe
            };

            HpsAccountVerify response = creditService.Verify()
                    .WithTrackData(trackData)
                    .WithRequestMultiUseToken(useTokens)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("85", response.ResponseCode);
        }

        [TestMethod]
        public void retail_002_CardVerifyMastercardSwipe() {
            HpsTrackData trackData = new HpsTrackData {
                Value = "%B5473500000000014^MC TEST CARD^251210199998888777766665555444433332?;5473500000000014=25121019999888877776?",
                Method = HpsTrackDataMethod.Swipe
            };

            HpsAccountVerify response = creditService.Verify()
                    .WithTrackData(trackData)
                    .WithRequestMultiUseToken(useTokens)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("85", response.ResponseCode);
        }

        [TestMethod]
        public void retail_003_CardVerifyDiscover() {
            HpsTrackData trackData = new HpsTrackData {
                Value = "%B6011000990156527^DIS TEST CARD^25121011000062111401?;6011000990156527=25121011000062111401?",
                Method = HpsTrackDataMethod.Swipe
            };

            HpsAccountVerify response = creditService.Verify()
                    .WithTrackData(trackData)
                    .WithRequestMultiUseToken(useTokens)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("85", response.ResponseCode);
        }

        // Address Verification

        [TestMethod]
        public void retail_004_CardVerifyAmex() {
            HpsCardHolder cardHolder = new HpsCardHolder {
                Address = new HpsAddress {
                    Zip = "75024"
                }
            };

            HpsCreditCard card = new HpsCreditCard {
                Number = "372700699251018",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "1234"
            };

            HpsAccountVerify response = creditService.Verify()
                    .WithCard(card)
                    .WithCardHolder(cardHolder)
                    .WithRequestMultiUseToken(useTokens)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // Balance Inquiry (for Prepaid)

        [TestMethod]
        public void retail_005_BalanceInquiryVisa() {
            HpsTrackData trackData = new HpsTrackData {
                Value = "%B4012002000060016^VI TEST CREDIT^251210118039000000000396?;4012002000060016=25121011803939600000?",
                Method = HpsTrackDataMethod.Swipe
            };

            HpsAuthorization response = creditService.PrePaidBalanceInquiry()
                    .WithTrackData(trackData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // CREDIT SALE (For multi-use token only)

        [TestMethod]
        public void retail_006_ChargeVisaSwipeToken() {
            HpsTrackData trackData = new HpsTrackData {
                Value = "%B4012002000060016^VI TEST CREDIT^251210118039000000000396?;4012002000060016=25121011803939600000?",
                Method = HpsTrackDataMethod.Swipe
            };

            HpsCharge response = creditService.Charge(15.01m)
                    .WithTrackData(trackData)
                    .WithRequestMultiUseToken(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            visaToken = response.TokenData.TokenValue;
        }

        [TestMethod]
        public void retail_007_ChargeMastercardSwipeToken() {
            HpsTrackData trackData = new HpsTrackData {
                Value = "%B5473500000000014^MC TEST CARD^251210199998888777766665555444433332?;5473500000000014=25121019999888877776?",
                Method = HpsTrackDataMethod.Swipe
            };

            HpsCharge response = creditService.Charge(15.02m)
                    .WithTrackData(trackData)
                    .WithRequestMultiUseToken(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            mastercardToken = response.TokenData.TokenValue;
        }

        [TestMethod]
        public void retail_008_ChargeDiscoverSwipeToken() {
            HpsTrackData trackData = new HpsTrackData {
                Value = "%B6011000990156527^DIS TEST CARD^25121011000062111401?;6011000990156527=25121011000062111401?",
                Method = HpsTrackDataMethod.Swipe
            };

            HpsCharge response = creditService.Charge(15.03m)
                    .WithTrackData(trackData)
                    .WithRequestMultiUseToken(true)
                    .WithAllowDuplicates(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            discoverToken = response.TokenData.TokenValue;
        }

        [TestMethod]
        public void retail_009_ChargeAmexSwipeToken() {
            HpsTrackData trackData = new HpsTrackData {
                Value = "%B3727 006992 51018^AMEX TEST CARD^2512990502700?;372700699251018=2512990502700?",
                Method = HpsTrackDataMethod.Swipe
            };

            HpsCharge response = creditService.Charge(15.04m)
                    .WithTrackData(trackData)
                    .WithRequestMultiUseToken(true)
                    .WithAllowDuplicates(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            amexToken = response.TokenData.TokenValue;
        }

        /*
            CREDIT SALE
            SWIPED
         */

        [TestMethod]
        public void retail_010_ChargeVisaSwipe() {
            HpsTrackData trackData = new HpsTrackData {
                Value = "%B4012002000060016^VI TEST CREDIT^251210118039000000000396?;4012002000060016=25121011803939600000?",
                Method = HpsTrackDataMethod.Swipe
            };

            HpsCharge response = creditService.Charge(15.01m)
                    .WithTrackData(trackData)
                    .WithAllowDuplicates(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            test010TransactionId = response.TransactionId;
        }

        [TestMethod]
        public void retail_011_ChargeMastercardSwipe() {
            HpsTrackData trackData = new HpsTrackData {
                Value = "%B5473500000000014^MC TEST CARD^251210199998888777766665555444433332?;5473500000000014=25121019999888877776?",
                Method = HpsTrackDataMethod.Swipe
            };

            HpsCharge response = creditService.Charge(15.02m)
                    .WithTrackData(trackData)
                    .WithAllowDuplicates(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void retail_012_ChargeDiscoverSwipe() {
            HpsTrackData trackData = new HpsTrackData {
                Value = "%B6011000990156527^DIS TEST CARD^25121011000062111401?;6011000990156527=25121011000062111401?",
                Method = HpsTrackDataMethod.Swipe
            };

            HpsCharge response = creditService.Charge(15.03m)
                    .WithTrackData(trackData)
                    .WithAllowDuplicates(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void retail_013_ChargeAmexSwipe() {
            HpsTrackData trackData = new HpsTrackData {
                Value = "%B3727 006992 51018^AMEX TEST CARD^2512990502700?;372700699251018=2512990502700?",
                Method = HpsTrackDataMethod.Swipe
            };

            HpsCharge response = creditService.Charge(15.04m)
                    .WithTrackData(trackData)
                    .WithAllowDuplicates(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void retail_014_ChargeJbcSwipe() {
            HpsTrackData trackData = new HpsTrackData {
                Value = "%B3566007770007321^JCB TEST CARD^2512101100000000000000000064300000?;3566007770007321=25121011000000076435?",
                Method = HpsTrackDataMethod.Swipe
            };

            HpsCharge response = creditService.Charge(15.05m)
                    .WithTrackData(trackData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            test014TransactionId = response.TransactionId;
        }

        [TestMethod]
        public void retail_015_ChargeVisaSwipe() {
            HpsTrackData trackData = new HpsTrackData {
                Value = "%B4012002000060016^VI TEST CREDIT^251210118039000000000396?;4012002000060016=25121011803939600000?",
                Method = HpsTrackDataMethod.Swipe
            };

            HpsCharge response = creditService.Charge(15.06m)
                    .WithTrackData(trackData)
                    .WithAllowDuplicates(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            test015TransactionId = response.TransactionId;
        }

        // Manually Entered - Card Present

        [TestMethod]
        public void retail_016_ChargeVisaManualCardPresent() {
            HpsCardHolder cardHolder = new HpsCardHolder {
                Address = new HpsAddress {
                    Zip = "750241234",
                    Address = "6860 Dallas Pkwy"
                }
            };

            HpsCreditCard card = new HpsCreditCard {
                Number = "4012002000060016",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "123"
            };

            HpsCharge response = creditService.Charge(16.01m)
                    .WithCard(card)
                    .WithCardHolder(cardHolder)
                    .WithCardPresent(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void retail_017_ChargeMasterCardManualCardPresent() {
            var cardHolder = new HpsCardHolder {
                Address = new HpsAddress {
                    Zip = "75024",
                    Address = "6860 Dallas Pkwy"
                }
            };

            HpsCreditCard card = new HpsCreditCard {
                Number = "5473500000000014",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "123"
            };

            HpsCharge response = creditService.Charge(16.02m)
                    .WithCard(card)
                    .WithCardHolder(cardHolder)
                    .WithCardPresent(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            test017TransactionId = response.TransactionId;
        }

        [TestMethod]
        public void retail_018_ChargeDiscoverManualCardPresent() {
            var cardHolder = new HpsCardHolder {
                Address = new HpsAddress {
                    Zip = "750241234"
                }
            };

            HpsCreditCard card = new HpsCreditCard {
                Number = "6011000990156527",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "123"
            };

            HpsCharge response = creditService.Charge(16.03m)
                    .WithCard(card)
                    .WithCardHolder(cardHolder)
                    .WithCardPresent(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void retail_019_ChargeAmexManualCardPresent() {
            var cardHolder = new HpsCardHolder {
                Address = new HpsAddress {
                    Zip = "75024",
                    Address = "6860"
                }
            };

            HpsCreditCard card = new HpsCreditCard {
                Number = "372700699251018",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "1234"
            };

            HpsCharge response = creditService.Charge(16.04m)
                    .WithCard(card)
                    .WithCardHolder(cardHolder)
                    .WithCardPresent(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void retail_020_ChargeJcbManualCardPresent() {
            var cardHolder = new HpsCardHolder {
                Address = new HpsAddress {
                    Zip = "75024"
                }
            };

            HpsCreditCard card = new HpsCreditCard {
                Number = "3566007770007321",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "123"
            };

            HpsCharge response = creditService.Charge(16.05m)
                    .WithCard(card)
                    .WithCardHolder(cardHolder)
                    .WithCardPresent(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void retail_021_ChargeDiscoverManualCardPresent() {
            HpsCardHolder cardHolder = new HpsCardHolder {
                Address = new HpsAddress {
                    Zip = "750241234",
                    Address = "6860 Dallas Pkwy"
                },
            };

            HpsCreditCard card = new HpsCreditCard {
                Number = "6011000990156527",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "123"
            };

            HpsCharge response = creditService.Charge(16.07m)
                    .WithCard(card)
                    .WithCardHolder(cardHolder)
                    .WithCardPresent(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            test021TransactionId = response.TransactionId;
        }

        // Manually Entered - Card Not Present

        [TestMethod]
        public void retail_022_ChargeVisaManualCardNotPresent() {
            var cardHolder = new HpsCardHolder {
                Address = new HpsAddress {
                    Zip = "750241234",
                    Address = "6860 Dallas Pkwy"
                }
            };

            HpsCreditCard card = new HpsCreditCard {
                Number = "4012002000060016",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "123"
            };

            CreditChargeBuilder builder;

            if (useTokens)
                builder = creditService.Charge(17.01m).WithToken(visaToken);
            else builder = creditService.Charge(17.01m).WithCard(card);

            HpsCharge response = builder.WithCardHolder(cardHolder).Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void retail_023_ChargeMasterCardManualCardNotPresent() {
            var cardHolder = new HpsCardHolder {
                Address = new HpsAddress {
                    Zip = "75024",
                    Address = "6860 Dallas Pkwy"
                }
            };

            HpsCreditCard card = new HpsCreditCard {
                Number = "5473500000000014",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "123"
            };

            CreditChargeBuilder builder;

            if (useTokens)
                builder = creditService.Charge(17.02m).WithToken(mastercardToken);
            else builder = creditService.Charge(17.02m).WithCard(card);

            HpsCharge response = builder.WithCardHolder(cardHolder).Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            test023TransactionId = response.TransactionId;
        }

        [TestMethod]
        public void retail_024_ChargeDiscoverManualCardNotPresent() {
            var cardHolder = new HpsCardHolder {
                Address = new HpsAddress {
                    Zip = "750241234"
                }
            };

            HpsCreditCard card = new HpsCreditCard {
                Number = "6011000990156527",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "123"
            };

            CreditChargeBuilder builder;

            if (useTokens)
                builder = creditService.Charge(17.03m).WithToken(discoverToken);
            else builder = creditService.Charge(17.03m).WithCard(card);

            HpsCharge response = builder.WithCardHolder(cardHolder).Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void retail_025_ChargeAmexManualCardNotPresent() {
            var cardHolder = new HpsCardHolder {
                Address = new HpsAddress {
                    Zip = "75024",
                    Address = "6860"
                }
            };

            HpsCreditCard card = new HpsCreditCard {
                Number = "372700699251018",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "1234"
            };

            CreditChargeBuilder builder;

            if (useTokens)
                builder = creditService.Charge(17.04m).WithToken(amexToken);
            else builder = creditService.Charge(17.04m).WithCard(card);

            HpsCharge response = builder.WithCardHolder(cardHolder).Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void retail_026_ChargeJcbManualCardNotPresent() {
            var cardHolder = new HpsCardHolder {
                Address = new HpsAddress {
                    Zip = "75024"
                }
            };

            HpsCreditCard card = new HpsCreditCard {
                Number = "3566007770007321",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "123"
            };

            HpsCharge response = creditService.Charge(17.05m)
                    .WithCard(card)
                    .WithCardHolder(cardHolder)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // Contactless

        [TestMethod]
        public void retail_027_ChargeVisaContactless() {
            HpsTrackData trackData = new HpsTrackData {
                Value = "%B4012002000060016^VI TEST CREDIT^251210118039000000000396?;4012002000060016=25121011803939600000?",
                Method = HpsTrackDataMethod.Proximity
            };

            HpsCharge response = creditService.Charge(18.01m)
                    .WithTrackData(trackData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void retail_028_ChargeMastercardContactless() {
            HpsTrackData trackData = new HpsTrackData {
                Value = "%B5473500000000014^MC TEST CARD^251210199998888777766665555444433332?;5473500000000014=25121019999888877776?",
                Method = HpsTrackDataMethod.Proximity
            };

            HpsCharge response = creditService.Charge(18.02m)
                    .WithTrackData(trackData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void retail_029_ChargeDiscoverContactless() {
            HpsTrackData trackData = new HpsTrackData {
                Value = "%B6011000990156527^DIS TEST CARD^25121011000062111401?;6011000990156527=25121011000062111401?",
                Method = HpsTrackDataMethod.Swipe
            };

            HpsCharge response = creditService.Charge(18.03m)
                    .WithTrackData(trackData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void retail_030_ChargeAmexContactless() {
            HpsTrackData trackData = new HpsTrackData {
                Value = "%B3727 006992 51018^AMEX TEST CARD^2512990502700?;372700699251018=2512990502700?",
                Method = HpsTrackDataMethod.Proximity
            };

            HpsCharge response = creditService.Charge(18.04m)
                    .WithTrackData(trackData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // AUTHORIZATION

        [TestMethod]
        public void retail_031_AuthorizeVisaSwipe() {
            HpsTrackData trackData = new HpsTrackData {
                Value = "%B4012002000060016^VI TEST CREDIT^251210118039000000000396?;4012002000060016=25121011803939600000?",
                Method = HpsTrackDataMethod.Swipe
            };

            // 031a authorize
            HpsAuthorization response = creditService.Authorize(15.08m)
                    .WithTrackData(trackData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            // 031b capture
            HpsTransaction captureResponse = creditService.Capture(response.TransactionId).Execute();
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("00", captureResponse.ResponseCode);
        }

        [TestMethod]
        public void retail_032_AuthorizeVisaSwipeAdditionalAuth() {
            HpsTrackData trackData = new HpsTrackData {
                Value = "%B4012002000060016^VI TEST CREDIT^251210118039000000000396?;4012002000060016=25121011803939600000?",
                Method = HpsTrackDataMethod.Swipe
            };

            // 032a authorize
            HpsAuthorization response = creditService.Authorize(15.09m)
                    .WithTrackData(trackData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            // 032b Additional Auth (restaurant only)

            // 032c Add to batch
            HpsTransaction captureResponse = creditService.Capture(response.TransactionId).Execute();
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("00", captureResponse.ResponseCode);
        }

        [TestMethod]
        public void retail_033_AuthorizeMasterCardSwipe() {
            HpsTrackData trackData = new HpsTrackData {
                Value = "%B5473500000000014^MC TEST CARD^251210199998888777766665555444433332?;5473500000000014=25121019999888877776?",
                Method = HpsTrackDataMethod.Swipe
            };

            // 033a authorize
            HpsAuthorization response = creditService.Authorize(15.10m)
                    .WithTrackData(trackData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            // 033b capture
            HpsTransaction captureResponse = creditService.Capture(response.TransactionId).Execute();
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("00", captureResponse.ResponseCode);
        }

        // AUTHORIZATION - Manually Entered, Card Present

        [TestMethod]
        public void retail_034_AuthorizeVisaManualCardPresent() {
            var cardHolder = new HpsCardHolder {
                Address = new HpsAddress {
                    Zip = "75024",
                    Address = "6860 Dallas Pkwy"
                }
            };

            HpsCreditCard card = new HpsCreditCard {
                Number = "4012002000060016",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "123"
            };

            // 034a authorize
            HpsAuthorization response = creditService.Authorize(16.08m)
                    .WithCard(card)
                    .WithCardHolder(cardHolder)
                    .WithCardPresent(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            // 034b capture
            HpsTransaction captureResponse = creditService.Capture(response.TransactionId).Execute();
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("00", captureResponse.ResponseCode);
        }

        [TestMethod]
        public void retail_035_AuthorizeVisaManualCardPresentAdditionalAuth() {
            var cardHolder = new HpsCardHolder {
                Address = new HpsAddress {
                    Zip = "75024",
                    Address = "6860 Dallas Pkwy"
                }
            };

            HpsCreditCard card = new HpsCreditCard {
                Number = "4012002000060016",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "123"
            };

            // 035a authorize
            HpsAuthorization response = creditService.Authorize(16.09m)
                    .WithCard(card)
                    .WithCardHolder(cardHolder)
                    .WithCardPresent(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            // 035b Additional Auth (restaurant only)

            // 035c Add to batch
            HpsTransaction captureResponse = creditService.Capture(response.TransactionId).Execute();
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("00", captureResponse.ResponseCode);
        }

        [TestMethod]
        public void retail_036_AuthorizeMasterCardManualCardPresent() {
            var cardHolder = new HpsCardHolder {
                Address = new HpsAddress {
                    Zip = "75024",
                    Address = "6860 Dallas Pkwy"
                }
            };

            HpsCreditCard card = new HpsCreditCard {
                Number = "5473500000000014",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "123"
            };

            // 036a authorize
            HpsAuthorization response = creditService.Authorize(16.10m)
                    .WithCard(card)
                    .WithCardHolder(cardHolder)
                    .WithCardPresent(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            // 036b capture
            HpsTransaction captureResponse = creditService.Capture(response.TransactionId).Execute();
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("00", captureResponse.ResponseCode);
        }

        // AUTHORIZATION - Manually Entered, Card Not Present

        [TestMethod]
        public void retail_037_AuthorizeVisaManual() {
            HpsCardHolder cardHolder = new HpsCardHolder {
                Address = new HpsAddress {
                    Zip = "750241234",
                    Address = "6860 Dallas Pkwy"
                }
            };

            HpsCreditCard card = new HpsCreditCard {
                Number = "4012002000060016",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "123"
            };

            // 034a authorize
            HpsAuthorization response = creditService.Authorize(17.08m)
                    .WithCard(card)
                    .WithCardHolder(cardHolder)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            // 034b capture
            HpsTransaction captureResponse = creditService.Capture(response.TransactionId).Execute();
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("00", captureResponse.ResponseCode);
        }

        [TestMethod]
        public void retail_038_AuthorizeMasterCardManual() {
            var cardHolder = new HpsCardHolder {
                Address = new HpsAddress {
                    Zip = "75024",
                    Address = "6860"
                }
            };

            HpsCreditCard card = new HpsCreditCard {
                Number = "5473500000000014",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "123"
            };

            // 036a authorize
            HpsAuthorization response = creditService.Authorize(17.09m)
                    .WithCard(card)
                    .WithCardHolder(cardHolder)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            // 036b capture
            HpsTransaction captureResponse = creditService.Capture(response.TransactionId).Execute();
            Assert.IsNotNull(captureResponse);
            Assert.AreEqual("00", captureResponse.ResponseCode);
        }

        // PARTIALLY APPROVED SALE (Required)

        [TestMethod]
        public void retail_039_ChargeDiscoverSwipePartialApproval() {
            HpsTrackData trackData = new HpsTrackData {
                Value = "%B6011000990156527^DIS TEST CARD^25121011000062111401?;6011000990156527=25121011000062111401?",
                Method = HpsTrackDataMethod.Swipe
            };

            HpsCharge response = creditService.Charge(40.00m)
                    .WithTrackData(trackData)
                    .WithAllowPartialAuth(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("10", response.ResponseCode);
            Assert.AreEqual(40.00m, response.AuthorizedAmount);
        }

        [TestMethod]
        public void retail_040_ChargeVisaSwipePartialApproval() {
            HpsTrackData trackData = new HpsTrackData {
                Value = "%B4012002000060016^VI TEST CREDIT^251210118039000000000396?;4012002000060016=25121011803939600000?",
                Method = HpsTrackDataMethod.Swipe
            };

            HpsCharge response = creditService.Charge(130.00m)
                    .WithTrackData(trackData)
                    .WithAllowPartialAuth(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("10", response.ResponseCode);
            Assert.AreEqual(110.00m, response.AuthorizedAmount);
        }

        [TestMethod]
        public void retail_041_ChargeMasterCardManualPartialApproval() {
            HpsCardHolder cardHolder = new HpsCardHolder {
                Address = new HpsAddress {
                    Zip = "75024"
                },
            };

            HpsCreditCard card = new HpsCreditCard {
                Number = "5473500000000014",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "123"
            };

            HpsCharge response = creditService.Charge(145.00m)
                    .WithCard(card)
                    .WithCardHolder(cardHolder)
                    .WithAllowPartialAuth(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("10", response.ResponseCode);
            Assert.AreEqual(65.00m, response.AuthorizedAmount);
        }

        [TestMethod]
        public void retail_042_ChargeDiscoverSwipePartialApproval() {
            HpsTrackData trackData = new HpsTrackData {
                Value = "%B6011000990156527^DIS TEST CARD^25121011000062111401?;6011000990156527=25121011000062111401?",
                Method = HpsTrackDataMethod.Swipe
            };

            HpsCharge response = creditService.Charge(155.00m)
                    .WithTrackData(trackData)
                    .WithAllowPartialAuth(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("10", response.ResponseCode);
            Assert.AreEqual(100.00m, response.AuthorizedAmount);
            test042TransactionId = response.TransactionId;
        }

        /*
            SALE WITH GRATUITY
            Tip Edit (Tip at Settlement)
         */

        [TestMethod]
        public void retail_043_ChargeVisaSwipeEditGratuity() {
            HpsTrackData trackData = new HpsTrackData {
                Value = "%B4012002000060016^VI TEST CREDIT^251210118039000000000396?;4012002000060016=25121011803939600000?",
                Method = HpsTrackDataMethod.Swipe
            };

            HpsCharge response = creditService.Charge(15.11m)
                    .WithTrackData(trackData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            HpsTransaction editResponse = creditService.Edit()
                .WithTransactionId(response.TransactionId)
                .WithAmount(18.11m)
                .WithGratuity(3.00m)
                .Execute();
            Assert.IsNotNull(editResponse);
            Assert.AreEqual("00", editResponse.ResponseCode);
        }

        [TestMethod]
        public void retail_044_ChargeMasterCardManualEditGratuity() {
            HpsCardHolder cardHolder = new HpsCardHolder {
                Address = new HpsAddress {
                    Zip = "75024"
                },
            };

            HpsCreditCard card = new HpsCreditCard {
                Number = "5473500000000014",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "123"
            };

            HpsCharge response = creditService.Charge(15.12m)
                    .WithCard(card)
                    .WithCardHolder(cardHolder)
                    .WithCardPresent(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            HpsTransaction editResponse = creditService.Edit()
                .WithTransactionId(response.TransactionId)
                .WithAmount(18.12m)
                .WithGratuity(3.00m)
                .Execute();
            Assert.IsNotNull(editResponse);
            Assert.AreEqual("00", editResponse.ResponseCode);
        }

        // Tip on Purchase

        [TestMethod]
        public void retail_045_ChargeVisaManualGratuity() {
            HpsCardHolder cardHolder = new HpsCardHolder {
                Address = new HpsAddress {
                    Zip = "75024"
                },
            };

            HpsCreditCard card = new HpsCreditCard {
                Number = "4012002000060016",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "123"
            };

            HpsCharge response = creditService.Charge(18.63m)
                    .WithCard(card)
                    .WithCardHolder(cardHolder)
                    .WithCardPresent(true)
                    .WithGratuity(3.50m)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void retail_046_ChargeMasterCardSwipeGratuity() {
            HpsTrackData trackData = new HpsTrackData {
                Value = "%B5473500000000014^MC TEST CARD^251210199998888777766665555444433332?;5473500000000014=25121019999888877776?",
                Method = HpsTrackDataMethod.Swipe
            };

            HpsCharge response = creditService.Charge(18.64m)
                    .WithTrackData(trackData)
                    .WithGratuity(3.50m)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            HpsTransaction editResponse = creditService.Edit()
                    .WithTransactionId(response.TransactionId)
                    .WithAmount(18.12m)
                    .WithGratuity(3.00m)
                    .Execute();
            Assert.IsNotNull(editResponse);
            Assert.AreEqual("00", editResponse.ResponseCode);
        }

        // LEVEL II CORPORATE PURCHASE CARD

        [TestMethod]
        public void retail_047_LevelIIVisaSwipeResponseB() {
            HpsTrackData trackData = new HpsTrackData {
                Value = "%B4012002000060016^VI TEST CREDIT^251210118039000000000396?;4012002000060016=25121011803939600000?",
                Method = HpsTrackDataMethod.Swipe
            };

            HpsCharge response = creditService.Charge(112.34m)
                    .WithTrackData(trackData)
                    .WithCpcReq(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual("B", response.CpcIndicator);

            HpsCpcData cpcData = new HpsCpcData("9876543210", taxTypeType.NOTUSED);
            HpsTransaction cpcResponse = creditService.CpcEdit(response.TransactionId).WithCpcData(cpcData).Execute();
            Assert.IsNotNull(cpcResponse);
            Assert.AreEqual("00", cpcResponse.ResponseCode);
        }

        [TestMethod]
        public void retail_048_LevelIIVisaSwipeResponseR() {
            HpsTrackData trackData = new HpsTrackData {
                Value = "%B4012002000060016^VI TEST CREDIT^251210118039000000000396?;4012002000060016=25121011803939600000?",
                Method = HpsTrackDataMethod.Swipe
            };

            HpsCharge response = creditService.Charge(123.45m)
                    .WithTrackData(trackData)
                    .WithCpcReq(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual("R", response.CpcIndicator);

            HpsCpcData cpcData = new HpsCpcData(taxTypeType.TAXEXEMPT);
            HpsTransaction cpcResponse = creditService.CpcEdit(response.TransactionId).WithCpcData(cpcData).Execute();
            Assert.IsNotNull(cpcResponse);
            Assert.AreEqual("00", cpcResponse.ResponseCode);
        }

        [TestMethod]
        public void retail_049_LevelIIVisaManualResponseS() {
            HpsCardHolder cardHolder = new HpsCardHolder {
                Address = new HpsAddress {
                    Zip = "75024"
                },
            };

            HpsCreditCard card = new HpsCreditCard {
                Number = "4012002000060016",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "123"
            };

            HpsCharge response = creditService.Charge(134.56m)
                    .WithCard(card)
                    .WithCardHolder(cardHolder)
                    .WithCardPresent(true)
                    .WithCpcReq(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual("S", response.CpcIndicator);

            HpsCpcData cpcData = new HpsCpcData("9876543210", taxTypeType.SALESTAX, 1.00m);
            HpsTransaction cpcResponse = creditService.CpcEdit(response.TransactionId).WithCpcData(cpcData).Execute();
            Assert.IsNotNull(cpcResponse);
            Assert.AreEqual("00", cpcResponse.ResponseCode);
        }

        [TestMethod]
        public void retail_050_LevelIIMasterCardSwipeResponseS() {
            HpsTrackData trackData = new HpsTrackData {
                Value = "%B5473500000000014^MC TEST CARD^251210199998888777766665555444433332?;5473500000000014=25121019999888877776?",
                Method = HpsTrackDataMethod.Swipe
            };

            HpsCharge response = creditService.Charge(111.06m)
                    .WithTrackData(trackData)
                    .WithCpcReq(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual("S", response.CpcIndicator);

            HpsCpcData cpcData = new HpsCpcData("9876543210", taxTypeType.NOTUSED);
            HpsTransaction cpcResponse = creditService.CpcEdit(response.TransactionId).WithCpcData(cpcData).Execute();
            Assert.IsNotNull(cpcResponse);
            Assert.AreEqual("00", cpcResponse.ResponseCode);
        }

        [TestMethod]
        public void retail_051_LevelIIMasterCardManualResponseS() {
            HpsCardHolder cardHolder = new HpsCardHolder {
                Address = new HpsAddress {
                    Zip = "75024"
                },
            };

            HpsCreditCard card = new HpsCreditCard {
                Number = "5473500000000014",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "123"
            };

            HpsCharge response = creditService.Charge(111.07m)
                    .WithCard(card)
                    .WithCardHolder(cardHolder)
                    .WithCardPresent(true)
                    .WithCpcReq(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual("S", response.CpcIndicator);

            HpsCpcData cpcData = new HpsCpcData("9876543210", taxTypeType.SALESTAX, 1.00m);
            HpsTransaction cpcResponse = creditService.CpcEdit(response.TransactionId).WithCpcData(cpcData).Execute();
            Assert.IsNotNull(cpcResponse);
            Assert.AreEqual("00", cpcResponse.ResponseCode);
        }

        [TestMethod]
        public void retail_052_LevelIIMasterCardManualResponseS() {
            HpsCardHolder cardHolder = new HpsCardHolder {
                Address = new HpsAddress {
                    Zip = "75024"
                },
            };

            HpsCreditCard card = new HpsCreditCard {
                Number = "5473500000000014",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "123"
            };

            HpsCharge response = creditService.Charge(111.09m)
                    .WithCard(card)
                    .WithCardHolder(cardHolder)
                    .WithCardPresent(true)
                    .WithCpcReq(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual("S", response.CpcIndicator);

            HpsCpcData cpcData = new HpsCpcData("9876543210", taxTypeType.TAXEXEMPT);
            HpsTransaction cpcResponse = creditService.CpcEdit(response.TransactionId).WithCpcData(cpcData).Execute();
            Assert.IsNotNull(cpcResponse);
            Assert.AreEqual("00", cpcResponse.ResponseCode);
        }

        [TestMethod]
        public void retail_053_LevelIIAmexSwipeNoResponse() {
            HpsTrackData trackData = new HpsTrackData {
                Value = "%B3727 006992 51018^AMEX TEST CARD^2512990502700?;372700699251018=2512990502700?",
                Method = HpsTrackDataMethod.Swipe
            };

            HpsCharge response = creditService.Charge(111.10m)
                    .WithTrackData(trackData)
                    .WithCpcReq(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual("0", response.CpcIndicator);

            HpsCpcData cpcData = new HpsCpcData(taxTypeType.SALESTAX, 1.00m);
            HpsTransaction cpcResponse = creditService.CpcEdit(response.TransactionId).WithCpcData(cpcData).Execute();
            Assert.IsNotNull(cpcResponse);
            Assert.AreEqual("00", cpcResponse.ResponseCode);
        }

        [TestMethod]
        public void retail_054_LevelIIAmexManualNoResponse() {
            HpsCardHolder cardHolder = new HpsCardHolder {
                Address = new HpsAddress {
                    Zip = "75024"
                },
            };

            HpsCreditCard card = new HpsCreditCard {
                Number = "372700699251018",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "1234"
            };

            HpsCharge response = creditService.Charge(111.11m)
                    .WithCard(card)
                    .WithCardHolder(cardHolder)
                    .WithCardPresent(true)
                    .WithCpcReq(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual("0", response.CpcIndicator);

            HpsCpcData cpcData = new HpsCpcData("9876543210", taxTypeType.NOTUSED);
            HpsTransaction cpcResponse = creditService.CpcEdit(response.TransactionId).WithCpcData(cpcData).Execute();
            Assert.IsNotNull(cpcResponse);
            Assert.AreEqual("00", cpcResponse.ResponseCode);
        }

        [TestMethod]
        public void retail_055_LevelIIAmexManualNoResponse() {
            HpsCardHolder cardHolder = new HpsCardHolder {
                Address = new HpsAddress {
                    Zip = "75024"
                },
            };

            HpsCreditCard card = new HpsCreditCard {
                Number = "372700699251018",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "1234"
            };

            HpsCharge response = creditService.Charge(111.12m)
                    .WithCard(card)
                    .WithCardHolder(cardHolder)
                    .WithCardPresent(true)
                    .WithCpcReq(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.AreEqual("0", response.CpcIndicator);

            HpsCpcData cpcData = new HpsCpcData("9876543210", taxTypeType.TAXEXEMPT);
            HpsTransaction cpcResponse = creditService.CpcEdit(response.TransactionId).WithCpcData(cpcData).Execute();
            Assert.IsNotNull(cpcResponse);
            Assert.AreEqual("00", cpcResponse.ResponseCode);
        }

        // OFFLINE SALE / AUTHORIZATION

        [TestMethod]
        public void retail_056_OfflineChargeVisaManual() {
            HpsCreditCard card = new HpsCreditCard {
                Number = "4012002000060016",
                ExpMonth = 12,
                ExpYear = 2025,
                Cvv = "123"
            };

            HpsTransaction response = creditService.OfflineCharge(15.11m)
                    .WithCard(card)
                    .WithOfflineAuthCode("654321")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void retail_056_OfflineAuthVisaManual() {
            HpsCreditCard card = new HpsCreditCard {
                Number = "4012002000060016",
                ExpMonth = 12,
                ExpYear = 2025
            };

            HpsTransaction response = creditService.OfflineAuth(15.11m)
                    .WithCard(card)
                    .WithOfflineAuthCode("654321")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // RETURN

        [TestMethod]
        public void retail_057_ReturnMasterCard() {
            HpsCreditCard card = new HpsCreditCard {
                Number = "5473500000000014",
                ExpMonth = 12,
                ExpYear = 2025
            };

            HpsRefund response = creditService.Refund(15.11m)
                    .WithCard(card)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, Ignore]
        public void retail_058_ReturnJcbTransactionId() {
            HpsRefund response = creditService.Refund(15.05m)
                    .WithTransactionId(test014TransactionId)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // ONLINE VOID / REVERSAL (Required)

        [TestMethod, Ignore]
        public void retail_059_ReversalVisa() {
            HpsReversal response = creditService.Reverse(15.01m)
                    .WithTransactionId(test010TransactionId)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, Ignore]
        public void retail_060_ReversalMasterCard() {
            HpsReversal response = creditService.Reverse(16.02m)
                    .WithTransactionId(test017TransactionId)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, Ignore]
        public void retail_061_ReversalMasterCard() {
            HpsReversal response = creditService.Reverse(17.02m)
                    .WithTransactionId(test023TransactionId)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, Ignore]
        public void retail_062_ReversalDiscover() {
            HpsReversal response = creditService.Reverse(100.00m)
                    .WithTransactionId(test042TransactionId)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, Ignore]
        public void retail_063_ReversalVisaPartial() {
            HpsReversal response = creditService.Reverse(15.06m)
                    .WithTransactionId(test015TransactionId)
                    .WithAuthAmount(5.06m)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, Ignore]
        public void retail_064_ReversalDiscoverPartial() {
            HpsReversal response = creditService.Reverse(16.07m)
                    .WithTransactionId(test021TransactionId)
                    .WithAuthAmount(6.07m)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // PIN DEBIT CARD FUNCTIONS

        [TestMethod]
        public void retail_065_DebitSaleVisaSwipe() {
            HpsTrackData trackData = new HpsTrackData {
                Value = "&lt;E1050711%B4012001000000016^VI TEST CREDIT^251200000000000000000000?|LO04K0WFOmdkDz0um+GwUkILL8ZZOP6Zc4rCpZ9+kg2T3JBT4AEOilWTI|+++++++Dbbn04ekG|11;4012001000000016=25120000000000000000?|1u2F/aEhbdoPixyAPGyIDv3gBfF|+++++++Dbbn04ekG|00|||/wECAQECAoFGAgEH2wYcShV78RZwb3NAc2VjdXJlZXhjaGFuZ2UubmV0PX50qfj4dt0lu9oFBESQQNkpoxEVpCW3ZKmoIV3T93zphPS3XKP4+DiVlM8VIOOmAuRrpzxNi0TN/DWXWSjUC8m/PI2dACGdl/hVJ/imfqIs68wYDnp8j0ZfgvM26MlnDbTVRrSx68Nzj2QAgpBCHcaBb/FZm9T7pfMr2Mlh2YcAt6gGG1i2bJgiEJn8IiSDX5M2ybzqRT86PCbKle/XCTwFFe1X|&gt;",
                Method = HpsTrackDataMethod.Swipe,
                EncryptionData = new HpsEncryptionData {
                    Version = "01"
                }
            };

            HpsDebitAuthorization response = debitService.Charge(14.01m)
                    .WithTrackData(trackData)
                    .WithPinBlock("32539F50C245A6A93D123412324000AA")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void retail_066_DebitSaleMasterCardSwipe() {
            HpsTrackData trackData = new HpsTrackData {
                Value = "&lt;E1052711%B5473501000000014^MC TEST CARD^251200000000000000000000000000000000?|GVEY/MKaKXuqqjKRRueIdCHPPoj1gMccgNOtHC41ymz7bIvyJJVdD3LW8BbwvwoenI+|+++++++C4cI2zjMp|11;5473501000000014=25120000000000000000?|8XqYkQGMdGeiIsgM0pzdCbEGUDP|+++++++C4cI2zjMp|00|||/wECAQECAoFGAgEH2wYcShV78RZwb3NAc2VjdXJlZXhjaGFuZ2UubmV0PX50qfj4dt0lu9oFBESQQNkpoxEVpCW3ZKmoIV3T93zphPS3XKP4+DiVlM8VIOOmAuRrpzxNi0TN/DWXWSjUC8m/PI2dACGdl/hVJ/imfqIs68wYDnp8j0ZfgvM26MlnDbTVRrSx68Nzj2QAgpBCHcaBb/FZm9T7pfMr2Mlh2YcAt6gGG1i2bJgiEJn8IiSDX5M2ybzqRT86PCbKle/XCTwFFe1X|&gt;",
                Method = HpsTrackDataMethod.Swipe,
                EncryptionData = new HpsEncryptionData {
                    Version = "01"
                }
            };

            HpsDebitAuthorization response = debitService.Charge(14.02m)
                    .WithTrackData(trackData)
                    .WithPinBlock("F505AD81659AA42A3D123412324000AB")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            test066TransactionId = response.TransactionId;
        }

        [TestMethod]
        public void retail_067_DebitSaleVisaSwipeCashBack() {
            HpsTrackData trackData = new HpsTrackData {
                Value = "&lt;E1050711%B4012001000000016^VI TEST CREDIT^251200000000000000000000?|LO04K0WFOmdkDz0um+GwUkILL8ZZOP6Zc4rCpZ9+kg2T3JBT4AEOilWTI|+++++++Dbbn04ekG|11;4012001000000016=25120000000000000000?|1u2F/aEhbdoPixyAPGyIDv3gBfF|+++++++Dbbn04ekG|00|||/wECAQECAoFGAgEH2wYcShV78RZwb3NAc2VjdXJlZXhjaGFuZ2UubmV0PX50qfj4dt0lu9oFBESQQNkpoxEVpCW3ZKmoIV3T93zphPS3XKP4+DiVlM8VIOOmAuRrpzxNi0TN/DWXWSjUC8m/PI2dACGdl/hVJ/imfqIs68wYDnp8j0ZfgvM26MlnDbTVRrSx68Nzj2QAgpBCHcaBb/FZm9T7pfMr2Mlh2YcAt6gGG1i2bJgiEJn8IiSDX5M2ybzqRT86PCbKle/XCTwFFe1X|&gt;",
                Method = HpsTrackDataMethod.Swipe,
                EncryptionData = new HpsEncryptionData {
                    Version = "01"
                }
            };

            HpsDebitAuthorization response = debitService.Charge(14.03m)
                    .WithTrackData(trackData)
                    .WithPinBlock("32539F50C245A6A93D123412324000AA")
                    .WithCashBack(5.00m)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // PARTIALLY APPROVED PURCHASE

        [TestMethod]
        public void retail_068_DebitSaleMasterCardPartialApproval() {
            HpsTrackData trackData = new HpsTrackData {
                Value = "&lt;E1052711%B5473501000000014^MC TEST CARD^251200000000000000000000000000000000?|GVEY/MKaKXuqqjKRRueIdCHPPoj1gMccgNOtHC41ymz7bIvyJJVdD3LW8BbwvwoenI+|+++++++C4cI2zjMp|11;5473501000000014=25120000000000000000?|8XqYkQGMdGeiIsgM0pzdCbEGUDP|+++++++C4cI2zjMp|00|||/wECAQECAoFGAgEH2wYcShV78RZwb3NAc2VjdXJlZXhjaGFuZ2UubmV0PX50qfj4dt0lu9oFBESQQNkpoxEVpCW3ZKmoIV3T93zphPS3XKP4+DiVlM8VIOOmAuRrpzxNi0TN/DWXWSjUC8m/PI2dACGdl/hVJ/imfqIs68wYDnp8j0ZfgvM26MlnDbTVRrSx68Nzj2QAgpBCHcaBb/FZm9T7pfMr2Mlh2YcAt6gGG1i2bJgiEJn8IiSDX5M2ybzqRT86PCbKle/XCTwFFe1X|&gt;",
                Method = HpsTrackDataMethod.Swipe,
                EncryptionData = new HpsEncryptionData {
                    Version = "01"
                }
            };

            HpsDebitAuthorization response = debitService.Charge(33.00m)
                    .WithTrackData(trackData)
                    .WithPinBlock("F505AD81659AA42A3D123412324000AB")
                    .WithAllowPartialAuth(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("10", response.ResponseCode);
            Assert.AreEqual(22.00m, response.AuthorizedAmount);
        }

        [TestMethod]
        public void retail_069_DebitSaleVisaPartialApproval() {
            HpsTrackData trackData = new HpsTrackData {
                Value = "&lt;E1050711%B4012001000000016^VI TEST CREDIT^251200000000000000000000?|LO04K0WFOmdkDz0um+GwUkILL8ZZOP6Zc4rCpZ9+kg2T3JBT4AEOilWTI|+++++++Dbbn04ekG|11;4012001000000016=25120000000000000000?|1u2F/aEhbdoPixyAPGyIDv3gBfF|+++++++Dbbn04ekG|00|||/wECAQECAoFGAgEH2wYcShV78RZwb3NAc2VjdXJlZXhjaGFuZ2UubmV0PX50qfj4dt0lu9oFBESQQNkpoxEVpCW3ZKmoIV3T93zphPS3XKP4+DiVlM8VIOOmAuRrpzxNi0TN/DWXWSjUC8m/PI2dACGdl/hVJ/imfqIs68wYDnp8j0ZfgvM26MlnDbTVRrSx68Nzj2QAgpBCHcaBb/FZm9T7pfMr2Mlh2YcAt6gGG1i2bJgiEJn8IiSDX5M2ybzqRT86PCbKle/XCTwFFe1X|&gt;",
                Method = HpsTrackDataMethod.Swipe,
                EncryptionData = new HpsEncryptionData {
                    Version = "01"
                }
            };

            HpsDebitAuthorization response = debitService.Charge(44.00m)
                    .WithTrackData(trackData)
                    .WithPinBlock("32539F50C245A6A93D123412324000AA")
                    .WithAllowPartialAuth(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("10", response.ResponseCode);
            Assert.AreEqual(33.00m, response.AuthorizedAmount);
            test069TransactionId = response.TransactionId;
        }

        // RETURN

        [TestMethod]
        public void retail_070_DebitReturnVisaSwipe() {
            HpsTrackData trackData = new HpsTrackData {
                Value = "&lt;E1050711%B4012001000000016^VI TEST CREDIT^251200000000000000000000?|LO04K0WFOmdkDz0um+GwUkILL8ZZOP6Zc4rCpZ9+kg2T3JBT4AEOilWTI|+++++++Dbbn04ekG|11;4012001000000016=25120000000000000000?|1u2F/aEhbdoPixyAPGyIDv3gBfF|+++++++Dbbn04ekG|00|||/wECAQECAoFGAgEH2wYcShV78RZwb3NAc2VjdXJlZXhjaGFuZ2UubmV0PX50qfj4dt0lu9oFBESQQNkpoxEVpCW3ZKmoIV3T93zphPS3XKP4+DiVlM8VIOOmAuRrpzxNi0TN/DWXWSjUC8m/PI2dACGdl/hVJ/imfqIs68wYDnp8j0ZfgvM26MlnDbTVRrSx68Nzj2QAgpBCHcaBb/FZm9T7pfMr2Mlh2YcAt6gGG1i2bJgiEJn8IiSDX5M2ybzqRT86PCbKle/XCTwFFe1X|&gt;",
                Method = HpsTrackDataMethod.Swipe,
                EncryptionData = new HpsEncryptionData {
                    Version = "01"
                }
            };

            HpsDebitAuthorization response = debitService.Refund(14.07m)
                    .WithTrackData(trackData)
                    .WithPinBlock("32539F50C245A6A93D123412324000AA")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // REVERSAL

        [TestMethod, Ignore]
        public void retail_071_DebitReversalMasterCard() {
            HpsTransaction response = debitService.Reverse(14.02m)
                    .WithTransactionId(test066TransactionId)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, Ignore]
        public void retail_072_DebitReversalVisa() {
            HpsTransaction response = debitService.Reverse(33.00m)
                    .WithTransactionId(test069TransactionId)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        /*
            ONE Card - GSB CARD FUNCTIONS
            Balance Inquiry
         */

        [TestMethod, Ignore]
        public void retail_073_BalanceInquiryGsbSwipe() {
            HpsTrackData trackData = new HpsTrackData {
                Value = "%B6277220572999800^   /                         ^49121010557010000016000000?F;6277220572999800=49121010557010000016?",
                Method = HpsTrackDataMethod.Swipe
            };

            HpsAuthorization response = creditService.PrePaidBalanceInquiry()
                    .WithTrackData(trackData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, Ignore]
        public void retail_074_BalanceInquiryGsbManual() {
            HpsCreditCard card = new HpsCreditCard {
                Number = "6277220572999800",
                ExpMonth = 12,
                ExpYear = 2049
            };

            HpsAuthorization response = creditService.PrePaidBalanceInquiry()
                    .WithCard(card)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // Add Value (LOAD)

        [TestMethod, Ignore]
        public void retail_075_AddValueGsbSwipe() {
            HpsTrackData trackData = new HpsTrackData {
                Value = "%B6277220572999800^   /                         ^49121010557010000016000000?F;6277220572999800=49121010557010000016?",
                Method = HpsTrackDataMethod.Swipe
            };

            HpsAuthorization response = creditService.PrePaidAddValue(5.00m)
                    .WithTrackData(trackData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, Ignore]
        public void retail_076_AddValueGsbManual() {
            HpsCreditCard card = new HpsCreditCard {
                Number = "6277220572999800",
                ExpMonth = 12,
                ExpYear = 2049
            };

            HpsAuthorization response = creditService.PrePaidAddValue(5.00m)
                    .WithCard(card)
                    .WithAllowDuplicates(true)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // SALE

        [TestMethod, Ignore]
        public void retail_077_ChargeGsbSwipeReversal() {
            HpsTrackData trackData = new HpsTrackData {
                Value = "%B6277220572999800^   /                         ^49121010557010000016000000?F;6277220572999800=49121010557010000016?",
                Method = HpsTrackDataMethod.Swipe
            };

            HpsCharge response = creditService.Charge(2.05m)
                    .WithTrackData(trackData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            HpsTransaction reversalResponse = creditService.Reverse(2.05m)
                    .WithTransactionId(response.TransactionId)
                    .Execute();
            Assert.IsNotNull(reversalResponse);
            Assert.AreEqual("00", reversalResponse.ResponseCode);
        }

        [TestMethod, Ignore]
        public void retail_078_ChargeGsbSwipe() {
            HpsTrackData trackData = new HpsTrackData {
                Value = "%B6277220572999800^   /                         ^49121010557010000016000000?F;6277220572999800=49121010557010000016?",
                Method = HpsTrackDataMethod.Swipe
            };

            HpsCharge response = creditService.Charge(2.10m)
                    .WithTrackData(trackData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod, Ignore]
        public void retail_079_ChargeGsbSwipePartialReversal() {
            HpsTrackData trackData = new HpsTrackData {
                Value = "%B6277220572999800^   /                         ^49121010557010000016000000?F;6277220572999800=49121010557010000016?",
                Method = HpsTrackDataMethod.Swipe
            };

            HpsCharge response = creditService.Charge(2.15m)
                    .WithTrackData(trackData)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);

            HpsTransaction reversalResponse = creditService.Reverse(2.15m)
                    .WithTransactionId(response.TransactionId)
                    .WithAuthAmount(1.15m)
                    .Execute();
            Assert.IsNotNull(reversalResponse);
            Assert.AreEqual("00", reversalResponse.ResponseCode);
        }

        /*
           EBT FUNCTIONS
            Food Stamp Purchase
         */

        [TestMethod]
        public void retail_080_EbtfsPurchaseVisaSwipe() {
            HpsTrackData trackData = new HpsTrackData {
                Value = "&lt;E1050711%B4012001000000016^VI TEST CREDIT^251200000000000000000000?|LO04K0WFOmdkDz0um+GwUkILL8ZZOP6Zc4rCpZ9+kg2T3JBT4AEOilWTI|+++++++Dbbn04ekG|11;4012001000000016=25120000000000000000?|1u2F/aEhbdoPixyAPGyIDv3gBfF|+++++++Dbbn04ekG|00|||/wECAQECAoFGAgEH2wYcShV78RZwb3NAc2VjdXJlZXhjaGFuZ2UubmV0PX50qfj4dt0lu9oFBESQQNkpoxEVpCW3ZKmoIV3T93zphPS3XKP4+DiVlM8VIOOmAuRrpzxNi0TN/DWXWSjUC8m/PI2dACGdl/hVJ/imfqIs68wYDnp8j0ZfgvM26MlnDbTVRrSx68Nzj2QAgpBCHcaBb/FZm9T7pfMr2Mlh2YcAt6gGG1i2bJgiEJn8IiSDX5M2ybzqRT86PCbKle/XCTwFFe1X|&gt;",
                Method = HpsTrackDataMethod.Swipe,
                EncryptionData = new HpsEncryptionData {
                    Version = "01"
                }
            };

            HpsEbtAuthorization response = ebtService.Purchase(101.01m)
                    .WithTrackData(trackData)
                    .WithPinBlock("32539F50C245A6A93D123412324000AA")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void retail_081_EbtfsPurchaseVisaManual() {
            HpsCreditCard card = new HpsCreditCard {
                Number = "4012002000060016",
                ExpMonth = 12,
                ExpYear = 2025
            };

            HpsEbtAuthorization response = ebtService.Purchase(102.01m)
                    .WithCard(card)
                    .WithPinBlock("32539F50C245A6A93D123412324000AA")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // Food Stamp Electronic Voucher (Manual Entry Only)

        [TestMethod]
        public void retail_082_EbtVoucherPurchaseVisa() {
            HpsCreditCard card = new HpsCreditCard {
                Number = "4012002000060016",
                ExpMonth = 12,
                ExpYear = 2025
            };

            HpsEbtAuthorization response = ebtService.VoucherPurchase(103.01m)
                    .WithCard(card)
                    .WithPinBlock("32539F50C245A6A93D123412324000AA")
                    .WithSerialNumber("123456789012345")
                    .WithApprovalCode("123456")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // Food Stamp Balance Inquiry

        [TestMethod]
        public void retail_083_EbtfsReturnVisaSwipe() {
            HpsTrackData trackData = new HpsTrackData {
                Value = "&lt;E1050711%B4012001000000016^VI TEST CREDIT^251200000000000000000000?|LO04K0WFOmdkDz0um+GwUkILL8ZZOP6Zc4rCpZ9+kg2T3JBT4AEOilWTI|+++++++Dbbn04ekG|11;4012001000000016=25120000000000000000?|1u2F/aEhbdoPixyAPGyIDv3gBfF|+++++++Dbbn04ekG|00|||/wECAQECAoFGAgEH2wYcShV78RZwb3NAc2VjdXJlZXhjaGFuZ2UubmV0PX50qfj4dt0lu9oFBESQQNkpoxEVpCW3ZKmoIV3T93zphPS3XKP4+DiVlM8VIOOmAuRrpzxNi0TN/DWXWSjUC8m/PI2dACGdl/hVJ/imfqIs68wYDnp8j0ZfgvM26MlnDbTVRrSx68Nzj2QAgpBCHcaBb/FZm9T7pfMr2Mlh2YcAt6gGG1i2bJgiEJn8IiSDX5M2ybzqRT86PCbKle/XCTwFFe1X|&gt;",
                Method = HpsTrackDataMethod.Swipe,
                EncryptionData = new HpsEncryptionData {
                    Version = "01"
                }
            };

            HpsEbtAuthorization response = ebtService.Refund(104.01m)
                    .WithTrackData(trackData)
                    .WithPinBlock("32539F50C245A6A93D123412324000AA")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void retail_084_EbtfsReturnVisaManual() {
            HpsCreditCard card = new HpsCreditCard {
                Number = "4012002000060016",
                ExpMonth = 12,
                ExpYear = 2025
            };

            HpsEbtAuthorization response = ebtService.Refund(105.01m)
                    .WithCard(card)
                    .WithPinBlock("32539F50C245A6A93D123412324000AA")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // Food Stamp Balance Inquiry

        [TestMethod]
        public void retail_085_EbtBalanceInquiryVisaSwipe() {
            HpsTrackData trackData = new HpsTrackData {
                Value = "&lt;E1050711%B4012001000000016^VI TEST CREDIT^251200000000000000000000?|LO04K0WFOmdkDz0um+GwUkILL8ZZOP6Zc4rCpZ9+kg2T3JBT4AEOilWTI|+++++++Dbbn04ekG|11;4012001000000016=25120000000000000000?|1u2F/aEhbdoPixyAPGyIDv3gBfF|+++++++Dbbn04ekG|00|||/wECAQECAoFGAgEH2wYcShV78RZwb3NAc2VjdXJlZXhjaGFuZ2UubmV0PX50qfj4dt0lu9oFBESQQNkpoxEVpCW3ZKmoIV3T93zphPS3XKP4+DiVlM8VIOOmAuRrpzxNi0TN/DWXWSjUC8m/PI2dACGdl/hVJ/imfqIs68wYDnp8j0ZfgvM26MlnDbTVRrSx68Nzj2QAgpBCHcaBb/FZm9T7pfMr2Mlh2YcAt6gGG1i2bJgiEJn8IiSDX5M2ybzqRT86PCbKle/XCTwFFe1X|&gt;",
                Method = HpsTrackDataMethod.Swipe,
                EncryptionData = new HpsEncryptionData {
                    Version = "01"
                }
            };

            HpsEbtAuthorization response = ebtService.BalanceInquiry()
                    .WithTrackData(trackData)
                    .WithPinBlock("32539F50C245A6A93D123412324000AA")
                    .WithInquiryType(EBTBalanceInquiryType.FOODSTAMP)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void retail_086_EbtBalanceInquiryVisaManual() {
            HpsCreditCard card = new HpsCreditCard {
                Number = "4012002000060016",
                ExpMonth = 12,
                ExpYear = 2025
            };

            HpsEbtAuthorization response = ebtService.BalanceInquiry()
                    .WithCard(card)
                    .WithPinBlock("32539F50C245A6A93D123412324000AA")
                    .WithCardPresent(true)
                    .WithReaderPresent(true)
                    .WithInquiryType(EBTBalanceInquiryType.FOODSTAMP)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        /*
            Assert.AreEqual("00", response.ResponseCode);
            EBT CASH BENEFITS
            Cash Back Purchase
         */

        [TestMethod]
        public void retail_087_EbtCashBackPurchaseVisaSwipe() {
            HpsTrackData trackData = new HpsTrackData {
                Value = "&lt;E1050711%B4012001000000016^VI TEST CREDIT^251200000000000000000000?|LO04K0WFOmdkDz0um+GwUkILL8ZZOP6Zc4rCpZ9+kg2T3JBT4AEOilWTI|+++++++Dbbn04ekG|11;4012001000000016=25120000000000000000?|1u2F/aEhbdoPixyAPGyIDv3gBfF|+++++++Dbbn04ekG|00|||/wECAQECAoFGAgEH2wYcShV78RZwb3NAc2VjdXJlZXhjaGFuZ2UubmV0PX50qfj4dt0lu9oFBESQQNkpoxEVpCW3ZKmoIV3T93zphPS3XKP4+DiVlM8VIOOmAuRrpzxNi0TN/DWXWSjUC8m/PI2dACGdl/hVJ/imfqIs68wYDnp8j0ZfgvM26MlnDbTVRrSx68Nzj2QAgpBCHcaBb/FZm9T7pfMr2Mlh2YcAt6gGG1i2bJgiEJn8IiSDX5M2ybzqRT86PCbKle/XCTwFFe1X|&gt;",
                Method = HpsTrackDataMethod.Swipe,
                EncryptionData = new HpsEncryptionData {
                    Version = "01"
                }
            };

            HpsEbtAuthorization response = ebtService.CashBackPurchase(106.01m)
                    .WithTrackData(trackData)
                    .WithPinBlock("32539F50C245A6A93D123412324000AA")
                    .WithCashBack(5.00m)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void retail_088_EbtCashBackPurchaseVisaManual() {
            HpsCreditCard card = new HpsCreditCard {
                Number = "4012002000060016",
                ExpMonth = 12,
                ExpYear = 2025
            };

            HpsEbtAuthorization response = ebtService.CashBackPurchase(107.01m)
                    .WithCard(card)
                    .WithPinBlock("32539F50C245A6A93D123412324000AA")
                    .WithCashBack(5.00m)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // No Cash Back Purchase

        [TestMethod]
        public void retail_089_EbtCashBackPurchaseVisaSwipeNoCashBack() {
            HpsTrackData trackData = new HpsTrackData {
                Value = "&lt;E1050711%B4012001000000016^VI TEST CREDIT^251200000000000000000000?|LO04K0WFOmdkDz0um+GwUkILL8ZZOP6Zc4rCpZ9+kg2T3JBT4AEOilWTI|+++++++Dbbn04ekG|11;4012001000000016=25120000000000000000?|1u2F/aEhbdoPixyAPGyIDv3gBfF|+++++++Dbbn04ekG|00|||/wECAQECAoFGAgEH2wYcShV78RZwb3NAc2VjdXJlZXhjaGFuZ2UubmV0PX50qfj4dt0lu9oFBESQQNkpoxEVpCW3ZKmoIV3T93zphPS3XKP4+DiVlM8VIOOmAuRrpzxNi0TN/DWXWSjUC8m/PI2dACGdl/hVJ/imfqIs68wYDnp8j0ZfgvM26MlnDbTVRrSx68Nzj2QAgpBCHcaBb/FZm9T7pfMr2Mlh2YcAt6gGG1i2bJgiEJn8IiSDX5M2ybzqRT86PCbKle/XCTwFFe1X|&gt;",
                Method = HpsTrackDataMethod.Swipe,
                EncryptionData = new HpsEncryptionData {
                    Version = "01"
                }
            };

            HpsEbtAuthorization response = ebtService.CashBackPurchase(108.01m)
                    .WithTrackData(trackData)
                    .WithPinBlock("32539F50C245A6A93D123412324000AA")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void retail_090_EbtCashBackPurchaseVisaManualNoCashBack() {
            HpsCreditCard card = new HpsCreditCard {
                Number = "4012002000060016",
                ExpMonth = 12,
                ExpYear = 2025
            };

            HpsEbtAuthorization response = ebtService.CashBackPurchase(109.01m)
                    .WithCard(card)
                    .WithPinBlock("32539F50C245A6A93D123412324000AA")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // Cash Back Balance Inquiry

        [TestMethod]
        public void retail_091_EbtBalanceInquiryVisaSwipeCash() {
            HpsTrackData trackData = new HpsTrackData {
                Value = "&lt;E1050711%B4012001000000016^VI TEST CREDIT^251200000000000000000000?|LO04K0WFOmdkDz0um+GwUkILL8ZZOP6Zc4rCpZ9+kg2T3JBT4AEOilWTI|+++++++Dbbn04ekG|11;4012001000000016=25120000000000000000?|1u2F/aEhbdoPixyAPGyIDv3gBfF|+++++++Dbbn04ekG|00|||/wECAQECAoFGAgEH2wYcShV78RZwb3NAc2VjdXJlZXhjaGFuZ2UubmV0PX50qfj4dt0lu9oFBESQQNkpoxEVpCW3ZKmoIV3T93zphPS3XKP4+DiVlM8VIOOmAuRrpzxNi0TN/DWXWSjUC8m/PI2dACGdl/hVJ/imfqIs68wYDnp8j0ZfgvM26MlnDbTVRrSx68Nzj2QAgpBCHcaBb/FZm9T7pfMr2Mlh2YcAt6gGG1i2bJgiEJn8IiSDX5M2ybzqRT86PCbKle/XCTwFFe1X|&gt;",
                Method = HpsTrackDataMethod.Swipe,
                EncryptionData = new HpsEncryptionData {
                    Version = "01"
                }
            };

            HpsEbtAuthorization response = ebtService.BalanceInquiry()
                    .WithTrackData(trackData)
                    .WithPinBlock("32539F50C245A6A93D123412324000AA")
                    .WithInquiryType(EBTBalanceInquiryType.CASH)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void retail_092_EbtBalanceInquiryVisaManualCash() {
            HpsCreditCard card = new HpsCreditCard {
                Number = "4012002000060016",
                ExpMonth = 12,
                ExpYear = 2025
            };

            HpsEbtAuthorization response = ebtService.BalanceInquiry()
                    .WithCard(card)
                    .WithPinBlock("32539F50C245A6A93D123412324000AA")
                    .WithCardPresent(true)
                    .WithReaderPresent(true)
                    .WithInquiryType(EBTBalanceInquiryType.CASH)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        // Bash Benefits Withdrawal

        [TestMethod]
        public void retail_093_EbtBenefitWithDrawalVisaSwipe() {
            HpsTrackData trackData = new HpsTrackData {
                Value = "&lt;E1050711%B4012001000000016^VI TEST CREDIT^251200000000000000000000?|LO04K0WFOmdkDz0um+GwUkILL8ZZOP6Zc4rCpZ9+kg2T3JBT4AEOilWTI|+++++++Dbbn04ekG|11;4012001000000016=25120000000000000000?|1u2F/aEhbdoPixyAPGyIDv3gBfF|+++++++Dbbn04ekG|00|||/wECAQECAoFGAgEH2wYcShV78RZwb3NAc2VjdXJlZXhjaGFuZ2UubmV0PX50qfj4dt0lu9oFBESQQNkpoxEVpCW3ZKmoIV3T93zphPS3XKP4+DiVlM8VIOOmAuRrpzxNi0TN/DWXWSjUC8m/PI2dACGdl/hVJ/imfqIs68wYDnp8j0ZfgvM26MlnDbTVRrSx68Nzj2QAgpBCHcaBb/FZm9T7pfMr2Mlh2YcAt6gGG1i2bJgiEJn8IiSDX5M2ybzqRT86PCbKle/XCTwFFe1X|&gt;",
                Method = HpsTrackDataMethod.Swipe,
                EncryptionData = new HpsEncryptionData {
                    Version = "01"
                }
            };

            HpsEbtAuthorization response = ebtService.CashBackPurchase(110.01m)
                    .WithTrackData(trackData)
                    .WithPinBlock("32539F50C245A6A93D123412324000AA")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void retail_094_EbtBenefitWithDrawalVisaManual() {
            HpsCreditCard card = new HpsCreditCard {
                Number = "4012002000060016",
                ExpMonth = 12,
                ExpYear = 2025
            };

            HpsEbtAuthorization response = ebtService.CashBackPurchase(111.01m)
                    .WithCard(card)
                    .WithPinBlock("32539F50C245A6A93D123412324000AA")
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        /*
            HMS GIFT - REWARDS
            GIFT
            ACTIVATE
         */

        [TestMethod]
        public void retail_095_ActivateGift1Swipe() {
            HpsGiftCard card = new HpsGiftCard {
                Value = "%B5022440000000000098^^391200081613?;5022440000000000098=391200081613?",
                ValueType = ItemChoiceType.TrackData
            };

            HpsGiftCardResponse response = giftService.Activate(6.00m)
                    .WithCard(card)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void retail_096_ActivateGift2Manual() {
            HpsGiftCard card = new HpsGiftCard {
                Value = "5022440000000000007"
            };

            HpsGiftCardResponse response = giftService.Activate(7.00m)
                    .WithCard(card)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        // ADD VALUE

        [TestMethod]
        public void retail_097_AddValueGift1Swipe() {
            HpsGiftCard card = new HpsGiftCard {
                Value = "%B5022440000000000098^^391200081613?;5022440000000000098=391200081613?",
                ValueType = ItemChoiceType.TrackData
            };

            HpsGiftCardResponse response = giftService.AddValue(8.00m)
                    .WithCard(card)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void retail_098_AddValueGift2Manual() {
            HpsGiftCard card = new HpsGiftCard {
                Value = "5022440000000000007"
            };

            HpsGiftCardResponse response = giftService.Activate(9.00m)
                    .WithCard(card)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        // BALANCE INQUIRY

        [TestMethod]
        public void retail_099_BalanceInquiryGift1Swipe() {
            HpsGiftCard card = new HpsGiftCard {
                Value = "%B5022440000000000098^^391200081613?;5022440000000000098=391200081613?",
                ValueType = ItemChoiceType.TrackData
            };

            HpsGiftCardResponse response = giftService.Balance()
                    .WithCard(card)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
            Assert.AreEqual(10.00m, response.BalanceAmount);
        }

        [TestMethod]
        public void retail_100_BalanceInquiryGift2Manual() {
            HpsGiftCard card = new HpsGiftCard {
                Value = "5022440000000000007"
            };

            HpsGiftCardResponse response = giftService.Balance()
                    .WithCard(card)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
            Assert.AreEqual(10.00m, response.BalanceAmount);
        }

        // REPLACE / TRANSFER

        [TestMethod]
        public void retail_101_ReplaceGift1Swipe() {
            HpsGiftCard oldCard = new HpsGiftCard {
                Value = "%B5022440000000000098^^391200081613?;5022440000000000098=391200081613?",
                ValueType = ItemChoiceType.TrackData
            };

            HpsGiftCard newCard = new HpsGiftCard {
                Value = "5022440000000000007",
                ValueType = ItemChoiceType.CardNbr
            };

            HpsGiftCardResponse response = giftService.Replace()
                    .WithOldCard(oldCard)
                    .WithNewCard(newCard)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void retail_102_ReplaceGift2Manual() {
            HpsGiftCard newCard = new HpsGiftCard {
                Value = "%B5022440000000000098^^391200081613?;5022440000000000098=391200081613?",
                ValueType = ItemChoiceType.TrackData
            };

            HpsGiftCard oldCard = new HpsGiftCard {
                Value = "5022440000000000007",
                ValueType = ItemChoiceType.CardNbr
            };

            HpsGiftCardResponse response = giftService.Replace()
                    .WithOldCard(oldCard)
                    .WithNewCard(newCard)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        // SALE / REDEEM

        [TestMethod]
        public void retail_103_SaleGift1Swipe() {
            HpsGiftCard card = new HpsGiftCard {
                Value = "%B5022440000000000098^^391200081613?;5022440000000000098=391200081613?",
                ValueType = ItemChoiceType.TrackData
            };

            HpsGiftCardSale response = giftService.Sale(1.00m)
                    .WithCard(card)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void retail_104_SaleGift2Manual() {
            HpsGiftCard card = new HpsGiftCard {
                Value = "5022440000000000007"
            };

            HpsGiftCardSale response = giftService.Sale(2.00m)
                    .WithCard(card)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void retail_105_SaleGift1VoidSwipe() {
            HpsGiftCard card = new HpsGiftCard {
                Value = "%B5022440000000000098^^391200081613?;5022440000000000098=391200081613?",
                ValueType = ItemChoiceType.TrackData
            };

            HpsGiftCardSale response = giftService.Sale(3.00m)
                    .WithCard(card)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
            test105TransactionId = response.TransactionId;
        }

        [TestMethod]
        public void retail_106_SaleGift2ReversalManual() {
            HpsGiftCard card = new HpsGiftCard {
                Value = "5022440000000000007"
            };

            HpsGiftCardSale response = giftService.Sale(4.00m)
                    .WithCard(card)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
            test106TransactionId = response.TransactionId;
        }

        // VOID

        [TestMethod, Ignore]
        public void retail_107_VoidGift() {
            HpsGiftCardResponse response = giftService.VoidSale(test105TransactionId).Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        // REVERSAL

        [TestMethod, Ignore]
        public void retail_108_ReversalGift() {
            HpsGiftCardResponse response = giftService.Reverse(4.00m)
                    .WithTransactionId(test106TransactionId)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        // DEACTIVATE

        [TestMethod]
        public void retail_109_DeactivateGift1() {
            HpsGiftCard card = new HpsGiftCard {
                Value = "%B5022440000000000098^^391200081613?;5022440000000000098=391200081613?",
                ValueType = ItemChoiceType.TrackData
            };

            HpsGiftCardResponse response = giftService.Deactivate()
                    .WithCard(card)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        // RECEIPTS MESSAGING

        [TestMethod]
        public void retail_110_ReceiptsMessaging() {
            // PRINT AND SCAN RECEIPT FOR TEST 107
        }

        /*
            REWARDS
            BALANCE INQUIRY
         */

        [TestMethod]
        public void retail_111_BalanceInquiryRewards1() {
            HpsGiftCard card = new HpsGiftCard {
                Value = "%B5022440000000000098^^391200081613?;5022440000000000098=391200081613?",
                ValueType = ItemChoiceType.TrackData
            };

            HpsGiftCardResponse response = giftService.Balance()
                    .WithCard(card)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
            Assert.AreEqual(0m, response.PointsBalanceAmount);
        }

        [TestMethod]
        public void retail_112_BalanceInquiryRewards2() {
            HpsGiftCard card = new HpsGiftCard {
                Value = "5022440000000000007"
            };

            HpsGiftCardResponse response = giftService.Balance()
                    .WithCard(card)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
            Assert.AreEqual(0m, response.PointsBalanceAmount);
        }

        // ALIAS

        [TestMethod]
        public void retail_113_CreateAliasGift1() {
            HpsGiftCardAlias response = giftService.Alias()
                    .WithAlias("9725550100")
                    .WithAction(GiftCardAliasReqBlock1TypeAction.CREATE)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void retail_114_CreateAliasGift2() {
            HpsGiftCardAlias response = giftService.Alias()
                    .WithAlias("9725550100")
                    .WithAction(GiftCardAliasReqBlock1TypeAction.CREATE)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void retail_115_AddAliasGift1() {
            HpsGiftCard card = new HpsGiftCard {
                Value = "%B5022440000000000098^^391200081613?;5022440000000000098=391200081613?",
                ValueType = ItemChoiceType.TrackData
            };

            HpsGiftCardAlias response = giftService.Alias()
                    .WithCard(card)
                    .WithAlias("2145550199")
                    .WithAction(GiftCardAliasReqBlock1TypeAction.ADD)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void retail_116_AddAliasGift2() {
            HpsGiftCard card = new HpsGiftCard {
                Value = "5022440000000000007"
            };

            HpsGiftCardAlias response = giftService.Alias()
                    .WithCard(card)
                    .WithAlias("2145550199")
                    .WithAction(GiftCardAliasReqBlock1TypeAction.ADD)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void retail_117_DeleteAliasGift1() {
            HpsGiftCard card = new HpsGiftCard {
                Value = "%B5022440000000000098^^391200081613?;5022440000000000098=391200081613?",
                ValueType = ItemChoiceType.TrackData
            };

            HpsGiftCardAlias response = giftService.Alias()
                    .WithCard(card)
                    .WithAlias("2145550199")
                    .WithAction(GiftCardAliasReqBlock1TypeAction.DELETE)
                    .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void retail_999_CloseBatch() {
            try {
                HpsBatch response = this.batchService.CloseBatch();
                if (response == null)
                    Assert.Fail("Response is null");
                Console.WriteLine(String.Format("Batch ID: {0}", response.Id));
                Console.WriteLine(String.Format("Sequence Number: {0}", response.SequenceNumber));
            }
            catch (HpsException exc) {
                if (exc.Message != "Transaction was rejected because it requires a batch to be open.")
                    Assert.Fail(exc.Message);
            }
        }
    }
}
