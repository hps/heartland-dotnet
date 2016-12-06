
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HpsServicesConfig.cs" company="Heartland Payment Systems">
//   Copyright (c) Heartland Payment Systems. All rights reserved.
// </copyright>
// <summary>
//   The HPS services config (best to use AppSettings).
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SecureSubmit.Services
{
    using Abstractions;
    using Infrastructure;
    using System;

    /// <summary>The HPS services config (best to use AppSettings).</summary>
    public class HpsServicesConfig : IHpsServicesConfig
    {
        private string secretApiKey;
        /// <summary>Gets or sets the credential token.</summary>
        public string CredentialToken { get; set; }

        /// <summary>Gets or sets the secret api key.</summary>
        public string SecretApiKey
        {
            get
            {
                return secretApiKey;
            }
            set
            {
                secretApiKey = value.Trim();
            }
        }

        /// <summary>Gets or sets the license id.</summary>
        public int LicenseId { get; set; }

        /// <summary>Gets or sets the site id.</summary>
        public int SiteId { get; set; }

        /// <summary>Gets or sets the device id.</summary>
        public int DeviceId { get; set; }

        /// <summary>Gets or sets the version number.</summary>
        public string VersionNumber { get; set; }

        /// <summary>Gets or sets the user name.</summary>
        public string UserName { get; set; }

        /// <summary>Gets or sets the password.</summary>
        public string Password { get; set; }

        /// <summary>Gets or sets the developer ID.</summary>
        public string DeveloperId { get; set; }

        /// <summary>Gets or sets the site trace.</summary>
        public string SiteTrace { get; set; }

        private string _serviceUrl;
        public virtual string ServiceUrl
        {
            get
            {
                // If the URI was explicitly set, use that
                if (!string.IsNullOrEmpty(_serviceUrl)) return _serviceUrl;

                // if there is a secret key check for cert/uat
                if (!string.IsNullOrEmpty(SecretApiKey))
                {
                    // If we have a secret key, return either the production URI...
                    if (SecretApiKey.Contains("_uat_"))
                        return "https://posgateway.uat.secureexchange.net/Hps.Exchange.PosGateway/PosGatewayService.asmx?wsdl";
                    else if (SecretApiKey.Contains("_cert_"))
                        return "https://cert.api2.heartlandportico.com/Hps.Exchange.PosGateway/PosGatewayService.asmx?wsdl";
                }

                // all else fails return the default
                return "https://api2.heartlandportico.com/Hps.Exchange.PosGateway/PosGatewayService.asmx?wsdl";
            }
            set
            {
                _serviceUrl = value;
            }
        }
    }

    public abstract class HpsRestServiceConfig : HpsServicesConfig
    {
        public string UatUrl { get; set; }
        public string CertUrl { get; set; }
        public string ProdUrl { get; set; }
    }

    public class HpsPayPlanServicesConfig : HpsRestServiceConfig
    {
        public HpsPayPlanServicesConfig()
        {

            // Set urls
            CertUrl = "https://cert.api2.heartlandportico.com/Portico.PayPlan.v2/";
            ProdUrl = "https://api2.heartlandportico.com/payplan.v2/";
            UatUrl = "https://api-uat.heartlandportico.com/payplan.v2/";

        }

        public override string ServiceUrl
        {
            get
            {
                var components = SecretApiKey.Split('_');
                var env = components[1].ToLower();

                if (env.Equals("prod"))
                {
                    return ProdUrl;
                }
                else if (env.Equals("cert"))
                {
                    return CertUrl;
                }
                else
                {
                    return UatUrl;
                }
            }
        }
    }

    public class HpsActivationServiceConfig : HpsRestServiceConfig
    {
        public bool IsTest { get; set; }
        public HpsActivationServiceConfig()
        {
            this.CertUrl = "https://huds.test.e-hps.com/config-server/v1/";
            this.ProdUrl = "https://huds.prod.e-hps.com/config-server/v1/";
        }
    }
}