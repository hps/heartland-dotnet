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
    public class Schedule
    {
        private readonly HpsPayPlanService _payPlanService = new HpsPayPlanService(new HpsServicesConfig
        {
            SecretApiKey = "skapi_uat_MY5OAAAUrmIFvLDRpO_ufLlFQkgg0Rms2G8WoI1THQ"
        });

        private readonly HpsPayPlanPaymentMethod _paymentMethod;

        public Schedule()
        {
            _payPlanService.SetPagination(1, 0);
            var searchFilters = new Dictionary<string, object>
            {
                {"customerIdentifier", "SecureSubmit"},
                {"paymentStatus", HpsPayPlanCustomerStatus.Active}
            };

            var paymentMethods = _payPlanService.FindAllPaymentMethods(searchFilters);
            _paymentMethod = paymentMethods.Results[0];
        }

        private static string GenerateScheduleId()
        {
            return new DateTime().ToString("yyyyMMdd") + "-SecureSubmit-" + Guid.NewGuid().ToString().Substring(0, 10);
        }

        private static string GetLastDayOfMonth()
        {
            var today = DateTime.Today;
            var lastDayOfMonth = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));

            return lastDayOfMonth.ToShortDateString();
        }

        [TestMethod]
        public void AddSchedule()
        {
            var id = GenerateScheduleId();
            var date = GetLastDayOfMonth();

            var schedule = new HpsPayPlanSchedule
            {
                ScheduleIdentifier = id,
                CustomerKey = _paymentMethod.CustomerKey,
                PaymentMethodKey = _paymentMethod.PaymentMethodKey,
                SubtotalAmount = new HpsPayPlanAmount("100"),
                StartDate = date,
                Frequency = HpsPayPlanScheduleFrequency.Weekly,
                Duration = HpsPayPlanScheduleDuration.LimitedNumber,
                NumberOfPayments = 3,
                ReprocessingCount = 2,
                EmailReceipt = "Never",
                EmailAdvanceNotice = "No",
                ScheduleStatus = HpsPayPlanScheduleStatus.Active
            };

            var result = _payPlanService.AddSchedule(schedule);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.ScheduleKey);
        }

        [TestMethod]
        [ExpectedException(typeof(HpsInvalidRequestException), "schedule must be an instance of HpsPayPlanSchedule.")]
        public void AddNullSchedule()
        {
            _payPlanService.AddSchedule(null);
        }

        [TestMethod]
        public void EditSchedule()
        {
            _payPlanService.SetPagination(1, 0);
            var searchFilter = new Dictionary<string, object> {{"customerIdentifier", "SecureSubmit"}};
            var schedules = _payPlanService.FindAllSchedules(searchFilter);
            Assert.IsNotNull(schedules);
            Assert.IsTrue(schedules.Results.Length >= 1);

            // Make the edit
            var schedule = schedules.Results[0];
            var scheduleStatus = schedule.ScheduleStatus.Equals(HpsPayPlanScheduleStatus.Active) ?
                HpsPayPlanScheduleStatus.Inactive : HpsPayPlanScheduleStatus.Active;
            schedule.ScheduleStatus = scheduleStatus;

            var editResult = _payPlanService.EditSchedule(schedule);
            Assert.IsNotNull(schedules);
            Assert.AreEqual(schedule.ScheduleKey, editResult.ScheduleKey);
            Assert.AreEqual(schedule.ScheduleStatus, editResult.ScheduleStatus);

            // Verify the edit
            var verifyResult = _payPlanService.GetSchedule(schedule.ScheduleKey);
            Assert.IsNotNull(schedules);
            Assert.AreEqual(schedule.ScheduleKey, verifyResult.ScheduleKey);
            Assert.AreEqual(schedule.ScheduleStatus, verifyResult.ScheduleStatus);
        }

        [TestMethod]
        [ExpectedException(typeof(HpsInvalidRequestException), "schedule must be an instance of HpsPayPlanSchedule.")]
        public void EditNullSchedule()
        {
            _payPlanService.EditSchedule(null);
        }

        [TestMethod]
        public void FindAllSchedules()
        {
            var results = _payPlanService.FindAllSchedules();
            Assert.IsNotNull(results);
            Assert.IsTrue(results.Results.Length >= 1);
        }

        [TestMethod]
        public void FindAllSchedulesWithPaging()
        {
            _payPlanService.SetPagination(1, 0);
            var results = _payPlanService.FindAllSchedules();
            Assert.IsNotNull(results);
            Assert.IsTrue(results.Results.Length == 1);
        }

        [TestMethod]
        public void FindAllSchedulesWithFilter()
        {
            var searchFilter = new Dictionary<string, object> {{"customerIdentifier", "SecureSubmit"}};
            var schedules = _payPlanService.FindAllSchedules(searchFilter);
            Assert.IsNotNull(schedules);
            Assert.IsTrue(schedules.Results.Length >= 1);
        }

        [TestMethod]
        public void GetSchedulesBySchedule()
        {
            _payPlanService.SetPagination(1, 0);
            var schedules = _payPlanService.FindAllSchedules();
            Assert.IsNotNull(schedules);
            Assert.IsTrue(schedules.Results.Length == 1);

            var result = _payPlanService.GetSchedule(schedules.Results[0]);
            Assert.IsNotNull(result);
            Assert.AreEqual(schedules.Results[0].ScheduleKey, result.ScheduleKey);
        }

        [TestMethod]
        public void GetSchedulesByScheduleId()
        {
            _payPlanService.SetPagination(1, 0);
            var schedules = _payPlanService.FindAllSchedules();
            Assert.IsNotNull(schedules);
            Assert.IsTrue(schedules.Results.Length == 1);

            var result = _payPlanService.GetSchedule(schedules.Results[0].ScheduleKey);
            Assert.IsNotNull(result);
            Assert.AreEqual(schedules.Results[0].ScheduleKey, result.ScheduleKey);
        }

        [TestMethod]
        public void DeleteSchedulesBySchedule()
        {
            AddSchedule();

            _payPlanService.SetPagination(1, 0);
            var schedules = _payPlanService.FindAllSchedules();
            Assert.IsNotNull(schedules);
            Assert.IsTrue(schedules.Results.Length == 1);

            var result = _payPlanService.DeleteSchedule(schedules.Results[0]);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void DeleteSchedulesByScheduleId()
        {
            AddSchedule();

            _payPlanService.SetPagination(1, 0);
            var schedules = _payPlanService.FindAllSchedules();
            Assert.IsNotNull(schedules);
            Assert.IsTrue(schedules.Results.Length == 1);

            var result = _payPlanService.DeleteSchedule(schedules.Results[0].ScheduleKey);
            Assert.IsNull(result);
        }
    }
}
