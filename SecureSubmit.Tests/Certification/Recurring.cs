using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecureSubmit.Entities;
using SecureSubmit.Fluent.Services;
using SecureSubmit.Infrastructure;
using SecureSubmit.Services;

namespace SecureSubmit.Tests.Certification
{
    [TestClass]
    public class Recurring
    {
        private static readonly HpsServicesConfig ServicesConfig = new HpsServicesConfig
        {
            SecretApiKey = "skapi_cert_MTyMAQBiHVEAewvIzXVFcmUd2UcyBge_eCpaASUp0A"
        };
        
        private static readonly HpsPayPlanServicesConfig PayPlanServicesConfig = new HpsPayPlanServicesConfig
        {
            SecretApiKey = "skapi_cert_MTyMAQBiHVEAewvIzXVFcmUd2UcyBge_eCpaASUp0A"
        };

        private readonly HpsPayPlanService _payPlanService = new HpsPayPlanService(PayPlanServicesConfig);
        private readonly HpsBatchService _batchService = new HpsBatchService(ServicesConfig);
        private readonly HpsFluentCreditService _creditService = new HpsFluentCreditService(ServicesConfig);
        private readonly HpsFluentCheckService _checkService = new HpsFluentCheckService(ServicesConfig);

        private static string _customerPersonKey;
        private static string _customerCompanyKey;
        private static string _paymentMethodKeyVisa;
        private static string _paymentMethodKeyMasterCard;
        private static string _paymentMethodKeyCheckPpd;
        private static string _paymentMethodKeyCheckCcd;
        private static string _scheduleKeyVisa;
        private static string _scheduleKeyMasterCard;
        private static string _scheduleKeyCheckPpd;
        private static string _scheduleKeyCheckCcd;

        private static readonly string TodayDate = DateTime.Today.ToString("yyyyMMdd");
        private static readonly string IdentifierBase = "{0}-{1}" + Guid.NewGuid().ToString().Substring(0, 10);

        private static string GetIdentifier(string identifier){
            var rValue = string.Format(IdentifierBase, TodayDate, identifier);
            Console.WriteLine(rValue);

            return rValue;
        }

        [TestMethod]
        public void recurring_000_CloseBatch()
        {
            try
            {
                var response = _batchService.CloseBatch();
                Assert.IsNotNull(response);
                Console.WriteLine(@"Batch ID: {0}", response.Id);
                Console.WriteLine(@"Sequence Number: {0}", response.SequenceNumber);
            }
            catch (HpsGatewayException e)
            {
                if (e.Code != HpsExceptionCodes.NoOpenBatch && e.Code != HpsExceptionCodes.UnknownIssuerError)
                    Assert.Fail("Something failed other than 'no open batch'.");
                else
                    Console.WriteLine(@"No batch open to close.");
            }
        }

        [TestMethod]
        public void recurring_000_CleanUp() {
            // Remove Schedules
            try {
                var schResults = _payPlanService.FindAllSchedules();
                foreach (var schedule in schResults.Results) {
                    _payPlanService.DeleteSchedule(schedule, true);
                }
            }
            catch { }

            // Remove Payment Methods
            try {
                var pmResults = _payPlanService.FindAllPaymentMethods();
                foreach (var pm in pmResults.Results) {
                    _payPlanService.DeletePaymentMethod(pm, true);
                }
            }
            catch { }

            // Remove Customers
            try {
                var custResults = _payPlanService.FindAllCustomers();
                foreach (var c in custResults.Results) {
                    _payPlanService.DeleteCustomer(c, true);
                }
            }
            catch { }
        }

        // CUSTOMER SETUP

        [TestMethod]
        public void recurring_001_AddCustomerPerson() {
            var customer = new HpsPayPlanCustomer {
                CustomerIdentifier = GetIdentifier("Person"),
                FirstName = "John",
                LastName = "Doe",
                CustomerStatus = HpsPayPlanCustomerStatus.Active,
                PrimaryEmail = "john.doe@email.com",
                AddressLine1 = "123 Main St",
                City = "Dallas",
                StateProvince = "TX",
                ZipPostalCode = "98765",
                Country = "USA",
                PhoneDay = "5551112222"
            };

            var response = _payPlanService.AddCustomer(customer);
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.CustomerKey);

            _customerPersonKey = response.CustomerKey;
        }

        [TestMethod]
        public void recurring_002_AddCustomerBusiness() {
            var customer = new HpsPayPlanCustomer {
                CustomerIdentifier = GetIdentifier("Business"),
                Company = "AcmeCo",
                CustomerStatus = HpsPayPlanCustomerStatus.Active,
                PrimaryEmail = "acme@email.com",
                AddressLine1 = "987 Elm St",
                City = "Princeton",
                StateProvince = "NJ",
                ZipPostalCode = "12345",
                Country = "USA",
                PhoneDay = "5551112222"
            };

            var response = _payPlanService.AddCustomer(customer);
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.CustomerKey);

            _customerCompanyKey = response.CustomerKey;
        }

        // PAYMENT METHOD SETUP

        [TestMethod]
        public void recurring_003_AddPaymentCreditVisa() {
            var paymentMethod = new HpsPayPlanPaymentMethod {
                PaymentMethodIdentifier = GetIdentifier("CreditV"),
                PaymentMethodType = HpsPayPlanPaymentMethodType.CreditCard,
                NameOnAccount = "John Doe",
                AccountNumber = "4012002000060016",
                ExpirationDate = "1225",
                CustomerKey = _customerPersonKey,
                Country = "USA"
            };

            var response = _payPlanService.AddPaymentMethod(paymentMethod);
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.PaymentMethodKey);
            Assert.IsNotNull(response.CreationDate);

            _paymentMethodKeyVisa = response.PaymentMethodKey;
        }

        [TestMethod]
        public void recurring_004_AddPaymentCreditMasterCard() {
            var paymentMethod = new HpsPayPlanPaymentMethod {
                PaymentMethodIdentifier = GetIdentifier("CreditMC"),
                PaymentMethodType = HpsPayPlanPaymentMethodType.CreditCard,
                NameOnAccount = "John Doe",
                AccountNumber = "5473500000000014",
                ExpirationDate = "1225",
                CustomerKey = _customerPersonKey,
                Country = "USA"
            };

            var response = _payPlanService.AddPaymentMethod(paymentMethod);
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.PaymentMethodKey);
            Assert.IsNotNull(response.CreationDate);

            _paymentMethodKeyMasterCard = response.PaymentMethodKey;
        }

        [TestMethod]
        public void recurring_005_AddPaymentCheckPPD() {
            var paymentMethod = new HpsPayPlanPaymentMethod {
                PaymentMethodIdentifier = GetIdentifier("CheckPPD"),
                PaymentMethodType = HpsPayPlanPaymentMethodType.Ach,
                AchType = "Checking",
                AccountType = "Personal",
                TelephoneIndicator = false,
                RoutingNumber = "490000018",
                NameOnAccount = "John Doe",
                DriversLicenseNumber = "7418529630",
                DriversLicenseState = "TX",
                AccountNumber = "24413815",
                AddressLine1 = "123 Main St",
                City = "Dallas",
                StateProvince = "TX",
                ZipPostalCode = "98765",
                CustomerKey = _customerPersonKey,
                Country = "USA",
                AccountHolderYob = "1989"
            };

            var response = _payPlanService.AddPaymentMethod(paymentMethod);
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.PaymentMethodKey);
            Assert.IsNotNull(response.CreationDate);

            _paymentMethodKeyCheckPpd = response.PaymentMethodKey;
        }

        [TestMethod]
        public void recurring_006_AddPaymentCheckCCD() {
            var paymentMethod = new HpsPayPlanPaymentMethod {
                PaymentMethodIdentifier = GetIdentifier("CheckCCD"),
                PaymentMethodType = HpsPayPlanPaymentMethodType.Ach,
                AchType = "Checking",
                AccountType = "Business",
                TelephoneIndicator = false,
                RoutingNumber = "490000018",
                NameOnAccount = "Acme Co",
                DriversLicenseNumber = "3692581470",
                DriversLicenseState = "TX",
                AccountNumber = "24413815",
                AddressLine1 = "987 Elm St",
                City = "Princeton",
                StateProvince = "NJ",
                ZipPostalCode = "13245",
                CustomerKey = _customerCompanyKey,
                Country = "USA",
                AccountHolderYob = "1989"
            };

            var response = _payPlanService.AddPaymentMethod(paymentMethod);
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.PaymentMethodKey);
            Assert.IsNotNull(response.CreationDate);

            _paymentMethodKeyCheckCcd = response.PaymentMethodKey;
        }

        // PAYMENT SETUP - DECLINED

        [TestMethod]
        [ExpectedException(typeof(HpsException))]
        public void recurring_007_AddPaymentCheckPPD() {
            var paymentMethod = new HpsPayPlanPaymentMethod {
                PaymentMethodIdentifier = GetIdentifier("CheckPPD"),
                PaymentMethodType = HpsPayPlanPaymentMethodType.Ach,
                AchType = "Checking",
                AccountType = "Personal",
                TelephoneIndicator = false,
                RoutingNumber = "490000018",
                NameOnAccount = "John Doe",
                DriversLicenseNumber = "7418529630",
                DriversLicenseState = "TX",
                AccountNumber = "24413815",
                AddressLine1 = "123 Main St",
                City = "Dallas",
                StateProvince = "TX",
                ZipPostalCode = "98765",
                CustomerKey = _customerPersonKey,
                Country = "USA",
                AccountHolderYob = "1989"
            };

            _payPlanService.AddPaymentMethod(paymentMethod);
        }

        // Recurring Billing using PayPlan - Managed Schedule

        [TestMethod]
        public void recurring_008_AddScheduleCreditVisa() {
            var schedule = new HpsPayPlanSchedule {
                ScheduleIdentifier = GetIdentifier("CreditV"),
                CustomerKey = _customerPersonKey,
                ScheduleStatus = HpsPayPlanScheduleStatus.Active,
                PaymentMethodKey = _paymentMethodKeyVisa,
                SubtotalAmount = new HpsPayPlanAmount("3001"),
                StartDate = "02012027",
                Frequency = HpsPayPlanScheduleFrequency.Weekly,
                Duration = HpsPayPlanScheduleDuration.Ongoing,
                ReprocessingCount = 1
            };

            var response = _payPlanService.AddSchedule(schedule);
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.ScheduleKey);

            _scheduleKeyVisa = response.ScheduleKey;
        }

        [TestMethod]
        public void recurring_009_AddScheduleCreditMasterCard() {
            var schedule = new HpsPayPlanSchedule {
                ScheduleIdentifier = GetIdentifier("CreditMC"),
                CustomerKey = _customerPersonKey,
                ScheduleStatus = HpsPayPlanScheduleStatus.Active,
                PaymentMethodKey = _paymentMethodKeyMasterCard,
                SubtotalAmount = new HpsPayPlanAmount("3002"),
                StartDate = "02012027",
                Frequency = HpsPayPlanScheduleFrequency.Weekly,
                Duration = HpsPayPlanScheduleDuration.EndDate,
                EndDate = "04012027",
                ReprocessingCount = 2
            };

            var response = _payPlanService.AddSchedule(schedule);
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.ScheduleKey);

            _scheduleKeyMasterCard = response.ScheduleKey;
        }

        [TestMethod]
        public void recurring_010_AddScheduleCheckPPD() {
            var schedule = new HpsPayPlanSchedule {
                ScheduleIdentifier = GetIdentifier("CheckPPD"),
                CustomerKey = _customerPersonKey,
                ScheduleStatus = HpsPayPlanScheduleStatus.Active,
                PaymentMethodKey = _paymentMethodKeyCheckPpd,
                SubtotalAmount = new HpsPayPlanAmount("3003"),
                StartDate = "02012027",
                Frequency = HpsPayPlanScheduleFrequency.Monthly,
                Duration = HpsPayPlanScheduleDuration.LimitedNumber,
                ReprocessingCount = 1,
                NumberOfPayments = 2,
                ProcessingDateInfo = "1"
            };

            var response = _payPlanService.AddSchedule(schedule);
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.ScheduleKey);

            _scheduleKeyCheckPpd = response.ScheduleKey;
        }

        [TestMethod]
        public void recurring_011_AddScheduleCheckCCD() {
            var schedule = new HpsPayPlanSchedule {
                ScheduleIdentifier = GetIdentifier("CheckCCD"),
                CustomerKey = _customerCompanyKey,
                ScheduleStatus = HpsPayPlanScheduleStatus.Active,
                PaymentMethodKey = _paymentMethodKeyCheckCcd,
                SubtotalAmount = new HpsPayPlanAmount("3004"),
                StartDate = "02012027",
                Frequency = HpsPayPlanScheduleFrequency.Biweekly,
                Duration = HpsPayPlanScheduleDuration.Ongoing,
                ReprocessingCount = 1
            };

            var response = _payPlanService.AddSchedule(schedule);
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.ScheduleKey);

            _scheduleKeyCheckCcd = response.ScheduleKey;
        }

        [TestMethod]
        [ExpectedException(typeof(HpsException))]
        public void recurring_012_AddScheduleCreditVisa() {
            var schedule = new HpsPayPlanSchedule {
                ScheduleIdentifier = GetIdentifier("CreditV"),
                CustomerKey = _customerPersonKey,
                ScheduleStatus = HpsPayPlanScheduleStatus.Active,
                PaymentMethodKey = _paymentMethodKeyVisa,
                SubtotalAmount = new HpsPayPlanAmount("3001"),
                StartDate = "02012027",
                Frequency = HpsPayPlanScheduleFrequency.Weekly,
                Duration = HpsPayPlanScheduleDuration.Ongoing,
                ReprocessingCount = 1
            };

            _payPlanService.AddSchedule(schedule);
        }

        [TestMethod]
        [ExpectedException(typeof(HpsException))]
        public void recurring_013_AddScheduleCCheckPPD() {
            var schedule = new HpsPayPlanSchedule {
                ScheduleIdentifier = GetIdentifier("CheckPPD"),
                CustomerKey = _customerPersonKey,
                ScheduleStatus = HpsPayPlanScheduleStatus.Active,
                PaymentMethodKey = _paymentMethodKeyCheckPpd,
                SubtotalAmount = new HpsPayPlanAmount("3003"),
                StartDate = "02012027",
                Frequency = HpsPayPlanScheduleFrequency.Monthly,
                Duration = HpsPayPlanScheduleDuration.LimitedNumber,
                ReprocessingCount = 1,
                NumberOfPayments = 2,
                ProcessingDateInfo = "1"
            };

            _payPlanService.AddSchedule(schedule);
        }

        // Recurring Billing using PayPlan - Managed Schedule

        [TestMethod]
        public void recurring_014_RecurringBillingVisa() {
            var response = _creditService.Recurring(20.01m)
                .WithPaymentMethodKey(_paymentMethodKeyVisa)
                .WithScheduleId(_scheduleKeyVisa)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void recurring_015_RecurringBillingMasterCard() {
            var response = _creditService.Recurring(20.02m)
                .WithPaymentMethodKey(_paymentMethodKeyMasterCard)
                .WithScheduleId(_scheduleKeyMasterCard)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void recurring_016_RecurringBillingCheckPPD() {
            var response = _checkService.Recurring(20.03m)
                .WithPaymentMethodKey(_paymentMethodKeyCheckPpd)
                .WithScheduleId(_scheduleKeyCheckPpd)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void recurring_017_RecurringBillingCheckCCD() {
            var response = _checkService.Recurring(20.04m)
                .WithPaymentMethodKey(_paymentMethodKeyCheckCcd)
                .WithScheduleId(_scheduleKeyCheckCcd)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        // One time bill payment

        [TestMethod]
        public void recurring_018_RecurringBillingVisa() {
            var response = _creditService.Recurring(20.06m)
                .WithPaymentMethodKey(_paymentMethodKeyVisa)
                .WithOneTime(true).Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void recurring_019_RecurringBillingMasterCard() {
            var response = _creditService.Recurring(20.07m)
                .WithPaymentMethodKey(_paymentMethodKeyMasterCard)
                .WithOneTime(true).Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("00", response.ResponseCode);
        }

        [TestMethod]
        public void recurring_020_RecurringBillingCheckPPD() {
            var response = _checkService.Recurring(20.08m)
                .WithPaymentMethodKey(_paymentMethodKeyCheckPpd)
                .WithOneTime(true).Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void recurring_021_RecurringBillingCheckCCD() {
            var response = _checkService.Recurring(20.09m)
                .WithPaymentMethodKey(_paymentMethodKeyCheckCcd)
                .WithOneTime(true).Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        // Onetime bill payment - declined

        [TestMethod]
        [ExpectedException(typeof(HpsCreditException))]
        public void recurring_022_RecurringBillingVisa() {
            _creditService.Recurring(10.08m)
                .WithPaymentMethodKey(_paymentMethodKeyVisa)
                .WithOneTime(true).Execute();
        }

        [TestMethod]
        [ExpectedException(typeof(HpsCheckException))]
        public void recurring_023_RecurringBillingCheckPPD() {
            _checkService.Recurring(25.02m)
                .WithPaymentMethodKey(_paymentMethodKeyCheckPpd)
                .WithOneTime(true).Execute();
        }

        [TestMethod]
        public void recurring_999_CloseBatch()
        {
            var response = _batchService.CloseBatch();
            Assert.IsNotNull(response);
            Console.WriteLine(@"Batch ID: {0}", response.Id);
            Console.WriteLine(@"Sequence Number: {0}", response.SequenceNumber);
        }
    }
}