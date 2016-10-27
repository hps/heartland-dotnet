using System;
using System.Collections.Generic;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Infrastructure;
using System.Text;

namespace SecureSubmit.Services
{
    public class HpsPayPlanService : HpsRestGatewayService
    {
        private HpsPayPlanServicesConfig _config;
        private Dictionary<string, string> _authHeader = new Dictionary<string, string>();
        private Dictionary<string, string> _pagination = null;

        public HpsPayPlanService(HpsPayPlanServicesConfig config)
            : base(config)
        {
            _config = (HpsPayPlanServicesConfig)config;

            var keyBytes = Encoding.UTF8.GetBytes(_config.SecretApiKey);
            var encoded = Convert.ToBase64String(keyBytes);
            var auth = String.Format("Basic {0}", encoded);
            _authHeader.Add("Authorization", auth);
        }

        /* CUSTOMER METHODS */
        public void SetPagination(int limit, int offset)
        {
            _pagination = new Dictionary<string, string>();
            _pagination.Add("limit", limit.ToString());
            _pagination.Add("offset", offset.ToString());
        }

        public void ResetPagination()
        {
            _pagination = null;
        }

        public HpsPayPlanCustomer AddCustomer(HpsPayPlanCustomer customer)
        {
            if (customer == null)
                throw new HpsInvalidRequestException(HpsExceptionCodes.InvalidArgument,
                    "Customer must be an instance of HpsPayPlanCustomer.", "customer");

            var response = DoRequest("POST", "customers", customer, _authHeader, _pagination);
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

            var response = DoRequest("POST", "searchCustomers", searchFields, _authHeader, _pagination);
            ResetPagination();
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
            var response = DoRequest("GET", "customers/" + customerId, null, _authHeader, _pagination);
            ResetPagination();
            return HydrateObject<HpsPayPlanCustomer>(response);
        }

        public HpsPayPlanCustomer EditCustomer(HpsPayPlanCustomer customer)
        {
            if (customer == null)
                throw new HpsInvalidRequestException(HpsExceptionCodes.InvalidArgument,
                    "customer must be an instance of HpsPayPlanCustomer.", "customer");

            var response = DoRequest("PUT", "customers/" + customer.CustomerKey, customer.GetEditableFieldsWithValues(), _authHeader, _pagination);
            ResetPagination();
            return HydrateObject<HpsPayPlanCustomer>(response);
        }

        public HpsPayPlanCustomer DeleteCustomer(string customerId, bool forceDelete = false)
        {
            if (string.IsNullOrEmpty(customerId))
                throw new HpsInvalidRequestException(HpsExceptionCodes.InvalidArgument,
                    "customerId must be a valid PayPlan customer ID.", "customerId");

            var data = new Dictionary<string, object> {{"forceDelete", forceDelete}};
            var response = DoRequest("DELETE", "customers/" + customerId, data, _authHeader, _pagination);
            ResetPagination();

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
            return HydrateObject<HpsPayPlanPaymentMethod>(DoRequest("GET", "paymentMethods/" + methodId, null, _authHeader, _pagination));
            ResetPagination();
        }

        public HpsPayPlanPaymentMethod GetPaymentMethod(HpsPayPlanPaymentMethod method)
        {
            if (method == null)
                throw new HpsInvalidRequestException(HpsExceptionCodes.InvalidArgument,
                    "method must be an instance of HpsPayPlanPaymentMethod.", "method");

            return HydrateObject<HpsPayPlanPaymentMethod>(DoRequest("GET", "paymentMethods/" + method.PaymentMethodKey, null, _authHeader,_pagination));
            ResetPagination();
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
            var response = DoRequest("POST", "searchPaymentMethods", sf, _authHeader, _pagination);
            ResetPagination();
            return HydrateObject<HpsPayPlanPaymentMethodCollection>(response);
        }

        public HpsPayPlanPaymentMethod DeletePaymentMethod(string methodId, bool forceDelete = false)
        {
            var data = new Dictionary<string, object> {{"forceDelete", forceDelete}};
            var response = HydrateObject<HpsPayPlanPaymentMethod>(DoRequest("DELETE", "paymentMethods/" + methodId, data, _authHeader, _pagination));

            ResetPagination();
            return response;
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

            var response = DoRequest("POST", "paymentMethodsACH", data, _authHeader, _pagination);

            ResetPagination();
            return response;
        }

        private string EditAch(HpsPayPlanPaymentMethod method)
        {
            var data = method.GetEditableFieldsWithValues();
            var response = DoRequest("PUT", "paymentMethodsACH/" + method.PaymentMethodKey, data, _authHeader, _pagination);

            ResetPagination();
            return response;
            
        }

        private string AddCreditCard(HpsPayPlanPaymentMethod method)
        {
            var data = method.GetEditableFieldsWithValues();
            data.Add("customerKey", method.CustomerKey);
            if (!string.IsNullOrEmpty(method.AccountNumber))
                data.Add("accountNumber", method.AccountNumber);
            else if (!string.IsNullOrEmpty(method.PaymentToken))
                data.Add("paymentToken", method.PaymentToken);

            var response = DoRequest("POST", "paymentMethodsCreditCard", data, _authHeader, _pagination);

            ResetPagination();
            return response;

        }

        private string EditCreditCard(HpsPayPlanPaymentMethod method)
        {
            var data = method.GetEditableFieldsWithValues();
            return DoRequest("PUT", "paymentMethodsCreditCard/" + method.PaymentMethodKey, data, _authHeader, _pagination);
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

            var response = DoRequest("POST", "schedules", data, _authHeader, _pagination);
            ResetPagination();
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
            var response = DoRequest("PUT", "schedules/" + schedule.ScheduleKey, data, _authHeader, _pagination);
            ResetPagination();
            return HydrateObject<HpsPayPlanSchedule>(response);
        }

        public HpsPayPlanScheduleCollection FindAllSchedules(Dictionary<string, object> searchFields = null)
        {
            var sf = searchFields ?? new Dictionary<string, object>();
            var response = DoRequest("POST", "searchSchedules", sf, _authHeader, _pagination);
            ResetPagination();
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
            var response = DoRequest("GET", "schedules/" + scheduleId, null, _authHeader, _pagination);
            ResetPagination();
            return HydrateObject<HpsPayPlanSchedule>(response);
        }

        public HpsPayPlanSchedule DeleteSchedule(string scheduleId, bool forceDelete = false)
        {
            var data = new Dictionary<string, object> {{"forceDelete", forceDelete}};
            var response = DoRequest("DELETE", "schedules/" + scheduleId, data, _authHeader, _pagination);
            ResetPagination();
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
