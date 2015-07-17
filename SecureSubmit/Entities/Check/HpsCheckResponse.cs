// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HpsCheckResponse.cs" company="Heartland Payment Systems">
//   Copyright (c) Heartland Payment Systems. All rights reserved.
// </copyright>
// <summary>
//   The HPS check response.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SecureSubmit.Entities
{
    using System.Collections.Generic;
    using Hps.Exchange.PosGateway.Client;

    public class HpsCheckResponse : HpsTransaction
    {
        /// <summary>Gets or sets the authorization code.</summary>
        public string AuthorizationCode { get; set; }

        /// <summary>Gets or sets the customer ID.</summary>
        public string CustomerId { get; set; }

        public List<HpsCheckResponseDetails> Details { get; set; }

        internal HpsCheckResponse FromResponse(PosResponseVer10 response) {
            base.FromResponse(response);

            if (response.Transaction.Item is PosCheckSaleRspType) {
                var trans = (PosCheckSaleRspType)response.Transaction.Item;

                ResponseCode = trans.RspCode.ToString();
                ResponseText = trans.RspMessage;
                AuthorizationCode = trans.AuthCode;

                if (trans.CheckRspInfo != null) {
                    Details = new List<HpsCheckResponseDetails>();
                    foreach (var rspInfo in trans.CheckRspInfo) {
                        var detail = new HpsCheckResponseDetails {
                            MessageType = rspInfo.Type,
                            Code = rspInfo.Code,
                            Message = rspInfo.Message,
                            FieldNumber = rspInfo.FieldNumber,
                            FieldName = rspInfo.FieldName
                        };
                        Details.Add(detail);
                    }
                }
            }
            else if (response.Transaction.Item is PosCheckVoidRspType) {
                var trans = (PosCheckVoidRspType)response.Transaction.Item;

                ResponseCode = trans.RspCode.ToString();
                ResponseText = trans.RspMessage;
                AuthorizationCode = trans.AuthCode;

                if (trans.CheckRspInfo != null) {
                    Details = new List<HpsCheckResponseDetails>();
                    foreach (var rspInfo in trans.CheckRspInfo) {
                        var detail = new HpsCheckResponseDetails {
                            MessageType = rspInfo.Type,
                            Code = rspInfo.Code,
                            Message = rspInfo.Message,
                            FieldNumber = rspInfo.FieldNumber,
                            FieldName = rspInfo.FieldName
                        };
                        Details.Add(detail);
                    }
                }
            }

            return this;
        }
    }
}
