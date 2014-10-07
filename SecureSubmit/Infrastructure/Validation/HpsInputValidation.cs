using System;

namespace SecureSubmit.Infrastructure.Validation
{
    class HpsInputValidation
    {
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
    }
}
