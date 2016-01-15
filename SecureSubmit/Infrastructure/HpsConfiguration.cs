using System.Configuration;

namespace SecureSubmit.Infrastructure
{
    /// <summary>The HPS configuration.</summary>
    public static class HpsConfiguration
    {
        /// <summary>The private variables used as placeholders.</summary>
        private static string _secretApiKey, _userName, _password, _developerId, _versionNumber,
            _siteTrace, _soapServiceUri, _payPlanBaseUri;

        /// <summary>The license id.</summary>
        private static int _licenseId = -1, _deviceId = -1, _siteId = -1;

        /// <summary>Gets or sets the public api key.</summary>
        internal static string SecretApiKey
        {
            get
            {
                if (string.IsNullOrEmpty(_secretApiKey))
                {
                    _secretApiKey = ConfigurationManager.AppSettings["HpsSecretAPIKey"];
                }

                return _secretApiKey;
            }
            set
            {
                _secretApiKey = value;
            }
        }

        /// <summary>Gets or sets the license id.</summary>
        internal static int LicenseId
        {
            get
            {
                if (_licenseId != -1) return _licenseId;
                
                int i;
                if (int.TryParse(ConfigurationManager.AppSettings["HpsLicenseId"], out i))
                {
                    _licenseId = i;
                }

                return _licenseId;
            }

            set
            {
                _licenseId = value;
            }
        }

        /// <summary>Gets or sets the site id.</summary>
        internal static int SiteId
        {
            get
            {
                if (_siteId != -1) return _siteId;
                
                int i;
                if (int.TryParse(ConfigurationManager.AppSettings["HpsSiteId"], out i))
                {
                    _siteId = i;
                }

                return _siteId;
            }

            set
            {
                _siteId = value;
            }
        }

        /// <summary>Gets or sets the device id.</summary>
        internal static int DeviceId
        {
            get
            {
                if (_deviceId != -1) return _deviceId;
                
                int i;
                if (int.TryParse(ConfigurationManager.AppSettings["HpsDeviceId"], out i))
                {
                    _deviceId = i;
                }

                return _deviceId;
            }

            set
            {
                _deviceId = value;
            }
        }

        /// <summary>Gets or sets the version number.</summary>
        internal static string VersionNumber
        {
            get
            {
                if (string.IsNullOrEmpty(_versionNumber))
                {
                    _versionNumber = ConfigurationManager.AppSettings["HpsVersionNumber"];
                }

                return _versionNumber;
            }

            set
            {
                _versionNumber = value;
            }
        }

        /// <summary>Gets or sets the user name.</summary>
        internal static string UserName
        {
            get
            {
                if (string.IsNullOrEmpty(_userName))
                {
                    _userName = ConfigurationManager.AppSettings["HpsUserName"];
                }

                return _userName;
            }

            set
            {
                _userName = value;
            }
        }

        /// <summary>Gets or sets the password.</summary>
        internal static string Password
        {
            get
            {
                if (string.IsNullOrEmpty(_password))
                {
                    _password = ConfigurationManager.AppSettings["HpsPassword"];
                }

                return _password;
            }

            set
            {
                _password = value;
            }
        }

        /// <summary>Gets or sets the developer ID.</summary>
        internal static string DeveloperId
        {
            get
            {
                if (string.IsNullOrEmpty(_developerId))
                {
                    _developerId = ConfigurationManager.AppSettings["HpsDeveloperId"];
                }

                return _developerId;
            }

            set
            {
                _developerId = value;
            }
        }

        /// <summary>Gets or sets the site trace.</summary>
        internal static string SiteTrace
        {
            get
            {
                if (string.IsNullOrEmpty(_siteTrace))
                {
                    _siteTrace = ConfigurationManager.AppSettings["HpsSiteTrace"];
                }

                return _siteTrace;
            }

            set
            {
                _siteTrace = value;
            }
        }

        /// <summary>Gets or sets the SOAP service URI.</summary>
        internal static string SoapServiceUri
        {
            get
            {
                // If the URI was explicitly set, use that
                if (!string.IsNullOrEmpty(_soapServiceUri)) return _soapServiceUri;

                // Check AppSettings for a URI
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["HpsSoapServiceUri"])) return ConfigurationManager.AppSettings["HpsSoapServiceUri"];

                // if there is a secret key check for cert/uat
                if (!string.IsNullOrEmpty(_secretApiKey))
                {
                    // If we have a secret key, return either the production URI...
                    if (_secretApiKey.Contains("_uat_"))
                        return "https://posgateway.uat.secureexchange.net/Hps.Exchange.PosGateway/PosGatewayService.asmx?wsdl";
                    else if (_secretApiKey.Contains("_cert_"))
                        return "https://cert.api2.heartlandportico.com/Hps.Exchange.PosGateway/PosGatewayService.asmx?wsdl";
                }

                // all else fails return the default
                return "https://api2.heartlandportico.com/Hps.Exchange.PosGateway/PosGatewayService.asmx?wsdl";
            }
            set
            {
                _soapServiceUri = value;
            }
        }

        /// <summary>Gets or sets the pay plan base URI.</summary>
        internal static string PayPlanBaseUri
        {
            get
            {
                if (!string.IsNullOrEmpty(_payPlanBaseUri)) return _payPlanBaseUri;
                
                _payPlanBaseUri = ConfigurationManager.AppSettings["HpsPayPlanBaseUri"];
                if (string.IsNullOrEmpty(_payPlanBaseUri))
                {
                    _payPlanBaseUri = "https://cert.api2.heartlandportico.com/Portico.PayPlan.v1/";
                }

                return _payPlanBaseUri;
            }
            set
            {
                _payPlanBaseUri = value;
            }
        }
    }
}
