using SecureSubmit.Infrastructure;
using System;

namespace SecureSubmit.Entities.PayPlan.Dto
{
    internal class PayPlanCustomer
    {
        public string CustomerKey { get; set; }

        public string CustomerIdentifier { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Company { get; set; }

        public string CustomerStatus { get; set; }

        public string Title { get; set; }

        public string Department { get; set; }

        public string PrimaryEmail { get; set; }

        public string SecondaryEmail { get; set; }

        public string PhoneDay { get; set; }

        public string PhoneDayExt { get; set; }

        public string PhoneEvening { get; set; }

        public string PhoneEveningExt { get; set; }

        public string PhoneMobile { get; set; }

        public string PhoneMobileExt { get; set; }

        public string Fax { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string City { get; set; }

        public string StateProvince { get; set; }

        public string ZipPostalCode { get; set; }

        public string Country { get; set; }

        public DateTime? StatusSetDate { get; set; }

        public DateTime? CreationDate { get; set; }

        public DateTime? LastChangeDate { get; set; }

        public static HpsPayPlanCustomer Map(PayPlanCustomer dto)
        {
            var result = new HpsPayPlanCustomer
            {
                Company = dto.Company,
                CustomerId = dto.CustomerIdentifier,
                CustomerStatus = dto.CustomerStatus == "Active" ? HpsPayPlanCustomerStatus.Active : HpsPayPlanCustomerStatus.Inactive,
                Department = dto.Department,
                Fax = dto.Fax,
                PhoneEvening = dto.PhoneEvening,
                PhoneEveningExt = dto.PhoneEveningExt,
                PhoneExt = dto.PhoneDayExt,
                PhoneMobile = dto.PhoneMobile,
                PhoneMobileExt = dto.PhoneMobileExt,
                SecondaryEmail = dto.SecondaryEmail,
                Title = dto.Title
            };

            return (HpsPayPlanCustomer)HydrateConsumerInfo(dto, result);
        }

        public static PayPlanCustomer Map(HpsPayPlanCustomerEditOptions options)
        {
            var result = new PayPlanCustomer
            {
                Company = options.Company,
                CustomerIdentifier = options.CustomerId,
                CustomerStatus = options.CustomerStatus == HpsPayPlanCustomerStatus.Active ? "Active" : "Inactive",
                Department = options.Department,
                PhoneDayExt = options.PhoneExt,
                PhoneEvening = options.PhoneEvening,
                PhoneEveningExt = options.PhoneEveningExt,
                PhoneMobile = options.PhoneMobile,
                PhoneMobileExt = options.PhoneMobileExt,
                SecondaryEmail = options.SecondaryEmail,
                Title = options.Title
            };

            return HydrateConsumerInfo(options, result);
        }

        private static PayPlanCustomer HydrateConsumerInfo(HpsPayPlanCustomerEditOptions options, PayPlanCustomer dto)
        {
            if (options.Address != null)
            {
                dto.AddressLine1 = options.Address.Address;
                dto.City = options.Address.City;
                dto.Country = options.Address.Country;
                dto.StateProvince = options.Address.State;
                dto.ZipPostalCode = options.Address.Zip;
            }

            dto.PrimaryEmail = options.Email;
            dto.FirstName = options.FirstName;
            dto.LastName = options.LastName;
            dto.PhoneDay = options.Phone;

            return dto;
        }

        private static HpsConsumer HydrateConsumerInfo(PayPlanCustomer dto, HpsConsumer consumer)
        {
            consumer.Address = new HpsAddress
            {
                Address = dto.AddressLine1,
                City = dto.City,
                Country = dto.Country,
                State = dto.StateProvince,
                Zip = dto.ZipPostalCode
            };

            consumer.Email = dto.PrimaryEmail;
            consumer.FirstName = dto.FirstName;
            consumer.LastName = dto.LastName;
            consumer.Phone = dto.PhoneDay;

            return consumer;
        }
    }
}
