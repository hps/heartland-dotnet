using Hps.Exchange.PosGateway.Client;

namespace SecureSubmit.Entities
{
    public class HpsGiftCard
    {
        public HpsGiftCard()
        {
            ValueType = ItemChoiceType.CardNbr;
        }

        public string Value { get; set; }

        public ItemChoiceType ValueType { get; set; }

        public string Pin { get; set; }

        public HpsEncryptionData EncryptionData { get; set; }

        internal static HpsGiftCard FromResponse(GiftCardDataRspType response) {
            var card = new HpsGiftCard {
                Pin = response.PIN
            };

            if (response.CardNbr != null) {
                card.Value = response.CardNbr;
                card.ValueType = ItemChoiceType.CardNbr;
            }
            else if (response.Alias != null) {
                card.Value = response.Alias;
                card.ValueType = ItemChoiceType.Alias;
            }

            return card;
        }
    }
}
