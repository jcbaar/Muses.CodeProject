using Microsoft.VisualStudio.TestTools.UnitTesting;
using Muses.CodeProject.API;
using Newtonsoft.Json;
using RichardSzalay.MockHttp;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Muses.CodeProject.Tests
{
    /// <summary>
    /// Tests for the <see cref="AccessToken"/> class. Uses 'MockHttp' to mock and 
    /// test the actual HttpClient call's.
    /// <seealso cref="https://github.com/richardszalay/mockhttp">MockHttp by Richard Szalay</seealso>
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class AccessTokenTests
    {
        [TestMethod]
        public void AccessToken_Is_IDisposable()
        {
            // Arrange
            AccessToken token = new AccessToken("client", "secret");

            // Act
            var disposable = token as IDisposable;

            // Assert
            Assert.IsNotNull(disposable);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AccessToken_Construct_InValidClientId_Throw()
        {
            // Arrange, act and assert.
            AccessToken token = new AccessToken("   \t \n", "sercret");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AccessToken_Construct_InValidClientSecret_Throw()
        {
            // Arrange, act and assert.
            AccessToken token = new AccessToken("id", "  \t \n");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AccessToken_Construct_NullClientId_Throw()
        {
            // Arrange, act and assert.
            AccessToken token = new AccessToken(null, "sercret");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AccessToken_Construct_NullClientSecret_Throw()
        {
            // Arrange, act and assert.
            AccessToken token = new AccessToken("id", null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AccessToken_Construct_EmptyClientId_Throw()
        {
            // Arrange, act and assert.
            AccessToken token = new AccessToken("", "sercret");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AccessToken_Construct_EmptyClientSecret_Throw()
        {
            // Arrange, act and assert.
            AccessToken token = new AccessToken("id", "");
        }

        [TestMethod]
        public async Task AccessToken_GetAccessToken_Client()
        {
            // Arrange
            MockHttpMessageHandler handler = new MockHttpMessageHandler();
            handler.Expect("https://api.codeproject.com/Token")
                .WithHeaders( "Accept", "application/json" )
                .WithFormData(new Dictionary<string, string>
                {
                    { "grant_type", "client_credentials" },
                    { "client_id", "id" },
                    { "client_secret", "secret" }
                })
                .Respond("application/json", "{ 'access_token': 'client_token', 'expires_in': '10000' }");

            using (var token = new AccessToken(handler, "id", "secret"))
            {
                // Act
                var bearer = await token.GetAccessToken();

                // Assert
                handler.VerifyNoOutstandingExpectation();
                Assert.AreEqual("client_token", bearer.Token);
            }
        }

        [TestMethod]
        public async Task AccessToken_GetAccessToken_InvalidToken()
        {
            // Arrange
            MockHttpMessageHandler handler = new MockHttpMessageHandler();
            handler.Expect("https://api.codeproject.com/Token")
                .WithHeaders("Accept", "application/json")
                .WithFormData(new Dictionary<string, string>
                {
                    { "grant_type", "client_credentials" },
                    { "client_id", "id" },
                    { "client_secret", "secret" }
                })
                .Respond("application/json", "{ 'access_token': '', 'expires_in': '10000' }");

            using (var token = new AccessToken(handler, "id", "secret"))
            {
                // Act
                var bearer = await token.GetAccessToken();

                // Assert
                handler.VerifyNoOutstandingExpectation();
                Assert.IsNull(bearer);
            }
        }

        [TestMethod]
        public async Task AccessToken_GetAccessToken_User()
        {
            // Arrange
            MockHttpMessageHandler handler = new MockHttpMessageHandler();
            handler.Expect("https://api.codeproject.com/Token")
                .WithHeaders("Accept", "application/json")
                .WithFormData(new Dictionary<string, string>
                {
                    { "grant_type", "password" },
                    { "username", "user" },
                    { "password", "password" },
                    { "client_id", "id" },
                    { "client_secret", "secret" }
                })
                .Respond("application/json", "{ 'access_token': 'user_token', 'expires_in': '10000' }");

            using (var token = new AccessToken(handler, "id", "secret"))
            {
                // Act
                var bearer = await token.GetAccessToken(new NetworkCredential("user", "password"));

                // Assert
                handler.VerifyNoOutstandingExpectation();
                Assert.AreEqual("user_token", bearer.Token);
            }
        }

        [TestMethod]
        public async Task AccessToken_GetAccessToken_NonOkResponse_ReturnsNull()
        {
            // Arrange
            MockHttpMessageHandler handler = new MockHttpMessageHandler();
            handler.Expect("https://api.codeproject.com/Token")
                .Respond(HttpStatusCode.InternalServerError);

            using (var token = new AccessToken(handler, "id", "secret"))
            {
                // Act
                var bearer = await token.GetAccessToken(new NetworkCredential("user", "password"));

                // Assert
                handler.VerifyNoOutstandingExpectation();
                Assert.IsNull(bearer);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(HttpRequestException))]
        public async Task AccessToken_GetUserAccessToken_Exception_Throws()
        {
            // Arrange
            MockHttpMessageHandler handler = new MockHttpMessageHandler();
            handler.Expect("https://api.codeproject.com/Token")
                .Throw(new HttpRequestException());

            using (var token = new AccessToken(handler, "id", "secret"))
            {
                // Act and assert
                var bearer = await token.GetAccessToken(new NetworkCredential("user", "password"));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(HttpRequestException))]
        public async Task AccessToken_GetClientAccessToken_Exception_Throws()
        {
            // Arrange
            MockHttpMessageHandler handler = new MockHttpMessageHandler();
            handler.Expect("https://api.codeproject.com/Token")
                .Throw(new HttpRequestException());

            using (var token = new AccessToken(handler, "id", "secret"))
            {
                // Act and assert
                var bearer = await token.GetAccessToken();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(JsonReaderException))]
        public async Task AccessToken_GetAccessToken_GarbageResponse_Throws()
        {
            // Arrange
            MockHttpMessageHandler handler = new MockHttpMessageHandler();
            handler.Expect("https://api.codeproject.com/Token")
                .Respond("application/json", "This is a garbage response.");

            using (var token = new AccessToken(handler, "id", "secret"))
            {
                // Act and assert
                var bearer = await token.GetAccessToken(new NetworkCredential("user", "password"));
            }
        }
    }
}
