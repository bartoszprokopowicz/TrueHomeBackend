using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ResourceServer.Models;
using ResourceServer.Resources;
using ResourceServer.JSONModels;

namespace ResourceServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ApartmentsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public ApartmentsController(
            IConfiguration configuration,
            ILogger<AuthController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        // GET: api/Apartments
        [HttpPost]
        public string Get(LimitOffset limitOffset)
        {
            var limit = limitOffset.limit;
            var offset = limitOffset.offset;

            var aps = TrueHomeContext.getApartments(limit, offset);

            foreach (var ap in aps.apartmentsList)
            {
                ap.ImgList = ap.ImgList?.Select(fileName =>
                    $"{_configuration["ResourceSrvUrl"]}/api/Pictures/{ap.ID_Ap}/{fileName}"
                ).ToArray();

                if (!string.IsNullOrEmpty(ap.ImgThumb))
                    ap.ImgThumb = $"{_configuration["ResourceSrvUrl"]}/api/Pictures/{ap.ID_Ap}/{ap.ImgThumb}";
            }
            
            return JsonConvert.SerializeObject(aps, Formatting.Indented);
        }

        // GET: api/Apartments/5
        [HttpGet("{id}", Name = "GetApartment")]
        [AllowAnonymous]
        public string Get(int id)
        {
            var ap = TrueHomeContext.getApartment(id);
            ap.ImgList = ap.ImgList.Select(fileName =>
                $"{_configuration["ResourceSrvUrl"]}/api/Pictures/{ap.ID_Ap}/{fileName}"
            ).ToArray();

            return JsonConvert.SerializeObject(ap, Formatting.Indented);
        }

        // CREATE POST: api/Apartments/add
        [HttpPost("add")]
        public async Task<JObject> Post(Apartment ap)
        {
            var userId = User.FindFirst("sub")?.Value;
            //var userId = "fddb44a3-43ae-44e2-b8a2-0962fa6be039";
            _logger.LogInformation("Adding new apartment owned by " + User.Identity.Name);

            var phoneNum = TrueHomeContext.getPhoneNumber(userId);
            if (phoneNum == null)
                TrueHomeContext.setPhoneNumber(ap.PhoneNumber, userId);

            ap.IDUser = userId;
            var id = await TrueHomeContext.createApartment(ap);
            return JObject.Parse("{\"id\": " + id + ", \"UploadStatus\": " + 1 + "}");
        }

        // UPDATE PUT: api/Apartments
        [HttpPut]
        public async Task<IActionResult> Put(Apartment ap)
        {
            var userId = User.FindFirst("sub")?.Value;
            if (ap.IDUser != userId) {
                return BadRequest();
            }
            TrueHomeContext.updateApartment(ap);
            return Ok();
        }

        // DELETE: api/Apartments
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] IDJSON id)
        {
            if (id.IntID == null) return BadRequest();
            TrueHomeContext.deleteApartment(id.IntID);
            return Ok();
        }
    }
}
