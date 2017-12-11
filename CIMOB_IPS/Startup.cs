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

namespace CIMOB_IPS
{
    public class Startup
    {
        //public static string connection = @"Data Source=esw-cimob-db.database.windows.net;Database=CIMOB_IPS_DB;
        //        Integrated Security=False;User ID=adminUser; Password=f00n!l06;Connect Timeout=60;Encrypt=False;
        //        TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            //Email.SendEmail("xthe.seven@gmail.com");
            //Email.SendEmail("barb.97@hotmail.com");
            //Email.SendEmail("brunop.esac@hotmail.com");
            //Email.SendEmail("fernandes_.10@hotmail.com");
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
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

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
                    defaults: new { controller = "Account", action = "ExecFYP" });

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

            });
        }
    }
}
