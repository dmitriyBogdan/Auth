using Auth.API.Initialization;
using Auth.BusinessLogicLayer;
using Auth.BusinessLogicLayer.Interfaces;
using Auth.DataAccessLayer;
using FluentValidation.AspNetCore;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Auth.API
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            this.Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc()
                    .AddFluentValidation(x => x.RegisterValidatorsFromAssemblyContaining<Startup>());

            services.AddIdentityServer()
                    .AddTemporarySigningCredential()
                    .AddInMemoryClients(FakeDataConfig.GetClients())
                    .AddInMemoryApiResources(FakeDataConfig.GetApiResources());

            string connectionString = this.Configuration.GetConnectionString("AuthDatabase");
            services.AddDbContext<AuthContext>(options => options.UseNpgsql(connectionString));

            services.AddTransient<ICrypto, Crypto>();
            services.AddTransient<IUserManager, UserManager>();
            services.AddTransient<IResourceOwnerPasswordValidator, ResourceOwnerPasswordValidator>();
            services.AddTransient<IProfileService, ProfileService>();
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

            if (this.Configuration.IsMigrateDatabaseOnStartup())
            {
                app.ApplyMigrations();
            }

            app.InitRoles();

            app.UseMvc();

            app.UseIdentityServer();
        }
    }
}