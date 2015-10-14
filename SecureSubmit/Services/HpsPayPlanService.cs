using System;
using System.Collections.Generic;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Infrastructure;

namespace SecureSubmit.Services
{
    public class HpsPayPlanService : HpsRestGatewayService
    {
        public HpsPayPlanService(IHpsServicesConfig config)
            : base(config)
        {
        }

        /* CUSTOMER METHODS */

        public HpsPayPlanCustomer AddCustomer(HpsPayPlanCustomer customer)
        {
            if (customer == null)
                throw new HpsInvalidRequestException(HpsExceptionCodes.InvalidArgument,
                    "Customer must be an instance of HpsPayPlanCustomer.", "customer");

            var response = DoRequest("POST", "customers", customer);
            return HydrateObject<HpsPayPlanCustomer>(response);
        }

        public HpsPayPlanCustomerCollection FindAllCustomers()
        {
            return FindAllCustomers(new Dictionary<string, object>());
        }

        public HpsPayPlanCustomerCollection FindAllCustomers(Dictionary<string, Object> searchFields)
        {
            if (searchFields == null)
                throw new HpsInvalidRequestException(HpsExceptionCodes.InvalidArgument,
                    "searchFields cannot be null.", "searchFields");

            var response = DoRequest("POST", "searchCustomers", searchFields);
            return HydrateObject<HpsPayPlanCustomerCollection>(response);
        }

        public HpsPayPlanCustomer GetCustomer(HpsPayPlanCustomer customer)
        {
            if (customer == null)
                throw new HpsInvalidRequestException(HpsExceptionCodes.InvalidArgument,
                    "customer must be an instance of HpsPayPlanCustomer.", "customer");

            return GetCustomer(customer.CustomerKey);
        }

        public HpsPayPlanCustomer GetCustomer(string customerId)
        {
            var response = DoRequest("GET", "customers/" + customerId);
            return HydrateObject<HpsPayPlanCustomer>(response);
        }

        public HpsPayPlanCustomer EditCustomer(HpsPayPlanCustomer customer)
        {
            if (customer == null)
                throw new HpsInvalidRequestException(HpsExceptionCodes.InvalidArgument,
                    "customer must be an instance of HpsPayPlanCustomer.", "customer");

            var response = DoRequest("PUT", "customers/" + customer.CustomerKey, customer.GetEditableFieldsWithValues());
            return HydrateObject<HpsPayPlanCustomer>(response);
        }

        public HpsPayPlanCustomer DeleteCustomer(string customerId, bool forceDelete = false)
        {
            if (string.IsNullOrEmpty(customerId))
                throw new HpsInvalidRequestException(HpsExceptionCodes.InvalidArgument,
                    "customerId must be a valid PayPlan customer ID.", "customerId");

            var data = new Dictionary<string, object> {{"forceDelete", forceDelete}};
            var response = DoRequest("DELETE", "customers/" + customerId, data);

            return HydrateObject<HpsPayPlanCustomer>(response);
        }

        public HpsPayPlanCustomer DeleteCustomer(HpsPayPlanCustomer customer, bool forceDelete = false)
        {
            if (customer == null)
                throw new HpsInvalidRequestException(HpsExceptionCodes.InvalidArgument,
                    "customer must be an instance of HpsPayPlanCustomer.", "customer");

            return DeleteCustomer(customer.CustomerKey, forceDelete);
        }

        /* PAYMENT METHODS */

        public HpsPayPlanPaymentMethod GetPaymentMethod(string methodId)
        {
            return HydrateObject<HpsPayPlanPaymentMethod>(DoRequest("GET", "paymentMethods/" + methodId));
        }

        public HpsPayPlanPaymentMethod GetPaymentMethod(HpsPayPlanPaymentMethod method)
        {
            if (method == null)
                throw new HpsInvalidRequestException(HpsExceptionCodes.InvalidArgument,
                    "method must be an instance of HpsPayPlanPaymentMethod.", "method");

            return HydrateObject<HpsPayPlanPaymentMethod>(DoRequest("GET", "paymentMethods/" + method.PaymentMethodKey));
        }

        public HpsPayPlanPaymentMethod AddPaymentMethod(HpsPayPlanPaymentMethod method)
        {
            if (method == null)
                throw new HpsInvalidRequestException(HpsExceptionCodes.InvalidArgument,
                    "method must be an instance of HpsPayPlanPaymentMethod.", "method");

            return HydrateObject<HpsPayPlanPaymentMethod>(method.PaymentMethodType.Equals(HpsPayPlanPaymentMethodType.Ach) ?
                AddAch(method) : AddCreditCard(method));
        }

        public HpsPayPlanPaymentMethod EditPaymentMethod(HpsPayPlanPaymentMethod method)
        {
            if (method == null)
                throw new HpsInvalidRequestException(HpsExceptionCodes.InvalidArgument,
                    "method must be an instance of HpsPayPlanPaymentMethod.", "method");

            return HydrateObject<HpsPayPlanPaymentMethod>(method.PaymentMethodType.Equals(HpsPayPlanPaymentMethodType.Ach) ?
                EditAch(method) : EditCreditCard(method));
        }

        public HpsPayPlanPaymentMethodCollection FindAllPaymentMethods(Dictionary<string, object> searchFields = null)
        {
            var sf = searchFields ?? new Dictionary<string, object>();
            var response = DoRequest("POST", "searchPaymentMethods", sf);
            return HydrateObject<HpsPayPlanPaymentMethodCollection>(response);
        }

        public HpsPayPlanPaymentMethod DeletePaymentMethod(string methodId, bool forceDelete = false)
        {
            var data = new Dictionary<string, object> {{"forceDelete", forceDelete}};
            return HydrateObject<HpsPayPlanPaymentMethod>(DoRequest("DELETE", "paymentMethods/" + methodId, data));
        }

        public HpsPayPlanPaymentMethod DeletePaymentMethod(HpsPayPlanPaymentMethod method, bool forceDelete = false)
        {
            if (method == null)
                throw new HpsInvalidRequestException(HpsExceptionCodes.InvalidArgument,
                    "method must be an instance of HpsPayPlanPaymentMethod.", "method");

            return DeletePaymentMethod(method.PaymentMethodKey, forceDelete);
        } 

        private string AddAch(HpsPayPlanPaymentMethod method)
        {
            var data = method.GetEditableFieldsWithValues();
            data.Add("customerKey", method.CustomerKey);
            return DoRequest("POST", "paymentMethodsACH", data);
        }

        private string EditAch(HpsPayPlanPaymentMethod method)
        {
            var data = method.GetEditableFieldsWithValues();
            return DoRequest("PUT", "paymentMethodsACH/" + method.PaymentMethodKey, data);
        }

        private string AddCreditCard(HpsPayPlanPaymentMethod method)
        {
            var data = method.GetEditableFieldsWithValues();
            data.Add("customerKey", method.CustomerKey);
            if (!string.IsNullOrEmpty(method.AccountNumber))
                data.Add("accountNumber", method.AccountNumber);
            else if (!string.IsNullOrEmpty(method.PaymentToken))
                data.Add("paymentToken", method.PaymentToken);
            return DoRequest("POST", "paymentMethodsCreditCard", data);
        }

        private string EditCreditCard(HpsPayPlanPaymentMethod method)
        {
            var data = method.GetEditableFieldsWithValues();
            return DoRequest("PUT", "paymentMethodsCreditCard/" + method.PaymentMethodKey, data);
        }

        /* SCHEDULE METHODS */

        public HpsPayPlanSchedule AddSchedule(HpsPayPlanSchedule schedule)
        {
            if (schedule == null)
                throw new HpsInvalidRequestException(HpsExceptionCodes.InvalidArgument,
                    "schedule must be an instance of HpsPayPlanSchedule.", "schedule");

            var data = schedule.GetEditableFieldsWithValues();
            data.Add("customerKey", schedule.CustomerKey);
            data.Add("numberOfPayments", schedule.NumberOfPayments);

            var response = DoRequest("POST", "schedules", data);
            return HydrateObject<HpsPayPlanSchedule>(response);
        }

        public HpsPayPlanSchedule EditSchedule(HpsPayPlanSchedule schedule)
        {
            if (schedule == null)
                throw new HpsInvalidRequestException(HpsExceptionCodes.InvalidArgument,
                    "schedule must be an instance of HpsPayPlanSchedule.", "schedule");

            schedule.EndDate = string.IsNullOrEmpty(schedule.EndDate) ? null : schedule.EndDate;
            schedule.StartDate = string.IsNullOrEmpty(schedule.EndDate) ? null : schedule.EndDate;
            schedule.NextProcessingDate = string.IsNullOrEmpty(schedule.NextProcessingDate) ? null : schedule.NextProcessingDate;
            schedule.PreviousProcessingDate = string.IsNullOrEmpty(schedule.PreviousProcessingDate) ? null : schedule.PreviousProcessingDate;

            var data = schedule.GetEditableFieldsWithValues();
            var response = DoRequest("PUT", "schedules/" + schedule.ScheduleKey, data);

            return HydrateObject<HpsPayPlanSchedule>(response);
        }

        public HpsPayPlanScheduleCollection FindAllSchedules(Dictionary<string, object> searchFields = null)
        {
            var sf = searchFields ?? new Dictionary<string, object>();
            var response = DoRequest("POST", "searchSchedules", sf);

            return HydrateObject<HpsPayPlanScheduleCollection>(response);
        }

        public HpsPayPlanSchedule GetSchedule(HpsPayPlanSchedule schedule)
        {
            if (schedule == null)
                throw new HpsInvalidRequestException(HpsExceptionCodes.InvalidArgument,
                    "schedule must be an instance of HpsPayPlanSchedule.", "schedule");

            return GetSchedule(schedule.ScheduleKey);
        }

        public HpsPayPlanSchedule GetSchedule(string scheduleId)
        {
            var response = DoRequest("GET", "schedules/" + scheduleId);
            return HydrateObject<HpsPayPlanSchedule>(response);
        }

        public HpsPayPlanSchedule DeleteSchedule(string scheduleId, bool forceDelete = false)
        {
            var data = new Dictionary<string, object> {{"forceDelete", forceDelete}};
            var response = DoRequest("DELETE", "schedules/" + scheduleId, data);

            return HydrateObject<HpsPayPlanSchedule>(response);
        }

        public HpsPayPlanSchedule DeleteSchedule(HpsPayPlanSchedule schedule, bool forceDelete = false)
        {
            if (schedule == null)
                throw new HpsInvalidRequestException(HpsExceptionCodes.InvalidArgument,
                    "schedule must be an instance of HpsPayPlanSchedule.", "schedule");

            return DeleteSchedule(schedule.ScheduleKey, forceDelete);
        }
    }
}
