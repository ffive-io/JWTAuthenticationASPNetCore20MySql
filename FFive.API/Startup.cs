using FFive.API.AuthPolicies;
using FFive.API.Models;
using FFive.API.Providers;
using FFive.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using System;
using System.Text;
using System.Threading.Tasks;

namespace FFive.API
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
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    builder.AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin()
                    .AllowCredentials();
                });
            });

            services.AddMvc(options =>
            {
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("UserManagement", policy => policy.RequireClaim("manage_user"));
                options.AddPolicy("Admin", policy => policy.RequireClaim("admin"));
                options.AddPolicy("User", policy =>
                        policy.RequireAssertion(context =>
                            context.User.HasClaim(c => (c.Type == "admin" || c.Type == "user"))));
                options.AddPolicy("Atleast18Years", policy => policy.Requirements.Add(new AgeRequirement(18)));
            });

            services.AddSingleton<IAuthorizationHandler, MinimumAgeHandler>();

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<KookifyDbContext>()
            .AddDefaultTokenProviders();

            //services.AddDbContext<KookifyDbContext>(options => options.UseSqlServer(Configuration["DefaultConnection"]));

            services.AddDbContextPool<KookifyDbContext>( // replace "YourDbContext" with the class name of your DbContext
                options => options.UseMySql(Configuration["DefaultConnection"],
                    mysqlOptions =>
                    {
                        mysqlOptions.ServerVersion(new Version(5, 5, 45), ServerType.MySql); // replace with your Server Version and Type
                    }
            ));

            // return 401 instead of redirect to login
            services.ConfigureApplicationCookie(options =>
            {
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.Headers["Location"] = context.RedirectUri;
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
            });

            services.AddAuthentication(sharedOptions =>
            {
                sharedOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                sharedOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(cfg =>
            {
                cfg.RequireHttpsMetadata = false;

                cfg.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["TokenAuthentication:Issuer"],
                    ValidAudience = Configuration["TokenAuthentication:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["TokenAuthentication:SecretKey"]))
                };

                cfg.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                        {
                            Console.WriteLine("OnAuthenticationFailed: " +
                                context.Exception.Message);
                            return Task.CompletedTask;
                        },
                    OnTokenValidated = context =>
                        {
                            Console.WriteLine("OnTokenValidated: " +
                                context.SecurityToken);
                            return Task.CompletedTask;
                        }
                };
            });

            services.AddTransient<TokenService, TokenService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory,
           Models.KookifyDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("CorsPolicy");

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            SeedData.Seed(userManager, roleManager).Wait();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<TokenProviderMiddleware>();
            app.UseMiddleware<RefreshTokenProviderMiddleware>();
            app.UseAuthentication();

            app.UseMvc();
        }
    }
}