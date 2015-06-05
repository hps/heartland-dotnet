using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Infrastructure.Validation;

namespace SecureSubmit.Services.Fluent.Credit
{
    public class EditBuilder : GatewayTransactionBuilder<EditBuilder, HpsTransaction>
    {
        public class EditUsingBuilder
        {
            private readonly EditBuilder _parent;

            public EditUsingBuilder(EditBuilder parent)
            {
                _parent = parent;
            }

            public EditBuilder WithTransactionId(int transactionId)
            {
                _parent.BuilderActions.Add(n =>
                    {
                        ((PosCreditTxnEditReqType) n.Transaction.Item).GatewayTxnId = transactionId;
                    });

                return _parent;
            }
        }

        public EditBuilder(IHpsServicesConfig config)
            : base(config)
        {
            BuilderActions.Add(n =>
                {
                    n.Transaction = new PosRequestVer10Transaction
                        {
                            Item = new PosCreditTxnEditReqType(),
                            ItemElementName = ItemChoiceType1.CreditTxnEdit
                        };
                });
        }

        public override HpsTransaction Execute()
        {
            BuilderActions.ForEach(ba => ba(this));

            var rsp = DoTransaction().Ver10;
            HpsGatewayResponseValidation.CheckResponse(rsp, ItemChoiceType2.CreditTxnEdit);

            return new HpsTransaction
            {
                Header = HydrateTransactionHeader(rsp.Header),
                TransactionId = rsp.Header.GatewayTxnId,
                ClientTransactionId = GetClientTransactionId(rsp.Header),
                ResponseCode = "00",
                ResponseText = string.Empty
            };
        }

        public EditBuilder WithGratuity(decimal gratuity)
        {
            BuilderActions.Add(n =>
            {
                ((PosCreditTxnEditReqType)n.Transaction.Item).GratuityAmtInfo = gratuity;
                ((PosCreditTxnEditReqType)n.Transaction.Item).GratuityAmtInfoSpecified = true;
            });

            return this;
        }

        public EditBuilder WithAmount(decimal amount)
        {
            BuilderActions.Add(n =>
            {
                ((PosCreditTxnEditReqType)n.Transaction.Item).Amt = amount;
                ((PosCreditTxnEditReqType)n.Transaction.Item).AmtSpecified = true;
            });

            return this;
        }

        public EditBuilder WithSurchargeAmount(decimal amount)
        {
            BuilderActions.Add(n =>
            {
                ((PosCreditTxnEditReqType)n.Transaction.Item).SurchargeAmtInfo = amount;
                ((PosCreditTxnEditReqType)n.Transaction.Item).SurchargeAmtInfoSpecified = true;
            });

            return this;
        }

        public EditBuilder WithDirectMarketData(HpsDirectMarketData directMarketData)
        {
            BuilderActions.Add(n => ((PosCreditTxnEditReqType)n.Transaction.Item).DirectMktData =
                                    HydrateDirectMktData(directMarketData));
            return this;
        }
    }
}
