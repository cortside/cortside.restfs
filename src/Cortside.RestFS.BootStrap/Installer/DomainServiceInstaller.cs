using System.Linq;
using System.Reflection;
using Cortside.Common.BootStrap;
using Cortside.RestFS.Facade;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cortside.RestFS.BootStrap.Installer {
    public class DomainServiceInstaller : IInstaller {
        public void Install(IServiceCollection services, IConfiguration configuration) {
            // register domain services
            typeof(FileSystemService).GetTypeInfo().Assembly.GetTypes()
                .Where(x => (x.Name.EndsWith("Service"))
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
