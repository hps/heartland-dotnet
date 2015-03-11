using System;
using System.Collections.Generic;

namespace SecureSubmit.Infrastructure.Validation
{
    class HpsIssuerResponseValidation
    {
        private static readonly Dictionary<String, HpsExceptionCodes> IssuerCodeToCreditExceptionCode;
        private static readonly Dictionary<HpsExceptionCodes, String> CreditExceptionCodeToMessage;

        static HpsIssuerResponseValidation()
        {
            List<string> declinedCodes = new List<string> { "02", "03", "04", "05", "41", "43", "44", "51", "56", "61", "62", "63", "65", "78" },
                processingErrorCodes = new List<string> { "06", "07", "12", "15", "19", "52", "53", "57", "58", "76", "77", "96", "EC" };

            IssuerCodeToCreditExceptionCode = new Dictionary<String, HpsExceptionCodes>
            {
                {"13", HpsExceptionCodes.InvalidAmount},
                {"14", HpsExceptionCodes.IncorrectNumber},
                {"54", HpsExceptionCodes.ExpiredCard},
                {"55", HpsExceptionCodes.InvalidPin},
                {"75", HpsExceptionCodes.PinRetriesExceeded},
                {"80", HpsExceptionCodes.InvalidExpiry},
                {"86", HpsExceptionCodes.PinVerification},
                {"91", HpsExceptionCodes.IssuerTimeout},
                {"EB", HpsExceptionCodes.IncorrectCvc},
                {"N7", HpsExceptionCodes.IncorrectCvc}
            };

            foreach (var code in declinedCodes)
                IssuerCodeToCreditExceptionCode.Add(code, HpsExceptionCodes.CardDeclined);

            foreach (var code in processingErrorCodes)
                IssuerCodeToCreditExceptionCode.Add(code, HpsExceptionCodes.ProcessingError);

            CreditExceptionCodeToMessage = new Dictionary<HpsExceptionCodes, string>
            {
                {HpsExceptionCodes.CardDeclined, "The card was declined"},
                {HpsExceptionCodes.ProcessingError, "An error occurred while processing the card."},
                {HpsExceptionCodes.InvalidAmount, "Must be greater than or equal 0."},
                {HpsExceptionCodes.ExpiredCard, "The card has expired."},
                {HpsExceptionCodes.InvalidPin, "The 4-digit pin is invalid."},
                {HpsExceptionCodes.PinRetriesExceeded, "Maximum number of pin retries exceeded."},
                {HpsExceptionCodes.InvalidExpiry, "Card expiration date is invalid."},
                {HpsExceptionCodes.PinVerification, "Can't verify card pin number."},
                {HpsExceptionCodes.IncorrectCvc, "The card's security code is incorrect."},
                {HpsExceptionCodes.IssuerTimeout, "The card issuer timed-out."},
                {HpsExceptionCodes.UnknownIssuerError, "An unknown issuer error has occurred."}
            };
        }

        public static void CheckResponse(int transactionId, string responseCode, string responseText)
        {
            var e = GetException(transactionId, responseCode, responseText);
            if(e != null) { throw e; }
        }

        public static HpsCreditException GetException(int transactionId, string responseCode, string responseText)
        {
            if (responseCode == "85" || responseCode == "00" || responseCode == "0") return null;

            HpsExceptionCodes code;
            string message;
            if (!IssuerCodeToCreditExceptionCode.TryGetValue(responseCode, out code))
                return new HpsCreditException(transactionId, HpsExceptionCodes.UnknownIssuerError,
                    CreditExceptionCodeToMessage[HpsExceptionCodes.UnknownIssuerError], responseCode, responseText);
            
            CreditExceptionCodeToMessage.TryGetValue(code, out message);
            return new HpsCreditException(transactionId, code, message ?? "Unknown issuer error.", responseCode, responseText);
        }
    }
}
