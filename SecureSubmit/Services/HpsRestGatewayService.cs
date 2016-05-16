using System;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SecureSubmit.Abstractions;
using SecureSubmit.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace SecureSubmit.Services
{
    public abstract class HpsRestGatewayService
    {   
        private readonly IHpsServicesConfig _servicesConfig;

        protected HpsRestGatewayService(IHpsServicesConfig config)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            if (config == null) return;
            _servicesConfig = config;
        }

        /// <summary>
        /// For Restful service endpoints using JSON
        /// </summary>
        /// <param name="verb"></param>
        /// <param name="endpoint"></param>
        /// <param name="data"></param>
        /// <param name="additionalHeaders"></param>
        /// <param name="queryStringParameters"></param>
        /// <returns></returns>
        protected string DoRequest(string verb, string endpoint, object data = null, Dictionary<string, string> additionalHeaders = null, Dictionary<string, string> queryStringParameters = null)
        {
            string url = _servicesConfig.ServiceUrl + endpoint;
            try
            {
                var method = verb.ToUpper();
                var queryString = "";

                //Query string
                if (queryStringParameters != null)
                {
                    //Load query string parameters
                    queryString = String.Format("?{0}",
                        String.Join("&", queryStringParameters.Select(kvp => String.Format("{0}={1}", Uri.EscapeDataString(kvp.Key), Uri.EscapeDataString(kvp.Value)))));
                }

                var uri = String.Format("{0}{1}", url, queryString);

                var request = (HttpWebRequest)WebRequest.Create(uri);
                request.ContentType = "application/json";                

                //Headers
                if (additionalHeaders != null)
                {
                    foreach (var item in additionalHeaders)
                    {
                        request.Headers.Add(item.Key, item.Value);
                    }
                }               
               
                //Payload
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
