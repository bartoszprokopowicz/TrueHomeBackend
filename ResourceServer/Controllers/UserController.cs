using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ResourceServer.Models;
using ResourceServer.JSONModels;

namespace ResourceServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public UserController(
            IConfiguration configuration,
            ILogger<AuthController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet("phoneNumber")]
        public JObject getPhoneNumber()
        {
            var userId = User.FindFirst("sub")?.Value;

            string phoneNumber = TrueHomeContext.getPhoneNumber(userId);
            string jsonData = null;
            if (phoneNumber == null)
                jsonData = "{ 'phoneNumber': null }";
            else
                jsonData = "{'phoneNumber': " + $"'{phoneNumber}'" + "'}";
            
            return JObject.Parse(jsonData);
        }

        // POST: api/User/details
        /**
         * gets details of user
         * **/
        [HttpGet("details")]
        public string Details(){
            var userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId))
                return "Missing user with this id";

            var userDetails = new UserDetailsJSON
            {
                personalData = TrueHomeContext.getPersonalDataByUserID(userId),
                user = TrueHomeContext.getUser(userId),
                apartmentList = TrueHomeContext.getUserApartmentList(userId)
            };

            if (userDetails.personalData != null && userDetails.user != null)
            {
                return JsonConvert.SerializeObject(userDetails, Formatting.Indented);
            }
            return "Missing user with this id";
        }
    }
}
