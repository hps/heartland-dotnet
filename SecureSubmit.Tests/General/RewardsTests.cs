using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecureSubmit.Entities;
using SecureSubmit.Fluent.Services;
using SecureSubmit.Services;

namespace SecureSubmit.Tests.General {
    [TestClass]
    public class RewardsTests {
        [TestMethod]
        public void ChargeWithRewards() {
            var client = new HpsFluentCreditService(new HpsServicesConfig {
                SecretApiKey = "skapi_cert_MTyMAQBiHVEAewvIzXVFcmUd2UcyBge_eCpaASUp0A"
            });

            var card = new HpsCreditCard {
                Number = "4111111111111111",
                ExpMonth = 12,
                ExpYear = 2020,
                Cvv = "123"
            };

            var response = client.Charge(10m)
                .WithCard(card)
                .WithRewards("5022440000000000098")
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.IsNotNull(response.RewardsResponse);
            Assert.AreEqual("0", response.RewardsResponse.ResponseCode);
        }

        [TestMethod]
        public void ChargeWithOutRewards() {
            var client = new HpsFluentCreditService(new HpsServicesConfig {
                SecretApiKey = "skapi_cert_MTyMAQBiHVEAewvIzXVFcmUd2UcyBge_eCpaASUp0A"
            });

            var card = new HpsCreditCard {
                Number = "4111111111111111",
                ExpMonth = 12,
                ExpYear = 2020,
                Cvv = "123"
            };

            var response = client.Charge(11m)
                .WithCard(card)
                .WithAllowDuplicates(true)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
            Assert.IsNull(response.RewardsResponse);
        }
    }
}
