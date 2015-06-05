using System.Globalization;
using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Infrastructure;
using SecureSubmit.Infrastructure.Validation;

namespace SecureSubmit.Services.Fluent.Check
{
    public class VoidBuilder : GatewayTransactionBuilder<VoidBuilder, HpsCheckResponse>
    {
        public class VoidUsingBuilder
        {
            private readonly VoidBuilder _parent;

            public VoidUsingBuilder(VoidBuilder parent)
            {
                _parent = parent;
            }

            public VoidBuilder WithTransactionId(int transactionId)
            {
                _parent.BuilderActions.Add(n =>
                {
                    ((CheckVoidReqBlock1Type)n.Transaction.Item).GatewayTxnId = transactionId;
                    ((CheckVoidReqBlock1Type)n.Transaction.Item).GatewayTxnIdSpecified = true;
                });

                return _parent;
            }

            public VoidBuilder WithClientTransactionId(int clientTransactionId)
            {
                _parent.BuilderActions.Add(n =>
                {
                    ((CheckVoidReqBlock1Type)n.Transaction.Item).ClientTxnId = clientTransactionId;
                    ((CheckVoidReqBlock1Type)n.Transaction.Item).ClientTxnIdSpecified = true;
                });

                return _parent;
            }
        }

        public VoidBuilder(IHpsServicesConfig config)
            : base(config)
        {
            BuilderActions.Add(n =>
                {
                    n.Transaction = new PosRequestVer10Transaction
                        {
                            Item = new PosCheckVoidReqType
                                {
                                    Block1 = new CheckVoidReqBlock1Type()
                                },
                            ItemElementName = ItemChoiceType1.CheckVoid
                        };
                });
        }

        public override HpsCheckResponse Execute()
        {
            BuilderActions.ForEach(ba => ba(this));

            var rsp = DoTransaction().Ver10;
            HpsGatewayResponseValidation.CheckResponse(rsp, ItemChoiceType2.CheckVoid);
            var voidRsp = (PosCheckVoidRspType) rsp.Transaction.Item;
            if (voidRsp.RspCode != 0)
            {
                throw new HpsCheckException(rsp.Header.GatewayTxnId, HydrateCheckResponseDetails(voidRsp.CheckRspInfo),
                    voidRsp.RspCode, voidRsp.RspMessage);
            }

            var response = new HpsCheckResponse
            {
                Header = HydrateTransactionHeader(rsp.Header),
                TransactionId = rsp.Header.GatewayTxnId,
                AuthorizationCode = voidRsp.AuthCode,
                ResponseCode = voidRsp.RspCode.ToString(CultureInfo.InvariantCulture),
                ResponseText = voidRsp.RspMessage,
                Details = HydrateCheckResponseDetails(voidRsp.CheckRspInfo)
            };

            return response;
        }
    }
}
