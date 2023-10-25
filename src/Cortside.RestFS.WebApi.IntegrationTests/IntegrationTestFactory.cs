using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Cortside.AspNetCore.Builder;
using Cortside.MockServer;
using Cortside.MockServer.AccessControl;
using Cortside.MockServer.AccessControl.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;

namespace Cortside.RestFS.WebApi.IntegrationTests {
    public class IntegrationTestFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class, IWebApiStartup {
        // dbname is created outside of the options so that it's constant and not reevaluated at instance creation time
        private readonly string dbName = Guid.NewGuid().ToString();
        private Subjects subjects;
        public MockHttpServer MockServer { get; private set; }
        private IConfiguration Configuration { get; set; }
        public JsonSerializerSettings SerializerSettings { get; private set; }

        protected override IHostBuilder CreateHostBuilder() {
            SetupConfiguration();
            SetupLogger();

            MockServer = new MockHttpServer(dbName)
                .ConfigureBuilder<CommonMock>()
                .ConfigureBuilder(new IdentityServerMock("./Data/discovery.json", "./Data/jwks.json"))
                .ConfigureBuilder(new SubjectMock("./Data/subjects.json"));

            Configuration["HealthCheckHostedService:Checks:0:Value"] = $"{MockServer.Url}/api/health";
            Configuration["HealthCheckHostedService:Checks:1:Value"] = $"{MockServer.Url}/api/health";

            Configuration["IdentityServer:Authority"] = MockServer.Url;
            Configuration["IdentityServer:BaseUrl"] = $"{MockServer.Url}/connect/token";
            Configuration["IdentityServer:RequireHttpsMetadata"] = "false";

            Configuration["PolicyServer:TokenClient:Authority"] = MockServer.Url;
            Configuration["PolicyServer:PolicyServerUrl"] = MockServer.Url;

            Configuration["DistributedLock:UseRedisLockProvider"] = "false";

            Configuration["LoanServicingApi:BaseUrl"] = MockServer.Url;
            Configuration["LoanServicingApi:Authentication:Url"] = $"{MockServer.Url}/connect/token";

            Configuration["CatalogApi:ServiceUrl"] = $"{MockServer.Url}";
            Configuration["CatalogApi:Authentication:Url"] = $"{MockServer.Url}/connect/token";

            MockServer.WaitForStart();

            return Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration(builder => builder.AddConfiguration(Configuration))
                .ConfigureWebHostDefaults(webbuilder => {
                    webbuilder
                    .UseConfiguration(Configuration)
                    .UseStartup<TStartup>()
                    .ConfigureTestServices(sc => {
                        ResolveSerializerSettings(sc);
                    });
                })
                .UseSerilog(Log.Logger);
        }

        private void ResolveSerializerSettings(IServiceCollection services) {
            // Build the service provider.
            var sp = services.BuildServiceProvider();

            // Create a scope to obtain a reference to the database context (DbContext).
            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var o = scopedServices.GetRequiredService<IOptions<MvcNewtonsoftJsonOptions>>();
            SerializerSettings = o.Value.SerializerSettings;
        }

        private void SetupLogger() {
            var loggerConfiguration = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .Enrich.FromLogContext();

            var serverUrl = Configuration["Seq:ServerUrl"];
            if (!string.IsNullOrWhiteSpace(serverUrl)) {
                loggerConfiguration.WriteTo.Seq(serverUrl);
            }

            var logFile = Configuration["LogFile:Path"];
            if (!string.IsNullOrWhiteSpace(logFile)) {
                loggerConfiguration.WriteTo.File(logFile);
            }
            Log.Logger = loggerConfiguration.CreateLogger();
        }

        private void SetupConfiguration() {
            Configuration = new ConfigurationBuilder()
                 .AddJsonFile("appsettings.integration.json", optional: false, reloadOnChange: true)
                 .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
                 .Build();
        }

        public HttpClient UnauthorizedClient {
            get {
                _UnauthorizedClient ??= CreateDefaultClient();
                return _UnauthorizedClient;
            }
        }

        private HttpClient _UnauthorizedClient { get; set; }

        public HttpClient CreateAuthorizedClient(string clientId) {
            var client = CreateDefaultClient();
            subjects ??= JsonConvert.DeserializeObject<Subjects>(File.ReadAllText("./Data/subjects.json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", subjects.SubjectsList.First(s => s.ClientId == clientId).ReferenceToken);
            return client;
        }

        public HttpClient CreateCustomAuthorizedClient(Uri uri, string clientId) {
            var client = CreateDefaultClient(uri);
            subjects ??= JsonConvert.DeserializeObject<Subjects>(File.ReadAllText("./Data/subjects.json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", subjects.SubjectsList.First(s => s.ClientId == clientId).ReferenceToken);
            return client;
        }

        public Subjects GetAllSubjects() {
            return subjects;
        }

        public Subject GetSubjectByName(string name) {
            return subjects.SubjectsList.First(x => x.ClientId == name);
        }
    }
}
