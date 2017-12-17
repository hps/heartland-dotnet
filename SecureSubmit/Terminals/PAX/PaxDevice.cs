using System;
using System.Collections;
using Hps.Exchange.PosGateway.Client;
using SecureSubmit.Entities;
using SecureSubmit.Fluent;
using SecureSubmit.Fluent.Services;
using SecureSubmit.Infrastructure;
using SecureSubmit.Services;
using SecureSubmit.Terminals.Abstractions;
using SecureSubmit.Terminals.Extensions;

namespace SecureSubmit.Terminals.PAX {
    public delegate void MessageSentEventHandler(string message);

    public class PaxDevice : IDisposable {
        ITerminalConfiguration _settings;
        IDeviceCommInterface _interface;

        public event MessageSentEventHandler OnMessageSent;

        public PaxDevice(ITerminalConfiguration settings) {
            settings.Validate();
            this._settings = settings;

            switch (_settings.ConnectionMode) {
                case ConnectionModes.TCP_IP:
                    _interface = new PaxTcpInterface(settings);
                    break;
                case ConnectionModes.HTTP:
                    _interface = new PaxHttpInterface(settings);
                    break;
                case ConnectionModes.SERIAL:
                case ConnectionModes.SSL_TCP:
                    throw new NotImplementedException();
            }

            _interface.Connect();
            _interface.OnMessageSent += (message) => {
                if (this.OnMessageSent != null)
                    OnMessageSent(message);
            };
        }

        #region Administration Messages
        // A00 - INITIALIZE
        public InitializeResponse Initialize() {
            var response = _interface.Send(TerminalUtilities.BuildRequest(PAX_MSG_ID.A00_INITIALIZE));
            return new InitializeResponse(response);
        }

        // A14 - CANCEL
        public void Cancel() {
            if (_settings.ConnectionMode == ConnectionModes.HTTP)
                throw new HpsMessageException("The cancel command is not available in HTTP mode");

            _interface.Send(TerminalUtilities.BuildRequest(PAX_MSG_ID.A14_CANCEL));
        }

        // A16 - RESET
        public PaxDeviceResponse Reset() {
            var response = _interface.Send(TerminalUtilities.BuildRequest(PAX_MSG_ID.A16_RESET));
            return new PaxDeviceResponse(response, PAX_MSG_ID.A17_RSP_RESET);
        }

        // A26 - REBOOT
        public PaxDeviceResponse Reboot() {
            var response = _interface.Send(TerminalUtilities.BuildRequest(PAX_MSG_ID.A26_REBOOT));
            return new PaxDeviceResponse(response, PAX_MSG_ID.A27_RSP_REBOOT);
        }

        // A30 - INPUT ACCOUNT
        public InputAccountBuilder InputAccount() {
            return new InputAccountBuilder(this);
        }
        #endregion

        #region Transaction Commands
        internal byte[] DoSend(string messageId, params object[] elements) {
            return _interface.Send(TerminalUtilities.BuildRequest(messageId, elements));
        }

        internal byte[] DoTransaction(string messageId, string txnType, params IRequestSubGroup[] subGroups) {
            var commands = new ArrayList(){ txnType, ControlCodes.FS };
            if (subGroups.Length > 0) {
                commands.Add(subGroups[0]);
                for (int i = 1; i < subGroups.Length; i++) {
                    commands.Add(ControlCodes.FS);
                    commands.Add(subGroups[i]);
                }
            }

            var message = TerminalUtilities.BuildRequest(messageId, commands.ToArray());
            return _interface.Send(message);
        }

        internal CreditResponse DoCredit(string txnType, AmountRequest amounts, AccountRequest accounts, TraceRequest trace, AvsRequest avs, CashierSubGroup cashier, CommercialRequest commercial, EcomSubGroup ecom, ExtDataSubGroup extData) {
            var response = DoTransaction(PAX_MSG_ID.T00_DO_CREDIT, txnType, amounts, accounts, trace, avs, cashier, commercial, ecom, extData);
            return new CreditResponse(response);
        }
        
        internal DebitResponse DoDebit(string txnType, AmountRequest amounts, AccountRequest accounts, TraceRequest trace, CashierSubGroup cashier, ExtDataSubGroup extData)
        {
            var response = DoTransaction(PAX_MSG_ID.T02_DO_DEBIT, txnType, amounts, accounts, trace, cashier, extData);
            return new DebitResponse(response);
        }
        
        internal EbtResponse DoEBT(string txnType, AmountRequest amounts, AccountRequest accounts, TraceRequest trace, CashierSubGroup cashier) {
            var response = DoTransaction(PAX_MSG_ID.T04_DO_EBT, txnType, accounts, trace, cashier, new ExtDataSubGroup());
            return new EbtResponse(response);
        }

        internal GiftResponse DoGift(string messageId, string txnType, AmountRequest amounts, AccountRequest accounts, TraceRequest trace, CashierSubGroup cashier, ExtDataSubGroup extData = null) {
            var response = DoTransaction(messageId, txnType, amounts, accounts, trace, cashier, extData);
            return new GiftResponse(response);
        }

        internal CashResponse DoCash(string txnType, AmountRequest amounts, TraceRequest trace, CashierSubGroup cashier) {
            var response = DoTransaction(PAX_MSG_ID.T10_DO_CASH, txnType, amounts, trace, cashier, new ExtDataSubGroup());
            return new CashResponse(response);
        }

        internal CheckSubResponse DoCheck(string txnType, AmountRequest amounts, CheckSubGroup check, TraceRequest trace, CashierSubGroup cashier, ExtDataSubGroup extData) {
            var response = DoTransaction(PAX_MSG_ID.T12_DO_CHECK, txnType, amounts, check, trace, cashier, extData);
            return new CheckSubResponse(response);
        }
        #endregion

        #region Credit Methods
        public CreditAuthBuilder CreditAuth(int referenceNumber, decimal? amount = null) {
            return new CreditAuthBuilder(this).WithReferenceNumber(referenceNumber).WithAmount(amount);
        }

        public CreditCaptureBuilder CreditCapture(int referenceNumber, decimal? amount = null) {
            return new CreditCaptureBuilder(this).WithReferenceNumber(referenceNumber).WithAmount(amount);
        }

        public CreditAdjustBuilder CreditAdjust(int referenceNumber, decimal? gratuity = null)
        {
            return new CreditAdjustBuilder(this).WithReferenceNumber(referenceNumber).WithGratuity(gratuity);
        }

        public CreditEditBuilder CreditEdit(decimal? amount = null) {
            if (!_settings.DeviceId.HasValue || !_settings.SiteId.HasValue || !_settings.LicenseId.HasValue || string.IsNullOrEmpty(_settings.UserName) || string.IsNullOrEmpty(_settings.Password))
                throw new HpsConfigurationException("Device is not configured properly for Credit Edit. Please provide the device credentials in the ConnectionConfig.");

            var service = new HpsFluentCreditService(new HpsServicesConfig() {
                DeviceId = _settings.DeviceId.Value,
                SiteId = _settings.SiteId.Value,
                LicenseId = _settings.LicenseId.Value,
                UserName = _settings.UserName,
                Password = _settings.Password,
                ServiceUrl = _settings.Url
            });
            return new CreditEditBuilder(service).WithAmount(amount);
        }

        public CreditReturnBuilder CreditReturn(int referenceNumber, decimal? amount = null) {
            return new CreditReturnBuilder(this).WithReferenceNumber(referenceNumber).WithAmount(amount);
        }

        public CreditSaleBuilder CreditSale(int referenceNumber, decimal? amount = null) {
            return new CreditSaleBuilder(this).WithReferenceNumber(referenceNumber).WithAmount(amount);
        }

        public CreditVerifyBuilder CreditVerify(int referenceNumber) {
            return new CreditVerifyBuilder(this).WithReferenceNumber(referenceNumber);
        }

        public CreditVoidBuilder CreditVoid(int referenceNumber) {
            return new CreditVoidBuilder(this).WithReferenceNumber(referenceNumber);
        }
        #endregion

        #region Debit Methods
        public DebitReturnBuilder DebitReturn(int referenceNumber, decimal? amount = null) {
            return new DebitReturnBuilder(this).WithReferenceNumber(referenceNumber).WithAmount(amount);
        }

        public DebitSaleBuilder DebitSale(int referenceNumber, decimal? amount = null) {
            return new DebitSaleBuilder(this).WithReferenceNumber(referenceNumber).WithAmount(amount);
        }
        #endregion

        #region EBT Methods
        #endregion

        #region Gift Methods
        public GiftSaleBuilder GiftSale(int referenceNumber, decimal? amount = null) {
            return new GiftSaleBuilder(this).WithReferenceNumber(referenceNumber).WithAmount(amount);
        }

        public GiftAddValueBuilder GiftAddValue(int referenceNumber) {
            return new GiftAddValueBuilder(this).WithReferenceNumber(referenceNumber);
        }

        public GiftVoidBuilder GiftVoid(int referenceNumber) {
            return new GiftVoidBuilder(this).WithReferenceNumber(referenceNumber);
        }

        public GiftBalanceBuilder GiftBalance(int referenceNumber) {
            return new GiftBalanceBuilder(this).WithReferenceNumber(referenceNumber);
        }
        #endregion

        #region Cash Methods
        #endregion

        #region Check Methods
        #endregion
        
        #region Batch Commands
        public BatchCloseResponse BatchClose() {
            var response = _interface.Send(TerminalUtilities.BuildRequest(PAX_MSG_ID.B00_BATCH_CLOSE, DateTime.Now.ToString("YYYYMMDDhhmmss")));
            return new BatchCloseResponse(response);
        }
        #endregion

        #region Reporting Commands
        #endregion

        public void Dispose() {
            _interface.Disconnect();
        }
    }
}
