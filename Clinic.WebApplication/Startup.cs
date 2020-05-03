using Clinic.DataContext;
using Clinic.Services.CaptchaService;
using Clinic.Services.FeedBackService;
using Clinic.Services.InitService;
using Clinic.Services.LoginService;
using Clinic.Services.MailService;
using Clinic.WebApplication.Hubs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Clinic.WebApplication
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddDbContextPool<AppDbContext>(context =>
            {
                var server = _configuration["Server"] ?? "mssqlserver";
                var userId = _configuration["UserId"] ?? "sa";
                var password = _configuration["Password"] ?? "Masoud&7559";
                var port = _configuration["Port"] ?? "1433";
                var dbName = _configuration["DbName"] ?? "Clinic";

                var connectionString = _configuration.GetConnectionString("AppConnectionString") ??
                                       $"Server={server},{port};" +
                                       $"Initial Catalog={dbName};" +
                                       $"User Id={userId};" +
                                       $"Password={password};";
                 
                context.UseSqlServer(connectionString);
            });
            services.AddScoped<IMailService, MyMailService>();
            services.AddScoped<ILoginService, MyLoginService>();
            services.AddScoped<IReCaptchaService, MyReCaptchaService>();
            services.AddScoped<IFeedBackService, MyFeedBackService>();
            services.AddScoped<IInitializerService, MyInitializerService>();
            services.AddHttpContextAccessor();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.Cookie.Name = "_ua";
            });
            services.AddSignalR();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
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
                    name: "Doctor",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                    );
                endpoints.MapControllerRoute(
                    name: "SiteAdmin",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );
                endpoints.MapControllerRoute(
                    name: "ClinicManager",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );
                endpoints.MapControllerRoute(
                    name: "Patient",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );
                endpoints.MapControllerRoute(
                    name: "Pharmacy",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );
                endpoints.MapControllerRoute(
                    name: "defualt",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapHub<ChatHub>("/OnlineChat");
            });

        }
    }
}
