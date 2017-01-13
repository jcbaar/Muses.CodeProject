﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Muses.CodeProject.API;
using Muses.CodeProject.API.Models;
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
    /// Tests for the <see cref="ApiBase"/> class. Uses 'MockHttp' to mock and 
    /// test the actual HttpClient call's.
    /// <seealso cref="https://github.com/richardszalay/mockhttp">MockHttp by Richard Szalay</seealso>
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class ApiBaseTests
    {
        /// <summary>
        /// Dummy model returned by the <see cref="DummyApi"/>
        /// </summary>
        public class DummyModel
        {
            public string message { get; set; }
        }

        /// <summary>
        /// Simple dummy API for testing the <see cref="ApiBase"/> 
        /// </summary>
        public class DummyApi : ApiBase, IDisposable
        {
            public DummyApi(HttpMessageHandler handler, BearerToken token)
                : base(handler, token)
            { }

            /// <summary>
            /// Mocks a call with or without query string parameters.
            /// </summary>
            /// <param name="parameter">The parameter to send. null to send no parameters.</param>
            /// <returns>A <see cref="DummyModel"/> or null in case of an error.</returns>
            public async Task<DummyModel> Send(string parameter = null)
            {
                if (parameter == null)
                {
                    return await GetRequest<DummyModel>("/v1/Test");
                }
                else
                {
                    // We go the ToQueryString() route here so we can test that aswell.
                    Dictionary<string, string> p = new Dictionary<string, string>()
                    {
                        { "parameter", parameter }
                    };
                    return await GetRequest<DummyModel>("/v1/Test" + ToQueryString(p));
                }
            }
        }

        BearerToken _dummy = new BearerToken()
        {
            Token = "whatever",
            ExpiresIn = 10000,
            TokenRequestedAt = DateTime.Now
        };

        [TestMethod]
        public void ApiBase_Is_IDisposable()
        {
            // Arrange
            ApiBase api = new ApiBase(_dummy);

            // Act
            var disposable = api as IDisposable;

            // Assert
            Assert.IsNotNull(disposable);
        }

        [TestMethod]
        public void ApiBase_InputToken_Equals_UsedToken()
        {
            // Arrange and act
            ApiBase api = new ApiBase(_dummy);

            // Assert
            Assert.AreEqual(api.RequestToken, _dummy);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ApiBase_TokenInvalid_Throws()
        {
            // Arrange
            ApiBase api = new ApiBase(_dummy);

            // Act and assert
            api.RequestToken = new BearerToken()
            {
                Token = "  \t \n"
            };
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ApiBase_TokenNull_Throws()
        {
            // Arrange
            ApiBase api = new ApiBase(_dummy);

            // Act and assert
            api.RequestToken = new BearerToken()
            {
                Token = null
            };
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ApiBase_InvalidEmpty_Throws()
        {
            // Arrange
            ApiBase api = new ApiBase(_dummy);

            // Act and assert
            api.RequestToken = new BearerToken()
            {
                Token = ""
            };
        }

        [TestMethod]
        public void ApiBase_SetToken_ChangesConstructorToken()
        {
            // Arrange
            ApiBase api = new ApiBase(_dummy);
            var newToken = new BearerToken()
            {
                Token = "A brand new token",
                ExpiresIn = 500
            };

            // Act
            api.RequestToken = newToken;

            // Assert
            Assert.AreEqual(newToken, api.RequestToken);
        }

        [TestMethod]
        public void ApiBase_Construct_IsOk()
        {
            // Arrange and act
            ApiBase api = new ApiBase(_dummy);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, api.HttpStatusCode);
            Assert.AreEqual("OK", api.HttpStatusMessage);
        }

        [TestMethod]
        public async Task ApiBase_RequestHeaders_AreOk()
        {
            // Arrange
            MockHttpMessageHandler handler = new MockHttpMessageHandler();
            handler.Expect("https://api.codeproject.com/v1/Test")
                .WithHeaders(new Dictionary<string, string>
                {
                    { "Accept", "application/json" },
                    { "Authorization", "Bearer whatever" }
                })
                .Respond("application/json", "{'message': 'ok'}");

            using (var api = new DummyApi(handler, _dummy))
            {
                // Act
                var result = await api.Send();

                // Assert
                Assert.AreEqual(result.message, "ok");
                handler.VerifyNoOutstandingExpectation();
            }
        }

        [TestMethod]
        public async Task ApiBase_RequestHeadersAndParams_AreOk()
        {
            // Arrange
            MockHttpMessageHandler handler = new MockHttpMessageHandler();
            handler.Expect("https://api.codeproject.com/v1/Test")
                .WithHeaders(new Dictionary<string, string>
                {
                    { "Accept", "application/json" },
                    { "Authorization", "Bearer whatever" }
                })
                .WithQueryString("parameter", "value_for_parameter")
                .Respond("application/json", "{'message': 'ok'}");

            using (var api = new DummyApi(handler, _dummy))
            {
                // Act
                var result = await api.Send("value_for_parameter");

                // Assert
                Assert.AreEqual(result.message, "ok");
                handler.VerifyNoOutstandingExpectation();
            }
        }

        [TestMethod]
        public async Task ApiBase_NonOkResponse_ReturnsNull()
        {
            // Arrange
            MockHttpMessageHandler handler = new MockHttpMessageHandler();
            handler.Expect("https://api.codeproject.com/v1/Test")
                .Respond(HttpStatusCode.InternalServerError);

            using (var api = new DummyApi(handler, _dummy))
            {
                // Act
                var data = await api.Send();

                // Assert
                handler.VerifyNoOutstandingExpectation();
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(HttpRequestException))]
        public async Task ApiBase_Exception_Throws()
        {
            // Arrange
            MockHttpMessageHandler handler = new MockHttpMessageHandler();
            handler.Expect("https://api.codeproject.com/v1/Test")
                .Throw(new HttpRequestException());

            using (var api = new DummyApi(handler, _dummy))
            {
                // Act and assert
                var data = await api.Send();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(JsonReaderException))]
        public async Task ApiBase_GarbageResponse_Throws()
        {
            // Arrange
            MockHttpMessageHandler handler = new MockHttpMessageHandler();
            handler.Expect("https://api.codeproject.com/v1/Test")
                .Respond("application/json", "This is a garbage response.");

            using (var api = new DummyApi(handler, _dummy))
            {
                // Act and assert
                var data = await api.Send();
            }
        }
    }
}