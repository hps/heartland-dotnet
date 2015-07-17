// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HpsChargeTransaction.cs" company="Heartland Payment Systems">
//   Copyright (c) Heartland Payment Systems. All rights reserved.
// </copyright>
// <summary>
//   The HPS transaction.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Hps.Exchange.PosGateway.Client;
namespace SecureSubmit.Entities {
    /// <summary>The HPS charge transaction.</summary>
    public class HpsCharge : HpsAuthorization {
        internal new HpsCharge FromResponse(PosResponseVer10 response) {
            base.FromResponse(response);
            return this;
        }
    }
}