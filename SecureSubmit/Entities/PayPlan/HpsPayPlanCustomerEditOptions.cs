using SecureSubmit.Infrastructure;

namespace SecureSubmit.Entities.PayPlan
{
    public class HpsPayPlanCustomerEditOptions : HpsConsumer
    {
        public string CustomerId { get; set; }

        public string Company { get; set; }

        public HpsPayPlanCustomerStatus CustomerStatus { get; set; }

        public string Title { get; set; }

        public string Department { get; set; }

        public string SecondaryEmail { get; set; }

        public string PhoneExt { get; set; }

        public string PhoneEvening { get; set; }

        public string PhoneEveningExt { get; set; }

        public string PhoneMobile { get; set; }

        public string PhoneMobileExt { get; set; }

        public string Fax { get; set; }
    }
}
