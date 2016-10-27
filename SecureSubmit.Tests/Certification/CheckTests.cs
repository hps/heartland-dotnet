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
        public void checks_001ConsumerPersonalChecking()
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
        public void checks_002ConsumerBusinessChecking()
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
        public void checks_003ConsumerPersonalSavings()
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
        public void checks_004ConsumerBusinessSavings()
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
        public void checks_005CorporatePersonalChecking()
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
        public void checks_006CorporateBuisnessChecking()
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
        public void checks_007CorporatePersonalSavings()
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
        public void checks_008CorporateBuisnessSavings()
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
        public void checks_009EgoldPersonalChecking()
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
        public void checks_010EgoldBuisnessChecking()
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
        public void checks_011EgoldPersonalSavings()
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
        public void checks_012EgoldBusinessSavings()
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
        public void checks_013EsilverPersonalChecking()
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
        public void checks_014EsilverBuisnessChecking()
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
        public void checks_015EsilverPersonalSavings()
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
        public void checks_016EsilverBuisnessSavings()
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

        [TestMethod, Ignore]
        public void checks_017EbronzePersonalChecking()
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

        [TestMethod, Ignore]
        public void checks_018EbronzePersonalChecking()
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

        [TestMethod, Ignore]
        public void checks_019EbronzePersonalChecking()
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

        [TestMethod, Ignore]
        public void checks_020EbronzeBusinessSavings()
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
        public void checks_021WebPersonalChecking()
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
        public void checks_022WebBuisnessChecking()
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
        public void checks_023WebPersonalSavings()
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
        public void checks_024WebBusinessSavings()
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

        [TestMethod, Ignore]
        public void checks_025PpdCheckVoid()
        {
            var response = _checkService.Void()
                .withTransactionId(test01TransactionId)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod, Ignore]
        public void checks_026CcdCheckVoid()
        {
            var response = _checkService.Void()
                .withTransactionId(test05TransactionId)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod, Ignore]
        public void checks_027PopCheckVoid()
        {
            var response = _checkService.Void()
                .withTransactionId(test10TransactionId)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod, Ignore]
        public void checks_028PopCheckVoid()
        {
            var response = _checkService.Void()
                .withTransactionId(test14TransactionId)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        [TestMethod, Ignore]
        public void checks_029WebCheckVoid()
        {
            var response = _checkService.Void()
                .withTransactionId(test23TransactionId)
                .Execute();
            Assert.IsNotNull(response);
            Assert.AreEqual("0", response.ResponseCode);
        }

        #endregion
    }

}
