using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Infrastructure.Validation;

namespace SecureSubmit.Services.Fluent.Batch
{
    public class CloseBuilder : GatewayTransactionBuilder<CloseBuilder, HpsBatch>
    {
        public CloseBuilder(IHpsServicesConfig config)
            : base(config)
        {
            BuilderActions.Add(n =>
                {
                    n.Transaction = new PosRequestVer10Transaction
                        {
                            ItemElementName = ItemChoiceType1.BatchClose,
                            Item = "BatchClose"
                        };
                });
        }

        public override HpsBatch Execute()
        {
            BuilderActions.ForEach(ba => ba(this));

            var rsp = DoTransaction().Ver10;
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
