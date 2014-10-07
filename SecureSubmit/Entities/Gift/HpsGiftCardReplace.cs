// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HpsGiftCardDeactivate.cs" company="Heartland Payment Systems">
//   Copyright (c) Heartland Payment Systems. All rights reserved.
// </copyright>
// <summary>
//   The HPS gift card deactivate response.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SecureSubmit.Entities
{
    public class HpsGiftCardDeactivate : HpsTransaction
    {
        /// <summary>Gets or sets the authorization code.</summary>
        public string AuthorizationCode { get; set; }

        /// <summary>Gets or sets the the balance that was on the card before it was deactivated
        /// and should be refunded to the cardholder.</summary>
        public decimal RefundAmount { get; set; }

        /// <summary>Gets or sets the new balance on the stored value account in points.</summary>
        public decimal PointsBalanceAmount { get; set; }
    }
}
