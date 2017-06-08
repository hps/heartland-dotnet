using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SecureSubmit.Infrastructure.Validation
{
    public class HpsInputValidation
    {


        private const int NAME_MAX_LENGTH = 26;
        private const int ADDRESS_MAX_LENGTH = 50;
        private const int CITY_MAX_LENGTH = 20;
        private const int STATE_MAX_LENGTH = 20;

        private static readonly string[] DefaultAllowedCurrencies = { "usd" };

        public static void CheckAmount(decimal amount)
        {
            if (amount < 0)
                throw new HpsInvalidRequestException(HpsExceptionCodes.InvalidAmount, "Must be greater than or equal 0.", "amount");
        }

        public static void CheckCurrency(string currency, string[] allowedCurrencies = null)
        {
            var currencies = allowedCurrencies ?? DefaultAllowedCurrencies;

            if (string.IsNullOrEmpty(currency))
                throw new HpsInvalidRequestException(HpsExceptionCodes.MissingCurrency, "Currency can't be null.", "currency");

            foreach (var c in currencies)
            {
                if (c == currency.ToLower()) { return; }
            }

            throw new HpsInvalidRequestException(HpsExceptionCodes.InvalidCurrency, "The only supported currency is \"usd\"", "currency");
        }

        public static void CheckDateNotFuture(DateTime? date)
        {
            if (date != null && date.Value > DateTime.UtcNow)
                throw new HpsInvalidRequestException(HpsExceptionCodes.InvalidDate, "Date can't be set in the future", "date");
        }

        //.NET SDK: Sanatize Data

        public static string CheckValidEmail(string emailAddress)
        {
            try
            {
                System.Net.Mail.MailAddress validEmailAddress = new System.Net.Mail.MailAddress(emailAddress);
                if (validEmailAddress.Address != emailAddress || (emailAddress.Length > 100))
                {
                    throw new HpsInvalidRequestException(HpsExceptionCodes.InvalidEmail, "Must be a valid E-mail address and should not be more than 100 characters", "emailAddress");
                }
            }
            catch (Exception)
            {
                throw new HpsInvalidRequestException(HpsExceptionCodes.InvalidEmail, "Must be a valid E-mail address and should not be more than 100 characters", "emailAddress");
            }

            return emailAddress;
        }

        public static string CheckPhoneNumber(string phoneNumber)
        {
            Regex repeatAlphaNumeric = new Regex("[^\\d]");
            phoneNumber = repeatAlphaNumeric.Replace(phoneNumber, string.Empty);
            string matchPhoneNumberPattern = @"^[0-9]*$";
            if ((!Regex.IsMatch(phoneNumber, matchPhoneNumberPattern)) || (phoneNumber.Length > 20))
            {
                throw new HpsInvalidRequestException(HpsExceptionCodes.InvalidPhonenumber, "Must be a valid phone number and should not be more than 20 characters", "phoneNumber");
            }
            return phoneNumber;
        }

        public static string CheckZipcode(string zipCode)
        {
            Regex repeatAlphaNumeric = new Regex("[^0-9A-Za-z]");
            zipCode = repeatAlphaNumeric.Replace(zipCode, string.Empty);
            string matchZipcode = @"^[a-zA-Z0-9]*$";
            if ((!Regex.IsMatch(zipCode, matchZipcode)) || (zipCode.Length > 9))
            {
                throw new HpsInvalidRequestException(HpsExceptionCodes.InvalidZipcode, "Must be a valid zipcode and should not be more than 9 characters", "zipCode");
            }
            return zipCode;
        }

        public static string CardHolderDetails(string cardHolderDetail, AddressFields fieldName)
        {
            IDictionary<Enum, int> dict = new Dictionary<Enum, int>();
            dict.Add(AddressFields.FirstName, NAME_MAX_LENGTH);
            dict.Add(AddressFields.LastName, NAME_MAX_LENGTH);
            dict.Add(AddressFields.Address, ADDRESS_MAX_LENGTH);
            dict.Add(AddressFields.City, CITY_MAX_LENGTH);
            dict.Add(AddressFields.State, STATE_MAX_LENGTH);
            if ((!string.IsNullOrEmpty(cardHolderDetail)) && (cardHolderDetail.Length > dict[fieldName]))
            {
                throw new HpsInvalidRequestException(HpsExceptionCodes.InvalidCardHolderDetail, "The value for " + fieldName + " should not more than" + dict[fieldName] + "characters", "cardHolderDetail");
            }
            return cardHolderDetail;
        }
    }
}