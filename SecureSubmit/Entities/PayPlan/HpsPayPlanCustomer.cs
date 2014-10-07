using System;
using SecureSubmit.Infrastructure;

namespace SecureSubmit.Entities.PayPlan
{
    public class HpsPayPlanCustomer : HpsConsumer
    {
        public string CustomerKey { get; set; }

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

        public DateTime StatusSetDate { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime LastChangeDate { get; set; }
    }
}
