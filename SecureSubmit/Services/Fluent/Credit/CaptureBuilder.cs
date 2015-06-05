using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Infrastructure.Validation;
using SecureSubmit.Services.Credit;

namespace SecureSubmit.Services.Fluent.Credit
{
    public class CaptureBuilder : GatewayTransactionBuilder<CaptureBuilder, HpsTransaction>
    {
        public CaptureBuilder(IHpsServicesConfig config, int transactionId)
            : base(config)
        {
            BuilderActions.Add(n =>
                {
                    n.Transaction = new PosRequestVer10Transaction
                        {
                            Item = new PosCreditAddToBatchReqType
                                {
                                    GatewayTxnId = transactionId
                                },
                            ItemElementName = ItemChoiceType1.CreditAddToBatch
                        };
                });
        }

        public override HpsTransaction Execute()
        {
            BuilderActions.ForEach(ba => ba(this));

            var rsp = DoTransaction().Ver10;
            HpsGatewayResponseValidation.CheckResponse(rsp, ItemChoiceType2.CreditAddToBatch);

            return new HpsTransaction
            {
                Header = HydrateTransactionHeader(rsp.Header),
                TransactionId = rsp.Header.GatewayTxnId,
                ClientTransactionId = GetClientTransactionId(rsp.Header),
                ResponseCode = "00",
                ResponseText = string.Empty
            };
        }

        public CaptureBuilder WithAmount(decimal amount)
        {
            BuilderActions.Add(n =>
                {
                    ((PosCreditAddToBatchReqType) n.Transaction.Item).Amt = amount;
                    ((PosCreditAddToBatchReqType) n.Transaction.Item).AmtSpecified = true;
                });

            return this;
        }

        public CaptureBuilder WithGratuity(decimal gratuityAmount)
        {
            BuilderActions.Add(n =>
                {
                    ((PosCreditAddToBatchReqType) n.Transaction.Item).GratuityAmtInfo = gratuityAmount;
                    ((PosCreditAddToBatchReqType) n.Transaction.Item).GratuityAmtInfoSpecified = true;
                });

            return this;
        }

        public CaptureBuilder WithDirectMarketData(HpsDirectMarketData directMarketData)
        {
            BuilderActions.Add(n => ((PosCreditAddToBatchReqType) n.Transaction.Item).DirectMktData =
                                    HydrateDirectMktData(directMarketData));
            return this;
        }
    }
}
