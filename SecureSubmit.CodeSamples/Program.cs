using SecureSubmit.Entities;
using SecureSubmit.Infrastructure;
using SecureSubmit.Services;
using SecureSubmit.Services.Credit;
using System;

namespace SecureSubmit.CodeSamples
{
    class Program
    {
        static void Main(string[] args) { }

        public static void CreateCardHolder()
        {
            var cardHolder = new HpsCardHolder
            {
                Email = "test@test.com",
                FirstName = "First",
                LastName = "Last",
                Phone = "555-555-5555",
                Address = new HpsAddress {Zip = "47130"} // Zip is the only required address field.
            };
        }

        public static void ChargeCard()
        {
            var chargeService = new HpsCreditService(
                new HpsServicesConfig {SecretApiKey = "<your secret api key goes here>"});

            var creditCard = new HpsCreditCard // Valid Visa
            {
                Cvv = "123",
                ExpMonth = 12,
                ExpYear = 2015,
                Number = "4012002000060016"
            };

            var cardHolder = new HpsCardHolder
            {
                Email = "test@test.com",
                FirstName = "First",
                LastName = "Last",
                Phone = "555-555-5555",
                Address = new HpsAddress {Zip = "47130"} // Zip is the only required address field.
            };

            chargeService.Charge(10, "usd", creditCard, cardHolder);
        }

        public static void AuthorizeCard()
        {
            var chargeService = new HpsCreditService(
                new HpsServicesConfig {SecretApiKey = "<your secret api key goes here>"});

            var creditCard = new HpsCreditCard // Valid Visa
            {
                Cvv = "123",
                ExpMonth = 12,
                ExpYear = 2015,
                Number = "4012002000060016"
            };

            var cardHolder = new HpsCardHolder
            {
                Email = "test@test.com",
                FirstName = "First",
                LastName = "Last",
                Phone = "555-555-5555",
                Address = new HpsAddress {Zip = "47130"} // Zip is the only required address field.
            };

            chargeService.Authorize(10, "usd", creditCard, cardHolder);
        }

        public static void CaptureCard()
        {
            var chargeService = new HpsCreditService(
                new HpsServicesConfig { SecretApiKey = "<your secret api key goes here>" });

            var creditCard = new HpsCreditCard                  // Valid Visa
            {
                Cvv = "123",
                ExpMonth = 12,
                ExpYear = 2015,
                Number = "4012002000060016"
            };

            var cardHolder = new HpsCardHolder
            {
                Email = "test@test.com",
                FirstName = "First",
                LastName = "Last",
                Phone = "555-555-5555",
                Address = new HpsAddress { Zip = "47130" }    // Zip is the only required address field.
            };

            var authResponse = chargeService.Authorize(10, "usd", creditCard, cardHolder);

            chargeService.Capture(authResponse.TransactionId);
        }

        public static void HandleErrors()
        {
            var chargeService = new HpsCreditService(
                new HpsServicesConfig {SecretApiKey = "<your secret api key goes here>"});

            var creditCard = new HpsCreditCard // Valid Visa
            {
                Cvv = "123",
                ExpMonth = 12,
                ExpYear = 2015,
                Number = "4012002000060016"
            };

            var cardHolder = new HpsCardHolder
            {
                Email = "test@test.com",
                FirstName = "First",
                LastName = "Last",
                Phone = "555-555-5555",
                Address = new HpsAddress {Zip = "47130"} // Zip is the only required address field.
            };

            try
            {
                chargeService.Charge(-5, "usd", creditCard, cardHolder);
            }
            catch (HpsInvalidRequestException e)
            {
                // handle error for amount less than zero dollars
                Console.WriteLine(e);
            }
            catch (HpsAuthenticationException e)
            {
                // handle errors related to your HpsServiceConfig
                Console.WriteLine(e);
            }
            catch (HpsCreditException e)
            {
                // handle card-related exceptions: card declined, processing error, etc
                Console.WriteLine(e);
            }
        }
    }
}
