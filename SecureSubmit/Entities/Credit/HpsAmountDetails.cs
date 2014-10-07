// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HpsAmountDetails.cs" company="Heartland Payment Systems">
//   Copyright (c) Heartland Payment Systems. All rights reserved.
// </copyright>
// <summary>
//   The transaction amount details.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SecureSubmit.Entities
{
    /// <summary>The HpsAmountDetails</summary>
    public class HpsAmountDetails
    {
        /// <summary>Gets or sets the gratuity amount. If any portion of the authorized 
        /// amount represented a gratuity, this element may be specified to record the 
        /// amount. The element is for informational purposes only and does not affect 
        /// the authorized amount.</summary>
        public decimal? Gratuity { get; set; }

        /// <summary>Gets or sets the convenience amount; or the portion of the 
        /// authorization amount that represents a convenience fee charged to the 
        /// cardholder.</summary>
        public decimal? ConvenienceFee { get; set; }

        /// <summary>Gets or sets the shipping amount information. The portion of the 
        /// authorization amount that represents shipping fee charged to the cardholder.</summary>
        public decimal? Shipping { get; set; }
    }
}
