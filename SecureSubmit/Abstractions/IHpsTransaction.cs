using System;
using System.Collections.Generic;
using System.Text;
using Hps.Exchange.PosGateway.Client;

namespace SecureSubmit.Abstractions
{
    interface IHpsTransaction<T>
    {
        T FromResponse(PosResponseVer10 response);
    }
}
