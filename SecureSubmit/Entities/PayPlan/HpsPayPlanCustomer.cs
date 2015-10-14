using System;
using System.Collections.Generic;

namespace SecureSubmit.Entities
{
    public class HpsPayPlanCustomer
    {
        public string CustomerKey { get; set; }
        public string CustomerIdentifier { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string CustomerStatus { get; set; }
        public string PrimaryEmail { get; set; }
        public string SecondaryEmail { get; set; }
        public string PhoneDay { get; set; }
        public string PhoneDayExt { get; set; }
        public string PhoneEvening { get; set; }
        public string PhoneEveningExt { get; set; }
        public string PhoneMobile { get; set; }
        public string PhoneMobileExt { get; set; }
        public string Fax { get; set; }
        public string Title { get; set; }
        public string Department { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string StateProvince { get; set; }
        public string ZipPostalCode { get; set; }
        public HpsPayPlanPaymentMethod[] PaymentMethods { get; set; }
        public HpsPayPlanSchedule[] Schedules { get; set; }

        private static IEnumerable<string> GetEditableFields()
        {
            return new[]
            {
                "CustomerIdentifier",
                "FirstName",
                "LastName",
                "Company",
                "CustomerStatus",
                "Title",
                "Department",
                "PrimaryEmail",
                "SecondaryEmail",
                "PhoneDay",
                "PhoneDayExt",
                "PhoneEvening",
                "PhoneEveningExt",
                "PhoneMobile",
                "PhoneMobileExt",
                "Fax",
                "AddressLine1",
                "AddressLine2",
                "City",
                "StateProvince",
                "ZipPostalCode",
                "Country"
            };
        }

        private string[] GetSearchableFields()
        {
            return new[]
            {
                "CustomerIdentifier",
                "Company",
                "FirstName",
                "LastName",
                "PrimaryEmail",
                "CustomerStatus",
                "PhoneNumber",
                "City",
                "StateProvince",
                "ZipPostalCode",
                "Country",
                "HasSchedules",
                "HasActiveSchedules",
                "HasPaymentMethods",
                "HasActivePaymentMethods"
            };
        }

        public Dictionary<String, Object> GetEditableFieldsWithValues()
        {
            var map = new Dictionary<string, object>();

            foreach (var fieldName in GetEditableFields())
            {
                var prop = GetType().GetProperty(fieldName);
                var value = prop.GetValue(this, null);
                if (value == null) continue;

                map.Add(fieldName, value);
            }

            return map;
        }
    }
}
