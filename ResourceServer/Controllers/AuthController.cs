using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Primitives;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ResourceServer.JSONModels;
using ResourceServer.Models;

namespace ResourceServer.Controllers
{
    [Route("api/[controller]/[action]")]
    [Produces("application/json")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IHttpClientFactory clientFactory,
            ILogger<AuthController> logger)
        {
            _clientFactory = clientFactory;
            _logger = logger;
        }

        // POST: api/Auth/login
        [HttpPost]
        public async Task<JObject> login(LoginJSON loginJson)
        {
            //TODO: maybe? check if user exists in Users table

            var client = _clientFactory.CreateClient("auth");
            var response = await client.PostAsync(
                "connect/token",
                new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("username", loginJson.Login),
                    new KeyValuePair<string, string>("password", loginJson.Password),
                    new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("scope", "offline_access")
                }));

            var str = await response.Content.ReadAsStringAsync();
            return JObject.Parse(str);
        }

        // POST: api/Auth/refresh
        [HttpPost]
        public async Task<JObject> refresh(RefreshJSON refreshJson)
        {
            var client = _clientFactory.CreateClient("auth");
            var response = await client.PostAsync(
                "connect/token",
                new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("grant_type", "refresh_token"),
                    new KeyValuePair<string, string>("refresh_token", refreshJson.RefreshToken)
                }));

            var str = await response.Content.ReadAsStringAsync();
            return JObject.Parse(str);
        }

        // POST: api/Auth/register
        [HttpPost]
        public async Task<JObject> register(RegisterJSON registerJson)
        {
            var client = _clientFactory.CreateClient("auth");
            var response = await client.PostAsync(
                "connect/register",
                new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Login", registerJson.Login),
                    new KeyValuePair<string, string>("Email", registerJson.Email),
                    new KeyValuePair<string, string>("Password", registerJson.Password)
                }));

            var str = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Response from authServer: " + str);

            if (str.Length <= 1)
                return JObject.Parse("{\"RegisterStatus\": " + str + "}");

            var user = new User
            {
                ID_User = str,
                Login = registerJson.Login,
                Email = registerJson.Email,
                Rate = null,
                isBlocked = false,
                IDRole = 1 //TODO: assign proper role ID
            };

            var personalData = new PersonalData
            {
                IDUser = str,
                FirstName = registerJson.Name,
                LastName = registerJson.Surname
            };

            await TrueHomeContext.addUser(user);
            await TrueHomeContext.addPersonalData(personalData);

            return JObject.Parse("{\"RegisterStatus\": 1}");
        }
    }
}
