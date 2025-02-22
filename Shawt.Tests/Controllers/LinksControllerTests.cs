using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shawt.Models;
using Shawt.Providers;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Shawt.Controllers.Tests
{
    [TestClass()]
    public class LinksControllerTests
    {
        [TestMethod()]
        public void CreatedLink_Should_Rewrite_RequestScheme_WhenPassed()
        {
            var schemeSource = "http";
            var schemeTarget = "https";
            var config = new Mock<IConfiguration>();
            config.SetupGet(x => x[It.Is<string>(k => k == "GeneratedShortUrls:Request:Scheme:From")]).Returns(schemeSource);
            config.SetupGet(x => x[It.Is<string>(k => k == "GeneratedShortUrls:Request:Scheme:To")]).Returns(schemeTarget);

            //var controller = new LinksController(new FakeLinksProvider(), new FakeShortUrlProvider(), config.Object);
            var mockLinksProvider = new Mock<ILinksProvider>();
            mockLinksProvider.Setup(x => x.CreateLink(It.IsAny<string>(), It.IsAny<string>())).Returns(1);

            var mockShortUrlProvider = new Mock<IShortUrlProvider>();
            mockShortUrlProvider.Setup(x => x.Encode(It.IsAny<int>())).Returns("x1");

            var controller = new LinksController(mockLinksProvider.Object, mockShortUrlProvider.Object, config.Object, new Mock<ILogger<LinksController>>().Object);

            var mockUrlHelper = new Mock<IUrlHelper>();
            mockUrlHelper
                .Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>()))
                .Returns($"{schemeSource}://shawt/x1");

            controller.Url = mockUrlHelper.Object;

            var mockIdentity = new Mock<ClaimsPrincipal>();
            mockIdentity
                .Setup(x => x.Identity)
                .Returns(new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimTypes.Name, "satish")
                }));

            var mockContext = new Mock<HttpContext>();
            mockContext.Setup(x => x.User).Returns(mockIdentity.Object);

            controller.ControllerContext = new ControllerContext { HttpContext = mockContext.Object };


            //Act
            var result = controller.Post(new LongUrl { Url = "google.com" });

            //Assert
            Assert.IsInstanceOfType(result, typeof(CreatedResult));
            var createdResult = result as CreatedResult;
            Assert.IsInstanceOfType(createdResult.Value, typeof(LinkDto));
            var link = createdResult.Value as LinkDto;


            Assert.AreEqual($"{schemeTarget}://shawt/x1", link.ShortLink);
        }
    }
}