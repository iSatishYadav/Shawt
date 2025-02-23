using System.Collections.Generic;
using System.Linq;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using NLog;
using Shawt.Data;
using Shawt.Providers;
using Shawt.Providers.RateLimiting;

namespace Shawt
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var rateLimitingConnection = Configuration.GetConnectionString(nameof(RateLimitingContext));
            services.AddDbContext<RateLimitingContext>(options => options.UseNpgsql(rateLimitingConnection));

            //load general configuration from appsettings.json
            services.Configure<ClientRateLimitOptions>(Configuration.GetSection("ClientRateLimiting"));
            // inject counter and rules stores           
            services.AddSingleton<IClientPolicyStore, SqlClientPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, SqlRateLimitCounterStore>();
            // configuration (resolvers, counter key builders)
            services.AddSingleton<IRateLimitConfiguration, IdentityRateLimitConfiguration>();
            services.AddSingleton<ClientRateLimitOptions, ClientRateLimitOptions>();
            services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();

            var connection = Configuration.GetConnectionString(nameof(LinksContext));
            services.AddDbContext<LinksContext>(options => options.UseNpgsql(connection));
            services.AddHttpClient();
            services.AddControllersWithViews();
            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

            services.AddAuthentication(x =>
            {
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.Authority = Configuration["Authorization:Authority"];
                    options.Audience = Configuration["Authorization:ClientId"];
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = Configuration["Authorization:NameClaimType"],
                        ValidateIssuer = true,
                        ValidIssuer = Configuration["Authorization:Issuer"],

                        ValidateAudience = true,
                        ValidAudience = Configuration["Authorization:ClientId"],

                        ValidateLifetime = true,
                    };
                });
            services.AddTransient<ILinksProvider, LinksProvider>();
            services.AddTransient<IShortUrlProvider, ShortUrlProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            GlobalDiagnosticsContext.Set("connectionString", Configuration.GetConnectionString(nameof(LinksContext)));
            app.Use(async (context, next) =>
            {
                context.Response.Headers.TryAdd("X-XSS-Protection", "1; mode=block");
                context.Response.Headers.TryAdd("X-Frame-Options", "SAMEORIGIN");
                context.Response.Headers.TryAdd("X-Content-Type-Options", "nosniff");

                await next().ConfigureAwait(true);
            });
            var debugEnvironments = new[] { "Local", "PublicLocal" };
            if (env.IsDevelopment() || debugEnvironments.Contains(env.EnvironmentName))
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }
            app.UseStaticFiles();
            if (!debugEnvironments.Contains(env.EnvironmentName))
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.Map("/api", api =>
            {
                api.UseRouting();
                api.UseClientRateLimiting();
                api.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllerRoute(
                        name: "default",
                        pattern: "{controller}/{action=Index}/{id?}");
                });
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (debugEnvironments.Contains(env.EnvironmentName))
                {
                    spa.UseProxyToSpaDevelopmentServer("http://localhost:4200/");
                    //spa.UseAngularCliServer(npmScript: "start");
                }
            });

        }
    }
}
