using Hps.Exchange.PosGateway.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureSubmit.Entities
{
   public class HpsTagDataType
    {
        public string TagData { get; set; }

        public TagDataTypeTagValuesSource Source { get; set; }
    }
}
