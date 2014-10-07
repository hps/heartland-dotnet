// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestCardHolder.cs" company="Heartland Payment Systems">
//   Copyright (c) Heartland Payment Systems. All rights reserved.
// </copyright>
// <summary>
//   Address info test data.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SecureSubmit.Tests.TestData
{
    using SecureSubmit.Entities;

    /// <summary>Address info test data.</summary>
    public static class TestCardHolder
    {
        /// <summary>Valid card holders.</summary>
        private static HpsCardHolder validCardHolder = new HpsCardHolder
                {
                    Address = new HpsAddress
                    {
                        Address = "One Heartland Way",
                        City = "Jeffersonville",
                        State = "IN",
                        Zip = "47130",
                        Country = "United States"
                    },
                    FirstName = "Bill",
                    LastName = "Johnson"
                },
                certCardHolderShortZip = new HpsCardHolder
                {
                    Address = new HpsAddress
                    {
                        Address = "6860 Dallas Pkwy",
                        City = "Irvine",
                        State = "TX",
                        Zip = "75024",
                        Country = "United States"
                    },
                    FirstName = "Bill",
                    LastName = "Johnson"
                },
                certCardHolderShortZipNoStreet = new HpsCardHolder
                {
                    Address = new HpsAddress
                    {
                        City = "Irvine",
                        State = "TX",
                        Zip = "75024",
                        Country = "United States"
                    },
                    FirstName = "Bill",
                    LastName = "Johnson"
                },
                certCardHolderLongZip = new HpsCardHolder
                {
                    Address = new HpsAddress
                    {
                        Address = "6860 Dallas Pkwy",
                        City = "Irvine",
                        State = "TX",
                        Zip = "750241234",
                        Country = "United States"
                    },
                    FirstName = "Bill",
                    LastName = "Johnson"
                },
                certCardHolderLongZipNoStreet = new HpsCardHolder
                {
                    Address = new HpsAddress
                    {
                        City = "Irvine",
                        State = "TX",
                        Zip = "750241234",
                        Country = "United States"
                    },
                    FirstName = "Bill",
                    LastName = "Johnson"
                };

        /// <summary>Gets a valid card holder.</summary>
        public static HpsCardHolder ValidCardHolder
        {
            get { return validCardHolder; }
        }

        /// <summary>Gets a valid card holder to use for certification testing.</summary>
        public static HpsCardHolder CertCardHolderShortZip
        {
            get { return certCardHolderShortZip; }
        }

        /// <summary>Gets a valid card holder to use for certification testing.</summary>
        public static HpsCardHolder CertCardHolderShortZipNoStreet
        {
            get { return certCardHolderShortZipNoStreet; }
        }

        /// <summary>Gets a valid card holder to use for certification testing.</summary>
        public static HpsCardHolder CertCardHolderLongZip
        {
            get { return certCardHolderLongZip; }
        }

        /// <summary>Gets a valid card holder to use for certification testing.</summary>
        public static HpsCardHolder CertCardHolderLongZipNoStreet
        {
            get { return certCardHolderLongZipNoStreet; }
        }
    }
}
