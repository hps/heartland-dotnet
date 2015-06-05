using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Entities.Credit;
using SecureSubmit.Infrastructure.Validation;

namespace SecureSubmit.Services.Fluent.Credit
{
    public class RecurringBuilder : GatewayTransactionBuilder<RecurringBuilder, HpsAuthorization>
    {
        public class RecurringPaymentTypeBuilder
        {
            private readonly RecurringBuilder _parent;

            public RecurringPaymentTypeBuilder(RecurringBuilder parent)
            {
                _parent = parent;
            }

            public RecurringBuilder WithCard(HpsCreditCard card)
            {
                _parent.BuilderActions.Add(
                    n => ((PosRecurringBillReqType) n.Transaction.Item).Block1.CardData = new CardDataType
                    {
                        Item = HydrateCardManualEntry(card)
                    });

                return _parent;
            }

            public RecurringBuilder WithToken(string token)
            {
                _parent.BuilderActions.Add(
                    n => ((PosRecurringBillReqType) n.Transaction.Item).Block1.CardData = new CardDataType
                    {
                        Item = new CardDataTypeTokenData
                        {
                            TokenValue = token
                        }
                    });

                return _parent;
            }

            public RecurringBuilder WithPaymentMethodKey(string paymentMethodKey)
            {
                _parent.BuilderActions.Add(n => ((PosRecurringBillReqType)n.Transaction.Item).Block1.PaymentMethodKey = paymentMethodKey);
                return _parent;
            }
        }

        public RecurringBuilder(IHpsServicesConfig config, decimal amount) : base(config)
        {
            BuilderActions.Add(n =>
                {
                    n.Transaction = new PosRequestVer10Transaction
                        {
                            Item = new PosRecurringBillReqType
                                {
                                    Block1 = new RecurringBillReqBlock1Type
                                        {
                                            AllowDup = booleanType.N,
                                            AllowDupSpecified = true,
                                            Amt = amount,
                                            RecurringData = new RecurringDataType
                                            {
                                                OneTime = booleanType.N,
                                                OneTimeSpecified = true
                                            }
                                        }
                                },
                            ItemElementName = ItemChoiceType1.RecurringBilling
                        };
                });
        }

        public override HpsAuthorization Execute()
        {
            BuilderActions.ForEach(ba => ba(this));

            var response = DoTransaction().Ver10;
            HpsGatewayResponseValidation.CheckResponse(response, ItemChoiceType2.RecurringBilling);

            var result = HydrateAuthorization<HpsAuthorization>(response);
            HpsIssuerResponseValidation.CheckResponse(result.TransactionId, result.ResponseCode, result.ResponseText);
            
            return result;
        }

        public RecurringBuilder WithScheduleId(string scheduleId)
        {
            BuilderActions.Add(n => ((PosRecurringBillReqType)n.Transaction.Item).Block1.RecurringData.ScheduleID = scheduleId);
            return this;
        }

        public RecurringBuilder WithCardHolder(HpsCardHolder cardHolder)
        {
            BuilderActions.Add(n => ((PosRecurringBillReqType)n.Transaction.Item).Block1.CardHolderData = HydrateCardHolderData(cardHolder));
            return this;
        }

        public RecurringBuilder RequestMultiuseToken()
        {
            BuilderActions.Add(n => ((PosRecurringBillReqType)n.Transaction.Item).Block1.CardData.TokenRequest = booleanType.Y);
            return this;
        }

        public RecurringBuilder AllowDuplicates()
        {
            BuilderActions.Add(n =>
                {
                    ((PosRecurringBillReqType)n.Transaction.Item).Block1.AllowDup = booleanType.Y;
                    ((PosRecurringBillReqType)n.Transaction.Item).Block1.AllowDupSpecified = true;
                });

            return this;
        }

        public RecurringBuilder WithAdditionalTransactionFields(HpsAdditionalTransactionFields additionalTransactionFields)
        {
            BuilderActions.Add(n => ((PosRecurringBillReqType)n.Transaction.Item).Block1.AdditionalTxnFields =
                                    HydrateAdditionalTxnFields(additionalTransactionFields));
            return this;
        }

        public RecurringBuilder WithEncryptionData(HpsEncryptionData encryptionData)
        {
            BuilderActions.Add(n => ((PosRecurringBillReqType)n.Transaction.Item).Block1.CardData.EncryptionData =
                                    HydrateEncryptionData(encryptionData));
            return this;
        }

        public RecurringBuilder OneTime()
        {
            BuilderActions.Add(n =>
            {
                ((PosRecurringBillReqType)n.Transaction.Item).Block1.RecurringData.OneTime = booleanType.Y;
            });

            return this;
        }
    }
}
