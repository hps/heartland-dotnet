using SecureSubmit.Infrastructure;

namespace SecureSubmit.Entities
{
    public class HpsTrackData
    {
        public HpsTrackDataMethod Method { get; set; }

        public string Value { get; set; }

        public HpsEncryptionData EncryptionData { get; set; }
    }
}
