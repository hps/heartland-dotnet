using System.Globalization;
using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Entities.Credit;
using SecureSubmit.Infrastructure;
using SecureSubmit.Infrastructure.Validation;

namespace SecureSubmit.Services.Fluent.Check
{
    public class RecurringBuilder : GatewayTransactionBuilder<RecurringBuilder, HpsCheckResponse>
    {
        public class RecurringPaymentTypeBuilder
        {
            private readonly RecurringBuilder _parent;

            public RecurringPaymentTypeBuilder(RecurringBuilder parent)
            {
                _parent = parent;
            }

            public RecurringBuilder WithPaymentMethodKey(string paymentMethodKey)
            {
                _parent.BuilderActions.Add(n => ((PosCheckSaleReqType)n.Transaction.Item).Block1.PaymentMethodKey = paymentMethodKey);
                return _parent;
            }
        }

        public RecurringBuilder(IHpsServicesConfig config, decimal amount) : base(config)
        {
            BuilderActions.Add(n =>
            {
                n.Transaction = new PosRequestVer10Transaction
                {
                    Item = new PosCheckSaleReqType
                    {
                        Block1 = new CheckSaleReqBlock1Type
                        {
                            CheckAction = checkActionType.SALE,
                            Amt = amount,
                            AmtSpecified = true,
                            RecurringData = new RecurringDataType
                            {
                                OneTime = booleanType.N,
                                OneTimeSpecified = true
                            }
                        }
                    },
                    ItemElementName = ItemChoiceType1.CheckSale
                };
            });
        }

        public override HpsCheckResponse Execute()
        {
            BuilderActions.ForEach(ba => ba(this));

            var rsp = DoTransaction().Ver10;
            HpsGatewayResponseValidation.CheckResponse(rsp, ItemChoiceType2.CheckSale);
            var saleRsp = (PosCheckSaleRspType)rsp.Transaction.Item;
            if (saleRsp.RspCode != 0)
            {
                throw new HpsCheckException(rsp.Header.GatewayTxnId, HydrateCheckResponseDetails(saleRsp.CheckRspInfo), saleRsp.RspCode, saleRsp.RspMessage);
            }

            var response = new HpsCheckResponse
            {
                Header = HydrateTransactionHeader(rsp.Header),
                TransactionId = rsp.Header.GatewayTxnId,
                AuthorizationCode = saleRsp.AuthCode,
                ResponseCode = saleRsp.RspCode.ToString(CultureInfo.InvariantCulture),
                ResponseText = saleRsp.RspMessage,
                Details = HydrateCheckResponseDetails(saleRsp.CheckRspInfo)
            };

            return response;
        }

        public RecurringBuilder WithScheduleId(string scheduleId)
        {
            BuilderActions.Add(n => ((PosCheckSaleReqType)n.Transaction.Item).Block1.RecurringData.ScheduleID = scheduleId);
            return this;
        }

        public RecurringBuilder WithAdditionalTransactionFields(HpsAdditionalTransactionFields additionalTransactionFields)
        {
            BuilderActions.Add(n => ((PosCheckSaleReqType)n.Transaction.Item).Block1.AdditionalTxnFields =
                                    HydrateAdditionalTxnFields(additionalTransactionFields));
            return this;
        }

        public RecurringBuilder OneTime()
        {
            BuilderActions.Add(n =>
            {
                ((PosCheckSaleReqType)n.Transaction.Item).Block1.RecurringData.OneTime = booleanType.Y;
            });

            return this;
        }
    }
}
