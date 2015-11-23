using Hps.Exchange.PosGateway.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecureSubmit.Fluent.Services;
using SecureSubmit.Infrastructure;
using SecureSubmit.Services;
using SecureSubmit.Tests.TestData;

namespace SecureSubmit.Tests.General
{
    [TestClass]
    public class CheckTests
    {
        #region setup
        private static readonly HpsServicesConfig ServicesConfig = new HpsServicesConfig
        {
            SecretApiKey = "skapi_cert_MTyMAQBiHVEAewvIzXVFcmUd2UcyBge_eCpaASUp0A"
        };
        private static readonly HpsFluentCheckService _checkService = new HpsFluentCheckService(ServicesConfig);

        private static int test01TransactionId;
        private static int test05TransactionId;
        private static int test10TransactionId;
        private static int test14TransactionId;
        private static int test23TransactionId;

        #endregion

        #region ACH Debit - Consumer

        [TestMethod]
        public void Test001ConsumerPersonalChecking()
        {
            var check = TestCheck.Certification;
            check.SecCode = HpsSECCode.PPD;
            check.DataEntryMode = dataEntryModeType.MANUAL;
            check.CheckType = checkTypeType.PERSONAL;
            check.AccountType = accountTypeType.CHECKING;
            var response = _checkService.Sale(11.00m)
                .WithCheck(check)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
            test01TransactionId = response.TransactionId;
        }

        [TestMethod]
        public void Test002ConsumerBusinessChecking()
        {
            var check = TestCheck.Certification;
            check.SecCode = HpsSECCode.PPD;
            check.DataEntryMode = dataEntryModeType.MANUAL;
            check.CheckType = checkTypeType.BUSINESS;
            check.AccountType = accountTypeType.CHECKING;
            var response = _checkService.Sale(12.00m)
                .WithCheck(check)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void Test003ConsumerPersonalSavings()
        {
            var check = TestCheck.Certification;
            check.SecCode = HpsSECCode.PPD;
            check.DataEntryMode = dataEntryModeType.MANUAL;
            check.CheckType = checkTypeType.PERSONAL;
            check.AccountType = accountTypeType.SAVINGS;
            var response = _checkService.Sale(13.00m)
                .WithCheck(check)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void Test004ConsumerBusinessSavings()
        {
            var check = TestCheck.Certification;
            check.SecCode = HpsSECCode.PPD;
            check.DataEntryMode = dataEntryModeType.MANUAL;
            check.CheckType = checkTypeType.BUSINESS;
            check.AccountType = accountTypeType.SAVINGS;
            var response = _checkService.Sale(14.00m)
                .WithCheck(check)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void Test005CorporatePersonalChecking()
        {
            var check = TestCheck.Certification;
            check.SecCode = HpsSECCode.CCD;
            check.DataEntryMode = dataEntryModeType.MANUAL;
            check.CheckType = checkTypeType.PERSONAL;
            check.AccountType = accountTypeType.CHECKING;
            check.CheckHolder.CheckName = "Heartland Pays";
            var response = _checkService.Sale(15.00m)
                .WithCheck(check)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
            test05TransactionId = response.TransactionId;
        }

        #endregion

        #region ACH Debit - Corporate

        [TestMethod]
        public void Test006CorporateBuisnessChecking()
        {
            var check = TestCheck.Certification;
            check.SecCode = HpsSECCode.CCD;
            check.DataEntryMode = dataEntryModeType.MANUAL;
            check.CheckType = checkTypeType.BUSINESS;
            check.AccountType = accountTypeType.CHECKING;
            check.CheckHolder.CheckName = "Heartland Pays";
            var response = _checkService.Sale(16.00m)
                .WithCheck(check)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void Test007CorporatePersonalSavings()
        {
            var check = TestCheck.Certification;
            check.SecCode = HpsSECCode.CCD;
            check.DataEntryMode = dataEntryModeType.MANUAL;
            check.CheckType = checkTypeType.PERSONAL;
            check.AccountType = accountTypeType.SAVINGS;
            check.CheckHolder.CheckName = "Heartland Pays";
            var response = _checkService.Sale(17.00m)
                .WithCheck(check)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void Test008CorporateBuisnessSavings()
        {
            var check = TestCheck.Certification;
            check.SecCode = HpsSECCode.CCD;
            check.DataEntryMode = dataEntryModeType.MANUAL;
            check.CheckType = checkTypeType.BUSINESS;
            check.AccountType = accountTypeType.SAVINGS;
            check.CheckHolder.CheckName = "Heartland Pays";
            var response = _checkService.Sale(18.00m)
                .WithCheck(check)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        #endregion

        #region eGold Checking Tests

        [TestMethod]
        public void Test009EgoldPersonalChecking()
        {
            var check = TestCheck.Certification;
            check.SecCode = HpsSECCode.POP;
            check.DataEntryMode = dataEntryModeType.MANUAL;
            check.CheckType = checkTypeType.PERSONAL;
            check.AccountType = accountTypeType.CHECKING;
            var response = _checkService.Sale(11.00m)
                .WithCheck(check)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void Test010EgoldBuisnessChecking()
        {
            var check = TestCheck.Certification;
            check.SecCode = HpsSECCode.POP;
            check.DataEntryMode = dataEntryModeType.MANUAL;
            check.CheckType = checkTypeType.BUSINESS;
            check.AccountType = accountTypeType.CHECKING;
            var response = _checkService.Sale(12.00m)
                .WithCheck(check)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
            test10TransactionId = response.TransactionId;
        }

        [TestMethod]
        public void Test011EgoldPersonalSavings()
        {
            var check = TestCheck.Certification;
            check.SecCode = HpsSECCode.POP;
            check.DataEntryMode = dataEntryModeType.MANUAL;
            check.CheckType = checkTypeType.PERSONAL;
            check.AccountType = accountTypeType.SAVINGS;
            var response = _checkService.Sale(13.00m)
                .WithCheck(check)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void Test012EgoldBusinessSavings()
        {
            var check = TestCheck.Certification;
            check.SecCode = HpsSECCode.POP;
            check.DataEntryMode = dataEntryModeType.MANUAL;
            check.CheckType = checkTypeType.BUSINESS;
            check.AccountType = accountTypeType.SAVINGS;
            var response = _checkService.Sale(14.00m)
                .WithCheck(check)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        #endregion

        #region eSilver 

        [TestMethod]
        public void Test013EsilverPersonalChecking()
        {
            var check = TestCheck.Certification;
            check.SecCode = HpsSECCode.POP;
            check.DataEntryMode = dataEntryModeType.MANUAL;
            check.CheckType = checkTypeType.PERSONAL;
            check.AccountType = accountTypeType.CHECKING;
            var response = _checkService.Sale(15.00m)
                .WithCheck(check)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void Test014EsilverBuisnessChecking()
        {
            var check = TestCheck.Certification;
            check.SecCode = HpsSECCode.POP;
            check.DataEntryMode = dataEntryModeType.MANUAL;
            check.CheckType = checkTypeType.BUSINESS;
            check.AccountType = accountTypeType.CHECKING;
            var response = _checkService.Sale(16.00m)
                .WithCheck(check)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
            test14TransactionId = response.TransactionId;
        }

        [TestMethod]
        public void Test015EsilverPersonalSavings()
        {
            var check = TestCheck.Certification;
            check.SecCode = HpsSECCode.POP;
            check.DataEntryMode = dataEntryModeType.MANUAL;
            check.CheckType = checkTypeType.PERSONAL;
            check.AccountType = accountTypeType.SAVINGS;
            var response = _checkService.Sale(17.00m)
                .WithCheck(check)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void Test016EsilverBuisnessSavings()
        {
            var check = TestCheck.Certification;
            check.SecCode = HpsSECCode.POP;
            check.DataEntryMode = dataEntryModeType.MANUAL;
            check.CheckType = checkTypeType.BUSINESS;
            check.AccountType = accountTypeType.SAVINGS;
            var response = _checkService.Sale(18.00m)
                .WithCheck(check)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        #endregion

        #region Bronze

        [TestMethod]
        public void Test017EbronzePersonalChecking()
        {
            var check = TestCheck.Certification;
            check.SecCode = HpsSECCode.EBRONZE;
            check.CheckVerify = true;
            check.DataEntryMode = dataEntryModeType.MANUAL;
            check.CheckType = checkTypeType.PERSONAL;
            check.AccountType = accountTypeType.CHECKING;
            var response = _checkService.Sale(19.00m)
                .WithCheck(check)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void Test018EbronzePersonalChecking()
        {
            var check = TestCheck.Certification;
            check.SecCode = HpsSECCode.EBRONZE;
            check.CheckVerify = true;
            check.DataEntryMode = dataEntryModeType.MANUAL;
            check.CheckType = checkTypeType.PERSONAL;
            check.AccountType = accountTypeType.CHECKING;
            var response = _checkService.Sale(20.00m)
                .WithCheck(check)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void Test019EbronzePersonalChecking()
        {
            var check = TestCheck.Certification;
            check.SecCode = HpsSECCode.EBRONZE;
            check.CheckVerify = true;
            check.DataEntryMode = dataEntryModeType.MANUAL;
            check.CheckType = checkTypeType.PERSONAL;
            check.AccountType = accountTypeType.SAVINGS;
            var response = _checkService.Sale(21.00m)
                .WithCheck(check)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void Test020EbronzeBusinessSavings()
        {
            var check = TestCheck.Certification;
            check.SecCode = HpsSECCode.EBRONZE;
            check.CheckVerify = true;
            check.DataEntryMode = dataEntryModeType.MANUAL;
            check.CheckType = checkTypeType.BUSINESS;
            check.AccountType = accountTypeType.SAVINGS;
            var response = _checkService.Sale(22.00m)
                .WithCheck(check)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        #endregion

        #region Checks-by-Web

        [TestMethod]
        public void Test021WebPersonalChecking()
        {
            var check = TestCheck.Certification;
            check.SecCode = HpsSECCode.WEB;
            check.DataEntryMode = dataEntryModeType.MANUAL;
            check.CheckType = checkTypeType.PERSONAL;
            check.AccountType = accountTypeType.CHECKING;
            var response = _checkService.Sale(23.00m)
                .WithCheck(check)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void Test022WebBuisnessChecking()
        {
            var check = TestCheck.Certification;
            check.SecCode = HpsSECCode.WEB;
            check.DataEntryMode = dataEntryModeType.MANUAL;
            check.CheckType = checkTypeType.BUSINESS;
            check.AccountType = accountTypeType.CHECKING;
            var response = _checkService.Sale(24.00m)
                .WithCheck(check)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void Test023WebPersonalSavings()
        {
            var check = TestCheck.Certification;
            check.SecCode = HpsSECCode.WEB;
            check.DataEntryMode = dataEntryModeType.MANUAL;
            check.CheckType = checkTypeType.PERSONAL;
            check.AccountType = accountTypeType.SAVINGS;
            var response = _checkService.Sale(25.00m)
                .WithCheck(check)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
            test23TransactionId = response.TransactionId;
        }

        [TestMethod]
        public void Test024WebBusinessSavings()
        {
            var check = TestCheck.Certification;
            check.SecCode = HpsSECCode.WEB;
            check.DataEntryMode = dataEntryModeType.MANUAL;
            check.CheckType = checkTypeType.BUSINESS;
            check.AccountType = accountTypeType.SAVINGS;
            var response = _checkService.Sale(5.00m)
                .WithCheck(check)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        #endregion

        #region Check Void

        [TestMethod]
        public void Test025PpdCheckVoid()
        {
            var response = _checkService.Void()
                .withClientTransactionId(test01TransactionId)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void Test026CcdCheckVoid()
        {
            var response = _checkService.Void()
                .withClientTransactionId(test05TransactionId)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void Test027PopCheckVoid()
        {
            var response = _checkService.Void()
                .withClientTransactionId(test10TransactionId)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void Test028PopCheckVoid()
        {
            var response = _checkService.Void()
                .withClientTransactionId(test14TransactionId)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod]
        public void Test029WebCheckVoid()
        {
            var response = _checkService.Void()
                .withClientTransactionId(test23TransactionId)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        #endregion
    }

}
