// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HpsDirectMarketData.cs" company="Heartland Payment Systems">
//   Copyright (c) Heartland Payment Systems. All rights reserved.
// </copyright>
// <summary>
//   The HPS direct market data.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace SecureSubmit.Entities
{
    /// <summary>The HPS token data.</summary>
    public class HpsDirectMarketData
    {
        public HpsDirectMarketData()
        {
            ShipMonth = DateTime.Today.Month;
            ShipDay = DateTime.Today.Day;
        }

        /// <summary>Gets or sets the invoice number.</summary>
        public string InvoiceNumber { get; set; }

        /// <summary>Gets or sets the ship month.</summary>
        public int ShipMonth { get; set; }

        /// <summary>Gets or sets the ship day.</summary>
        public int ShipDay { get; set; }
    }
}