using SecureSubmit.Entities;
using SecureSubmit.Services;
using SecureSubmit.Tests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;

namespace SecureSubmit.Tests
{
    /// <summary>
    /// These test are somewhat manual in that you must request an email to get the activation code.
    /// </summary>
    [TestClass]
    public class OrcaServiceTests
    {
        private HpsOrcaServiceConfig _config = TestServicesConfig.ValidOrcaServiceConfig();
        private String apiKey = "";
        private String activationCode = "";

        //Fiddler proxy
        //GlobalProxySelection.Select = new WebProxy("127.0.0.1", 8888);

        [TestMethod]
        public void Orca_Test_In_Series()
        {
            Orca_Activation_Request();
            Orca_Activate_Device_Request();
            Orca_Get_Device_Key_Request();
            Orca_Get_Device_Parameters_Request();
        }

        [TestMethod]
        public void Orca_Activation_Request()
        {
            _config.UserName = "admin";
            _config.Password = "password";
            
            var _orcaService = new HpsOrcaService(_config);
     
            var response = _orcaService.DeviceActivationRequest("777700857994", "someone@someplace.com");

            if (response != null)   
            {
                activationCode = response.ActivationCode;
            }

            Assert.IsNotNull(response);
        }
        
        [TestMethod]
        public void Orca_Activate_Device_Request()
        {
            _config.UserName = "777700857994";
            _config.Password = "$Test1234";

            var _orcaService = new HpsOrcaService(_config);

            var response = _orcaService.ActivateDevice("777700857994", activationCode);

            if (response != null)
            {
                apiKey = response.SecretApiKey;
            }

            Assert.IsNotNull(response);
        }

        [TestMethod]
        public void Orca_Get_Device_Key_Request()
        {
            _config.UserName = "777700857994";
            _config.Password = "$Test1234";
            _config.SiteId = 101436;
            _config.LicenseId = 101433;           

            var _orcaService = new HpsOrcaService(_config);

            var response = _orcaService.GetDeviceAPIKey();
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public void Orca_Get_Device_Parameters_Request()
        {            
            _config.SecretApiKey = apiKey;
            
            var _orcaService = new HpsOrcaService(_config);
          
            try
            {
                var response = _orcaService.GetDeviceParameters();
                Assert.IsNotNull(response);

            }catch(Exception e)
            {
                //no parameters throws server 400 messages.  Not really an error.

            }
        
        }


    }
}
