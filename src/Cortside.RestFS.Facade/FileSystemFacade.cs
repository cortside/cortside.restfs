using Microsoft.Extensions.Logging;

namespace Cortside.RestFS.Facade {
    public class FileSystemFacade {
        private readonly ILogger<FileSystemFacade> logger;

        public FileSystemFacade(ILogger<FileSystemFacade> logger) {
            this.logger = logger;
        }
    }
}
