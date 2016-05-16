using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureSubmit.Entities
{
    public class DeviceActivationRequest
    {
        public string MerchantId { get; set; }
        public string DeviceId { get; set; }
        public string Email { get; set; }
        public string ApplicationId { get; set; }
        public string HardwareTypeName { get; set; }
        public string SoftwareVersion { get; set; }
        public string ConfigurationName { get; set; }
        public string PeripheralName { get; set; }
        public string PeripheralSoftware { get; set; }
    }

    public class DeviceActivationResponse : DeviceActivationRequest
    {
        public string ActivationCode { get; set; }
    }

    public class DeviceActivationKeyResponse
    {
        public string MerchantId { get; set; }
        public string DeviceId { get; set; }
        public string ApplicationId { get; set; }
        public string ActivationCode { get; set; }
        public string SecretApiKey { get; set; }
    }

    public class DeviceParametersResponse
    {
        public string Parameters { get; set; }
        public string DeviceId { get; set; }
        public string ApplicationId { get; set; }
        public string HardwareTypeName { get; set; }
    }

    public class DeviceApiKeyResponse
    {
        public string SiteId { get; set; }
        public string DeviceId { get; set; }
        public string SecretApiKey { get; set; }
    }
}
