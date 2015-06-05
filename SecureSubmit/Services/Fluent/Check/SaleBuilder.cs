using System.Globalization;
using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Entities.Credit;
using SecureSubmit.Infrastructure;
using SecureSubmit.Infrastructure.Validation;

namespace SecureSubmit.Services.Fluent.Check
{
    public class SaleBuilder : GatewayTransactionBuilder<SaleBuilder, HpsCheckResponse>
    {
        public SaleBuilder(IHpsServicesConfig config, checkActionType action)
            : base(config)
        {
            BuilderActions.Add(n =>
                {
                    n.Transaction = new PosRequestVer10Transaction
                        {
                            Item = new PosCheckSaleReqType
                                {
                                    Block1 = new CheckSaleReqBlock1Type
                                        {
                                            AccountInfo = new AccountInfoType(),
                                            CheckAction = action
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

        public SaleBuilder WithAccountNumber(string accountNumber)
        {
            BuilderActions.Add(n => ((PosCheckSaleReqType) n.Transaction.Item).Block1.AccountInfo.AccountNumber = accountNumber);
            return this;
        }

        public SaleBuilder WithRoutingNumber(string routingNumber)
        {
            BuilderActions.Add(n => ((PosCheckSaleReqType) n.Transaction.Item).Block1.AccountInfo.RoutingNumber =routingNumber);
            return this;
        }

        public SaleBuilder WithCheckNumber(string checkNumber)
        {
            BuilderActions.Add(n => ((PosCheckSaleReqType) n.Transaction.Item).Block1.AccountInfo.CheckNumber = checkNumber);
            return this;
        }

        public SaleBuilder WithMicrData(string micrData)
        {
            BuilderActions.Add(n => ((PosCheckSaleReqType) n.Transaction.Item).Block1.AccountInfo.MICRData = micrData);
            return this;
        }

        public SaleBuilder WithAccountType(accountTypeType accountType)
        {
            BuilderActions.Add(n =>
            {
                ((PosCheckSaleReqType) n.Transaction.Item).Block1.AccountInfo.AccountType = accountType;
                ((PosCheckSaleReqType) n.Transaction.Item).Block1.AccountInfo.AccountTypeSpecified = true;
            });

            return this;
        }

        public SaleBuilder WithDataEntryMode(dataEntryModeType dataEntryMode)
        {
            BuilderActions.Add(n =>
            {
                ((PosCheckSaleReqType)n.Transaction.Item).Block1.DataEntryMode = dataEntryMode;
                ((PosCheckSaleReqType)n.Transaction.Item).Block1.DataEntryModeSpecified = true;
            });

            return this;
        }

        public SaleBuilder WithCheckType(checkTypeType checkType)
        {
            BuilderActions.Add(n =>
            {
                ((PosCheckSaleReqType)n.Transaction.Item).Block1.CheckType = checkType;
                ((PosCheckSaleReqType)n.Transaction.Item).Block1.CheckTypeSpecified = true;
            });

            return this;
        }

        public SaleBuilder WithAmount(decimal amount)
        {
            BuilderActions.Add(n =>
            {
                ((PosCheckSaleReqType)n.Transaction.Item).Block1.Amt = amount;
                ((PosCheckSaleReqType)n.Transaction.Item).Block1.AmtSpecified = true;
            });

            return this;
        }

        public SaleBuilder WithSecCode(string secCode)
        {
            BuilderActions.Add(n => ((PosCheckSaleReqType)n.Transaction.Item).Block1.SECCode = secCode);
            return this;
        }

        public SaleBuilder WithCheckHolder(HpsCheckHolder checkHolder)
        {
            BuilderActions.Add(n =>
            {
                ((PosCheckSaleReqType)n.Transaction.Item).Block1.ConsumerInfo = new ConsumerInfoType
                {
                    Address1 = checkHolder.Address != null ? checkHolder.Address.Address : null,
                    CheckName = checkHolder.CheckName,
                    City = checkHolder.Address != null ? checkHolder.Address.City : null,
                    CourtesyCard = checkHolder.CourtesyCard,
                    DLNumber = checkHolder.DlNumber,
                    DLState = checkHolder.DlState,
                    EmailAddress = checkHolder.Email,
                    FirstName = checkHolder.FirstName,
                    LastName = checkHolder.LastName,
                    PhoneNumber = checkHolder.Phone,
                    State = checkHolder.Address != null ? checkHolder.Address.State : null,
                    Zip = checkHolder.Address != null ? checkHolder.Address.Zip : null,
                    IdentityInfo = new IdentityInfoType
                    {
                        DOBYear = checkHolder.DobYear.HasValue ? checkHolder.DobYear.Value.ToString(CultureInfo.InvariantCulture) : null,
                        SSNL4 = checkHolder.Ssl4
                    }
                };
            });

            return this;
        }

        public SaleBuilder WithAdditionalTransactionFields(HpsAdditionalTransactionFields additionalTransactionFields)
        {
            BuilderActions.Add(n => ((PosCheckSaleReqType)n.Transaction.Item).Block1.AdditionalTxnFields =
                                    HydrateAdditionalTxnFields(additionalTransactionFields));
            return this;
        }

        public SaleBuilder WithPaymentMethodKey(string paymentMethodKey)
        {
            BuilderActions.Add(n => ((PosCheckSaleReqType)n.Transaction.Item).Block1.PaymentMethodKey = paymentMethodKey);
            return this;
        }

        public SaleBuilder WithTokenValue(string tokenValue)
        {
            BuilderActions.Add(n => ((PosCheckSaleReqType)n.Transaction.Item).Block1.TokenValue = tokenValue);
            return this;
        }

        public SaleBuilder WithRecurringData(HpsRecurringData recurringData)
        {
            BuilderActions.Add(n => ((PosCheckSaleReqType)n.Transaction.Item).Block1.RecurringData = new RecurringDataType
            {
                OneTime = recurringData.OneTimePayment ? booleanType.Y : booleanType.N,
                OneTimeSpecified = true,
                ScheduleID = recurringData.ScheduleId
            });
            return this;
        }
    }
}
