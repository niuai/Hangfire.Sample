using Hangfire.Dashboard.BasicAuthorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Hangfire.Sample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddHangfire(configuration =>
            {
                configuration.UseRedisStorage("127.0.0.1");
            });
            //services.AddHangfireServer();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseHangfireDashboard();
            }
            else
            {
                app.UseHangfireDashboard("/hangfire", new DashboardOptions
                {
                    Authorization = new[]
                    {
                        new BasicAuthAuthorizationFilter(new BasicAuthAuthorizationFilterOptions
                        {
                            RequireSsl = false,
                            SslRedirect = false,
                            LoginCaseSensitive = true,
                            Users = new [] { new BasicAuthAuthorizationUser { Login = "admin", PasswordClear =  "test" } }
                        })
                    }
                });
            }

            app.UseHangfireServer();

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
