using System;
using System.Collections.Generic;

namespace SecureSubmit.Infrastructure.Validation
{
    class HpsIssuerResponseValidation
    {
        private static readonly Dictionary<String, HpsExceptionCodes> IssuerCodeToCreditExceptionCode;
        private static readonly Dictionary<HpsExceptionCodes, String> CreditExceptionCodeToMessage;
        private static readonly Dictionary<String, HpsExceptionCodes> IssuerCodeToGiftExceptionCode;

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


            IssuerCodeToGiftExceptionCode = new Dictionary<String, HpsExceptionCodes>
            {
                { "1", HpsExceptionCodes.UnknownGiftError},
                {"2", HpsExceptionCodes.UnknownGiftError},
                {"11",HpsExceptionCodes.UnknownGiftError},
                {"13",HpsExceptionCodes.PartialApproval},
                {"14",HpsExceptionCodes.InvalidPin},
                {"3", HpsExceptionCodes.InvalidCardData},
                {"8", HpsExceptionCodes.InvalidCardData},
                {"4", HpsExceptionCodes.ExpiredCard},
                {"5", HpsExceptionCodes.CardDeclined},
                {"12",HpsExceptionCodes.CardDeclined},
                {"6", HpsExceptionCodes.ProcessingError},
                {"7", HpsExceptionCodes.ProcessingError},
                {"9", HpsExceptionCodes.InvalidAmount},
                {"10",HpsExceptionCodes.ProcessingError}
            };
            foreach (var code in declinedCodes)
                IssuerCodeToCreditExceptionCode.Add(code, HpsExceptionCodes.CardDeclined);

            foreach (var code in processingErrorCodes)
                IssuerCodeToCreditExceptionCode.Add(code, HpsExceptionCodes.ProcessingError);

            CreditExceptionCodeToMessage = new Dictionary<HpsExceptionCodes, string>
            {
                {HpsExceptionCodes.CardDeclined, "The card was declined"},
                {HpsExceptionCodes.ProcessingError, "An error occurred while processing the card."},
                {HpsExceptionCodes.InvalidAmount, "Must be greater than or equal to 0."},
                {HpsExceptionCodes.ExpiredCard, "The card has expired."},
                {HpsExceptionCodes.InvalidPin, "The 4-digit pin is invalid."},
                {HpsExceptionCodes.PinRetriesExceeded, "Maximum number of pin retries exceeded."},
                {HpsExceptionCodes.InvalidExpiry, "Card expiration date is invalid."},
                {HpsExceptionCodes.PinVerification, "Can't verify card pin number."},
                {HpsExceptionCodes.IncorrectCvc, "The card's security code is incorrect."},
                {HpsExceptionCodes.IssuerTimeout, "The card issuer timed-out."},
                {HpsExceptionCodes.UnknownIssuerError, "An unknown issuer error has occurred."},
                {HpsExceptionCodes.UnknownGiftError, "An unknown gift error has occurred."},
                {HpsExceptionCodes.PartialApproval, "The amount was partially approved."},
                {HpsExceptionCodes.InvalidCardData, "The card data is invalid."}
            };
        }

        public static void CheckResponse(long transactionId, string responseCode, string responseText, HpsCardType type = HpsCardType.Credit)
        {
            var e = GetException(transactionId, responseCode, responseText, type);
            if (e != null) { throw e; }
        }

        public static HpsCreditException GetException(long transactionId, string responseCode, string responseText, HpsCardType type = HpsCardType.Credit)
        {
            if (responseCode == "00" || responseCode == "0") return null;

            HpsExceptionCodes code;
            string message;
            switch (type)
            {
                case HpsCardType.Credit:
                    if (responseCode == "85" || responseCode == "10") return null;
                    if (IssuerCodeToCreditExceptionCode.TryGetValue(responseCode, out code))
                    {
                        CreditExceptionCodeToMessage.TryGetValue(code, out message);
                        return new HpsCreditException(transactionId, code, message ?? "Unknown issuer error.", responseCode, responseText);
                    }
                    break;
                case HpsCardType.Gift:
                    if (responseCode == "13") return null;
                    if (IssuerCodeToGiftExceptionCode.TryGetValue(responseCode, out code))
                    {
                        CreditExceptionCodeToMessage.TryGetValue(code, out message);
                        return new HpsCreditException(transactionId, code, message ?? "Unknown issuer error.", responseCode, responseText);
                    }
                    break;
            }
            return new HpsCreditException(transactionId, HpsExceptionCodes.UnknownIssuerError, CreditExceptionCodeToMessage[HpsExceptionCodes.UnknownIssuerError], responseCode, responseText);

        }
    }
}
