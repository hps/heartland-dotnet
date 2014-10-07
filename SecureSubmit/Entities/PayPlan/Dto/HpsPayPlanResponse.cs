using System.Collections.Generic;

namespace SecureSubmit.Entities.PayPlan.Dto
{
    public class HpsPayPlanResponse<T>
    {
        public int TotalResultCount { get; set; }
        
        public List<T> Results { get; set; }
    }
}
