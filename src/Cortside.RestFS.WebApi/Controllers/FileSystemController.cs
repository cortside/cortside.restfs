using System.Threading.Tasks;
using Cortside.RestFS.Data.Repositories;
using Cortside.RestFS.Domain.Entities;
using Cortside.RestFS.WebApi.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Cortside.RestFS.WebApi.Controllers {
    [Route("api/v{version:apiVersion}/files")]
    [ApiController]
    [ApiVersion("1")]
    [Produces("application/json")]
    public class FileSystemController : ControllerBase {
        private readonly ILogger<FileSystemController> logger;
        private readonly IFileSystemRepository repository;

        public FileSystemController(ILogger<FileSystemController> logger, IFileSystemRepository repository) {
            this.logger = logger;
            this.repository = repository;
        }

        [HttpGet("{**path}")]
        [ProducesResponseType(typeof(SettingsModel), 200)]
        [ResponseCache(CacheProfileName = "Default")]
        public async Task<IActionResult> GetAsync(string root, string path, [FromQuery] FileSystemSearch search) {
            path ??= "/";
            var info = repository.Info(path, search);
            if (info == null) {
                return NotFound();
            }

            return Ok(info);
        }
    }
}

