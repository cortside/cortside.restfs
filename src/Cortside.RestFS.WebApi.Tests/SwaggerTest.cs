using System.Linq;
using Xunit;

namespace Cortside.RestFS.WebApi.Tests {
    public class SwaggerTest {
        [Fact]
        public void Foo() {
            var controllers = typeof(Startup)
                .Assembly
                .GetTypes()
                .Where(t => typeof(Microsoft.AspNetCore.Mvc.Controller).IsAssignableFrom(t))
                .ToList();

            foreach (var controller in controllers) {
                // do something
                Assert.NotNull(controller);
            }
        }
    }
}
