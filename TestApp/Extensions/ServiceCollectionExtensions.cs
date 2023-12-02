using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using System.Reflection;
using TestApp.AppServices.Services;
using TestApp.Data;
using TestApp.Data.Repositories;

namespace TestApp.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDbContextWithRepos(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
            services.AddScoped<DbContext, AppDbContext>();

            services.AddScoped<DepartmentRepository>();
            services.AddScoped<EmployeeRepository>();
            services.AddScoped<JobTitleRepository>();

            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<DepartmentService>();
            services.AddScoped<EmployeeService>();
            services.AddScoped<JobService>();

            return services;
        }

        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "TestApp API",
                });
                options.ResolveConflictingActions(a => a.First());
            });

            return services;
        }
    }
}
