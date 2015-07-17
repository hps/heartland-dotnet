// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HpsRefund.cs" company="Heartland Payment Systems">
//   Copyright (c) Heartland Payment Systems. All rights reserved.
// </copyright>
// <summary>
//   The HPS refund.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Hps.Exchange.PosGateway.Client;
namespace SecureSubmit.Entities
{
    /// <summary>The HPS refund.</summary>
    public class HpsRefund : HpsTransaction
    {
        internal new HpsRefund FromResponse(PosResponseVer10 response) {
            base.FromResponse(response);
            return this;
        }
    }
}