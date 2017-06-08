using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SecureSubmit.Infrastructure;
using SecureSubmit.Infrastructure.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecureSubmit.Entities;

namespace SecureSubmit.Tests.Validation
{

    [TestClass]
    public class HpsInputValidationTests
    {
        #region ValidEmailTest

        [TestMethod]
        public void EmailWithDoubleDot()
        {
            HpsCardHolder ValidCardHolder = new HpsCardHolder { Email = @"Abc..123@example.com" };
            Assert.AreEqual(ValidCardHolder.Email, @"Abc..123@example.com");
        }

        [TestMethod]
        public void EmailWithOutCom()
        {
            HpsCardHolder ValidCardHolder = new HpsCardHolder { Email = "email@example.web" };
            Assert.AreEqual(ValidCardHolder.Email, "email@example.web");
        }

        [TestMethod]
        public void EmailAlphabetsWithOutSpace()
        {
            HpsCardHolder ValidCardHolder = new HpsCardHolder { Email = "email@example.com" };
            Assert.AreEqual(ValidCardHolder.Email, "email@example.com");
        }

        [TestMethod]
        public void EmailWithPostNumber()
        {
            HpsCardHolder ValidCardHolder = new HpsCardHolder { Email = "email@111.222.333.44444" };
            Assert.AreEqual(ValidCardHolder.Email, "email@111.222.333.44444");
        }

        [TestMethod]
        public void EmailWithNumbers()
        {
            HpsCardHolder ValidCardHolder = new HpsCardHolder { Email = "testmail123456@hps.com" };
            Assert.AreEqual(ValidCardHolder.Email, "testmail123456@hps.com");
        }

        [TestMethod]
        public void EmailWithExtraDots()
        {
            HpsCardHolder ValidCardHolder = new HpsCardHolder { Email = "test.email@gmail.io.uk" };
            Assert.AreEqual(ValidCardHolder.Email, "test.email@gmail.io.uk");
        }

        [TestMethod]
        public void EmailWithUnderScoresValue()
        {
            HpsCardHolder ValidCardHolder = new HpsCardHolder { Email = "_____@example.com" };
            Assert.AreEqual(ValidCardHolder.Email, "_____@example.com");
        }

        [TestMethod]
        public void EmailWithHyphenValue()
        {
            HpsCardHolder ValidCardHolder = new HpsCardHolder { Email = "firstname-lastname@example.com" };
            Assert.AreEqual(ValidCardHolder.Email, "firstname-lastname@example.com");
        }

        [TestMethod]
        public void EmailWithExtraPostfixValue()
        {
            HpsCardHolder ValidCardHolder = new HpsCardHolder { Email = "email@example.co.jp" };
            Assert.AreEqual(ValidCardHolder.Email, "email@example.co.jp");
        }

        [TestMethod]
        public void EmailWithPlusValue()
        {
            HpsCardHolder ValidCardHolder = new HpsCardHolder { Email = "firstname+lastname@example.com" };
            Assert.AreEqual(ValidCardHolder.Email, "firstname+lastname@example.com");
        }

        [TestMethod]
        public void EmailWithDoubleInvertedCommaValue()
        {
            HpsCardHolder ValidCardHolder = new HpsCardHolder { Email = "\"email\"@example.com" };
            Assert.AreEqual(ValidCardHolder.Email, "\"email\"@example.com");
        }

        [TestMethod]
        public void EmailWithSingleInvertedCommaValue()
        {
            HpsCardHolder ValidCardHolder = new HpsCardHolder { Email = "'email'@example.com" };
            Assert.AreEqual(ValidCardHolder.Email, "'email'@example.com");
        }

        [TestMethod]
        public void EmailWithPostfixNumberValue()
        {
            HpsCardHolder ValidCardHolder = new HpsCardHolder { Email = @"email@[123.123.123.123]" };
            Assert.AreEqual(ValidCardHolder.Email, @"email@[123.123.123.123]");
        }

        #endregion

        #region InvalidEmailTest

        [TestMethod, ExpectedException(typeof(HpsInvalidRequestException))]
        public void EmailWithEmptyValue()
        {
            HpsCardHolder InValidCardHolder = new HpsCardHolder { Email = "" };
        }

        [TestMethod, ExpectedException(typeof(HpsInvalidRequestException))]
        public void EmailWithPostFixDoubleDot()
        {
            HpsCardHolder InValidCardHolder = new HpsCardHolder { Email = "email @example..com" };
        }

        [TestMethod, ExpectedException(typeof(HpsInvalidRequestException))]
        public void EmailAlphabetsWithSpace()
        {
            HpsCardHolder InValidCardHolder = new HpsCardHolder { Email = "email @example.com" };
        }


        //Negative test
        [TestMethod, ExpectedException(typeof(HpsInvalidRequestException))]
        public void EmailWithInvalidFormats()
        {
            HpsCardHolder InValidCardHolder = new HpsCardHolder { Email = "www.testemailgmail.com" };
        }

        [TestMethod, ExpectedException(typeof(HpsInvalidRequestException))]
        public void EmailWithoutMatchPattern()
        {
            HpsCardHolder InValidCardHolder = new HpsCardHolder { Email = "www.testemail.gmail.com" };
        }

        [TestMethod, ExpectedException(typeof(HpsInvalidRequestException))]
        public void EmailWithLimitExceed()
        {
            HpsCardHolder InValidCardHolder = new HpsCardHolder { Email = "loremipsum.loremipsumloremipsumloremipsumloremipsumloremipsumloremipsumloremipsumloremipsumloremipsum@test.com" };
        }

        [TestMethod, ExpectedException(typeof(HpsInvalidRequestException))]
        public void EmailWithSpecialCharacter()
        {
            HpsCardHolder InValidCardHolder = new HpsCardHolder { Email = "#@%^%#$@#$@#.com" };
        }

        [TestMethod, ExpectedException(typeof(HpsInvalidRequestException))]
        public void InCompleteEmailValue()
        {
            HpsCardHolder InValidCardHolder = new HpsCardHolder { Email = "@example.com" };
        }

        [TestMethod, ExpectedException(typeof(HpsInvalidRequestException))]
        public void EmailWithSpaceAndSpecialcharacter()
        {
            HpsCardHolder InValidCardHolder = new HpsCardHolder { Email = "Joe Smith<email@example.com>" };
        }

        [TestMethod, ExpectedException(typeof(HpsInvalidRequestException))]
        public void EmailWithExtraAt()
        {
            HpsCardHolder InValidCardHolder = new HpsCardHolder { Email = "email@example@example.com" };
        }

        #endregion

        #region ValidPhoneNumber
        [TestMethod]
        public void PhoneNumberWithHyphens()
        {
            HpsCardHolder ValidCardHolder = new HpsCardHolder { Phone = "555-569-5626" };
            Assert.AreEqual(ValidCardHolder.Phone, "5555695626");
        }

        [TestMethod]
        public void PhoneNumberWithParanthesis()
        {
            HpsCardHolder ValidCardHolder = new HpsCardHolder { Phone = "(256)555-{5555}" };
            Assert.AreEqual(ValidCardHolder.Phone, "2565555555");
        }

        [TestMethod]
        public void PhoneNumberWithDots()
        {
            HpsCardHolder ValidCardHolder = new HpsCardHolder { Phone = "256.555.5555" };
            Assert.AreEqual(ValidCardHolder.Phone, "2565555555");
        }

        [TestMethod]
        public void PhoneNumberWithAlphabets()
        {
            HpsCardHolder ValidCardHolder = new HpsCardHolder { Phone = "abcdf2565555555" };
            Assert.AreEqual(ValidCardHolder.Phone, "2565555555");
        }

        [TestMethod]
        public void PhoneNumberWithDoubleQuotation()
        {
            HpsCardHolder ValidCardHolder = new HpsCardHolder { Phone = "256\"5555\"555" };
            Assert.AreEqual(ValidCardHolder.Phone, "2565555555");
        }

        [TestMethod]
        public void PhoneNumberWithSingleQuotation()
        {
            HpsCardHolder ValidCardHolder = new HpsCardHolder { Phone = "256'5555'555" };
            Assert.AreEqual(ValidCardHolder.Phone, "2565555555");
        }

        [TestMethod]
        public void PhoneNumberWithQuestionMark()
        {
            HpsCardHolder ValidCardHolder = new HpsCardHolder { Phone = "256?5555?555" };
            Assert.AreEqual(ValidCardHolder.Phone, "2565555555");
        }

        [TestMethod]
        public void PhoneNumberWithSpecialCharacter()
        {
            HpsCardHolder ValidCardHolder = new HpsCardHolder { Phone = "256&#5555?^%555*!" };
            Assert.AreEqual(ValidCardHolder.Phone, "2565555555");
        }

        [TestMethod]
        public void PhoneNumberWithSquareBracketsr()
        {
            HpsCardHolder ValidCardHolder = new HpsCardHolder { Phone = "256[5555][555]" };
            Assert.AreEqual(ValidCardHolder.Phone, "2565555555");
        }

        [TestMethod]
        public void PhoneNumberWithOr()
        {
            HpsCardHolder ValidCardHolder = new HpsCardHolder { Phone = "256|5555|555|" };
            Assert.AreEqual(ValidCardHolder.Phone, "2565555555");
        }

        [TestMethod]
        public void PhoneNumberWithEmpty()
        {
            HpsCardHolder ValidCardHolder = new HpsCardHolder { Phone = "" };
            Assert.AreEqual(ValidCardHolder.Phone, "");
        }

        [TestMethod]
        public void PhoneNumberWithOnlyAlphabets()
        {
            HpsCardHolder ValidCardHolder = new HpsCardHolder { Phone = "abcdefghijk" };
            Assert.AreEqual(ValidCardHolder.Phone, "");
        }

        #endregion

        #region InvalidPhoneNumber

        [TestMethod, ExpectedException(typeof(HpsInvalidRequestException))]
        public void CheckPhoneNumberWithLimitExceed()
        {
            HpsCardHolder InValidCardHolder = new HpsCardHolder { Phone = "1234567894560321654987" };
        }

        #endregion

        #region  ValidZipCode

        [TestMethod]
        public void ZipCodeWithNumbers()
        {
            HpsCardHolder ValidCardHolder = new HpsCardHolder { Address = new HpsAddress { Zip = "9654266" } };
            Assert.AreEqual(ValidCardHolder.Address.Zip, "9654266");
        }

        [TestMethod]
        public void ZipCodeWithSpecialCharacters()
        {
            HpsCardHolder ValidCardHolder = new HpsCardHolder { Address = new HpsAddress { Zip = "96-54-(266)" } };
            Assert.AreEqual(ValidCardHolder.Address.Zip, "9654266");
        }

        [TestMethod]
        public void ZipCodeWithAlphaNumericalValues()
        {
            HpsCardHolder ValidCardHolder = new HpsCardHolder { Address = new HpsAddress { Zip = "-d@#@^fd324(789)" } };
            Assert.AreEqual(ValidCardHolder.Address.Zip, "dfd324789");
        }

        [TestMethod]
        public void ZipCodeWithSpecialCharacter()
        {
            HpsCardHolder ValidCardHolder = new HpsCardHolder { Address = new HpsAddress { Zip = "-d@#@&*^fd32%!~`4(789)" } };
            Assert.AreEqual(ValidCardHolder.Address.Zip, "dfd324789");
        }

        [TestMethod]
        public void ZipCodeWithSquareBracket()
        {
            HpsCardHolder ValidCardHolder = new HpsCardHolder { Address = new HpsAddress { Zip = "-d[fd32]4[789]" } };
            Assert.AreEqual(ValidCardHolder.Address.Zip, "dfd324789");
        }

        [TestMethod]
        public void ZipCodeWithQuestionMark()
        {
            HpsCardHolder ValidCardHolder = new HpsCardHolder { Address = new HpsAddress { Zip = "-d?fd32?4?789?" } };
            Assert.AreEqual(ValidCardHolder.Address.Zip, "dfd324789");
        }

        [TestMethod]
        public void ZipCodeWithDoubleQuotation()
        {
            HpsCardHolder ValidCardHolder = new HpsCardHolder { Address = new HpsAddress { Zip = "-d\"fd32\"4\"789\"" } };
            Assert.AreEqual(ValidCardHolder.Address.Zip, "dfd324789");
        }

        [TestMethod]
        public void ZipCodeWithSingleQuotation()
        {
            HpsCardHolder ValidCardHolder = new HpsCardHolder { Address = new HpsAddress { Zip = "-d'fd32'4'789'" } };
            Assert.AreEqual(ValidCardHolder.Address.Zip, "dfd324789");
        }
        [TestMethod]
        public void ZipCodeWithOrMark()
        {
            HpsCardHolder ValidCardHolder = new HpsCardHolder { Address = new HpsAddress { Zip = "-d|fd32|4|789?" } };
            Assert.AreEqual(ValidCardHolder.Address.Zip, "dfd324789");
        }

        [TestMethod]
        public void ZipCodeWithEmptyString()
        {
            HpsCardHolder ValidCardHolder = new HpsCardHolder { Address = new HpsAddress { Zip = "" } };
            Assert.AreEqual(ValidCardHolder.Address.Zip, "");
        }

        #endregion

        #region InvalidZipCode

        [TestMethod, ExpectedException(typeof(HpsInvalidRequestException))]
        public void ZipCodeWithLimits()
        {
            HpsCardHolder InValidCardHolder = new HpsCardHolder { Address = new HpsAddress { Zip = "1234567890" } };
        }

        #endregion

        #region ValidCardDetails

        [TestMethod]
        public void CardDetailsForFirstname()
        {
            HpsCardHolder ValidCardHolder = new HpsCardHolder { FirstName = "Testfirstname" };
            Assert.AreEqual(ValidCardHolder.FirstName, "Testfirstname");
        }

        [TestMethod]
        public void FirstnamewithSpecialcharacter()
        {
            HpsCardHolder ValidCardHolder = new HpsCardHolder { FirstName = "$%^!~@^&*()firstname" };
            Assert.AreEqual(ValidCardHolder.FirstName, "$%^!~@^&*()firstname");
        }

        [TestMethod]
        public void FirstnamewithQuestionMark()
        {
            HpsCardHolder ValidCardHolder = new HpsCardHolder { FirstName = "?f?irstna?me" };
            Assert.AreEqual(ValidCardHolder.FirstName, "?f?irstna?me");
        }

        [TestMethod]
        public void FirstnamewithDoubleQuotation()
        {
            HpsCardHolder ValidCardHolder = new HpsCardHolder { FirstName = "\"f\"irstna\"me" };
            Assert.AreEqual(ValidCardHolder.FirstName, "\"f\"irstna\"me");
        }

        [TestMethod]
        public void FirstnamewithSingleQuotation()
        {
            HpsCardHolder ValidCardHolder = new HpsCardHolder { FirstName = "'f'irstna'me" };
            Assert.AreEqual(ValidCardHolder.FirstName, "'f'irstna'me");
        }

        [TestMethod]
        public void CardDetailsForLastname()
        {
            HpsCardHolder ValidCardHolder = new HpsCardHolder { LastName = "Testlastname" };
            Assert.AreEqual(ValidCardHolder.LastName, "Testlastname");
        }

        [TestMethod]
        public void LastnamewithSpecialcharacter()
        {
            HpsCardHolder ValidCardHolder = new HpsCardHolder { LastName = "$%^!~@^&*()Lastname" };
            Assert.AreEqual(ValidCardHolder.LastName, "$%^!~@^&*()Lastname");
        }

        [TestMethod]
        public void LastnamewithQuestionMark()
        {
            HpsCardHolder ValidCardHolder = new HpsCardHolder { LastName = "?f?irstna?me" };
            Assert.AreEqual(ValidCardHolder.LastName, "?f?irstna?me");
        }

        [TestMethod]
        public void LastnamewithDoubleQuotation()
        {
            HpsCardHolder ValidCardHolder = new HpsCardHolder { LastName = "\"f\"irstna\"me" };
            Assert.AreEqual(ValidCardHolder.LastName, "\"f\"irstna\"me");
        }

        [TestMethod]
        public void LastnamewithSingleQuotation()
        {
            HpsCardHolder ValidCardHolder = new HpsCardHolder { LastName = "'f'irstna'me" };
            Assert.AreEqual(ValidCardHolder.LastName, "'f'irstna'me");
        }

        [TestMethod]
        public void CardDetailsForAddress()
        {
            HpsCardHolder ValidCardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "Testaddress, sample address" } };
            Assert.AreEqual(ValidCardHolder.Address.Address, "Testaddress, sample address");
        }

        [TestMethod]
        public void AddresswithSpecialcharacter()
        {
            HpsCardHolder ValidCardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "~`!@#$%^&*adress, sample address" } };
            Assert.AreEqual(ValidCardHolder.Address.Address, "~`!@#$%^&*adress, sample address");
        }

        [TestMethod]
        public void AddresswithQuestionMark()
        {
            HpsCardHolder ValidCardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "?Testaddress?, sample?address" } };
            Assert.AreEqual(ValidCardHolder.Address.Address, "?Testaddress?, sample?address");
        }

        [TestMethod]
        public void AddresswithDoubleQuotation()
        {
            HpsCardHolder ValidCardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "Testaddress,\" sample\"address" } };
            Assert.AreEqual(ValidCardHolder.Address.Address, "Testaddress,\" sample\"address");
        }

        [TestMethod]
        public void AddresswithSingleQuotation()
        {
            HpsCardHolder ValidCardHolder = new HpsCardHolder { Address = new HpsAddress { Address = "Testaddress,' sample' address'" } };
            Assert.AreEqual(ValidCardHolder.Address.Address, "Testaddress,' sample' address'");
        }

        [TestMethod]
        public void CardDetailsForCity()
        {
            HpsCardHolder ValidCardHolder = new HpsCardHolder { Address = new HpsAddress { City = "TestCity" } };
            Assert.AreEqual(ValidCardHolder.Address.City, "TestCity");
        }

        [TestMethod]
        public void CitywithSpecialcharacter()
        {
            HpsCardHolder ValidCardHolder = new HpsCardHolder { Address = new HpsAddress { City = "~`!@#$%^&*city," } };
            Assert.AreEqual(ValidCardHolder.Address.City, "~`!@#$%^&*city,");
        }

        [TestMethod]
        public void CitywithQuestionMark()
        {
            HpsCardHolder ValidCardHolder = new HpsCardHolder { Address = new HpsAddress { City = "?city?" } };
            Assert.AreEqual(ValidCardHolder.Address.City, "?city?");
        }

        [TestMethod]
        public void CitywithDoubleQuotation()
        {
            HpsCardHolder ValidCardHolder = new HpsCardHolder { Address = new HpsAddress { City = "city\" " } };
            Assert.AreEqual(ValidCardHolder.Address.City, "city\" ");
        }

        [TestMethod]
        public void CitywithSingleQuotation()
        {
            HpsCardHolder ValidCardHolder = new HpsCardHolder { Address = new HpsAddress { City = "'sample'" } };
            Assert.AreEqual(ValidCardHolder.Address.City, "'sample'");
        }

        [TestMethod]
        public void CardDetailsForState()
        {
            HpsCardHolder ValidCardHolder = new HpsCardHolder { Address = new HpsAddress { State = "Teststate" } };
            Assert.AreEqual(ValidCardHolder.Address.State, "Teststate");
        }

        [TestMethod]
        public void StatewithSpecialcharacter()
        {
            HpsCardHolder ValidCardHolder = new HpsCardHolder { Address = new HpsAddress { State = "~`!@#$%^&*state" } };
            Assert.AreEqual(ValidCardHolder.Address.State, "~`!@#$%^&*state");
        }

        [TestMethod]
        public void StatewithQuestionMark()
        {
            HpsCardHolder ValidCardHolder = new HpsCardHolder { Address = new HpsAddress { State = "?state?" } };
            Assert.AreEqual(ValidCardHolder.Address.State, "?state?");
        }

        [TestMethod]
        public void StatewithDoubleQuotation()
        {
            HpsCardHolder ValidCardHolder = new HpsCardHolder { Address = new HpsAddress { State = "State\"" } };
            Assert.AreEqual(ValidCardHolder.Address.State, "State\"");
        }

        [TestMethod]
        public void StatewithSingleQuotation()
        {
            HpsCardHolder ValidCardHolder = new HpsCardHolder { Address = new HpsAddress { State = "'state''" } };
            Assert.AreEqual(ValidCardHolder.Address.State, "'state''");
        }

        [TestMethod]
        public void CardDetailsWithEmpty()
        {
            HpsCardHolder ValidCardHolder = new HpsCardHolder { Address = new HpsAddress { State = "" } };
            Assert.AreEqual(ValidCardHolder.Address.State, "");
        }

        [TestMethod]
        public void FirstNameWithSpecialChars()
        {
            HpsCardHolder InValidCardHolder = new HpsCardHolder { FirstName = "<name#$%>" };
            Assert.AreEqual(InValidCardHolder.FirstName, "<name#$%>");
        }

        #endregion

        #region InvalidCardDeatails

        [TestMethod, ExpectedException(typeof(HpsInvalidRequestException))]
        public void FirstNameWithLimits()
        {
            HpsCardHolder InValidCardHolder = new HpsCardHolder { FirstName = "Testdata sample lorem Testdata sample lorem Testdata sample lorem" };
        }

        [TestMethod, ExpectedException(typeof(HpsInvalidRequestException))]
        public void LastNameWithLimits()
        {
            HpsCardHolder InValidCardHolder = new HpsCardHolder { LastName = "Test last name test last name me test last name me test last name " };
        }

        [TestMethod, ExpectedException(typeof(HpsInvalidRequestException))]
        public void CityWithLimits()
        {
            HpsCardHolder InValidCardHolder = new HpsCardHolder { Address = new HpsAddress { City = "Test city test city name me test last name me test city " } };
        }

        [TestMethod, ExpectedException(typeof(HpsInvalidRequestException))]
        public void StateWithLimits()
        {
            HpsCardHolder InValidCardHolder = new HpsCardHolder { Address = new HpsAddress { City = "Test state test city name me test last name me state city " } };
        }

        [TestMethod, ExpectedException(typeof(HpsInvalidRequestException))]
        public void AddressWithLimits()
        {
            HpsCardHolder InValidCardHolder = new HpsCardHolder { Address = new HpsAddress { City = "Test address test address name me test address name me state address " } };
        }
        
        #endregion
    }
}