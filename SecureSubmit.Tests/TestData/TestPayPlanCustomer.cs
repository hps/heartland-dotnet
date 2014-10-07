using System.Globalization;
using SecureSubmit.Entities.PayPlan;

namespace SecureSubmit.Tests.TestData
{
    using Entities;
    using Infrastructure;
    using System;

    public static class TestPayPlanCustomer
    {
        /// <summary>Valid pay plan customer.</summary>
        private static HpsPayPlanCustomerEditOptions validPayPlanCustomer = new HpsPayPlanCustomerEditOptions
        {
            FirstName = "Bill",
            LastName = "Johnson",
            CustomerStatus = HpsPayPlanCustomerStatus.Active,
            Phone = "1234567890",
            Address = new HpsAddress
            {
                Address = "One Heartland Way",
                City = "Jeffersonville",
                State = "IN",
                Zip = "47130",
                Country = "United States"
            }
        };

        /// <summary>Gets a valid Pay Plan customer.</summary>
        public static HpsPayPlanCustomerEditOptions ValidPayPlanCustomer
        {
            get
            {
                var rand = new Random();
                return new HpsPayPlanCustomerEditOptions
                {
                    CustomerId = rand.Next(10000, 99999).ToString(CultureInfo.InvariantCulture),
                    FirstName = validPayPlanCustomer.FirstName,
                    LastName = validPayPlanCustomer.LastName,
                    Company = validPayPlanCustomer.Company,
                    CustomerStatus = validPayPlanCustomer.CustomerStatus,
                    Phone = validPayPlanCustomer.Phone,
                    Address = validPayPlanCustomer.Address
                };
            }
        }
    }
}
