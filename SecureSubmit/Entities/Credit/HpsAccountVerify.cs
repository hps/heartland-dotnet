// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HpsAccountVerify.cs" company="Heartland Payment Systems">
//   Copyright (c) Heartland Payment Systems. All rights reserved.
// </copyright>
// <summary>
//   The HPS account verify.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Hps.Exchange.PosGateway.Client;
namespace SecureSubmit.Entities
{
    /// <summary>The HPS account verify.</summary>
    public class HpsAccountVerify : HpsAuthorization
    {
        internal new HpsAccountVerify FromResponse(PosResponseVer10 response) {
            base.FromResponse(response);
            return this;
        }
    }
}