// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HpsGiftCardAlias.cs" company="Heartland Payment Systems">
//   Copyright (c) Heartland Payment Systems. All rights reserved.
// </copyright>
// <summary>
//   The HPS gift card alias response.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SecureSubmit.Entities
{
    public class HpsGiftCardAlias : HpsTransaction
    {
        /// <summary>Gets or sets the gift card data.</summary>
        public HpsGiftCard GiftCard { get; set; }
    }
}
