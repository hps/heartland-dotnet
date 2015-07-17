using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Abstractions;
using SecureSubmit.Entities;
using SecureSubmit.Fluent.Services;
using SecureSubmit.Infrastructure.Validation;

namespace SecureSubmit.Fluent {
    public class CreditCpcEditBuilder : HpsBuilderAbstract<HpsFluentCreditService, HpsTransaction> {
        int? transactionId;
        HpsCpcData cpcData;

        public CreditCpcEditBuilder WithTransactionId(int? transactionId) {
            this.transactionId = transactionId;
            return this;
        }
        public CreditCpcEditBuilder WithCpcData(HpsCpcData cpcData) {
            this.cpcData = cpcData;
            return this;
        }

        public CreditCpcEditBuilder(HpsFluentCreditService service)
            : base(service) {
        }

        public override HpsTransaction Execute() {
            base.Execute();

            var transaction = new PosRequestVer10Transaction {
                Item = new PosCreditCPCEditReqType {
                    GatewayTxnId = transactionId.Value,
                    CPCData = service.HydrateCpcData(cpcData)
                },
                ItemElementName = ItemChoiceType1.CreditCPCEdit
            };

            var response = service.SubmitTransaction(transaction);
            var trans = new HpsTransaction().FromResponse(response);
            trans.ResponseCode = "00";
            trans.ResponseText = string.Empty;
            return trans;
        }

        protected override void SetupValidations() {
            AddValidation(() => { return transactionId != null; }, "TransactionId is required.");
            AddValidation(() => { return cpcData != null; }, "CpcData is required.");
        }
    }
}
