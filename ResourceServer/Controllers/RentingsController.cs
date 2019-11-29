using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ResourceServer.Models;

namespace ResourceServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RentingsController : ControllerBase
    {
        private readonly ILogger<RatingsController> _logger;

        public RentingsController(ILogger<RatingsController> logger)
        {
            _logger = logger;
        }

        // GET: api/Renting
        [HttpGet]
        public IActionResult GetAll()
        {
            var rentingList = TrueHomeContext.getAllRentings();
            if (rentingList == null)
            {
                return NotFound();
            }
            return Ok(JsonConvert.SerializeObject(rentingList, Formatting.Indented));
        }

        // GET: api/Rentings/:id
        [HttpGet("getByUser")]
        public IActionResult GetByUser()
        {
            var userId = User.FindFirst("sub")?.Value;

            var renting = TrueHomeContext.getRentingByUser(userId);
            if (renting == null)
            {
                return NotFound();
            }
            return Ok(JsonConvert.SerializeObject(renting, Formatting.Indented));
        }

        //Post api/Rentings
        [HttpPost]
        public async Task<IActionResult> Add(Renting ret)
        {
            var userId = User.FindFirst("sub")?.Value;
            _logger.LogInformation("Adding new renting from " + User.Identity.Name);

            ret.IDUser = userId;
            if (ret.date_to < ret.date_from || ret.date_to <= System.DateTime.Today)
            {
                return BadRequest("Given date is smaller than today");
            }
            var id = await TrueHomeContext.createRenting(ret);
            if (id == -1)
            {
                return BadRequest("Something went wrong!");
            }
            return Ok(JObject.Parse("{\"id\": " + id + ", \"UploadStatus\": " + 1 + "}"));
        }

        // UPDATE PUT: api/Rating/5
        [HttpPut("{id}")]
        public IActionResult Edit(int id, Rating rat)
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
