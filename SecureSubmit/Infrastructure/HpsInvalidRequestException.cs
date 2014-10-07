// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvalidRequestException.cs" company="Heartland Payment Systems">
//   Copyright (c) Heartland Payment Systems. All rights reserved.
// </copyright>
// <summary>
//   Invalid parameters were supplied to the HPS gateway.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SecureSubmit.Infrastructure
{
    public class HpsInvalidRequestException : HpsException
    {
        public HpsExceptionCodes Code { get; set; }

        public string ParamName { get; private set; }

        public HpsInvalidRequestException(HpsExceptionCodes code, string message, string paramName)
            : base(message)
        {
            Code = code;
            ParamName = paramName;
        }
    }
}