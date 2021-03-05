using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureAD.Client
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
            services.AddControllersWithViews(op => op.Filters.Add(new AuthorizeFilter()));
            services.AddHttpClient();

            //services.AddAuthentication(op =>
            //{
            //    op.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //    op.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            //    op.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //})
            //.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
            //.AddOpenIdConnect(op =>
            //{
            //    op.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //    op.Authority = "https://login.microsoftonline.com/tenant/v2.0";
            //    op.ClientId = "";
            //    op.ClientSecret = "";
            //    //op.Scope.Add("apione"); // appid for API1
            //    //op.Scope.Add("api2");//for api2
            //    op.ResponseType = "code"; //for auth flow with PKCE
            //                              // op.UsePkce = true;
            //    op.SaveTokens = true;
            //});



            services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApp(Configuration)
              .EnableTokenAcquisitionToCallDownstreamApi(new string[] { "user.read" })
              .AddInMemoryTokenCaches();


            //1. register a webapp and set the redirect and logout urls
            //2. Create a secret for the webapp and paste use it inside the oidc configuration shown above

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
