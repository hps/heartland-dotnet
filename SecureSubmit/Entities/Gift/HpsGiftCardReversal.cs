// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HpsGiftCardReversal.cs" company="Heartland Payment Systems">
//   Copyright (c) Heartland Payment Systems. All rights reserved.
// </copyright>
// <summary>
//   The HPS gift card reversal response.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SecureSubmit.Entities
{
    public class HpsGiftCardReversal : HpsTransaction
    {
        /// <summary>Gets or sets the authorization code.</summary>
        public string AuthorizationCode { get; set; }

        /// <summary>Gets or sets the new balance on the gift card.</summary>
        public decimal BalanceAmount { get; set; }
    }
}
