namespace SecureSubmit.Entities.PayPlan
{
    public static class HpsPayPlanPaymentMethodStatus
    {
        public static string Active
        {
            get { return "Active"; }
        }

        public static string Inactive
        {
            get { return "Inactive"; }
        }

        public static string Invalid
        {
            get { return "Invalid"; }
        }

        public static string Revoked
        {
            get { return "Revoked"; }
        }

        public static string Expired
        {
            get { return "Expired"; }
        }

        public static string LostStolen
        {
            get { return "Lost/Stolen"; }
        }
    }
}
