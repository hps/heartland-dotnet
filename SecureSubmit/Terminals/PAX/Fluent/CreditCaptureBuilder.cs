using System;
using SecureSubmit.Entities;
using SecureSubmit.Fluent;
using SecureSubmit.Infrastructure;
using SecureSubmit.Terminals.Extensions;

namespace SecureSubmit.Terminals.PAX {
    public class CreditCaptureBuilder : HpsBuilderAbstract<PaxDevice, CreditResponse> {
        private int referenceNumber;
        decimal? amount;
        decimal? gratuity;
        int? transactionId;

        public CreditCaptureBuilder WithAmount(decimal? value) {
            this.amount = value;
            return this;
        }
        public CreditCaptureBuilder WithGratuity(decimal? value) {
            this.gratuity = value;
            return this;
        }
        public CreditCaptureBuilder WithTransactionId(int? value) {
            this.transactionId = value;
            return this;
        }
        public CreditCaptureBuilder WithReferenceNumber(int referenceNumber) {
            this.referenceNumber = referenceNumber;
            return this;
        }

        public CreditCaptureBuilder(PaxDevice device)
            : base(device) {
        }

        public override CreditResponse Execute() {
            base.Execute();

            var amounts = new AmountRequest();
            if (amount.HasValue) {
                var _amount = amount;
                // remove the gratuity from the capture amount
                if (gratuity.HasValue) {
                    _amount -= gratuity;
                }
                amounts.TransactionAmount = "{0:c}".FormatWith(_amount).ToNumeric();
            }
            
            // ADDING THIS HERE CAUSES IT TO FAIL SKIPPING IT HERE
            //if (gratuity.HasValue)
            //    amounts.TipAmount = "{0:c}".FormatWith(gratuity).ToNumeric();

            var trace = new TraceRequest {
                ReferenceNumber = referenceNumber.ToString(),
            };

            var extData = new ExtDataSubGroup();
            if (transactionId.HasValue)
                extData[EXT_DATA.HOST_REFERENCE_NUMBER] = transactionId.Value.ToString();

            var response = service.DoCredit(
                PAX_TXN_TYPE.POSTAUTH,
                amounts,
                new AccountRequest(),
                trace,
                new AvsRequest(),
                new CashierSubGroup(),
                new CommercialRequest(),
                new EcomSubGroup(),
                extData
            );

            // AND JUST ADDING IT TO THE RESPONSE
            if (gratuity.HasValue) {
                try {
                    var editResponse = service.CreditEdit(amount)
                        .WithTransactionId(response.TransactionId)
                        .WithGratuity(gratuity)
                        .Execute();
                    response.SubTransaction = editResponse;
                    response.TipAmount = gratuity.Value;
                }
                catch (HpsException exc) {
                    var voidResponse = service.CreditVoid(referenceNumber)
                        .WithTransactionId(response.TransactionId)
                        .Execute();
                    throw new HpsException("Failed to edit transaction, the capture transaction has been reversed.", exc);
                }
            }

            return response;
        }

        protected override void SetupValidations() {
            AddValidation(() => { return transactionId.HasValue; }, "Transaction ID is required.");
        }
    }
}
