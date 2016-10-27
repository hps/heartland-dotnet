using Hps.Exchange.PosGateway.Client;

namespace SecureSubmit.Entities
{
    public class HpsAttachment : HpsTransaction
    {
        /// <summary>Gets or sets the AttachmentType.</summary>
        public string AttachmentType { get; set; }
        /// <summary>Gets or sets the AttachmentData. This will be a base64 encoded value representing the image or document</summary>
        public string AttachmentData { get; set; }
        /// <summary>Gets or sets the AttachmentFormat. </summary>
        public string AttachmentFormat { get; set; }
        /// <summary>Gets or sets the Height in pixels. </summary>
        public int Height { get; set; }
        /// <summary>Gets or sets the Width in pixels.</summary>
        public int Width { get; set; }
        /// <summary>Gets or sets the AttachmentName.</summary>
        public string AttachmentName { get; set; }
        /// <summary>Gets or sets the AttachmentDataId.</summary>
        public int AttachmentDataId { get; set; }

        internal new HpsAttachment FromResponse(PosResponseVer10 response)
        {
            base.FromResponse(response);
            this.ResponseCode = response.Header.GatewayRspCode.ToString();
            this.ResponseText = response.Header.GatewayRspMsg;
            if (response.Transaction == null)
                return this;
            var attachResponse = (PosGetAttachmentsRspType)response.Transaction.Item;

            
            if (attachResponse != null && attachResponse.Details != null)
                {
                this.AttachmentData = attachResponse.Details[0].AttachmentData;
                this.AttachmentDataId = attachResponse.Details[0].AttachmentDataId;
                this.AttachmentType = attachResponse.Details[0].AttachmentType;
                this.AttachmentFormat = attachResponse.Details[0].AttachmentFormat;
                this.AttachmentName = attachResponse.Details[0].AttachmentName;
                this.Height = attachResponse.Details[0].Height;
                this.Width = attachResponse.Details[0].Width;
                }
            return this;
            
        }
    }
}
