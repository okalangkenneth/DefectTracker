using Defect_Tracker.Server.Data;
using Defect_Tracker.Server.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;
using DefectTracker.Server.Data;

namespace Defect_Tracker.Server
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
            

            services.AddDbContext<ApplicationDbContext>(options=>options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddDefaultIdentity<ApplicationUser>()
            .AddRoles<IdentityRole>() // Add roles.
            .AddEntityFrameworkStores<ApplicationDbContext>();

           
            // Configure identity server to put the role claim into the id token
            // and the access token and prevent the default mapping for roles
            // in the JwtSecurityTokenHandler.

            services.AddIdentityServer()
            .AddApiAuthorization<ApplicationUser,ApplicationDbContext>(options =>
            {

             options.IdentityResources["openid"].UserClaims.Add("role");
             options.ApiResources.Single().UserClaims.Add("role");
             });

            // Need to do this as it maps "role" to ClaimTypes.Role and causes issues.
            System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler
           .DefaultInboundClaimTypeMap.Remove("role");

            services.AddAuthentication()
                    .AddIdentityServerJwt();

            services.AddControllersWithViews()
                    .AddNewtonsoftJson(
                    options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            services.AddRazorPages();

            // To access Defect Tracker tables
            services.AddDbContext<DefecttrackerContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}
