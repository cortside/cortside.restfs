using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Enumeration;
using System.Linq;

namespace Cortside.RestFS.Domain.Entities {
    public class FileSystemEntry {
        public FileSystemEntry(Mount mount, FileInfo info) {
            Type = FileSystemEntryType.File;
            PopulateFromFileSystemInfo(mount, info);

            Size = info.Length;
            Extension = info.Extension;

            Directory = GetMountedPath(mount, info.DirectoryName);
        }

        public FileSystemEntry(Mount mount, DirectoryInfo info, FileSystemSearch search, int recurseLevels) {
            Type = FileSystemEntryType.Directory;
            PopulateFromFileSystemInfo(mount, info);

            if (Path == "/" + mount.Name) {
                Directory = "/";
                Name = mount.Name;
            } else {
                Directory = GetMountedPath(mount, info.Parent.FullName);
            }

            if (recurseLevels <= 0) {
                return;
            }

            recurseLevels -= 1;
            Entries = new List<FileSystemEntry>();
            Entries.AddRange(info.EnumerateFileSystemInfos()
                .Where(e => Filter(e, search))
                .Select(i => GetFileSystemEntry(mount, search, recurseLevels, i)).Where(s => s != null));
        }

        private static FileSystemEntry GetFileSystemEntry(Mount mount, FileSystemSearch search, int recurseLevels, FileSystemInfo info) {
            if (info is FileInfo i) {
                return new FileSystemEntry(mount, i);
            }

            var di = (DirectoryInfo)info;
            var e = new FileSystemEntry(mount, di, search, recurseLevels);
            if ((e.Entries == null || e.Entries.Count == 0) && !Filter2(di, search)) {
                return null;
            }

            return e;
        }

        private bool Filter(FileSystemInfo fileSystemInfo, FileSystemSearch search) {
            if (fileSystemInfo is DirectoryInfo) {
                return true;
            }

            return Filter2(fileSystemInfo, search);
        }

        private static bool Filter2(FileSystemInfo info, FileSystemSearch search) {
            if (!FileSystemName.MatchesSimpleExpression(search.SearchPattern.AsSpan(), info.Name.AsSpan(), true)) {
                return false;
            }

            if (search.CreationTime.HasValue && info.CreationTimeUtc < search.CreationTime.Value) {
                return false;
            }

            if (search.LastAccessTime.HasValue && info.LastAccessTimeUtc < search.LastAccessTime.Value) {
                return false;
            }

            if (search.LastWriteTime.HasValue && info.LastWriteTimeUtc < search.LastWriteTime.Value) {
                return false;
            }

            return true;
        }

        private void PopulateFromFileSystemInfo(Mount mount, FileSystemInfo info) {
            Entries = null;

            Name = info.Name;
            Path = info.FullName;
            CreationTime = info.CreationTimeUtc;
            LastAccessTime = info.LastAccessTimeUtc;
            LastWriteTime = info.LastWriteTimeUtc;

            Path = GetMountedPath(mount, Path);
        }

        private string GetMountedPath(Mount mount, string path) {
            var p = path.Replace("\\", "/");
            if (p == mount.Path) {
                return "/" + mount.Name;
            }

            var s = p.Replace(mount.Path + "/", "/" + mount.Name + "/");
            return s;
        }

        public string Path { get; set; }
        public FileSystemEntryType Type { get; set; }
        public string Directory { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public long? Size { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime LastAccessTime { get; set; }
        public DateTime LastWriteTime { get; set; }
        public List<FileSystemEntry> Entries { get; set; }
    }
}
