using System.Linq;
using System.Reflection;
using Cortside.Common.BootStrap;
using Cortside.RestFS.Facade;
using Cortside.RestFS.Facade.Mappers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cortside.RestFS.BootStrap.Installer {
    public class FacadeInstaller : IInstaller {
        public void Install(IServiceCollection services, IConfiguration configuration) {
            typeof(FileSystemFacade).GetTypeInfo().Assembly.GetTypes()
                .Where(x => (x.Name.EndsWith("Facade"))
                    && x.GetTypeInfo().IsClass
                    && !x.GetTypeInfo().IsAbstract)
                .ToList()
                .ForEach(x => {
                    x.GetInterfaces().ToList()
                        .ForEach(i => services.AddScoped(i, x));
                });

            typeof(SubjectMapper).GetTypeInfo().Assembly.GetTypes()
                .Where(x => (x.Name.EndsWith("Mapper"))
                    && x.GetTypeInfo().IsClass
                    && !x.GetTypeInfo().IsAbstract)
                .ToList()
                .ForEach(x => services.AddSingleton(x));
        }
    }
}
