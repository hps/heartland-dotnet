using System;
using Hps.Exchange.PosGateway.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecureSubmit.Entities;
using SecureSubmit.Infrastructure;
using SecureSubmit.Services;
using SecureSubmit.Tests.TestData;

namespace SecureSubmit.Services.Tests
{
    [TestClass]
    public class HpsAttachmentServiceTests
    {
        public HpsAttachmentService AttachmentService;
        public HpsAttachmentServiceTests()
        {
            HpsServicesConfig config = new HpsServicesConfig
            {
                SecretApiKey = "skapi_cert_MYl2AQAowiQAbLp5JesGKh7QFkcizOP2jcX9BrEMqQ"
            };

            AttachmentService = new HpsAttachmentService(config);
        }
        [TestMethod]
        public void GetAttachmentsByGatewayTxnId1012040132ShouldReturnHpsAttachmentAttachmentDataId47032()
        {
            var result = AttachmentService.GetAttachment(1012040132);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.AttachmentData);
            Assert.IsTrue(result.AttachmentData.StartsWith("iVBORw0KGgoAAAANSUhEUgAAA1YAAAF3AQAAAACOSOd"));
        }
        [TestMethod]
        public void GetAttachmentsByGatewayTxnId1012040131ShouldReturnHpsAttachmentAttachmentDataNull()
        {
            var result = AttachmentService.GetAttachment(1012040131);
            Assert.IsNotNull(result);
            Assert.IsNull(result.AttachmentData);
            Assert.IsTrue(result.ResponseCode.Equals("3"));
        }
        [TestMethod]
        public void GetAttachmentsByGatewayTxnId1012040132AndAttachmentTypeEqualsCustomer_ImageShouldReturnHpsAttachmentAttachmentDataId47032()
        {
            var result = AttachmentService.GetAttachment(1012040132, attachmentTypeType.CUSTOMER_IMAGE);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.AttachmentData);
            Assert.IsTrue(result.AttachmentData.StartsWith("iVBORw0KGgoAAAANSUhEUgAAA1YAAAF3AQAAAACOSOd"));
        }
        [TestMethod]
        public void GetAttachmentsByGatewayTxnId1012040132AndAttachmentTypeEqualsSignature_ImageShouldReturnHpsAttachmentAndNullAttachmentData()
        {
            var result = AttachmentService.GetAttachment(1012040132, attachmentTypeType.SIGNATURE_IMAGE );
            Assert.IsNotNull(result);;
            Assert.IsNull(result.AttachmentData);
            Assert.IsTrue(result.ResponseCode.Equals("0"));
        }
        [TestMethod]
        public void GetAttachmentsByGatewayTxnId1012040132AndAttachmentDataId47032ShouldReturnHpsAttachmentAndAttachmentData()
        {
            var result = AttachmentService.GetAttachment(1012040132,null,false, 47032);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.AttachmentData);
            Assert.IsTrue(result.AttachmentData.StartsWith("iVBORw0KGgoAAAANSUhEUgAAA1YAAAF3AQAAAACOSOd"));
        }
        [TestMethod]
        public void GetAttachmentsByGatewayTxnId1012040132AndWrongAttachmentDataId47033ShouldReturnHpsAttachmentAndNoAttachmentData()
        {
            var result = AttachmentService.GetAttachment(1012040132, null, false, 47033);
            Assert.IsNotNull(result); ;
            Assert.IsNull(result.AttachmentData);
            Assert.IsTrue(result.ResponseCode.Equals("0"));
        }
        [TestMethod]
        public void GetAttachmentsByGatewayTxnId1012040132AndAttachmentDataId47032AndReturnAttachmentTypesOnlyTrueShouldReturnHpsAttachmentAndNoAttachmentData()
        {
            var result = AttachmentService.GetAttachment(1012040132, null, true, 47032);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.AttachmentType);
            Assert.IsTrue(result.AttachmentFormat.Equals("PNG"));
            Assert.IsNull(result.AttachmentData);
        }
    }
}
