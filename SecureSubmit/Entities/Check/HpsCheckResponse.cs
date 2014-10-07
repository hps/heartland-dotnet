// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HpsCheckResponse.cs" company="Heartland Payment Systems">
//   Copyright (c) Heartland Payment Systems. All rights reserved.
// </copyright>
// <summary>
//   The HPS check response.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SecureSubmit.Entities
{
    using System.Collections.Generic;

    public class HpsCheckResponse : HpsTransaction
    {
        /// <summary>Gets or sets the authorization code.</summary>
        public string AuthorizationCode { get; set; }

        /// <summary>Gets or sets the customer ID.</summary>
        public string CustomerId { get; set; }

        public List<HpsCheckResponseDetails> Details { get; set; }
    }
}
