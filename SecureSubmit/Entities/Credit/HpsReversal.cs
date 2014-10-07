// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HpsReversal.cs" company="Heartland Payment Systems">
//   Copyright (c) Heartland Payment Systems. All rights reserved.
// </copyright>
// <summary>
//   The HPS reversal.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SecureSubmit.Entities
{
    public class HpsReversal : HpsTransaction
    {
        /// <summary>Gets or sets the AVS result code.</summary>
        public string AvsResultCode { get; set; }

        /// <summary>Gets or sets the CVV result code.</summary>
        public string CvvResultCode { get; set; }

        /// <summary>Gets or sets the AVS result text.</summary>
        public string AvsResultText { get; set; }

        /// <summary>Gets or sets the CVV result text.</summary>
        public string CvvResultText { get; set; }

        /// <summary>Gets or sets the CPC indicator.</summary>
        public string CpcIndicator { get; set; }
    }
}
