using System;
using System.Collections.Generic;

namespace SecureSubmit.Entities
{
    public class HpsPayPlanPaymentMethod : HpsPayPlanResource
    {
        public string PaymentMethodKey { get; set; }
        public string PaymentMethodType { get; set; }
        public string PreferredPayment { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentMethodIdentifier { get; set; }
        public string CustomerKey { get; set; }
        public string CustomerIdentifier { get; set; }
        public string CustomerStatus { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string NameOnAccount { get; set; }
        public string AccountNumber { get; set; }
        public string AccountNumberLast4 { get; set; }
        public string PaymentMethod { get; set; }
        public string CardBrand { get; set; }
        public string ExpirationDate { get; set; }
        public string CvvResponseCode { get; set; }
        public string AvsResponseCode { get; set; }
        public string AchType { get; set; }
        public string AccountType { get; set; }
        public string RoutingNumber { get; set; }
        public bool TelephoneIndicator { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string StateProvince { get; set; }
        public string ZipPostalCode { get; set; }
        public string Country { get; set; }
        public string AccountHolderYob { get; set; }
        public string DriversLicenseState { get; set; }
        public string DriversLicenseNumber { get; set; }
        public string SocialSecurityLast4 { get; set; }
        public string HasSchedules { get; set; }
        public string HasActiveSchedules { get; set; }
        public string PaymentToken { get; set; }

        private IEnumerable<string> GetEditableFields()
        {
            var fields = new List<String>
            {
                "PreferredPayment",
                "PaymentStatus",
                "PaymentMethodIdentifier",
                "NameOnAccount",
                "AddressLine1",
                "AddressLine2",
                "City",
                "StateProvince",
                "ZipPostalCode"
            };

            if (PaymentMethodType.Equals(HpsPayPlanPaymentMethodType.Ach))
            {
                fields.Add("TelephoneIndicator");
                fields.Add("AccountHolderYob");
                fields.Add("DriversLicenseState");
                fields.Add("DriversLicenseNumber");
                fields.Add("SocialSecurityLast4");
                fields.Add("AchType");
                fields.Add("RoutingNumber");
                fields.Add("AccountNumber");
                fields.Add("AccountType");
            }
            else if (PaymentMethodType.Equals(HpsPayPlanPaymentMethodType.CreditCard))
            {
                fields.Add("ExpirationDate");
                fields.Add("Country");
            }

            return fields.ToArray();
        }

        internal Dictionary<string, Object> GetEditableFieldsWithValues()
        {
            var map = new Dictionary<string, object>();

            foreach (var fieldName in GetEditableFields())
            {
                var prop = GetType().GetProperty(fieldName);
                if (prop.GetValue(this, null) != null)
                {
                    map.Add(fieldName, prop.GetValue(this, null));
                }
            }

            return map;
        }
    }
}