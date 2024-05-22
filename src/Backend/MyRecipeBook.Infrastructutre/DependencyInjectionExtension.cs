using FluentMigrator.Runner;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Infrastructutre.DataAccess;
using MyRecipeBook.Infrastructutre.DataAccess.Repositories;
using MyRecipeBook.Infrastructutre.Extension;
using System.Reflection;

namespace MyRecipeBook.Infrastructutre;
public static class DependencyInjectionExtension
{
    public static void AddInfrastructure(this IServiceCollection services,IConfiguration configuration)
    {
        AddRepositories(services);

        if (configuration.IsUnitTestEnvironment())
            return;

        AddDbContext(services,configuration);
        AddFluentMigrator(services,configuration);  

    }

    private static void AddDbContext(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.ConnectionString();
        var serverVersion = new MySqlServerVersion(new Version(8,0,36));
        services.AddDbContext<MyRecipeBookDbContext>(dbContextOptions =>
        {
            dbContextOptions.UseMySql(connectionString, serverVersion);
        });
    }
    private static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserWriteOnlyRepository, UserRepository>();
        services.AddScoped<IUserReadOnlyRepository, UserRepository>();

    }

    private static void AddFluentMigrator(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.ConnectionString();
        services.AddFluentMigratorCore().ConfigureRunner(options =>
        {
            options
            .AddMySql8()
            .WithGlobalConnectionString(connectionString)
            .ScanIn(Assembly.Load("MyRecipeBook.Infrastructutre")).For.All();
        });
    }
}
