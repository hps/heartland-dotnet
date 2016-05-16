using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace SecureSubmit.Services
{
    public class HpsOrcaService : HpsRestGatewayService
    {
        private string _baseUrl { get; set; }
        private HpsOrcaServiceConfig _config { get; set; }
         
        public HpsOrcaService(HpsOrcaServiceConfig config)
            : base(config)
        {
            _config = config;
            _config.ServiceUrl = _config.IsTest ? _config.CertUrl : _config.ProdUrl;
            
        }

        /// <summary>
        /// Start the request that sends the email.
        /// </summary>
        /// <param name="merchantId"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public DeviceActivationResponse DeviceActivationRequest(string merchantId, string email)
        {
            if (_config.DeviceId <= 0)
                throw new HpsInvalidRequestException(HpsExceptionCodes.InvalidArgument,
                    "DeviceId is required.", "deviceId");

            if (string.IsNullOrEmpty(merchantId))
                throw new HpsInvalidRequestException(HpsExceptionCodes.InvalidArgument,
                    "MerchantId is required.", "merchantId");

            if (string.IsNullOrEmpty(email))
                throw new HpsInvalidRequestException(HpsExceptionCodes.InvalidArgument,
                    "Email is required.", "email");

            if (string.IsNullOrEmpty(_config.ApplicationId))
                throw new HpsInvalidRequestException(HpsExceptionCodes.InvalidArgument,
                    "ApplicationId is required.", "ApplicationId");

            Dictionary<string, string> headers = new Dictionary<string, string>();
            string usernamepair = String.Format("{0}:{1}", _config.UserName, _config.Password);
            var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(usernamepair));
            headers.Add("Authorization", "Basic " + encoded);

            Dictionary<string, string> payload = new Dictionary<string, string>();
            payload.Add("deviceId", _config.DeviceId.ToString());
            payload.Add("merchantId", merchantId);
            payload.Add("applicationId", _config.ApplicationId);
            payload.Add("hardwareTypeName", _config.HardwareTypeName);
            payload.Add("softwareVersion", _config.SoftwareVersion);
            payload.Add("configurationName", _config.ConfigurationName);
            payload.Add("peripheralName", _config.PeripheralName);
            payload.Add("peripheralSoftware", _config.PeripheralSoftware);
            payload.Add("email", email);
            

            var response = DoRequest("POST", "deviceActivation", payload, headers);
            return HydrateObject<DeviceActivationResponse>(response);
        }

        /// <summary>
        /// Active using the activation code assigned to the device data.
        /// </summary>
        /// <param name="merchantId"></param>
        /// <param name="activationCode"></param>
        /// <returns></returns>
        public DeviceActivationKeyResponse ActivateDevice(string merchantId, string activationCode)
        {
            if (string.IsNullOrEmpty(merchantId))
                throw new HpsInvalidRequestException(HpsExceptionCodes.InvalidArgument,
                    "MerchantId is required.", "merchantId");

            if (string.IsNullOrEmpty(activationCode))
                throw new HpsInvalidRequestException(HpsExceptionCodes.InvalidArgument,
                    "ActivationCode is required.", "activationCode");

            if (string.IsNullOrEmpty(_config.ApplicationId))
                throw new HpsInvalidRequestException(HpsExceptionCodes.InvalidArgument,
                    "ApplicationId is required.", "ApplicationId");

            Dictionary<string, string> qs = new Dictionary<string, string>();
            qs.Add("merchantId", merchantId);
            qs.Add("applicationId", _config.ApplicationId);
            qs.Add("activationCode", activationCode);
          
            var response = DoRequest("GET", "deviceActivationKey", null, null, qs);
            return HydrateObject<DeviceActivationKeyResponse>(response);

        }

        /// <summary>
        /// Get the API key.
        /// </summary>
        /// <returns></returns>
        public string GetDeviceAPIKey()
        {
            if (_config.DeviceId <= 0)
                throw new HpsInvalidRequestException(HpsExceptionCodes.InvalidArgument,
                    "DeviceId is required.", "deviceId");

            if (_config.SiteId <= 0)
                throw new HpsInvalidRequestException(HpsExceptionCodes.InvalidArgument,
                    "SiteId is required.", "SiteId");

            if (_config.LicenseId <= 0)
                throw new HpsInvalidRequestException(HpsExceptionCodes.InvalidArgument,
                    "LicenseId is required.", "LicenseId");

            Dictionary<string, string> headers = new Dictionary<string, string>();
            string usernamepair = String.Format("{0}:{1}", _config.UserName, _config.Password);
            var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(usernamepair));
            headers.Add("Authentication", "Basic " + encoded);
            headers.Add("siteId", _config.SiteId.ToString());
            headers.Add("licenseId", _config.LicenseId.ToString());
            headers.Add("deviceId", _config.DeviceId.ToString());             

            var response = DoRequest("POST", "deviceApiKey", null, headers);
            return response;

        }
        
        /// <summary>
        /// Get Parameters back as a JSON String since they are all custom.
        /// </summary>
        /// <returns></returns>
        public string GetDeviceParameters()
        {
            if (_config.DeviceId <= 0)
                throw new HpsInvalidRequestException(HpsExceptionCodes.InvalidArgument,
                    "DeviceId is required.", "deviceId");

            if (string.IsNullOrEmpty(_config.ApplicationId))
                throw new HpsInvalidRequestException(HpsExceptionCodes.InvalidArgument,
                    "ApplicationId is required.", "ApplicationId");            

            Dictionary<string, string> headers = new Dictionary<string, string>();
            string usernamepair = String.Format("{0}:", _config.SecretApiKey);
            var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(usernamepair), Base64FormattingOptions.None);
            headers.Add("Authentication", "Basic " + encoded);

            Dictionary<string, string> qs = new Dictionary<string, string>();
            qs.Add("applicationId", _config.ApplicationId);
            qs.Add("hardwareTypeName", _config.HardwareTypeName);
            qs.Add("deviceId", _config.DeviceId.ToString());

            var response = DoRequest("GET", "deviceParameters", null, headers, qs);
            return response;

        }
    }
}
