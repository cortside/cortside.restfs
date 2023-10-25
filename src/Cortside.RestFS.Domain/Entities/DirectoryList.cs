using System.Collections.Generic;

namespace Cortside.RestFS.Domain.Entities {
    public class DirectoryList {
        public List<FileAttributes> Entries { get; } = new List<FileAttributes>();
    }
}
