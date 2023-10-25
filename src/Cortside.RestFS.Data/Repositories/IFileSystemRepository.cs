using System.IO;
using Cortside.RestFS.Domain.Entities;

namespace Cortside.RestFS.Data.Repositories {
    public interface IFileSystemRepository {
        FileSystemEntry Info(string file, FileSystemSearch search);

        Stream OpenRead(string path);
    }
}
