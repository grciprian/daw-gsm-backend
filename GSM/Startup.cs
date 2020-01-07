using AutoMapper;
using GSM.Data;
using GSM.Helpers;
using GSM.Models;
using GSM.Repositories;
using GSM.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using System.Threading.Tasks;

namespace GSM
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env) // IConfiguration configuration
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<GSMDBContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            //services.AddIdentity<ApplicationUser, IdentityRole>(options => { }).AddEntityFrameworkStores<GSMDBContext>();
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<GSMDBContext>()
                .AddDefaultTokenProviders();

            // Auto Mapper Configurations
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });
            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IGadgetRepository, GadgetRepository>();
            services.AddScoped<IContractRepository, ContractRepository>();
            services.AddScoped<IAccountService, AccountService>();

            services.AddControllers();

            // configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            // configure jwt authentication
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            // global cors policy
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            InitialSeed(serviceProvider).Wait();
        }

        public async Task InitialSeed(IServiceProvider serviceProvider)
        {
            var _context = serviceProvider.GetRequiredService<GSMDBContext>();
            var _userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var _roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var _logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger<Startup>();

            if (await _context.Roles.AnyAsync())
            {
                // do not waste time
                _logger.LogInformation("Exists Roles.");
                return;
            }

            var adminRole = Role.Admin;
            var roleNames = new string[] { adminRole, "CUSTOMER", "EMPLOYEE" };

            foreach (var roleName in roleNames)
            {
                var role = await _roleManager.RoleExistsAsync(roleName);
                if (!role)
                {
                    var result = await _roleManager.CreateAsync(new IdentityRole { Name = roleName });
                    _logger.LogInformation("Create {0}: {1}", roleName, result.Succeeded);
                }
            }

            // administrator
            var user = new ApplicationUser
            {
                UserName = "super.admin@gsm.com",
                Email = "super.admin@gsm.com",
                EmailConfirmed = true
            };
            var i = await _userManager.FindByEmailAsync(user.Email);
            if (i == null)
            {
                var adminUser = await _userManager.CreateAsync(user, "123qweQWE!@#*");
                if (adminUser.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, adminRole);
                    _logger.LogInformation("Create {0}", user.UserName);
                }
            }
        }

    }
}
