using Microsoft.Extensions.Logging;

namespace Cortside.RestFS.Facade {
    public class FileSystemService {
        private readonly ILogger<FileSystemService> logger;

        public FileSystemService(ILogger<FileSystemService> logger) {
            this.logger = logger;
        }
    }
}
