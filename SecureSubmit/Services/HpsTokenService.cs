using Newtonsoft.Json;
using SecureSubmit.Entities;
using SecureSubmit.Serialization;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace SecureSubmit.Services
{
    public class HpsTokenService
    {
        private string PublicApiKey { get; set; }
        private string Url { get; set; }

        /// <summary>Token service creator.</summary>
        /// <param name="publicApiKey">The Public API for which you are requesting a token.</param>
        public HpsTokenService(string publicApiKey)
        {
            PublicApiKey = publicApiKey;

            if (string.IsNullOrEmpty(publicApiKey))
            {
                throw new ArgumentNullException("publicApiKey");
            }

            var components = publicApiKey.Split('_');
            if (components.Length < 3)
            {
                throw new ArgumentException(@"Public API Key must contain at least three underscores.", "publicApiKey");
            }

            var env = components[1].ToLower();

            Url = env.Equals("prod") ? "https://api.heartlandportico.com/SecureSubmit.v1/api/token" :
                "https://posgateway.cert.secureexchange.net/Hps.Exchange.PosGateway.Hpf.v1/api/token";
        }

        private HpsToken RequestToken(HpsToken inputToken)
        {
            var request = WebRequest.Create(Url);
            request.Method = "POST";
            request.ContentType = "application/json";

            var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(PublicApiKey));
            request.Headers.Add("Authorization", "Basic " + encoded);

            var payload = JsonConvert.SerializeObject(inputToken);
            var outStream = new StreamWriter(request.GetRequestStream(), Encoding.UTF8);
            outStream.Write(payload);
            outStream.Close();

            try
            {
                var inStream = new StreamReader(request.GetResponse().GetResponseStream());
                var token = new HpsToken();
                var json = JsonConvert.DeserializeAnonymousType(inStream.ReadToEnd(), token);

                inStream.Close();

                return json;
            }
            catch (WebException ex)
            {
                using (var response = ex.Response)
                {
                    using (var data = response.GetResponseStream())
                        if (data != null && data.Length != 0)
                            using (var reader = new StreamReader(data))
                            {
                                var token = new HpsToken();
                                var text = reader.ReadToEnd();
                                return JsonConvert.DeserializeAnonymousType(text, token);
                            }
                }

                throw;
            }
        }

        /// <summary>
        /// Get a token for a given credit card.
        /// </summary>
        /// <param name="cardData">An instance representing a credit card.</param>
        /// <returns>A token with expiration information.</returns>
        public HpsToken GetToken(HpsCreditCard cardData)
        {
            return RequestToken(new HpsCardToken(cardData));
        }

        public HpsToken GetSwipeToken(string track)
        {
            return RequestToken(new HpsE3SwipeToken(track));
        }

        /// <summary>
        /// Get a token for a given credit card swipe.
        /// </summary>
        /// <param name="track">Trackdata from the card reader</param>
        /// <returns>A token with expiration information.</returns>
        public HpsToken GetTrackToken(string track, string trackNumber, string ktb, string pinBlock = "")
        {
            return RequestToken(new HpsTrackDataToken(track, trackNumber, ktb, pinBlock));
        }
    }
}