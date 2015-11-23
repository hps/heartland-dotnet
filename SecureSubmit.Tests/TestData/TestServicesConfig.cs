// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestServicesConfig.cs" company="Heartland Payment Systems">
//   Copyright (c) Heartland Payment Systems. All rights reserved.
// </copyright>
// <summary>
//   The services config.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SecureSubmit.Tests.TestData
{
    using SecureSubmit.Services;

    /// <summary>The services config.</summary>
    public static class TestServicesConfig
    {
        private const string UatServiceUri = "https://posgateway.uat.secureexchange.net/Hps.Exchange.PosGateway/PosGatewayService.asmx";
        private const string CertServiceUri = "https://cert.api2.heartlandportico.com/Hps.Exchange.PosGateway/PosGatewayService.asmx";
        private const string PayPlanBaseUri = "https://posgateway.cert.secureexchange.net/Portico.PayPlan.v1/";

        /// <summary>A valid HPS services config.</summary>
        /// <returns>The <see cref="HpsServicesConfig"/>.</returns>
        public static HpsServicesConfig ValidServicesConfig()
        {
            return new HpsServicesConfig
            {
                DeviceId = 1519321,
                LicenseId = 20855,
                Password = "$Test1234",
                SiteId = 20856,
                SiteTrace = "trace0001",
                UserName = "777700004035",
                SoapServiceUri = CertServiceUri,
                PayPlanBaseUri = PayPlanBaseUri
            };
        }

        public static HpsServicesConfig ValidTokenServiceConfig()
        {
            return new HpsServicesConfig
            {
                DeviceId = 1525384,
                LicenseId = 21081,
                Password = "$Test1234",
                SiteId = 21084,
                SiteTrace = "trace0001",
                UserName = "777700005974A",
                VersionNumber = "1234",
                SoapServiceUri = UatServiceUri,
                PayPlanBaseUri = PayPlanBaseUri
            };
        }

        public static HpsServicesConfig ValidSecretKeyConfig()
        {
            return new HpsServicesConfig
                {
                    CredentialToken = "pkapi_cert_P6dRqs1LzfWJ6HgGVZ",
                    SecretApiKey = "skapi_cert_MYl2AQAowiQAbLp5JesGKh7QFkcizOP2jcX9BrEMqQ",
                    PayPlanBaseUri = PayPlanBaseUri
                };
        }

        /// <summary>An invalid HPS services config.</summary>
        /// <returns>The <see cref="HpsServicesConfig"/>.</returns>
        public static HpsServicesConfig BadLicenseId()
        {
            return new HpsServicesConfig
            {
                DeviceId = 1519321,
                LicenseId = 11111,
                Password = "$Test1234",
                SiteId = 20856,
                SiteTrace = "trace0001",
                UserName = "777700004035",
                SoapServiceUri = UatServiceUri,
                PayPlanBaseUri = PayPlanBaseUri
            };
        }

        /// <summary>HPS services config. for SDK certification.</summary>
        /// <returns>The <see cref="HpsServicesConfig"/>.</returns>
        public static HpsServicesConfig CertServicesConfig()
        {
            return new HpsServicesConfig
            {
                DeviceId = 1522326,
                LicenseId = 20994,
                Password = "$Test1234",
                SiteId = 20995,
                SiteTrace = "trace0001",
                UserName = "777700005412",
                DeveloperId = "123456",
                VersionNumber = "1234"
            };
        }

        /// <summary>HPS services config. for SDK certification including TxnDescriptor.</summary>
        /// <returns>The <see cref="HpsServicesConfig"/>.</returns>
        public static HpsServicesConfig CertServicesConfigWithDescriptor()
        {
            return new HpsServicesConfig
            {
                DeviceId = 1520053,
                LicenseId = 20903,
                Password = "$Test1234",
                SiteId = 20904,
                SiteTrace = "trace0001",
                UserName = "777700004597",
                DeveloperId = "123456",
                VersionNumber = "1234",
                SoapServiceUri = CertServiceUri,
                PayPlanBaseUri = PayPlanBaseUri
            };
        }
    }
}
