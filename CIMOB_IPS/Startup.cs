using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CIMOB_IPS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace CIMOB_IPS
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
            services.AddAuthentication(options =>
            {
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie();

            CIMOB_IPS_DBContext.ConnectionString = Configuration.GetConnectionString("CIMOB_IPS_DB");

            services.AddMvc().AddSessionStateTempDataProvider();

            services.AddDbContext<CIMOB_IPS_DBContext>(options => options.UseSqlServer(CIMOB_IPS_DBContext.ConnectionString));

            services.AddSingleton<IFileProvider>(
            new PhysicalFileProvider(
                Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));

            services.AddSession();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseAuthentication();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/ErrorView");
            }

            app.UseStaticFiles(new StaticFileOptions
            {
                ServeUnknownFileTypes = true, //allow unkown file types also to be served
                DefaultContentType = "application/vnd.microsoft.portable-executable" //content type to returned if fileType is not known.
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    "LoginRoute", "Login",
                    defaults: new { controller = "Account"  , action = "Login"});

                routes.MapRoute(
                    "ExecLoginRoute", "Login",
                    defaults: new { controller = "Account", action = "ExecLoginAsync" });
                routes.MapRoute(
                    "ExecFypRoute", "Login",
                    defaults: new { controller = "Account", action = "ForgotYourPassword" });

                routes.MapRoute(
                    "RegisterRoute", "Register",
                    defaults: new { controller = "Account", action = "Register" });

                routes.MapRoute(
                    "RegisterStudentRoute", "RegisterStudent",
                    defaults: new { controller = "Account", action = "RegisterStudent" });

                routes.MapRoute(
                    "RegisterTechnicianRoute", "RegisterTechnician",
                    defaults: new { controller = "Account", action = "RegisterTechnician" });

                routes.MapRoute(
                    "PreRegisterRoute", "Register",
                    defaults: new { controller = "Account", action = "PreRegister" });

                routes.MapRoute(
                    "MyApplicationsRoute", "MyApplications",
                    defaults: new { controller = "Application", action = "MyApplications" });

                routes.MapRoute(
                    "ContactsRoute", "Contact",
                    defaults: new { controller = "Home", action = "Contact" });

                routes.MapRoute(
                    "Profile",
                    "Profile/{id}",
                    new { controller = "Profile", action = "Get" }
               
                );

            });
        }
    }
}
