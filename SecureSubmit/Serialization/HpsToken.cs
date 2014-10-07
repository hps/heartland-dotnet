using SecureSubmit.Entities;

namespace SecureSubmit.Serialization
{
    public class HpsToken
    {
        public string @object { get; set; }
        public string token_type { get; set; }
        public string token_value { get; set; }
        public string token_expire { get; set; }
        public HpsError error { get; set; }

        public HpsToken()
        {
            @object = "token";
            token_type = "supt";
        }
    }
    public class HpsCardToken : HpsToken
    {
        public Card card { get; set; }
        
        public HpsCardToken() : base() { }
        public HpsCardToken(string number, int cvc, int expMonth, int expYear) : this()
        {
            card = new Card(number, cvc, expMonth, expYear);
        }
        public HpsCardToken(HpsCreditCard cardData) : this(cardData.Number, cardData.Cvv, cardData.ExpMonth, cardData.ExpYear) { }
    }
    public class HpsEncryptedSwipeToken : HpsToken
    {
        public EncryptedSwipe card { get; set; }

        public HpsEncryptedSwipeToken() : base() { }
        public HpsEncryptedSwipeToken(EncryptedSwipe card) : this()
        {
            this.card = card;
        }
        public HpsEncryptedSwipeToken(string track)
        {
            this.card = new EncryptedSwipe(track);
        }
    }
    public class HpsEncryptedCardToken : HpsToken
    {
        public EncryptedCard encryptedcard { get; set; }

        public HpsEncryptedCardToken() : base() { }
        public HpsEncryptedCardToken(EncryptedCard card) : this()
        {
            this.encryptedcard = card;
        }
        public HpsEncryptedCardToken(string track, string trackNumber, string ktb, string pinBlock = "") : this()
        {
            encryptedcard = new EncryptedCard(track, trackNumber, ktb);
            if (!string.IsNullOrEmpty(pinBlock))
                encryptedcard.pin_block = pinBlock;
        }
    }

    public class Card
    {
        public string number { get; set; }
        public int cvc { get; set; }
        public int exp_month { get; set; }
        public int exp_year { get; set; }

        public Card() { }
        public Card(string number, int cvc, int expMonth, int expYear)
        {
            this.number = number;
            this.cvc = cvc;
            this.exp_month = expMonth;
            this.exp_year = expYear;
        }
    }
    public class EncryptedSwipe
    {
        public string track_method { get; set; }
        public string track { get; set; }

        public EncryptedSwipe() { }
        public EncryptedSwipe(string track)
        {
            this.track = track;
            this.track_method = "swipe";
        }
    }
    public class EncryptedCard
    {
        public string track { get; set; }
        public string track_method { get; set; }
        public string track_number { get; set; }
        public string pin_block { get; set; }
        public string ktb { get; set; }

        public EncryptedCard() { }
        public EncryptedCard(string track, string trackNumber, string ktb)
        {
            this.track = track;
            this.track_number = trackNumber;
            this.ktb = ktb;
            
            this.track_method = "swipe";
        }
    }
}
