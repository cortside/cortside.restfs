using System;
using System.Collections.Generic;
using System.IO;
using Cortside.RestFS.Domain.Entities;

namespace Cortside.RestFS.Data.Repositories {
    public class FileSystemRepository : IFileSystemRepository {
        private readonly List<Mount> mounts;

        public FileSystemRepository(List<Mount> mounts) {
            this.mounts = mounts;
        }

        private string SecurePathCombine(params string[] values) {
            var path = Path.Combine(values);
            var canonicalPath = Path.GetFullPath(path).Replace("\\", "/");

            // Prevent directory traversal by checking if the requested 
            // canonical file starts with the root directory.
            if (!mounts.Exists(m => canonicalPath.StartsWith(m.Path))) {
                throw new UnauthorizedAccessException();
            }

            return path.Replace("\\", "/");
        }

        public Stream OpenRead(string path) {
            var mount = GetMountFromPath(path);
            var fullPath = GetFullPath(mount, path);
            return File.OpenRead(fullPath);
        }

        public FileSystemEntry Info(string file, FileSystemSearch search) {
            var mount = GetMountFromPath(file);
            var fullPath = GetFullPath(mount, file);

            var fi = new FileInfo(fullPath);
            if (fi.Exists) {
                return new FileSystemEntry(mount, fi);
            }

            var di = new DirectoryInfo(fullPath);
            return di.Exists ? new FileSystemEntry(mount, di, search, search.RecurseLevels) : null;
        }

        private string GetFullPath(Mount mount, string path) {
            if (path == mount.Name) {
                path = "";
            }
            path = path.Replace(mount.Name + "/", "");
            var file = SecurePathCombine(mount.Path, path);
            return file;
        }

        private Mount GetMountFromPath(string file) {
            foreach (var mount in mounts) {
                if (file.StartsWith(mount.Name + "/") || file == mount.Name) {
                    return mount;
                }
            }

            throw new UnauthorizedAccessException();
        }
    }
}
