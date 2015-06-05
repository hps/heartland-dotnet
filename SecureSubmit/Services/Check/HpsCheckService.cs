using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Infrastructure;
using SecureSubmit.Infrastructure.Validation;
using System;
using System.Collections.Generic;
using System.Globalization;
using SecureSubmit.Services.Fluent.Check;

namespace SecureSubmit.Services.Check
{
    public class HpsCheckService : HpsSoapGatewayService
    {
        /// <summary>Initializes a new instance of the <see cref="HpsCheckService"/> class.</summary>
        /// <param name="config">The HPS services config.</param>
        public HpsCheckService(IHpsServicesConfig config = null)
            : base(config) { }

        public SaleBuilder Sale(checkActionType actionType)
        {
            return new SaleBuilder(ServicesConfig, actionType);
        }

        public VoidBuilder.VoidUsingBuilder Void()
        {
            return new VoidBuilder.VoidUsingBuilder(new VoidBuilder(ServicesConfig));
        }

        public RecurringBuilder.RecurringPaymentTypeBuilder Recurring(decimal amount)
        {
            HpsInputValidation.CheckAmount(amount);
            return new RecurringBuilder.RecurringPaymentTypeBuilder(new RecurringBuilder(ServicesConfig, amount));
        }

        /// <summary>
        /// A <b>Sale</b> transaction is used to process transactions using bank account information as the payment method.
        /// The transaction service can be used to perform a Sale or Return transaction by indicating the Check Action.
        /// <br></br><br></br>
        /// <b>NOTE:</b> The Portico Gateway supports both GETI and HPS Colonnade for processing check transactions. While
        /// the available services are the same regardless of the check processor, the services may have different behaviors.
        /// For example, GETI-processed Check Sale transactions support the ability to override a Check Sale transaction
        /// already presented as well as the ability to verify a check.
        /// </summary>
        /// <param name="action">Type of Check Action (Sale, Return, Override).</param>
        /// <param name="check">The Check information.</param>
        /// <param name="amount">The amount of the sale.</param>
        /// <returns>The <see cref="HpsCheckResponse"/>.</returns>
        public HpsCheckResponse Sale(checkActionType action, HpsCheck check, decimal amount)
        {
            HpsInputValidation.CheckAmount(amount);

            /* Build the transaction request. */
            var transaction = new PosRequestVer10Transaction
            {
                Item = new PosCheckSaleReqType
                {
                    Block1 = new CheckSaleReqBlock1Type
                    {
                        Amt = amount,
                        AccountInfo = new AccountInfoType
                        {
                            AccountNumber = check.AccountNumber,
                            CheckNumber = check.CheckNumber,
                            MICRData = check.MicrNumber,
                            RoutingNumber = check.RoutingNumber
                        },
                        AmtSpecified = true,
                        CheckAction = action,
                        SECCode = check.SecCode
                    }
                },
                ItemElementName = ItemChoiceType1.CheckSale
            };

            var block1 = ((PosCheckSaleReqType)transaction.Item).Block1;

            if (check.AccountType.HasValue)
            {
                block1.AccountInfo.AccountType = check.AccountType.Value;
                block1.AccountInfo.AccountTypeSpecified = true;
            }
            else
            {
                block1.AccountInfo.AccountTypeSpecified = false;
            }

            if (check.CheckType.HasValue)
            {
                block1.CheckType = check.CheckType.Value;
                block1.CheckTypeSpecified = true;
            }
            else
            {
                block1.CheckTypeSpecified = false;
            }

            if (check.DataEntryMode.HasValue)
            {
                block1.DataEntryMode = check.DataEntryMode.Value;
                block1.DataEntryModeSpecified = true;
            }
            else
            {
                block1.DataEntryModeSpecified = false;
            }

            if (check.CheckHolder != null)
            {
                block1.ConsumerInfo = new ConsumerInfoType
                {
                    Address1 = check.CheckHolder.Address != null ? check.CheckHolder.Address.Address : null,
                    CheckName = check.CheckHolder.CheckName,
                    City = check.CheckHolder.Address != null ? check.CheckHolder.Address.City : null,
                    CourtesyCard = check.CheckHolder.CourtesyCard,
                    DLNumber = check.CheckHolder.DlNumber,
                    DLState = check.CheckHolder.DlState,
                    EmailAddress = check.CheckHolder.Email,
                    FirstName = check.CheckHolder.FirstName,
                    LastName = check.CheckHolder.LastName,
                    PhoneNumber = check.CheckHolder.Phone,
                    State = check.CheckHolder.Address != null ? check.CheckHolder.Address.State : null,
                    Zip = check.CheckHolder.Address != null ? check.CheckHolder.Address.Zip : null
                };
            }

            /* Submit the transaction. */
            var rsp = DoTransaction(transaction).Ver10;
            HpsGatewayResponseValidation.CheckResponse(rsp, ItemChoiceType2.CheckSale);

            var saleRsp = (PosCheckSaleRspType)rsp.Transaction.Item;
            if (saleRsp.RspCode != 0)
            {
                throw new HpsCheckException(rsp.Header.GatewayTxnId, GetResponseDetails(saleRsp.CheckRspInfo), saleRsp.RspCode, saleRsp.RspMessage);
            }

            /* Start to fill out a new transaction response (HpsCheckResponse). */
            var response = new HpsCheckResponse
            {
                Header = HydrateTransactionHeader(rsp.Header),
                TransactionId = rsp.Header.GatewayTxnId,
                AuthorizationCode = saleRsp.AuthCode,
                ResponseCode = saleRsp.RspCode.ToString(CultureInfo.InvariantCulture),
                ResponseText = saleRsp.RspMessage,
                Details = GetResponseDetails(saleRsp.CheckRspInfo)
            };

            return response;
        }

        /// <summary>
        /// A <b>Void</b> transaction is used to cancel a previously successful sale transaction. The original sale transaction
        /// can be identified by the GatewayTxnid of the original or by the ClientTxnId of the original if provided on the original
        /// Sale transaction.
        /// </summary>
        /// <param name="transactionId">The transaction ID of charge to void.</param>
        /// <param name="clientTransactionId">The client transaction ID of charge to void.</param>
        /// <returns>The <see cref="HpsGiftCardActivate"/>.</returns>
        public HpsCheckResponse Void(int? transactionId = null, long? clientTransactionId = null)
        {
            if ((!transactionId.HasValue && !clientTransactionId.HasValue) || (transactionId.HasValue && clientTransactionId.HasValue))
            {
                throw new ArgumentException(Resource.HpsCheckService_Invalid_Input_Id, "transactionId");
            }

            /* Build the transaction request. */
            var transaction = new PosRequestVer10Transaction
            {
                Item = new PosCheckVoidReqType
                {
                    Block1 = new CheckVoidReqBlock1Type()
                },
                ItemElementName = ItemChoiceType1.CheckVoid
            };

            var block1 = ((PosCheckVoidReqType)transaction.Item).Block1;

            if (transactionId.HasValue)
            {
                block1.GatewayTxnId = transactionId.Value;
                block1.GatewayTxnIdSpecified = true;
                block1.ClientTxnIdSpecified = false;
            }
            else
            {
                block1.GatewayTxnIdSpecified = false;
                block1.ClientTxnId = clientTransactionId.Value;
                block1.ClientTxnIdSpecified = true;
            }

            /* Submit the transaction. */
            var rsp = DoTransaction(transaction).Ver10;
            HpsGatewayResponseValidation.CheckResponse(rsp, ItemChoiceType2.CheckVoid);

            var voidRsp = (PosCheckVoidRspType)rsp.Transaction.Item;
            if (voidRsp.RspCode != 0)
            {
                throw new HpsCheckException(rsp.Header.GatewayTxnId, GetResponseDetails(voidRsp.CheckRspInfo), voidRsp.RspCode, voidRsp.RspMessage);
            }

            /* Start to fill out a new transaction response (HpsCheckResponse). */
            var response = new HpsCheckResponse
            {
                Header = HydrateTransactionHeader(rsp.Header),
                TransactionId = rsp.Header.GatewayTxnId,
                AuthorizationCode = voidRsp.AuthCode,
                ResponseCode = voidRsp.RspCode.ToString(CultureInfo.InvariantCulture),
                ResponseText = voidRsp.RspMessage,
                Details = GetResponseDetails(voidRsp.CheckRspInfo)
            };

            return response;
        }

        private static List<HpsCheckResponseDetails> GetResponseDetails(IEnumerable<CheckRspInfoType> responseInfo)
        {
            var result = new List<HpsCheckResponseDetails>();
            foreach (var info in responseInfo)
            {
                result.Add(new HpsCheckResponseDetails
                {
                    Code = info.Code,
                    FieldName = info.FieldName,
                    FieldNumber = info.FieldNumber,
                    Message = info.Message,
                    MessageType = info.Type
                });
            }

            return result;
        }
    }
}
