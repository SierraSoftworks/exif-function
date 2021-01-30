using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace EXIF
{
    public static class ImageExifData
    {
        [FunctionName("ImageExifData")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/exifdata")] HttpRequest req,
            ILogger log)
        {
            var metadata = ExifHelper.GetMetadata(req.Body);

            if (metadata == null) return new NotFoundResult();

            return new OkObjectResult(metadata);
        }
    }
}
