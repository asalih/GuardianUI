using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Guardian.Domain;
using Guardian.Infrastructure.Data;
using MediatR;
using Guardian.Infrastructure.Repository.EF;
using Guardian.Infrastructure.Repository.Specs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Guardian.Infrastructure.Security;
using Guardian.Infrastructure.Security.Specs;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using Guardian.Domain.FirewallRule.Serialzation;

namespace Guardian.Web.UI
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
            services.AddMediatR(typeof(Representor).Assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(DBContextTransactionPipelineBehavior<,>));

            services.AddScoped(typeof(ITargetRepository), typeof(TargetRepository));
            services.AddScoped(typeof(IAccountRepository), typeof(AccountRepository));
            services.AddScoped(typeof(IFirewallRuleRepository), typeof(FirewallRuleRepository));
            services.AddScoped(typeof(IRuleLogRepository), typeof(RuleLogRepository));
            services.AddScoped(typeof(IHTTPLogRepository), typeof(HTTPLogRepository));
            services.AddScoped(typeof(IParser), typeof(Parser));

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IIdentityHelper, IdentityHelper>();

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.Lax;
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(auth =>
            {
                auth.Cookie.HttpOnly = true;
                auth.LoginPath = new PathString("/Account/Login");
                auth.AccessDeniedPath = new PathString("/Account/Forbidden");

                auth.Events = new CookieAuthenticationEvents()
                {
                    OnRedirectToAccessDenied = ctx =>
                    {
                        if (ctx.HttpContext.User?.Identity?.IsAuthenticated ?? false)
                        {
                            ctx.Response.Redirect("/Account/Login");
                        }

                        return Task.CompletedTask;
                    }
                };
            });

            services.AddDbContext<GuardianDataContext>(options =>
            {
                options.UseNpgsql(Configuration.GetValue<string>("ConnectionString"));
            });

            services.AddAutoMapper(typeof(Representor).Assembly);

            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement{
                    {
                        new OpenApiSecurityScheme{
                            Reference = new OpenApiReference{
                                Id = "Bearer", //The name of the previously defined security scheme.
                                Type = ReferenceType.SecurityScheme
                            }
                        },new List<string>()
                    }
                });

                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Guardian API", Version = "v1" });
            });

            services
                .AddMvcCore(opts => opts.EnableEndpointRouting = false)
                .AddApiExplorer()
                .AddViews()
                .AddRazorViewEngine()
                .AddDataAnnotations()
                .AddFluentValidation(cfg =>
                {
                    cfg.RegisterValidatorsFromAssemblyContaining<Representor>();
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.EnvironmentName.ToLowerInvariant() == "development")
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
            app.UseCookiePolicy();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Guardian API V1");
            });

            UpdateDatabase(app);
        }

        private static void UpdateDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<GuardianDataContext>())
                {
                    context.Database.Migrate();
                }
            }
        }
    }
}
