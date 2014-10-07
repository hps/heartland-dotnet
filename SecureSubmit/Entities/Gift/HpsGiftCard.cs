namespace SecureSubmit.Entities
{
    public class HpsGiftCard
    {
        /// <summary>Set default values.</summary>
        public HpsGiftCard()
        {
            IsTrackData = false;
        }

        /// <summary>Gets or sets the gift card number.</summary>
        public string Number { get; set; }

        /// <summary>Gets or sets the expiration month.</summary>
        public int ExpMonth { get; set; }

        /// <summary>Gets or sets the expiration year.</summary>
        public int ExpYear { get; set; }

        /// <summary>Gets or sets whether the number represents track data [default = `false`]. Set to `true` 
        /// if cardNumber was read from the card by a cardReader.</summary>
        public bool IsTrackData {get; set;}

        /// <summary>Gets or sets E3 encryption data group (optional). <b>Note:</b> encryptionData is required
        /// only when the supplied card data is encrypted; it must <b>not</b> be set when the associated card
        /// data is not encrypted.</summary>
        public HpsEncryptionData EncryptionData { get; set; }
    }
}
