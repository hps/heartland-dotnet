using SecureSubmit.Entities;

namespace SecureSubmit.Infrastructure
{
    public enum HpsExceptionCodes
    {
        /* General Codes */
        AuthenticationError,
        InvalidConfiguration,
        InvalidArgument,

        /* Input Codes */
        InvalidAmount,
        MissingCurrency,
        InvalidCurrency,
        InvalidDate,
        InvalidCardHolderDetail,
        InvalidEmail,
        InvalidPhonenumber,
        InvalidZipcode,

        /* Gateway Codes */
        UnknownGatewayError,
        InvalidOriginalTransaction,
        NoOpenBatch,
        InvalidCpcData,
        InvalidCardData,
        InvalidNumber,
        GatewayTimeout,
        UnexpectedGatewayResponse,
        GatewayTimeoutReversalError,

        /* Credit Issuer Codes */
        IncorrectNumber,
        ExpiredCard,
        InvalidPin,
        PinRetriesExceeded,
        InvalidExpiry,
        PinVerification,
        IssuerTimeout,
        IncorrectCvc,
        CardDeclined,
        ProcessingError,
        IssuerTimeoutReversalError,
        UnknownIssuerError,

        /* Gift Issuer Codes */
        CardAlreadyActivated,

        MissingCheckName
    }

    public enum HpsTransactionType
    {
        Authorize,
        Capture,
        Charge,
        Refund,
        Reverse,
        Verify,
        List,
        Get,
        Void,
        SecurityError,
        BatchClose
    }

    public enum HpsGiftCardAliasAction
    {
        Delete,
        Add,
        Create
    }

    public enum HpsTrackDataMethod
    {
        Swipe,
        Proximity
    }

    public abstract class HpsSECCode
    {
        public const string PPD = "PPD";
        public const string CCD = "CCD";
        public const string POP = "POP";
        public const string WEB = "WEB";
        public const string TEL = "TEL";
        public const string EBRONZE = "EBRONZE";
    }

    public enum AddressFields
    {
        FirstName,
        LastName,
        Address,
        City,
        State
    }

}
