// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestCreditCard.cs" company="Heartland Payment Systems">
//   Copyright (c) Heartland Payment Systems. All rights reserved.
// </copyright>
// <summary>
//   The credit card info.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SecureSubmit.Tests.TestData
{
    using Entities;

    /// <summary>The credit card info.</summary>
    public static class TestCreditCard
    {
        /// <summary>Various test credit cards</summary>
        public static readonly HpsCreditCard ValidVisa = new HpsCreditCard
        {
            Cvv = "123",
            ExpMonth = 12,
            ExpYear = 2015,
            Number = "4012002000060016"
        };

        /// <summary>Various test credit cards</summary>
        public static readonly HpsCreditCard ValidMasterCard = new HpsCreditCard
        {
            Cvv = "123",
            ExpMonth = 12,
            ExpYear = 2015,
            Number = "5473500000000014"
        };

        /// <summary>Various test credit cards</summary>
        public static readonly HpsCreditCard ValidDiscover = new HpsCreditCard
        {
            Cvv = "123",
            ExpMonth = 12,
            ExpYear = 2015,
            Number = "6011000990156527"
        };

        /// <summary>Various test credit cards</summary>
        public static readonly HpsCreditCard ValidAmex = new HpsCreditCard
        {
            Cvv = "1234",
            ExpMonth = 12,
            ExpYear = 2015,
            Number = "372700699251018"
        };

        /// <summary>Various test credit cards</summary>
        public static readonly HpsCreditCard ValidJcb = new HpsCreditCard
        {
            Cvv = "123",
            ExpMonth = 12,
            ExpYear = 2015,
            Number = "3566007770007321"
        };

        /// <summary>Various test credit cards</summary>
        public static readonly HpsCreditCard InvalidCard = new HpsCreditCard
        {
            Cvv = "123",
            ExpMonth = 12,
            ExpYear = 2015,
            Number = "12345"
        };
    }
}
