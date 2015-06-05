namespace SecureSubmit.Entities
{
    public class HpsRecurringData
    {
        /// <summary>Intended for use with a Portico PayPlan schedule of recurring bill payments.</summary>
        public string ScheduleId { get; set; }
        
        /// <summary>Indicates whether this is a one time payment (true) or a recurring payment.</summary>
        public bool OneTimePayment { get; set; }
    }
}
