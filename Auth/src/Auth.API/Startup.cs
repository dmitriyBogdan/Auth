using System.IO;
using System.Security.Cryptography.X509Certificates;
using Auth.Initialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Auth.API
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        private readonly IHostingEnvironment environment;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            this.environment = env;

            this.Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var cert = new X509Certificate2(
                 Path.Combine(this.environment.ContentRootPath, this.Configuration.GetSection("Hosting:CertName").Value),
                              this.Configuration.GetSection("Hosting:CertPassword").Value);

            services.AddMvc();

            services.AddIdentityServer()
                    .AddSigningCredential(cert)
                    .AddInMemoryClients(FakeDataConfig.GetClients())
                    .AddInMemoryApiResources(FakeDataConfig.GetApiResources());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                loggerFactory.AddConsole(this.Configuration.GetSection("Logging"));
                loggerFactory.AddDebug();

                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            app.UseIdentityServer();
        }
    }
}