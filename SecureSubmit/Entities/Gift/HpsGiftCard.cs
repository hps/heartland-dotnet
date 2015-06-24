using Hps.Exchange.PosGateway.Client;

namespace SecureSubmit.Entities
{
    public class HpsGiftCard
    {
        public HpsGiftCard()
        {
            NumberType = ItemChoiceType.CardNbr;
        }

        public string Number { get; set; }

        public int ExpMonth { get; set; }

        public int ExpYear { get; set; }

        public ItemChoiceType NumberType { get; set; }

        /// <summary>Gets or sets E3 encryption data group (optional). <b>Note:</b> encryptionData is required
        /// only when the supplied card data is encrypted; it must <b>not</b> be set when the associated card
        /// data is not encrypted.</summary>
        public HpsEncryptionData EncryptionData { get; set; }
    }
}
