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
    using SecureSubmit.Entities;

    /// <summary>The credit card info.</summary>
    public static class TestCreditCard
    {
        /// <summary>Various test credit cards</summary>
        private static HpsCreditCard validVisa = new HpsCreditCard
        {
            Cvv = 123,
            ExpMonth = 12,
            ExpYear = 2015,
            Number = "4012002000060016"
        },
        validMasterCard = new HpsCreditCard
        {
            Cvv = 123,
            ExpMonth = 12,
            ExpYear = 2015,
            Number = "5473500000000014"
        },
        validDiscover = new HpsCreditCard
        {
            Cvv = 123,
            ExpMonth = 12,
            ExpYear = 2015,
            Number = "6011000990156527"
        },
        validAmex = new HpsCreditCard
        {
            Cvv = 1234,
            ExpMonth = 12,
            ExpYear = 2015,
            Number = "372700699251018"
        },
        validJcb = new HpsCreditCard
        {
            Cvv = 123,
            ExpMonth = 12,
            ExpYear = 2015,
            Number = "3566007770007321"
        },
        invalidCard = new HpsCreditCard
        {
            Cvv = 123,
            ExpMonth = 12,
            ExpYear = 2015,
            Number = "12345"
        };

        /// <summary>Gets a valid Visa.</summary>
        public static HpsCreditCard ValidVisa
        {
            get
            {
                return validVisa;
            }
        }

        /// <summary>Gets a valid MasterCard.</summary>
        public static HpsCreditCard ValidMasterCard
        {
            get
            {
                return validMasterCard;
            }
        }

        /// <summary>Gets a valid American Express card.</summary>
        public static HpsCreditCard ValidAmex
        {
            get
            {
                return validAmex;
            }
        }

        /// <summary>Gets a valid Discover card.</summary>
        public static HpsCreditCard ValidDiscover
        {
            get
            {
                return validDiscover;
            }
        }

        /// <summary>Gets a valid JCB card.</summary>
        public static HpsCreditCard ValidJcb
        {
            get
            {
                return validJcb;
            }
        }

        /// <summary>Gets an invalid card.</summary>
        public static HpsCreditCard InvalidCard
        {
            get
            {
                return invalidCard;
            }
        }
    }
}
