using Hps.Exchange.PosGateway.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureSubmit.Entities
{
    public partial class HpsEmvDataType
    {
        public string TagData { get; set; }

        public emvChipConditionType ChipCondition { get; set; }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ChipConditionSpecified { get; set; }

        public string PinBlock { get; set; }
    }
}
