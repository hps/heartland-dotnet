// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HpsAddress.cs" company="Heartland Payment Systems">
//   Copyright (c) Heartland Payment Systems. All rights reserved.
// </copyright>
// <summary>
//   Defines the HpsAddressInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SecureSubmit.Entities
{
    /// <summary>
    /// The HPS address info.
    /// </summary>
    public class HpsAddress
    {
        /// <summary>Gets or sets the address.</summary>
        public string Address { get; set; }

        /// <summary>Gets or sets the city.</summary>
        public string City { get; set; }

        /// <summary>Gets or sets the state.</summary>
        public string State { get; set; }

        /// <summary>Gets or sets the zip.</summary>
        public string Zip { get; set; }

        /// <summary>Gets or sets the country.</summary>
        public string Country { get; set; }
    }
}
