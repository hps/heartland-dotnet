// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestGiftCard.cs" company="Heartland Payment Systems">
//   Copyright (c) Heartland Payment Systems. All rights reserved.
// </copyright>
// <summary>
//   The gift card info.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SecureSubmit.Tests.TestData
{
    using SecureSubmit.Entities;

    /// <summary>The gift card info.</summary>
    public static class TestGiftCard
    {
        public static class Manual
        {
            /// <summary>Various test gift cards</summary>
            public static HpsGiftCard validGiftCardNotEncrypted = new HpsGiftCard
            {
                Number = "5022440000000000098",
                ExpMonth = 12,
                ExpYear = 39
            };

            /// <summary>Various test gift cards</summary>
            public static HpsGiftCard validGiftCardNotEncrypted2 = new HpsGiftCard
            {
                Number = "5022440000000000007",
                ExpMonth = 12,
                ExpYear = 39
            };

            public static HpsGiftCard invalidGiftCardNotEncrypted = new HpsGiftCard
            {
                Number = "12345",
                ExpMonth = 12,
                ExpYear = 39
            };
        }
    }
}
