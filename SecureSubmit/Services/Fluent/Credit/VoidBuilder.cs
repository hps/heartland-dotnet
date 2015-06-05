using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Infrastructure.Validation;

namespace SecureSubmit.Services.Fluent.Credit
{
    public class VoidBuilder : GatewayTransactionBuilder<VoidBuilder, HpsTransaction>
    {
        public VoidBuilder(IHpsServicesConfig config, int transactionId)
            : base(config)
        {
            BuilderActions.Add(n =>
                {
                    n.Transaction = new PosRequestVer10Transaction
                        {
                            Item = new PosCreditVoidReqType
                                {
                                    GatewayTxnId = transactionId
                                },
                            ItemElementName = ItemChoiceType1.CreditVoid
                        };
                });
        }

        public override HpsTransaction Execute()
        {
            BuilderActions.ForEach(ba => ba(this));

            var rsp = DoTransaction().Ver10;
            HpsGatewayResponseValidation.CheckResponse(rsp, ItemChoiceType2.CreditVoid);

            return new HpsTransaction
            {
                Header = HydrateTransactionHeader(rsp.Header),
                TransactionId = rsp.Header.GatewayTxnId,
                ClientTransactionId = GetClientTransactionId(rsp.Header),
                ResponseCode = "00",
                ResponseText = string.Empty
            };
        }
    }
}
