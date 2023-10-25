using System.Threading.Tasks;
using Cortside.RestFS.Data.Repositories;
using Cortside.RestFS.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Cortside.RestFS.WebApi.Controllers {
    [Route("api/v{version:apiVersion}/contents")]
    [ApiController]
    [ApiVersion("1")]
    public class ContentsController : ControllerBase {
        private readonly ILogger<ContentsController> logger;
        private readonly IFileSystemRepository repository;

        public ContentsController(ILogger<ContentsController> logger, IFileSystemRepository repository) {
            this.logger = logger;
            this.repository = repository;
        }

        [HttpGet("{**path}")]
        public async Task<IActionResult> GetContentsAsync(string root, string path) {
            var info = repository.Info(path, new FileSystemSearch());
            if (info == null || info.Type == FileSystemEntryType.Directory) {
                return NotFound();
            }

            var stream = repository.OpenRead(path);
            return File(stream, "application/octet-stream", info.Name);
        }
    }
}
