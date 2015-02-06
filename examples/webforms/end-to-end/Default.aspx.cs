using System;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web.UI;
using SecureSubmit.Entities;
using SecureSubmit.Infrastructure;
using SecureSubmit.Services;
using SecureSubmit.Services.Credit;

namespace end_to_end
{
    public partial class Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Context.Request.QueryString.HasKeys())
                ProcessPayment();
        }

        private void ProcessPayment()
        {
            var details = GetOrderDetails();

            var config = new HpsServicesConfig
            {
                // The following variables will be provided to you during certification
                SecretApiKey = "skapi_cert_MYl2AQAowiQAbLp5JesGKh7QFkcizOP2jcX9BrEMqQ",                
                VersionNumber = "0000",
                DeveloperId = "000000"
            };

            var chargeService = new HpsCreditService(config);

            var numbers = new Regex("^[0-9]+$");

            var address = new HpsAddress
            {
                Address = details.Address,
                City = details.City,
                State = details.State,
                Country = "United States",
                Zip = numbers.Match(details.Zip ?? string.Empty).ToString()
            };

            var validCardHolder = new HpsCardHolder
            {
                FirstName = details.FirstName,
                LastName = details.LastName,
                Address = address,
                Phone = numbers.Match(details.PhoneNumber ?? string.Empty).ToString()
            };

            var suToken = new HpsTokenData
            {
                TokenValue = details.Token_value
            };

            try
            {
                var authResponse = chargeService.Charge(15.15m, "usd", suToken.TokenValue, validCardHolder);

                SendEmail();

                Response.Write("<h1>Success!</h1><p>Thank you, " + 
                               details.FirstName + 
                               ", for your order of $15.15.</p>" +
                               "Transaction Id: " + authResponse.TransactionId);                
            }
            catch (HpsInvalidRequestException e)
            {
                // handle error for amount less than zero dollars
                Response.Write("<h3>Error</h3>" +  "<strong>amount less than zero dollars: " + e.Message + "</strong>");
            }
            catch (HpsAuthenticationException e)
            {
                // handle errors related to your HpsServiceConfig
                Response.Write("<h3>Error</h3>" + "<strong>Bad Config: " + e.Message + "</strong>");
            }
            catch (HpsCreditException e)
            {
                // handle card-related exceptions: card declined, processing error, etc
                Response.Write("<h3>Error</h3>" + "<strong>card declined, processing error, etc: " + e.Message + "</strong>");
            }
            catch (HpsGatewayException e)
            {
                // handle gateway-related exceptions: invalid cc number, gateway-timeout, etc
                Response.Write("<h3>Error</h3>" + "<strong>invalid cc number, gateway-timeout, etc: " + e.Message + "</strong>");
            }            
        }

        public void SendEmail()
        {
            // This information would need to be replaced with your own
            // or call your own email sending methods

            try
            {
                var client = new SmtpClient {
                    Host = "my.smtpserver.com",
                    Port = 123,
                    EnableSsl = false, 
                    Credentials = new NetworkCredential("username", "password"),
                    UseDefaultCredentials = false,                    
                };

                client.Send(
                    "email@mail.com", 
                    "anotheremail@mail.com",
                    "SecureSubmit Payment",
                    "Congratulations, you have just completed a SecureSubmit payment!"
                );
            }
            catch (Exception)
            {
                Response.Write("<strong>Couldn't Send Email</strong>");
            }
        }        

        private OrderDetails GetOrderDetails()
        {
            var query = Context.Request.QueryString;

            return new OrderDetails {
                FirstName = query["FirstName"],
                LastName = query["LastName"],
                PhoneNumber = query["PhoneNumber"],
                Email = query["Email"],
                Address = query["Address"],
                City = query["City"],
                Zip = query["Zip"],
                Token_value = query["token_value"],
                Card_type = query["card_type"],
                Exp_month = query["exp_month"],
                Exp_year = query["exp_year"],
                Last_four = query["last_four"],
            };
        }
    }

    internal class OrderDetails
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Token_value { get; set; }
        public string Card_type { get; set; }
        public string Last_four { get; set; }
        public string Exp_month { get; set; }
        public string Exp_year { get; set; }
    }
}