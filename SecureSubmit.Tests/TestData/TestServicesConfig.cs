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
        private const string PayPlanBaseUri = "https://cert.api2.heartlandportico.com/Portico.PayPlan.v1/";

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
                ServiceUrl = CertServiceUri
            };
        }

        public static HpsOrcaServiceConfig ValidOrcaServiceConfig()
        {
            return new HpsOrcaServiceConfig
            {
                DeviceId = 5315938,
                LicenseId = 101433,
                Password = "$Test1234",
                SiteId = 101436,
                SiteTrace = "trace0001",
                UserName = "777700857994",
                VersionNumber = "1234",
                IsTest = true,
                ApplicationId = "Mobuyle Retail",
                HardwareTypeName = "Heartland Mobuyle"
            };
        }

        public static HpsPayPlanServicesConfig ValidPayplanServiceConfig()
        {
            return new HpsPayPlanServicesConfig
            {
                DeviceId = 1525384,
                LicenseId = 21081,
                Password = "$Test1234",
                SiteId = 21084,
                SiteTrace = "trace0001",
                UserName = "777700005974A",
                VersionNumber = "1234"
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
                ServiceUrl = CertServiceUri
            };
        }

        public static HpsServicesConfig ValidSecretKeyConfig()
        {
            return new HpsServicesConfig
                {
                    CredentialToken = "pkapi_cert_P6dRqs1LzfWJ6HgGVZ",
                    SecretApiKey = "skapi_cert_MYl2AQAowiQAbLp5JesGKh7QFkcizOP2jcX9BrEMqQ",
                    ServiceUrl = PayPlanBaseUri
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
                ServiceUrl = CertServiceUri
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
                ServiceUrl = CertServiceUri
            };
        }
    }
}
