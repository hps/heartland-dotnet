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
                Value = "5022440000000000098",
            };

            /// <summary>Various test gift cards</summary>
            public static HpsGiftCard validGiftCardNotEncrypted2 = new HpsGiftCard
            {
                Value = "5022440000000000007",
            };

            public static HpsGiftCard invalidGiftCardNotEncrypted = new HpsGiftCard
            {
                Value = "12345",
            };
        }
    }
}
