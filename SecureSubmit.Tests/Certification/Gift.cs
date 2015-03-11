using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecureSubmit.Services.GiftCard;
using SecureSubmit.Tests.TestData;

namespace SecureSubmit.Tests.Certification
{
    [TestClass]
    public class Gift
    {
        [TestMethod]
        public void GiftCard_ManualCard_ShouldActivate()
        {
            var giftCardSvc = new HpsGiftCardService(TestServicesConfig.ValidSecretKeyConfig());
            var response = giftCardSvc.Activate(100.00M, "usd", TestGiftCard.Manual.validGiftCardNotEncrypted);
            if (response == null)
            {
                Assert.Fail("Response is null.");
            }

            StringAssert.Matches(response.ResponseCode, new Regex("^0$"));
        }
    }
}
