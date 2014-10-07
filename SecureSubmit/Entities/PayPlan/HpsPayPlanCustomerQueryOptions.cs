// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HpsPayPlanCustomerQueryOptions.cs" company="Heartland Payment Systems">
//   Copyright (c) Heartland Payment Systems. All rights reserved.
// </copyright>
// <summary>
//   The HPS pay plan customer query options.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using SecureSubmit.Infrastructure;

namespace SecureSubmit.Entities.PayPlan
{
    public class HpsPayPlanCustomerQueryOptions
    {
        /// <summary>Wildcard (contains)</summary>
        public string CustomerId { get; set; }

        /// <summary>Wildcard (contains)</summary>
        public string Company { get; set; }

        /// <summary>Wildcard (contains)</summary>
        public string FirstName { get; set; }

        /// <summary>Wildcard (contains)</summary>
        public string LastName { get; set; }

        /// <summary>Wildcard (contains)</summary>
        public string PrimaryEmail { get; set; }

        /// <summary>Exact match (Active or Inactive)</summary>
        public HpsPayPlanCustomerStatus? CustomerStatus { get; set; }

        /// <summary>Searches all phone number fields (PhoneDay, PhoneEvening, Phone Mobile)</summary>
        public string PhoneNumber { get; set; }

        /// <summary>Wildcard (contains)</summary>
        public string City { get; set; }

        /// <summary>Wildcard (contains)</summary>
        public string StateProvince { get; set; }

        /// <summary>Wildcard (contains)</summary>
        public string ZipPostalCode { get; set; }

        /// <summary>Wildcard (contains)</summary>
        public string Country { get; set; }

        /// <summary>True or False (optional)</summary>
        public bool? HasSchedules { get; set; }

        /// <summary>True or False (optional)</summary>
        public bool? HasActiveSchedules { get; set; }

        /// <summary>True or False (optional)</summary>
        public bool? HasPaymentMethods { get; set; }

        /// <summary>True or False (optional)</summary>
        public bool? HasActivePaymentMethods { get; set; }
    }
}
