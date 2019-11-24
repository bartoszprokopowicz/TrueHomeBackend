using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using ResourceServer.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace ResourceServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PicturesController : ControllerBase
    {
        private readonly IContentTypeProvider _contentTypeProvider;
        private readonly ILogger<PicturesController> _logger;

        public PicturesController(
            IContentTypeProvider contentTypeProvider,
            ILogger<PicturesController> logger)
        {
            _contentTypeProvider = contentTypeProvider;
            _logger = logger;
        }

        [HttpGet("{idAp}/{filename}")]
        [AllowAnonymous]
        public async Task<IActionResult> Get(int idAp, string filename)
        {
            var path = $"/data/pictures/{idAp}/{filename}";
            try
            {
                _logger.LogDebug("Fetching picture from " + path);
                var pic = await System.IO.File.ReadAllBytesAsync(path);
                return File(pic, MapContentType(path));
            }
            catch (FileNotFoundException)
            {
                _logger.LogError("Picture not found!");
                return NotFound();
            }
        }

        [HttpPost("{idAp}")]
        public async Task<IActionResult> UploadImg(int idAp)
        {
            IFormFile image;
            try
            {
                if (HttpContext.Request.Form.Files[0] != null)
                {
                    image = HttpContext.Request.Form.Files[0];
                }
                else
                    throw new ArgumentNullException( "image","Could not bind uploaded image or no image sent");
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return BadRequest(e.ToString());
            }

            if (image.Length > 0 && image.ContentType.Contains("image"))
            {
                var dirPath = $"/data/pictures/{idAp}";
                var filePath = $"{dirPath}/{image.FileName}";
                string thumbName = null;

                if (System.IO.File.Exists(filePath))
                {
                    _logger.LogWarning($"File {filePath} already exists, aborting.");
                    return Ok();
                }

                try
                {
                    var ap = TrueHomeContext.getApartment(idAp);

                    if (string.IsNullOrEmpty(ap.ImgThumb))
                    {
                        _logger.LogInformation($"Received first picture for apartment {idAp}, creating thumbnail copy");
                        Directory.CreateDirectory(dirPath);

                        thumbName = Path.GetFileNameWithoutExtension(image.FileName) +
                                    "_thumb" +
                                    Path.GetExtension(image.FileName);

                        using (var inputStream = image.OpenReadStream())
                        using (var imgThumb = Image.Load(inputStream))
                        {
                            imgThumb.Mutate(x => x
                                .Resize(0, 300));

                            imgThumb.Save($"{dirPath}/{thumbName}");
                        }

                        ap.ImgThumb = thumbName;
                    }

                    using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                    {
                        await image.CopyToAsync(fileStream);
                    }

                    ap.ImgList = ap.ImgList
                                     ?.Concat(new[] {image.FileName}).ToArray()
                                 ?? new[] {image.FileName};

                    TrueHomeContext.updateApartment(ap);

                    _logger.LogInformation("Uploaded new picture to apartment " + idAp);
                    return Ok();
                }
                catch (Npgsql.PostgresException e)
                {
                    _logger.LogError(e, "Database error while saving picture. Deleting any saved pictures");

                    if (thumbName != null && System.IO.File.Exists($"{dirPath}/{thumbName}"))
                    {
                        System.IO.File.Delete($"{dirPath}/{thumbName}");
                        _logger.LogInformation($"Deleted thumbnail file {dirPath}/{thumbName}");
                    }

                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                        _logger.LogInformation($"Deleted file {filePath}");
                    }
                    return BadRequest(e);

                }
                catch (IOException e)
                {
                    _logger.LogError(e,"IO exception while saving file! Database not updated");
                    return BadRequest(e);
                }
                catch (Exception e)
                {
                    _logger.LogError("Error while saving file: " + e);
                    return BadRequest(e);
                }
            }

            _logger.LogError("Wrong file format or no file given.");
            return BadRequest();
        }

        [HttpDelete("{idAp}/{filename}")]
        public IActionResult DeleteImg(int idAp, string filename)
        {
            var path = $"/data/pictures/{idAp}/{filename}";
            try
            {
                _logger.LogDebug("Deleting picture " + path);

                System.IO.File.SetAttributes(path, FileAttributes.Normal);
                System.IO.File.Delete(path);

                TrueHomeContext.DeletePictureRef(idAp, filename);

                return Ok();
            }
            catch (FileNotFoundException)
            {
                _logger.LogError("Picture not found!");
                return NotFound();
            }
        }

        private string MapContentType(string path)
        {
            if (!_contentTypeProvider.TryGetContentType(path, out var contentType))
                contentType = "application/octet-stream";
            return contentType;
        }
    }
}