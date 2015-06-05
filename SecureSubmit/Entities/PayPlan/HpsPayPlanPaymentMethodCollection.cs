namespace SecureSubmit.Entities.PayPlan
{
    public class HpsPayPlanPaymentMethodCollection
    {
        public int Offset { get; set; }
        public int Limit { get; set; }
        public int TotalMatchingRecords { get; set; }
        public HpsPayPlanPaymentMethod[] Results { get; set; }
    }
}
