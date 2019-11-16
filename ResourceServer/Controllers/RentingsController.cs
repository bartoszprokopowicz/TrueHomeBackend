using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ResourceServer.Models;

namespace ResourceServer.Controllers
{
    public class RentingsController
    {
        [Route("api/[controller]")]
        [ApiController]
        //[Authorize]
        public class RatingsController : ControllerBase
        {
            private readonly ILogger<RatingsController> _logger;

            public RatingsController(ILogger<RatingsController> logger)
            {
                _logger = logger;
            }

            // GET: api/Rentings
            [HttpGet]
            [AllowAnonymous]
            public string Get()
            {
                var rentingList = TrueHomeContext.getAllRentings();

                return JsonConvert.SerializeObject(rentingList, Formatting.Indented);
            }

            //Post api/Renting
            [HttpPost]
            public async Task<JObject> Post(Renting ret)
            {
                var userId = User.FindFirst("sub")?.Value;
                _logger.LogInformation("Adding new rating from " + User.Identity.Name);

                ret.IDUser = userId;
                var id = await TrueHomeContext.createRenting(ret);
                return JObject.Parse("{\"id\": " + id + ", \"UploadStatus\": " + 1 + "}");
            }

            // UPDATE PUT: api/Rating/5
            [HttpPut("{id}")]
            public IActionResult Put(int id, Rating rat)
            {
                return Ok();
            }

            // DELETE: api/Renting/5
            [HttpDelete("{id}")]
            public IActionResult Delete(int id)
            {
                TrueHomeContext.deleteRenting(id);
                return Ok();
            }
        }
    }
}