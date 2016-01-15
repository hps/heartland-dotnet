using System.Collections.Generic;
using SecureSubmit.Entities;
using SecureSubmit.Infrastructure;
using SecureSubmit.Serialization;
using SecureSubmit.Services;

namespace SecureSubmit.Tests.Certification
{
    public class TestData
    {
        private static HpsTokenService _tokenService;
        private static readonly Dictionary<string, HpsToken> Tokens = new Dictionary<string, HpsToken>();

        public TestData(string publicApiKey)
        {
            _tokenService = new HpsTokenService(publicApiKey);
        }

        private static HpsCreditCard Card()
        {
            return new HpsCreditCard {ExpMonth = 12, ExpYear = 2025};
        }

        public static HpsToken TokenizeCard(HpsCreditCard card)
        {
            if (Tokens.ContainsKey(card.Number))
            {
                return Tokens[card.Number];
            }

            var token = _tokenService.GetToken(card);
            Tokens.Add(card.Number, token);

            return token;
        }

        // CREDIT CARDS

        //public static HpsCreditCard VisaCard(bool cvv)
        //{
        //    var card = Card();
        //    card.Number = "4012002000060016";
        //    if (cvv) { card.Cvv = "123"; }

        //    return card;
        //}

        //public static HpsCreditCard MasterCard(bool cvv)
        //{
        //    var card = Card();
        //    card.Number = ;
        //    if (cvv) { card.Cvv = "123"; }

        //    return card;
        //}

        //public static HpsCreditCard DiscoverCard(bool cvv)
        //{
        //    var card = Card();
        //    card.Number = "6011000990156527";
        //    if (cvv) { card.Cvv = "123"; }

        //    return card;
        //}

        //public static HpsCreditCard AmexCard(bool cvv)
        //{
        //    var card = Card();
        //    card.Number = "372700699251018";
        //    if (cvv) { card.Cvv = "1234"; }

        //    return card;
        //}

        //public static HpsCreditCard JcbCard(bool cvv)
        //{
        //    var card = Card();
        //    card.Number = "3566007770007321";
        //    if (cvv) { card.Cvv = "1234"; }

        //    return card;
        //}

        // CARD MULTI_USE TOKENS

        //public enum Industry { ECommerce, Retail }

        //public static string VisaMultiUseToken(Industry industry)
        //{
        //    switch (industry)
        //    {
        //        case Industry.ECommerce: return "Wf4LD1084WQUbf1S6Kcd0016";
        //        case Industry.Retail: return "Wf4LD1084WQUbf1S6Kcd0016";
        //        default: return "Wf4LD1084WQUbf1S6Kcd0016";
        //    }
        //}

        //public static string MasterCardMultiUseToken(Industry industry)
        //{
        //    switch (industry)
        //    {
        //        case Industry.ECommerce: return "e51Af108vAsB6Tx0PfmG0014";
        //        case Industry.Retail: return "e51Af108vAsB6Tx0PfmG0014";
        //        default: return "e51Af108vAsB6Tx0PfmG0014";
        //    }
        //}

        //public static string DiscoverMultiUseToken(Industry industry)
        //{
        //    switch (industry)
        //    {
        //        case Industry.ECommerce: return "ubeZ3f08W1aaEV643ZJa6527";
        //        case Industry.Retail: return "ubeZ3f08W1aaEV643ZJa6527";
        //        default: return "ubeZ3f08W1aaEV643ZJa6527";
        //    }
        //}

        //public static string AmexMultiUseToken(Industry industry)
        //{
        //    switch (industry)
        //    {
        //        case Industry.ECommerce: return "94oIbE08e30G0fVcH12d1018";
        //        case Industry.Retail: return "94oIbE08e30G0fVcH12d1018";
        //        default: return "94oIbE08e30G0fVcH12d1018";
        //    }
        //}

        //public static HpsCreditCard GsbCardECommerce()
        //{
        //    return new HpsCreditCard {Number = "6277220020010671", ExpYear = 2049, ExpMonth = 12};
        //}

        // CARD SWIPE DATA

        public static HpsTrackData VisaSwipe(HpsTrackDataMethod method)
        {
            return new HpsTrackData
            {
                Value = "%B4012002000060016^VI TEST CREDIT^251210118039000000000396?;4012002000060016=25121011803939600000?",
                Method = method
            };
        }

        public static HpsTrackData MasterCardSwipe(HpsTrackDataMethod method)
        {
            return new HpsTrackData
            {
                Value = "%B5473500000000014^MC TEST CARD^251210199998888777766665555444433332?;5473500000000014=25121019999888877776?",
                Method = method
            };
        }

        public static HpsTrackData AmexSwipe(HpsTrackDataMethod method)
        {
            return new HpsTrackData
            {
                Value = "%B3727 006992 51018^AMEX TEST CARD^2512990502700?;372700699251018=2512990502700?",
                Method = method
            };
        }

        public static HpsTrackData DiscoverSwipe(HpsTrackDataMethod method)
        {
            return new HpsTrackData
            {
                Value = "%B6011000990156527^DIS TEST CARD^25121011000062111401?;6011000990156527=25121011000062111401?",
                Method = method
            };
        }

        public static HpsTrackData JcbSwipe(HpsTrackDataMethod method)
        {
            return new HpsTrackData
            {
                Value = "%B3566007770007321^JCB TEST CARD^2512101100000000000000000064300000?;3566007770007321=25121011000000076435?",
                Method = method
            };
        }

        public static HpsTrackData GsbSwipe(HpsTrackDataMethod method)
        {
            return new HpsTrackData
            {
                Value = "%B6277220020010671^   /                         ^49121010228710000209000000?;6277220020" +
                        "010671=49121010228710000209?",
                Method = method
            };
        }

        public static HpsTrackData VisaDebitSwipe()
        {
            return new HpsTrackData
            {
                Value = "&lt;E1050711%B4012001000000016^VI TEST CREDIT^251200000000000000000000?|LO04K0WFOmdkDz0" +
                        "um+GwUkILL8ZZOP6Zc4rCpZ9+kg2T3JBT4AEOilWTI|+++++++Dbbn04ekG|11;4012001000000016=251200000000000000" +
                        "00?|1u2F/aEhbdoPixyAPGyIDv3gBfF|+++++++Dbbn04ekG|00|||/wECAQECAoFGAgEH2wYcShV78RZwb3NAc2VjdXJlZXhj" +
                        "aGFuZ2UubmV0PX50qfj4dt0lu9oFBESQQNkpoxEVpCW3ZKmoIV3T93zphPS3XKP4+DiVlM8VIOOmAuRrpzxNi0TN/DWXWSjUC8" +
                        "m/PI2dACGdl/hVJ/imfqIs68wYDnp8j0ZfgvM26MlnDbTVRrSx68Nzj2QAgpBCHcaBb/FZm9T7pfMr2Mlh2YcAt6gGG1i2bJgi" +
                        "EJn8IiSDX5M2ybzqRT86PCbKle/XCTwFFe1X|&gt;",
                Method = HpsTrackDataMethod.Swipe,
                EncryptionData = new HpsEncryptionData
                {
                    Version = "01"
                }
            };
        }

        public static string VisaDebitPinBlock()
        {
            return "32539F50C245A6A93D123412324000AA";
        }

        public static HpsTrackData MasterCardDebitSwipe()
        {
            return new HpsTrackData
            {
                Value = "&lt;E1052711%B5473501000000014^MC TEST CARD^251200000000000000000000000000000000?|GVEY/" +
                        "MKaKXuqqjKRRueIdCHPPoj1gMccgNOtHC41ymz7bIvyJJVdD3LW8BbwvwoenI+|+++++++C4cI2zjMp|11;547350100000001" +
                        "4=25120000000000000000?|8XqYkQGMdGeiIsgM0pzdCbEGUDP|+++++++C4cI2zjMp|00|||/wECAQECAoFGAgEH2wYcShV7" +
                        "8RZwb3NAc2VjdXJlZXhjaGFuZ2UubmV0PX50qfj4dt0lu9oFBESQQNkpoxEVpCW3ZKmoIV3T93zphPS3XKP4+DiVlM8VIOOmAu" +
                        "RrpzxNi0TN/DWXWSjUC8m/PI2dACGdl/hVJ/imfqIs68wYDnp8j0ZfgvM26MlnDbTVRrSx68Nzj2QAgpBCHcaBb/FZm9T7pfMr" +
                        "2Mlh2YcAt6gGG1i2bJgiEJn8IiSDX5M2ybzqRT86PCbKle/XCTwFFe1X|&gt;",
                Method = HpsTrackDataMethod.Swipe,
                EncryptionData = new HpsEncryptionData
                {
                    Version = "01"
                }
            };
        }

        public static string MasterCardDebitPinBlock()
        {
            return "F505AD81659AA42A3D123412324000AB";
        }

        // GIFT CARDS

        //public static HpsGiftCard GiftCard1()
        //{
        //    return new HpsGiftCard {Number = "5022440000000000098"};
        //}

        //public static HpsGiftCard GiftCard2()
        //{
        //    return new HpsGiftCard {Number = "5022440000000000098"};
        //}

        //public static HpsGiftCard GiftCardSwipe1()
        //{
        //    return new HpsGiftCard
        //    {
        //        Number = "%B5022440000000000098^^391200081613?;5022440000000000098=391200081613?"
        //    };
        //}
    }
}
