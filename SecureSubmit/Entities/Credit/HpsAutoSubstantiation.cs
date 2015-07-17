using System;

namespace SecureSubmit.Entities.Credit
{
    public class HpsAutoSubstantiation
    {
        public string MerchantVerificationValue { get; set; }
        public bool RealTimeSubstantiation { get; set; }
        public HpsAdditionalAmount[] AdditionalAmounts { get; set; }
    }
}
