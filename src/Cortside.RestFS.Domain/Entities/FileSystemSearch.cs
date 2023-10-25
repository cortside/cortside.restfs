using System;

namespace Cortside.RestFS.Domain.Entities {
    public class FileSystemSearch {
        public string SearchPattern { get; set; } = "*";
        public DateTime? CreationTime { get; set; }
        public DateTime? LastAccessTime { get; set; }
        public DateTime? LastWriteTime { get; set; }
        public int RecurseLevels { get; set; } = 1;

        /**
         *  /O  List by files in sorted order.
         *    sortorder    N  By name (alphabetic)       S  By size (smallest first)
         *                 E  By extension (alphabetic)  D  By date/time (oldest first)
         *                 G  Group directories first    -  Prefix to reverse order
         */
        public string OrderBy { get; set; } = "type,name";
    }
}
