namespace SecureSubmit.Entities.PayPlan
{
    public class HpsPayPlanCustomerCollection
    {
        public int Offset { get; set; }
        public int Limit { get; set; }
        public int TotalMatchingRecords { get; set; }
        public HpsPayPlanCustomer[] Results { get; set; }
    }
}
