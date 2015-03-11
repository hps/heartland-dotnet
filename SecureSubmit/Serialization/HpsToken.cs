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
        public HpsCardToken(string number, string cvc, int expMonth, int expYear) : this()
        {
            card = new Card(number, cvc, expMonth, expYear);
        }

        public HpsCardToken(HpsCreditCard cardData) : this(cardData.Number, cardData.Cvv, cardData.ExpMonth, cardData.ExpYear) { }
    }

    public class HpsE3SwipeToken : HpsToken
    {
        public E3Swipe card { get; set; }

        public HpsE3SwipeToken() : base() { }
        public HpsE3SwipeToken(E3Swipe card) : this()
        {
            this.card = card;
        }
        public HpsE3SwipeToken(string track)
        {
            this.card = new E3Swipe(track);
        }
    }

    public class HpsTrackDataToken : HpsToken
    {
        public TrackData encryptedcard { get; set; }

        public HpsTrackDataToken() : base() { }
        public HpsTrackDataToken(TrackData card) : this()
        {
            this.encryptedcard = card;
        }
        public HpsTrackDataToken(string track, string trackNumber, string ktb, string pinBlock = "") : this()
        {
            encryptedcard = new TrackData(track, trackNumber, ktb);
            if (!string.IsNullOrEmpty(pinBlock))
                encryptedcard.pin_block = pinBlock;
        }
    }

    public class Card
    {
        public string number { get; set; }
        public string cvc { get; set; }
        public int exp_month { get; set; }
        public int exp_year { get; set; }

        public Card(string number, string cvc, int expMonth, int expYear)
        {
            this.number = number;
            this.cvc = cvc;
            exp_month = expMonth;
            exp_year = expYear;
        }
    }

    public class E3Swipe
    {
        public string track_method { get; set; }
        public string track { get; set; }

        public E3Swipe() { }
        public E3Swipe(string track)
        {
            this.track = track;
            this.track_method = "swipe";
        }
    }

    public class TrackData
    {
        public string track { get; set; }
        public string track_method { get; set; }
        public string track_number { get; set; }
        public string pin_block { get; set; }
        public string ktb { get; set; }

        public TrackData() { }
        public TrackData(string track, string trackNumber, string ktb)
        {
            this.track = track;
            this.track_number = trackNumber;
            this.ktb = ktb;
            this.track_method = "swipe";
        }
    }
}
