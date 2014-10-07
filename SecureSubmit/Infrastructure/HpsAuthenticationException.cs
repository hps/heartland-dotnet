// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthenticationException.cs" company="Heartland Payment Systems">
//   Copyright (c) Heartland Payment Systems. All rights reserved.
// </copyright>
// <summary>
//   Authentication with HPS's API failed. This is most likely due to an invalid HPS configuration.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SecureSubmit.Infrastructure
{
    public class HpsAuthenticationException : HpsException 
    {
        public HpsExceptionCodes Code { get; set; }

        public HpsAuthenticationException(HpsExceptionCodes sdkCode, string message) : base(message)
        {
            Code = sdkCode;
        }
    }
}