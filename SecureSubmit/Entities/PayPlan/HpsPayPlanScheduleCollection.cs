namespace SecureSubmit.Entities
{
    public class HpsPayPlanScheduleCollection
    {
        public int Offset { get; set; }
        public int Limit { get; set; }
        public int TotalMatchingRecords { get; set; }
        public HpsPayPlanSchedule[] Results { get; set; }
    }
}
