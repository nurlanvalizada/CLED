using CledAcademy.Core.Domain;
using CledAcademy.Core.Models;
using CledAcademy.DataAccess;
using CledAcademy.Repository.UOW;
using CledAcademy.Web.Services.Abstract;
using CledAcademy.Web.Services.Concret;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySQL.Data.EntityFrameworkCore.Extensions;

namespace CledAcademy.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("config.json", optional: true, reloadOnChange: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                //builder.AddUserSecrets();

                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddDbContext<ApplicationDbContext>(options =>
             options.UseMySQL(Configuration.GetConnectionString("MySqlConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc();

            // services.Configure<IISOptions>(options => { options.AutomaticAuthentication = true; });

            // Configure Identity
            services.Configure<IdentityOptions>(options =>
            {
                // User settings
                options.User.RequireUniqueEmail = true;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 5;
                options.Password.RequireNonAlphanumeric = false;

                // Cookie settings
                options.Cookies.ApplicationCookie.LoginPath = "/Account/Login";
                options.Cookies.ApplicationCookie.LogoutPath = "/Account/LogOff";
                options.Cookies.ApplicationCookie.AutomaticChallenge = true;
            });

            services.Configure<AppSecretOptions>(Configuration.GetSection("userSecrets").GetSection("facebook"));

            #region Data Repositories Bindings

            services.AddScoped<DbContext, ApplicationDbContext>();
            services.AddScoped<IUnitOfWorkManager, UnitOfWorkManager>();

            #endregion

            #region Services Bindings

            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
            services.AddScoped<IDataDictionaryService, DataDictionaryService>();

            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, ApplicationDbContext context, RoleManager<IdentityRole> roleManager, IOptions<AppSecretOptions> appSecretOptions)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseApplicationInsightsRequestTelemetry();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStatusCodePages();

            app.UseStatusCodePagesWithRedirects("/Home/Error/{0}");

            app.UseApplicationInsightsExceptionTelemetry();

            app.UseStaticFiles();

            app.UseIdentity();

            //Add external authentication middleware below.
            app.UseFacebookAuthentication(new FacebookOptions
            {
                AppId = appSecretOptions.Value.AppId,
                AppSecret = appSecretOptions.Value.Secret,
                Fields = { "birthday", "first_name", "gender", "last_name", "email", "picture", "hometown" },
                Scope = { "public_profile", "email", "user_birthday" }
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            DbInitializer.Initialize(context, roleManager).Wait();
        }
    }
}