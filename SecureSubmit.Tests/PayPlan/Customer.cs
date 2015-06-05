using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecureSubmit.Entities.PayPlan;
using SecureSubmit.Infrastructure;
using SecureSubmit.Services;
using SecureSubmit.Services.PayPlan;

namespace SecureSubmit.Tests.PayPlan
{
    [TestClass]
    public class Customer
    {
        private readonly HpsPayPlanService _payPlanService = new HpsPayPlanService(new HpsServicesConfig
        {
            SecretApiKey = "skapi_uat_MY5OAAAUrmIFvLDRpO_ufLlFQkgg0Rms2G8WoI1THQ"
        });

        private static string GenerateCustomerId()
        {
            return new DateTime().ToString("yyyyMMdd") + "-SecureSubmit-" + Guid.NewGuid().ToString().Substring(0, 10);
        }

        private static string GenerateRandomPhone()
        {
            var random = new Random();

            var rvalue = "";
            for (var i = 0; i < 7; i++)
                rvalue += random.Next(10);

            return rvalue;
        }

        [TestMethod]
        public void AddCustomer()
        {
            var newCustomer = new HpsPayPlanCustomer
            {
                CustomerStatus = HpsPayPlanCustomerStatus.Active,
                CustomerIdentifier = GenerateCustomerId(),
                FirstName = "Bill",
                LastName = "Johnson",
                Company = "Heartland Payment Systems",
                Country = "USA"
            };

            var result = _payPlanService.AddCustomer(newCustomer);

            // Fluent version (to be fully implemented at a later date)
            // HpsPayPlanCustomer result = payPlanService.addCustomer(id, "Bill", "Johnson", "USA").withCity("Dallas").execute();

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.CustomerKey);
        }

        [TestMethod]
        [ExpectedException(typeof(HpsInvalidRequestException), "Customer must be an instance of HpsPayPlanCustomer.")]
        public void AddNullCustomer()
        {
            _payPlanService.AddCustomer(null);
        }

        [TestMethod]
        public void FindAllCustomers()
        {
            var response = _payPlanService.FindAllCustomers();
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Results.Length > 0);
        }

        [TestMethod]
        public void FindAllCustomersWithPaging()
        {
            _payPlanService.SetPagination(1, 0);

            var response = _payPlanService.FindAllCustomers();
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Results.Length == 1);
        }

        [TestMethod]
        public void FindAllCustomersWithFilter()
        {
            var searchParams = new Dictionary<string, object> {{"customerIdentifier", "SecureSubmit"}};
            var response = _payPlanService.FindAllCustomers(searchParams);
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Results.Length > 0);
        }

        [TestMethod]
        [ExpectedException(typeof(HpsInvalidRequestException), "searchFields cannot be null.")]
        public void FindAllCustomersNull()
        {
            _payPlanService.FindAllCustomers(null);
        }

        [TestMethod]
        public void GetCustomerByCustomer()
        {
            _payPlanService.SetPagination(1, 0);
            var response = _payPlanService.FindAllCustomers();
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Results.Length == 1);

            var customer = _payPlanService.GetCustomer(response.Results[0]);
            Assert.IsNotNull(response);
            Assert.AreEqual(customer.CustomerKey, response.Results[0].CustomerKey);
        }

        [TestMethod]
        public void GetCustomerByCustomerKey()
        {
            _payPlanService.SetPagination(1, 0);
            var response = _payPlanService.FindAllCustomers();
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Results.Length == 1);

            var customer = _payPlanService.GetCustomer(response.Results[0].CustomerKey);
            Assert.IsNotNull(response);
            Assert.AreEqual(customer.CustomerKey, response.Results[0].CustomerKey);
        }

        [TestMethod]
        public void EditCustomer()
        {
            var searchParams = new Dictionary<string, object> {{"customerIdentifier", "SecureSubmit"}};
            _payPlanService.SetPagination(1, 0);
            var response = _payPlanService.FindAllCustomers(searchParams);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.Results.Length == 1);

            // Make the edit
            var phoneDay = "555" + GenerateRandomPhone();
            var customer = response.Results[0];
            customer.PhoneDay = phoneDay;

            var editResponse = _payPlanService.EditCustomer(customer);
            Assert.IsNotNull(editResponse);
            Assert.AreEqual(customer.CustomerKey, editResponse.CustomerKey);
            Assert.AreEqual(phoneDay, editResponse.PhoneDay);

            // Verify the edit
            var getResponse = _payPlanService.GetCustomer(customer.CustomerKey);
            Assert.IsNotNull(getResponse);
            Assert.AreEqual(customer.CustomerKey, getResponse.CustomerKey);
            Assert.AreEqual(phoneDay, getResponse.PhoneDay);
        }

        [TestMethod]
        [ExpectedException(typeof(HpsInvalidRequestException), "Customer must be an instance of HpsPayPlanCustomer.")]
        public void EditCustomerWithNull()
        {
            _payPlanService.EditCustomer(null);
        }

        [TestMethod]
        public void DeleteByCustomer()
        {
            AddCustomer();

            _payPlanService.SetPagination(1, 0);
            var response = _payPlanService.FindAllCustomers();
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Results.Length == 1);

            var deleteResposne = _payPlanService.DeleteCustomer(response.Results[0]);
            Assert.IsNull(deleteResposne);
        }

        [TestMethod]
        public void DeleteByCustomerKey()
        {
            AddCustomer();

            _payPlanService.SetPagination(1, 0);
            var response = _payPlanService.FindAllCustomers();
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Results.Length == 1);

            var deleteResposne = _payPlanService.DeleteCustomer(response.Results[0].CustomerKey);
            Assert.IsNull(deleteResposne);
        }
    }
}
