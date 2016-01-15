using System.Net;
using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Infrastructure.Validation;

namespace SecureSubmit.Services
{
    /// <summary>The HPS batch service.</summary>
    public class HpsBatchService : HpsSoapGatewayService
    {
        /// <summary>Initializes a new instance of the <see cref="HpsBatchService"/> class.</summary>
        /// <param name="config">The HPS services config.</param>
        public HpsBatchService(IHpsServicesConfig config = null)
            : base(config)
        {
        }

        /// <summary>
        /// A <b>Batch Close</b> transaction instructs the Heartland POS Gateway to close the current open
        /// batch and settle it. If a batch is not open, an error will be returned. 
        /// </summary>
        /// <returns>The HPS batch.</returns>
        public HpsBatch CloseBatch()
        {
            /* Build the transaction request. */
            var transaction = new PosRequestVer10Transaction
            {
                ItemElementName = ItemChoiceType1.BatchClose,
                Item = "BatchClose"
            };

            /* Submit the transaction. */
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            var rsp = DoTransaction(transaction).Ver10;
            HpsGatewayResponseValidation.CheckResponse(rsp, ItemChoiceType2.BatchClose);
            
            var batchClose = (PosBatchCloseRspType)rsp.Transaction.Item;
            return new HpsBatch
            {
                Id = batchClose.BatchId,
                SequenceNumber = batchClose.BatchSeqNbr,
                TotalAmount = batchClose.TotalAmt,
                TransactionCount = batchClose.TxnCnt
            };
        }
    }
}