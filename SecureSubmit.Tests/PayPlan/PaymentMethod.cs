using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecureSubmit.Entities.PayPlan;
using SecureSubmit.Infrastructure;
using SecureSubmit.Services;
using SecureSubmit.Services.PayPlan;

namespace SecureSubmit.Tests.PayPlan
{
    [TestClass]
    public class PaymentMethod
    {
        private readonly HpsPayPlanCustomer _customer;

        private readonly HpsPayPlanService _payPlanService = new HpsPayPlanService(new HpsServicesConfig
        {
            SecretApiKey = "skapi_uat_MY5OAAAUrmIFvLDRpO_ufLlFQkgg0Rms2G8WoI1THQ"
        });

        public PaymentMethod()
        {
            _payPlanService.SetPagination(1, 0);
            _customer = _payPlanService.FindAllCustomers().Results[0];
        }

        [TestMethod]
        public void AddPaymentMethod()
        {
            var paymentMethod = new HpsPayPlanPaymentMethod
            {
                CustomerKey = _customer.CustomerKey,
                PaymentMethodType = HpsPayPlanPaymentMethodType.CreditCard,
                NameOnAccount = string.Format("{0} {1}", _customer.FirstName, _customer.LastName),
                AccountNumber = "4111111111111111",
                ExpirationDate = "0120",
                Country = "USA"
            };

            var result = _payPlanService.AddPaymentMethod(paymentMethod);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.PaymentMethodKey);
        }

        [TestMethod]
        [ExpectedException(typeof(HpsInvalidRequestException), "method must be ain instance of HpsPayPlanPaymentMethod.")]
        public void AddNullPaymentMethod()
        {
            _payPlanService.AddPaymentMethod(null);
        }

        [TestMethod]
        public void EditPaymentMethod()
        {
            _payPlanService.SetPagination(1, 0);
            var searchFilter = new Dictionary<string, object> {{"customerIdentifier", "SecureSubmit"}};
            var methodsResponse = _payPlanService.FindAllPaymentMethods(searchFilter);
            Assert.IsNotNull(methodsResponse);
            Assert.IsTrue(methodsResponse.Results.Length >= 1);

            // Make the edit
            var method = methodsResponse.Results[0];
            var paymentStatus = method.PaymentStatus.Equals(HpsPayPlanCustomerStatus.Active)
                ? HpsPayPlanCustomerStatus.Inactive
                : HpsPayPlanCustomerStatus.Active;
            method.PaymentStatus = paymentStatus;

            var editResponse = _payPlanService.EditPaymentMethod(method);
            Assert.IsNotNull(editResponse);
            Assert.AreEqual(method.PaymentMethodKey, editResponse.PaymentMethodKey);
            Assert.AreEqual(method.PaymentStatus, editResponse.PaymentStatus);

            // Verify the edit
            var verifyResponse = _payPlanService.GetPaymentMethod(method);
            Assert.IsNotNull(verifyResponse);
            Assert.AreEqual(method.PaymentMethodKey, verifyResponse.PaymentMethodKey);
            Assert.AreEqual(method.PaymentStatus, verifyResponse.PaymentStatus);
        }

        [TestMethod]
        [ExpectedException(typeof(HpsInvalidRequestException), "method must be ain instance of HpsPayPlanPaymentMethod.")]
        public void EditNullPaymentMethod()
        {
            _payPlanService.EditPaymentMethod(null);
        }

        [TestMethod]
        public void FindAllPaymentMethods()
        {
            var results = _payPlanService.FindAllPaymentMethods();
            Assert.IsNotNull(results);
            Assert.IsTrue(results.Results.Length >= 1);
        }

        [TestMethod]
        public void FindAllPaymentMethodsWithPaging()
        {
            _payPlanService.SetPagination(1, 0);
            var results = _payPlanService.FindAllPaymentMethods();
            Assert.IsNotNull(results);
            Assert.IsTrue(results.Results.Length == 1);
        }

        [TestMethod]
        public void FindAllPaymentMethodsWithFilter()
        {
            var searchFilter = new Dictionary<string, object> {{"customerIdentifier", "SecureSubmit"}};
            var results = _payPlanService.FindAllPaymentMethods(searchFilter);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.Results.Length >= 1);
        }

        [TestMethod]
        public void GetPaymentMethodByMethod()
        {
            _payPlanService.SetPagination(1, 0);
            var methods = _payPlanService.FindAllPaymentMethods();
            Assert.IsNotNull(methods);
            Assert.IsTrue(methods.Results.Length == 1);

            var result = _payPlanService.GetPaymentMethod(methods.Results[0]);
            Assert.IsNotNull(result);
            Assert.AreEqual(methods.Results[0].PaymentMethodKey, result.PaymentMethodKey);
        }

        [TestMethod]
        public void GetPaymentMethodByMethodId()
        {
            _payPlanService.SetPagination(1, 0);
            var methods = _payPlanService.FindAllPaymentMethods();
            Assert.IsNotNull(methods);
            Assert.IsTrue(methods.Results.Length == 1);

            var result = _payPlanService.GetPaymentMethod(methods.Results[0].PaymentMethodKey);
            Assert.IsNotNull(result);
            Assert.AreEqual(methods.Results[0].PaymentMethodKey, result.PaymentMethodKey);
        }

        [TestMethod]
        public void DeletePaymentMethodByMethod()
        {
            AddPaymentMethod();

            _payPlanService.SetPagination(1, 0);
            var methods = _payPlanService.FindAllPaymentMethods();
            Assert.IsNotNull(methods);
            Assert.IsTrue(methods.Results.Length == 1);

            var result = _payPlanService.DeletePaymentMethod(methods.Results[0]);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void DeletePaymentMethodByMethodId()
        {
            AddPaymentMethod();

            _payPlanService.SetPagination(1, 0);
            var methods = _payPlanService.FindAllPaymentMethods();
            Assert.IsNotNull(methods);
            Assert.IsTrue(methods.Results.Length == 1);

            var result = _payPlanService.DeletePaymentMethod(methods.Results[0].PaymentMethodKey);
            Assert.IsNull(result);
        }
    }
}
