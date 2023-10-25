using System;
using Cortside.RestApiClient.Authenticators.OpenIDConnect;

namespace Cortside.RestFS.Configuration {
    public class IdentityServerConfiguration {
        public string Authority { get; set; }
        public bool RequireHttpsMetadata { get; set; }
        public string ApiName { get; set; }
        public string ApiSecret { get; set; }
        public bool EnableCaching { get; set; }
        public TimeSpan CacheDuration { get; set; }
        public TokenRequest Authentication { get; set; }
    }
}
