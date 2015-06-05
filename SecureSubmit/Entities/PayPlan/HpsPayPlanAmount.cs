namespace SecureSubmit.Entities.PayPlan
{
    public class HpsPayPlanAmount
    {
        public string Value { get; set; }
        public string Currency { get; set; }

        internal HpsPayPlanAmount() {}

        public HpsPayPlanAmount(string value)
        {
            Value = value;
            Currency = "USD";
        }

        public HpsPayPlanAmount(string value, string currency)
        {
            Value = value;
            Currency = currency;
        }
    }
}
