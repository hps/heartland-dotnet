using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Entities.Credit;
using SecureSubmit.Infrastructure.Validation;

namespace SecureSubmit.Services.Fluent.Debit
{
    public class ReverseBuilder : GatewayTransactionBuilder<ReverseBuilder, HpsTransaction>
    {
        public class ReverseUsingBuilder
        {
            private readonly ReverseBuilder _parent;

            public ReverseUsingBuilder(ReverseBuilder parent)
            {
                _parent = parent;
            }

            public ReverseBuilder WithTransactionId(int transactionId)
            {
                _parent.BuilderActions.Add(n =>
                {
                    ((PosDebitReversalReqType)n.Transaction.Item).Block1.GatewayTxnId = transactionId;
                    ((PosDebitReversalReqType)n.Transaction.Item).Block1.GatewayTxnIdSpecified = true;
                });

                return _parent;
            }

            public ReverseBuilder WithTrackData(string trackData)
            {
                _parent.BuilderActions.Add(n => ((PosDebitReversalReqType)n.Transaction.Item).Block1.TrackData = trackData);
                return _parent;
            }
        }

        public ReverseBuilder(IHpsServicesConfig config, decimal amount) : base(config)
        {
            BuilderActions.Add(n =>
                {
                    n.Transaction = new PosRequestVer10Transaction
                        {
                            Item = new PosDebitReversalReqType
                                {
                                    Block1 = new DebitReversalReqBlock1Type
                                        {
                                            Amt = amount
                                        }
                                },
                            ItemElementName = ItemChoiceType1.DebitReversal
                        };
                });
        }

        public override HpsTransaction Execute()
        {
            BuilderActions.ForEach(ba => ba(this));

            var rsp = DoTransaction().Ver10;
            HpsGatewayResponseValidation.CheckResponse(rsp, ItemChoiceType2.DebitReversal);

            return new HpsTransaction
            {
                Header = HydrateTransactionHeader(rsp.Header),
                TransactionId = rsp.Header.GatewayTxnId,
                ClientTransactionId = GetClientTransactionId(rsp.Header),
                ResponseCode = "00",
                ResponseText = string.Empty
            };
        }

        public ReverseBuilder WithAuthAmount(decimal authAmount)
        {
            BuilderActions.Add(n =>
            {
                ((PosDebitReversalReqType) n.Transaction.Item).Block1.AuthAmt = authAmount;
                ((PosDebitReversalReqType)n.Transaction.Item).Block1.AuthAmtSpecified = true;

            });
            return this;
        }

        public ReverseBuilder WithAdditionalTransactionFields(HpsAdditionalTransactionFields additionalTransactionFields)
        {
            BuilderActions.Add(n => ((PosDebitReversalReqType)n.Transaction.Item).Block1.AdditionalTxnFields =
                                    HydrateAdditionalTxnFields(additionalTransactionFields));
            return this;
        }

        public ReverseBuilder WithEncryptionData(HpsEncryptionData encryptionData)
        {
            BuilderActions.Add(n => ((PosDebitReversalReqType)n.Transaction.Item).Block1.EncryptionData =
                                    HydrateEncryptionData(encryptionData));
            return this;
        }
    }
}
