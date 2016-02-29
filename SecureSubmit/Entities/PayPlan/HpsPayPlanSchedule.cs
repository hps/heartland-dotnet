using System;
using System.Collections.Generic;

namespace SecureSubmit.Entities
{
    public class HpsPayPlanSchedule : HpsPayPlanResource
    {
        private string _emailReceipt = "Never";
        private string _emailAdvanceNotice = "No";

        public string ScheduleKey { get; set; }
        public string ScheduleIdentifier { get; set; }
        public string CustomerKey { get; set; }
        public string ScheduleName { get; set; }
        public string ScheduleStatus { get; set; }
        public string PaymentMethodKey { get; set; }
        public HpsPayPlanAmount SubtotalAmount { get; set; }
        public HpsPayPlanAmount TaxAmount { get; set; }
        public HpsPayPlanAmount TotalAmount { get; set; }
        public int? DeviceId { get; set; }

        [FormatDate]
        public string StartDate { get; set; }
        public string ProcessingDateInfo { get; set; }
        public string Frequency { get; set; }
        public string Duration { get; set; }
        
        [FormatDate]
        public string EndDate { get; set; }
        public int? ReprocessingCount { get; set; }

        public string EmailReceipt
        {
            get { return _emailReceipt; }
            set { _emailReceipt = value; }
        }

        public string EmailAdvanceNotice
        {
            get { return _emailAdvanceNotice; }
            set { _emailAdvanceNotice = value; }
        }

        public string NextProcessingDate { get; set; }
        public string PreviousProcessingDate { get; set; }
        public int? ApprovedTransactionCount { get; set; }
        public int? FailureCount { get; set; }
        private HpsPayPlanAmount TotalApprovedAmountToDate { get; set; }
        public int? NumberOfPayments { get; set; }
        public int? NumberOfPaymentsRemaining { get; set; }
        
        [FormatDate]
        public string CancellationDate { get; set; }
        public string ScheduleStarted { get; set; }

        public HpsPayPlanSchedule()
        {
            this.EmailReceipt = "Never";
            this.EmailAdvanceNotice = "No";
        }

        private static IEnumerable<string> GetEditableFields(bool isStarted)
        {
            var rvalue = new List<string> {
                "ScheduleName",
                "ScheduleStatus",
                "DeviceId",
                "PaymentMethodKey",
                "SubtotalAmount",
                "TaxAmount",
                "NumberOfPaymentsRemaining",
                "EndDate",
                "CancellationDate",
                "ReprocessingCount",
                "EmailReceipt",
                "EmailAdvanceNotice",
                "ProcessingDateInfo",
            };

            if (!isStarted) {
                rvalue.AddRange(new[] {
                    "ScheduleIdentifier",
                    "StartDate",
                    "Frequency",
                    "Duration",
                });
            }
            else rvalue.Add("NextProcessingDate");
            return rvalue;
        }

        internal Dictionary<String, Object> GetEditableFieldsWithValues()
        {
            var map = new Dictionary<string, object>();

            foreach (var fieldName in GetEditableFields(bool.Parse(ScheduleStarted ?? "False")))
            {
                var prop = GetType().GetProperty(fieldName);
                var value = prop.GetValue(this, null);
                if (value == null) continue;
                if (prop.GetCustomAttributes(typeof (FormatDate), true).Length > 0)
                {
                    value = FormatDate((string)value);
                }
                    
                map.Add(fieldName, value);
            }

            return map;
        }

        private static string FormatDate(string input)
        {
            DateTime d;
            return DateTime.TryParse(input, out d) ? d.ToString("MMddyyyy") : input;
        }
    }
}
