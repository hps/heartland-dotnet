using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Hps.Exchange.PosGateway.Client;

namespace SecureSubmit.Entities
{
    public class HpsCpcData
    {
        private string _cardHolderPoNumber;
        private decimal? _taxAmount;

        public taxTypeType? TaxType { get; set; }

        public string CardHolderPoNumber
        {
            get { return _cardHolderPoNumber; }
            set
            {
                if (value != null && value.Length > 17)
                    throw new ArgumentException("cardHolderPoNumber can't be greater than 17 characters");
                _cardHolderPoNumber = value;
            }
        }

        public decimal? TaxAmount
        {
            get { return _taxAmount; }
            set
            {
                var rgx = new Regex("^(\\d{0,10})(\\.\\d{2})?$");
                if (value.HasValue && !rgx.IsMatch(value.Value.ToString(CultureInfo.InvariantCulture)))
                    throw new ArgumentException("taxAmt must be <= 12 digits (10 before the decimal and 2 after).");
                _taxAmount = value;
            }
        }
    }
}
