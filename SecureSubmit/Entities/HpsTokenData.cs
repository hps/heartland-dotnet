// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HpsTokenData.cs" company="Heartland Payment Systems">
//   Copyright (c) Heartland Payment Systems. All rights reserved.
// </copyright>
// <summary>
//   The HPS token data.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SecureSubmit.Entities
{
    /// <summary>The HPS token data.</summary>
    public class HpsTokenData
    {
        /// <summary>Gets or sets the token RSP code.</summary>
        public int? TokenRspCode { get; set; }

        /// <summary>Gets or sets the token RSP message.</summary>
        public string TokenRspMsg { get; set; }

        /// <summary>Gets or sets the token value.</summary>
        public string TokenValue { get; set; }
    }
}