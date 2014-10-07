namespace SecureSubmit.Entities
{
    public class HpsCheckHolder : HpsConsumer
    {
        /// <summary>Business name on check. NOTE: If processing with Colonnade, CheckName is required.</summary>
        public string CheckName { get; set; }

        /// <summary>Driver license state of the consumer.</summary>
        public string DlState { get; set; }

        /// <summary>Driver license number of the consumer.</summary>
        public string DlNumber { get; set; }

        /// <summary>Last four digits of the consumer's SSN.</summary>
        public string Ssl4 { get; set; }

        /// <summary>Date of the birth year of the consumer.</summary>
        public int? DobYear { get; set; }

        /// <summary>Courtesy card number.</summary>
        public string CourtesyCard { get; set; }
    }
}
