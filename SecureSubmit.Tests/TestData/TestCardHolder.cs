namespace SecureSubmit.Tests.TestData
{
    using Entities;

    public static class TestCardHolder
    {
        public static readonly HpsCardHolder ValidCardHolder = new HpsCardHolder
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
                };

        public static readonly HpsCardHolder CertCardHolderShortZip = new HpsCardHolder
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
                };

        public static readonly HpsCardHolder CertCardHolderShortZipNoStreet = new HpsCardHolder
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
                };

        public static readonly HpsCardHolder CertCardHolderLongZip = new HpsCardHolder
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
                };

        public static readonly HpsCardHolder CertCardHolderLongZipNoStreet = new HpsCardHolder
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

        public static readonly HpsCardHolder CertCardHolderStreetZipOnly = new HpsCardHolder
                {
                    Address = new HpsAddress
                    {
                        Address = "6860 Dallas Pkwy",
                        Zip = "75024",
                    }
                };

        public static readonly HpsCardHolder CertCardHolderStreetNumberZipOnly = new HpsCardHolder
        {
            Address = new HpsAddress
            {
                Address = "6860",
                Zip = "75024",
            }
        };
    }
}
