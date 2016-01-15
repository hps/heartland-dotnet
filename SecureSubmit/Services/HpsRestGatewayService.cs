using System;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SecureSubmit.Abstractions;
using SecureSubmit.Infrastructure;

namespace SecureSubmit.Services
{
    public abstract class HpsRestGatewayService
    {
        private const string CertUrl = "https://cert.api2.heartlandportico.com/Portico.PayPlan.v2/";
        private const string ProdUrl = "https://api-cert.heartlandportico.com/payplan.v2/";
        private const string UatUrl = "https://api-uat.heartlandportico.com/payplan.v2/";

        int _limit = -1;
        int _offset = -1;

        private readonly IHpsServicesConfig _servicesConfig;
        private readonly string _url;

        public void SetPagination(int limit, int offset)
        {
            _limit = limit;
            _offset = offset;
        }

        private void ResetPagination()
        {
            _limit = -1;
            _offset = -1;
        }

        protected HpsRestGatewayService(IHpsServicesConfig config)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            if (config == null) return;
            _servicesConfig = config;

            var components = config.SecretApiKey.Split('_');
            var env = components[1].ToLower();

            if (env.Equals("prod"))
            {
                _url = ProdUrl;
            }
            else if (env.Equals("cert"))
            {
                _url = CertUrl;
            }
            else
            {
                _url = UatUrl;
            }
        }

        protected string DoRequest(string verb, string endpoint, object data = null)
        {
            try
            {
                var queryString = "";
                if (_limit != -1 && _offset != -1)
                {
                    queryString += "?limit=" + _limit;
                    queryString += "&offset=" + _offset;
                }

                var uri = _url + endpoint + queryString;
                var keyBytes = Encoding.UTF8.GetBytes(_servicesConfig.SecretApiKey);
                var encoded = Convert.ToBase64String(keyBytes);
                var auth = String.Format("Basic {0}", encoded);

                var request = (HttpWebRequest)WebRequest.Create(uri);
                request.ContentType = "application/json";
                request.Headers.Add("Authorization", auth);

                var method = verb.ToUpper();
                if (method != "GET")
                {
                    request.Method = method;

                    if (data != null)
                    {
                        var payload = JsonConvert.SerializeObject(data, Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore,
                                DefaultValueHandling = DefaultValueHandling.Ignore,
                                ContractResolver = new CamelCasePropertyNamesContractResolver()
                            });

                        var payloadBytes = Encoding.UTF8.GetBytes(payload);
                        request.ContentLength = payloadBytes.Length;

                        using (var stream = request.GetRequestStream())
                            stream.Write(payloadBytes, 0, payloadBytes.Length);
                    }
                }

                var response = (HttpWebResponse)request.GetResponse();
                var responseStream = response.GetResponseStream();

                ResetPagination();

                return responseStream != null ? new StreamReader(responseStream).ReadToEnd() : "";
            }
            catch (WebException e)
            {
                var response = e.Response as HttpWebResponse;
                if (response != null && response.StatusCode == HttpStatusCode.BadRequest)
                {
                    String errorMessage = string.Empty;
                    try
                    {
                        Stream errorStream = response.GetResponseStream();
                        errorMessage = new StreamReader(errorStream).ReadToEnd();
                    }
                    catch { errorMessage = e.Message; }
                    throw new HpsException(errorMessage, e);
                }
                else { throw new HpsException(e.Message, e); }
            }
        }

        protected T HydrateObject<T>(string data)
        {
            return JsonConvert.DeserializeObject<T>(data);
        }
    }
}
