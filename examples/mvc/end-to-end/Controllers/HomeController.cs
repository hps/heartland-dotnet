using System;
using System.Text.RegularExpressions;
using System.Web.Helpers;
using System.Web.Mvc;
using SecureSubmit.Entities;
using SecureSubmit.Infrastructure;
using SecureSubmit.Services;
using SecureSubmit.Services.Credit;

namespace end_to_end.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ProcessPayment(OrderDetails details)
        {
            var config = new HpsServicesConfig
            {
                SecretApiKey = "skapi_cert_MYl2AQAowiQAbLp5JesGKh7QFkcizOP2jcX9BrEMqQ",
                // The following variables will be provided to you during certification
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

                return View("Success", new SuccessModel {
                    FirstName = details.FirstName,
                    TransactionId = authResponse.TransactionId
                });
            }
            catch (HpsInvalidRequestException e)
            {
                // handle error for amount less than zero dollars
                return View("Error", model: "amount less than zero dollars: " + e.Message);
            }
            catch (HpsAuthenticationException e)
            {
                // handle errors related to your HpsServiceConfig
                return View("Error", model: "Bad Config: " + e.Message);
            }
            catch (HpsCreditException e)
            {
                // handle card-related exceptions: card declined, processing error, etc
                return View("Error", model: "card declined, processing error, etc: " + e.Message);
            }
            catch (HpsGatewayException e)
            {
                // handle gateway-related exceptions: invalid cc number, gateway-timeout, etc
                return View("Error", model: "invalid cc number, gateway-timeout, etc: " + e.Message);
            }            
        }

        public void SendEmail()
        {
            // This information would need to be replaced with your own
            // or call your own email sending methods
            try
            {
                WebMail.SmtpServer = "my.smtpserver.com";
                WebMail.SmtpPort = 123;
                WebMail.EnableSsl = false;
                WebMail.UserName = "username";
                WebMail.Password = "password";
                WebMail.From = "email@mail.com";
                WebMail.SmtpUseDefaultCredentials = false;

                WebMail.Send(
                    to: "anotheremail@mail.com",
                    subject: "SecureSubmit Payment",
                    body: "Congratulations, you have just completed a SecureSubmit payment!"
                    );
            }
            catch (Exception e)
            {
                Response.Write("<strong>Couldn't Send Email</strong>");
            }
        }        
    }

    public class OrderDetails
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

    public class ChargeResponse
    {
        public bool HasError { get; set; }
        public int TransactionId { get; set; }
    }

    public class SuccessModel
    {
        public string FirstName { get; set; }
        public int TransactionId { get; set; }
    }
}