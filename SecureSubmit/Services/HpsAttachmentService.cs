using System.Globalization;
using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Infrastructure;
using SecureSubmit.Infrastructure.Validation;
using System;
using System.Collections.Generic;

namespace SecureSubmit.Services
{
    public class HpsAttachmentService : HpsSoapGatewayService
    {
        /// <summary>Initializes a new instance of the <see cref="HpsAttachmentService"/> class.</summary>
        /// <param name="config">The HPS services config.</param>
        public HpsAttachmentService(IHpsServicesConfig config = null)
            : base(config)
        {
        }
       
        public HpsAttachment GetAttachment(int transactionId, attachmentTypeType? attachmentType = null, bool returnAttachmentTypesOnly = false, int? attachmentDataId = null)
        {
            /* Build the transaction request. */
            var attType = getAttachmentTypeType(attachmentType);
            var transaction = new PosRequestVer10Transaction
            {
                ItemElementName = ItemChoiceType1.GetAttachments,
                Item = new PosGetAttachmentReqType
                {
                    GatewayTxnId = transactionId,
                    ReturnAttachmentTypesOnly = returnAttachmentTypesOnly,
                    AttachmentType = attType,
                    ReturnAttachmentTypesOnlySpecified = returnAttachmentTypesOnly,
                    AttachmentDataIdSpecified = attType.Length < 1 && attachmentDataId != null,
                    AttachmentDataId = attachmentDataId != null?(int) attachmentDataId:0,
                }
            };
            var resp = SubmitGet(transaction);
            return resp;
        }
        /// <summary>Submit a refund transaction.</summary>
        /// <param name="transaction">The GetAttachments transaction.</param>
        /// <returns>The <see cref="HpsAttachment"/>.</returns>
        private HpsAttachment SubmitGet(PosRequestVer10Transaction transaction)
        {
            var rsp = DoTransaction(transaction).Ver10;
            ProcessChargeGatewayResponse(rsp, ItemChoiceType2.GetAttachments);
            HpsAttachment resp = new HpsAttachment();
            resp.FromResponse(rsp);
            return resp; 
        }

        private void ProcessChargeGatewayResponse(PosResponseVer10 rsp, ItemChoiceType2 expectedResponseType)
        {
            if (rsp.Header.GatewayRspCode == 0) return;
            if (rsp.Header.GatewayRspCode == 3) return;
            HpsGatewayResponseValidation.CheckResponse(rsp, expectedResponseType);
        }
        private attachmentTypeType[] getAttachmentTypeType(attachmentTypeType? attachmentType = null)
        {
            attachmentTypeType[] attTypeType = new attachmentTypeType[0] { }; ;
            if (attachmentType != null)
            {
                Array.Resize<attachmentTypeType>(ref attTypeType, 1);
                attTypeType[0] = (attachmentTypeType)attachmentType ;
            }
            return attTypeType;
        }

    }
}
