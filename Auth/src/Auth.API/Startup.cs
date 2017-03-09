using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Auth.API.Initialization;
using Auth.BLL;
using Auth.BLL.DomainManagmant;
using Auth.BLL.Interfaces;
using Auth.BLL.UserManagement;
using Auth.DAL;
using Auth.DAL.ProxyContext;
using FluentValidation.AspNetCore;
using IdentityServer4;
using IdentityServer4.Endpoints;
using IdentityServer4.Hosting;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection.Internal;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Auth.API
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        private readonly bool isProxy;
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
            this.isProxy = Convert.ToBoolean(this.Configuration["Settings:IsProxy"]);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var cert = new X509Certificate2(
                Path.Combine(this.environment.ContentRootPath, this.Configuration.GetSection("Hosting:CertName").Value),
                this.Configuration.GetSection("Hosting:CertPassword").Value);

            services.AddMvc()
                .AddFluentValidation(x => x.RegisterValidatorsFromAssemblyContaining<Startup>());

            services.AddIdentityServer()
                .AddSigningCredential(cert)
                .AddInMemoryClients(FakeDataConfig.GetClients())
                .AddInMemoryApiResources(FakeDataConfig.GetApiResources())
                .AddInMemoryIdentityResources(FakeDataConfig.GetIdentityResources());
            if (this.isProxy)
            {
                string connectionString = this.Configuration.GetConnectionString("AuthDatabase");
                services.AddDbContext<ProxyContext>(options => options.UseNpgsql(connectionString));
                services.AddTransient<DbContext, ProxyContext>();
                services.AddTransient<IDomainManager, DomainManager>();
                services.AddTransient<IResourceOwnerPasswordValidator, DomainValidator>();
            }
            else
            {
                string connectionString = this.Configuration.GetConnectionString("AuthDatabase");
                services.AddDbContext<AuthContext>(options => options.UseNpgsql(connectionString));
                services.AddTransient<DbContext, AuthContext>();
                services.AddTransient<IUserManager, UserManager>();
                services.AddTransient<IResourceOwnerPasswordValidator, ResourceOwnerPasswordValidator>();
                services.AddTransient<IProfileService, ProfileService>();
                services.AddTransient<ICrypto, Crypto>();
            }
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

            app.UseExceptionHandler("/Error");
            if (this.Configuration.IsMigrateDatabaseOnStartup())
            {
                app.ApplyMigrations();
            }

            app.UseIdentityServer();
            app.UseCookies(this.Configuration);
            if (!this.isProxy)
            {
                app.InitRoles();
                app.UseProxy(this.Configuration);
                app.UseFacebook(this.Configuration);
                app.UseLinkedin(this.Configuration);
            }

            app.UseMvc();
        }
    }
}