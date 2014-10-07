// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HpsChargeExceptions.cs" company="Heartland Payment Systems">
//   Copyright (c) Heartland Payment Systems. All rights reserved.
// </copyright>
// <summary>
//   The set of possible HPS charge exceptions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SecureSubmit.Entities
{
    using Infrastructure;

    /// <summary>The HPS Charge Exceptions</summary>
    public class HpsChargeExceptions
    {
        /// <summary>Gets or sets the card exception thrown upon execution of original transaction.</summary>
        public HpsCreditException IssuerException { get; set; }

        /// <summary>Gets or sets the gateway exception thrown upon execution of original transaction.</summary>
        public HpsException GatewayException { get; set; }
    }
}