using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Cortside.Common.BootStrap;
using Cortside.RestFS.Data.Repositories;
using Cortside.RestFS.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cortside.RestFS.BootStrap.Installer {
    public class RepositoryInstaller : IInstaller {
        public void Install(IServiceCollection services, IConfiguration configuration) {
            var config = configuration.GetSection("Mounts").Get<List<Mount>>();
            foreach (var mount in config) {
                var di = new DirectoryInfo(mount.Path);
                mount.Path = di.FullName.Replace("\\", "/");
            }
            services.AddSingleton(config);

            // register repositories
            typeof(FileSystemRepository).GetTypeInfo().Assembly.GetTypes()
                .Where(x => (x.Name.EndsWith("Repository"))
                    && x.GetTypeInfo().IsClass
                    && !x.GetTypeInfo().IsAbstract
                    && x.GetInterfaces().Length > 0)
                .ToList().ForEach(x => {
                    x.GetInterfaces().ToList()
                        .ForEach(i => services.AddScoped(i, x));
                });
        }
    }
}
