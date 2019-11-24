using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    public class RatingsController : ControllerBase
    {
        private readonly ILogger<RatingsController> _logger;

        public RatingsController(ILogger<RatingsController> logger)
        {
            _logger = logger;
        }

        // GET: api/Ratings
        [HttpGet]
        [AllowAnonymous]
        public string Get(int idAp, int limit, int offset)
        {
            var rat = TrueHomeContext.getRatings(idAp, limit, offset);

            return JsonConvert.SerializeObject(rat, Formatting.Indented);
        }

        [HttpPost]
        public async Task<JObject> Post(Rating rat, int idRenting)
        {
            var userId = User.FindFirst("sub")?.Value;
            _logger.LogInformation("Adding new rating from " + User.Identity.Name);

            rat.IDUser = userId;
            rat.IDRenting = idRenting;
            var id = await TrueHomeContext.createRating(rat);
            return JObject.Parse("{\"id\": " + id + ", \"UploadStatus\": " + 1 + "}");
        }

        // UPDATE PUT: api/Rating/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, Rating rat)
        {
            if (id == rat.ID_Rating)
            {
                TrueHomeContext.updateRating(rat);
            }
            return Ok();
        }

        // DELETE: api/Rating/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            TrueHomeContext.deleteRating(id);
            return Ok();
        }
    }
}