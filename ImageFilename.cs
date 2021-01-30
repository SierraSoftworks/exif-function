using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Scriban;

namespace EXIF
{
    public static class ImageFilename
    {
        [FunctionName("ImageFilename")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/name")] HttpRequest req,
            ILogger log)
        {
            var filename = (string)req.Query["filename"];
            var nameTemplate = (string)req.Query["template"] ?? "{{ FileName }}{{ Extension }}";

            var tmpl = Template.Parse(nameTemplate);
            var metadata = ExifHelper.GetMetadata(req.Body);

            metadata["FileName"] = Path.GetFileNameWithoutExtension(filename);
            metadata["Extension"] = Path.GetExtension(filename);
            metadata["FileNameClean"] = FilenameHelper.Fix(Path.GetFileNameWithoutExtension(filename));


            var rendered = tmpl.Render(metadata);

            return new OkObjectResult(new
            {
                Filename = string.Concat(rendered.Split(Path.GetInvalidFileNameChars()))
            });
        }
    }
}
