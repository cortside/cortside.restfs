using System.Linq;
using System.Reflection;
using Cortside.Common.BootStrap;
using Cortside.RestFS.WebApi.Mappers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cortside.RestFS.WebApi.Installers {
    public class ModelMapperInstaller : IInstaller {
        public void Install(IServiceCollection services, IConfiguration configuration) {
            typeof(AddressModelMapper).GetTypeInfo().Assembly.GetTypes()
                .Where(x => (x.Name.EndsWith("Mapper"))
                    && x.GetTypeInfo().IsClass
                    && !x.GetTypeInfo().IsAbstract)
                .ToList().ForEach(x => {
                    services.AddScoped(x);
                });
        }
    }
}
