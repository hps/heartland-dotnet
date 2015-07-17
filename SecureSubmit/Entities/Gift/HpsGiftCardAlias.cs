// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HpsGiftCardAlias.cs" company="Heartland Payment Systems">
//   Copyright (c) Heartland Payment Systems. All rights reserved.
// </copyright>
// <summary>
//   The HPS gift card alias response.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Hps.Exchange.PosGateway.Client;
namespace SecureSubmit.Entities
{
    public class HpsGiftCardAlias : HpsTransaction
    {
        public HpsGiftCard GiftCard { get; set; }
        internal new HpsGiftCardAlias FromResponse(PosResponseVer10 response) {
            base.FromResponse(response);

            var transaction = (PosGiftCardAliasRspType)response.Transaction.Item;
            GiftCard = HpsGiftCard.FromResponse(transaction.CardData);
            ResponseCode = transaction.RspCode.ToString();
            ResponseText = transaction.RspText;

            return this;
        }
    }
}
