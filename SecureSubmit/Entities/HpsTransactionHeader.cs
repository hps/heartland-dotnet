// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HpsTransactionHeader.cs" company="Heartland Payment Systems">
//   Copyright (c) Heartland Payment Systems. All rights reserved.
// </copyright>
// <summary>
//   The HPS transaction header.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SecureSubmit.Entities
{
    using System;

    /// <summary>The HPS charge header.</summary>
    public class HpsTransactionHeader
    {
        /// <summary>Gets or sets the gateway RSP code.</summary>
        public int GatewayRspCode { get; set; }

        /// <summary>Gets or sets the gateway RSP MSG.</summary>
        public string GatewayRspMsg { get; set; }

        /// <summary>Gets or sets the RSP DateTime.</summary>
        public DateTime? RspDt { get; set; }

        /// <summary>Gets or sets the client TXN ID.</summary>
        public long? ClientTxnId { get; set; }
    }
}