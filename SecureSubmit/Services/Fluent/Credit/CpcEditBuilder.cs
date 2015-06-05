using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Infrastructure.Validation;

namespace SecureSubmit.Services.Fluent.Credit
{
    public class CpcEditBuilder : GatewayTransactionBuilder<CpcEditBuilder, HpsTransaction>
    {
        public CpcEditBuilder(IHpsServicesConfig config, int transactionId)
            : base(config)
        {
            BuilderActions.Add(n =>
                {
                    n.Transaction = new PosRequestVer10Transaction
                        {
                            Item = new PosCreditCPCEditReqType
                                {
                                    GatewayTxnId = transactionId
                                },
                            ItemElementName = ItemChoiceType1.CreditCPCEdit
                        };
                });
        }

        public override HpsTransaction Execute()
        {
            BuilderActions.ForEach(ba => ba(this));

            var rsp = DoTransaction().Ver10;
            HpsGatewayResponseValidation.CheckResponse(rsp, ItemChoiceType2.CreditCPCEdit);

            return new HpsTransaction
            {
                Header = HydrateTransactionHeader(rsp.Header),
                TransactionId = rsp.Header.GatewayTxnId,
                ClientTransactionId = GetClientTransactionId(rsp.Header),
                ResponseCode = "00",
                ResponseText = string.Empty
            };
        }

        public CpcEditBuilder WithCpcData(HpsCpcData cpcData)
        {
            BuilderActions.Add(n => ((PosCreditCPCEditReqType)n.Transaction.Item).CPCData = HydrateCpcData(cpcData));
            return this;
        }
    }
}
