// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HpsGiftCardSale.cs" company="Heartland Payment Systems">
//   Copyright (c) Heartland Payment Systems. All rights reserved.
// </copyright>
// <summary>
//   The HPS gift card sale response.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SecureSubmit.Entities
{
    public class HpsGiftCardSale : HpsTransaction
    {
        /// <summary>Gets or sets the authorization code.</summary>
        public string AuthorizationCode { get; set; }

        /// <summary>Gets or sets the new balance on the gift card.</summary>
        public decimal BalanceAmount { get; set; }

        /// <summary>If there are insufficient funds on the gift card to complete the sale,
        /// this is the split tender portion of the total sale that was subtracted from the
        /// gift card balance.</summary>
        public decimal SplitTenderCardAmount { get; set; }

        /// <summary>If there are insufficient funds on gift card to complete sale, this is
        /// the portion of the total sale that was not funded by the gift card and thus is
        /// still due from the cardholder.</summary>
        public decimal SplitTenderBalanceDue { get; set; }

        /// <summary>Gets or sets the new balance on the stored value account in points.</summary>
        public decimal PointsBalanceAmount { get; set; }

        /// <summary>Gets or sets the rewards (dollars or points) added to the account as a
        /// result of a transaction.</summary>
        public string Rewards{ get; set; }

        /// <summary>Gets or sets the notes. Notes contain reward messages to be displayed
        /// on a receipt, mobile app, or web page to inform an account holder about special
        /// rewards or promotions available on the account.</summary>
        public string Notes { get; set; }
    }
}
