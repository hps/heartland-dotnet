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
        /// <summary>Gets or sets the credential token.</summary>
        public string CredentialToken { get; set; }

        /// <summary>Gets or sets the secret api key.</summary>
        public string SecretApiKey
        {
            get { return HpsConfiguration.SecretApiKey; }
            set { HpsConfiguration.SecretApiKey = value; }
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

        ///// <summary>Gets or sets the SOAP URI (leave null to use SDK default) .</summary>
        //public string SoapServiceUri
        //{
        //    get { return HpsConfiguration.SoapServiceUri; }
        //    set { HpsConfiguration.SoapServiceUri = value ?? HpsConfiguration.SoapServiceUri; }
        //}

        ///// <summary>Gets or sets the PayPlan base URI (leave null to use SDK default) .</summary>
        //public string PayPlanBaseUri
        //{
        //    get { return HpsConfiguration.PayPlanBaseUri; }
        //    set { HpsConfiguration.PayPlanBaseUri = value ?? HpsConfiguration.PayPlanBaseUri; }
        //}

        public string ServiceUrl { get; set; }
    }

    public abstract class HpsRestServiceConfig : HpsServicesConfig
    {
        public string UatUrl { get; set; }
        public string CertUrl { get; set; }
        public string ProdUrl { get; set; }
    }

    public class HpsPayPlanServicesConfig : HpsRestServiceConfig {
        public HpsPayPlanServicesConfig() {
           
            // Set urls
            CertUrl = "https://cert.api2.heartlandportico.com/Portico.PayPlan.v2/";
            ProdUrl = "https://api-cert.heartlandportico.com/payplan.v2/";
            UatUrl = "https://api-uat.heartlandportico.com/payplan.v2/";
            
        }

        //public string ServiceUrl {
        //    get {
        //        var components = SecretApiKey.Split('_');
        //        var env = components[1].ToLower();

        //        if (env.Equals("prod"))
        //        {
        //            return ProdUrl;
        //        }
        //        else if (env.Equals("cert"))
        //        {
        //            return CertUrl;
        //        }
        //        else
        //        {
        //            return UatUrl;
        //        }
        //    }
        //}
    }

    public class HpsOrcaServiceConfig : HpsRestServiceConfig {

        public string ApplicationId { get; set; }
        public string HardwareTypeName { get; set; }
        public string SoftwareVersion { get; set; }
        public string ConfigurationName { get; set; }
        public string PeripheralName { get; set; }
        public string PeripheralSoftware { get; set; }

        public bool IsTest { get; set; }

        public HpsOrcaServiceConfig() {
            this.CertUrl = "https://huds.test.e-hps.com/config-server/v1/";
            this.ProdUrl = "https://huds.prod.e-hps.com/config-server/v1/";
        }

        //public string ServiceUrl {
        //    get { return IsTest ? CertUrl : ProdUrl; }
        //}
    }
}