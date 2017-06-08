// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HpsAddress.cs" company="Heartland Payment Systems">
//   Copyright (c) Heartland Payment Systems. All rights reserved.
// </copyright>
// <summary>
//   Defines the HpsAddressInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using SecureSubmit.Infrastructure.Validation;
using SecureSubmit.Infrastructure;
namespace SecureSubmit.Entities
{
    /// <summary>
    /// The HPS address info.
    /// </summary>
    public class HpsAddress
    {
        private string address;
        private string city;
        private string state;
        private string zip;

        /// <summary>Gets or sets the address.</summary>
        public string Address
        {
            get
            {
                return address;
            }
            set
            {
                address = HpsInputValidation.CardHolderDetails(value, AddressFields.Address);
            }
        }

        /// <summary>Gets or sets the city.</summary>
        public string City
        {
            get
            {
                return city;
            }
            set
            {
                city = HpsInputValidation.CardHolderDetails(value, AddressFields.City);
            }
        }

        /// <summary>Gets or sets the state.</summary>
        public string State
        {
            get
            {
                return state;
            }
            set
            {
                state = HpsInputValidation.CardHolderDetails(value, AddressFields.State);
            }
        }

        /// <summary>Gets or sets the zip.</summary>
        public string Zip
        {
            get
            {
                return zip;
            }
            set
            {
                zip = HpsInputValidation.CheckZipcode(value);
            }
        }

        /// <summary>Gets or sets the country.</summary>
        public string Country { get; set; }
    }
}
