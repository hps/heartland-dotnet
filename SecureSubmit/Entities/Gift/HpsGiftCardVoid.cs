// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HpsGiftCardVoid.cs" company="Heartland Payment Systems">
//   Copyright (c) Heartland Payment Systems. All rights reserved.
// </copyright>
// <summary>
//   The HPS gift card void response.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SecureSubmit.Entities
{
    public class HpsGiftCardVoid : HpsTransaction
    {
        /// <summary>Gets or sets the authorization code.</summary>
        public string AuthorizationCode { get; set; }

        /// <summary>Gets or sets the new balance on the gift card.</summary>
        public decimal BalanceAmount { get; set; }

        /// <summary>Gets or sets the new balance on the stored value account in points.</summary>
        public decimal PointsBalanceAmount { get; set; }

        /// <summary>Gets or sets the notes. Notes contain reward messages to be displayed
        /// on a receipt, mobile app, or web page to inform an account holder about special
        /// rewards or promotions available on the account.</summary>
        public string Notes { get; set; }
    }
}
