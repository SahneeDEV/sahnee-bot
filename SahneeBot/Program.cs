using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SahneeBotController;
using SahneeBotModel;

Console.WriteLine("Hello, World!");

var config = new ConfigurationBuilder()
    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    .AddJsonFile("appsettings.json")
    .AddUserSecrets<Program>()
    .Build();

Console.WriteLine("Secret config is " + config["Test:Key"]);

//Variables
IServiceProvider _serviceProvider;

//Create a configuration
IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
configurationBuilder.AddJsonFile("appsettings.json");
IConfiguration configuration = configurationBuilder.Build();

//Register DI Services
ServiceCollection? services = new ServiceCollection();

//add services
services.AddDbContext<SahneeBotModelContext>(options =>
{
    options.UseNpgsql()
        .LogTo(Console.WriteLine, LogLevel.Information)
        .EnableSensitiveDataLogging()
        .EnableDetailedErrors();
});
services.AddSingleton<IConfiguration>(provider => configuration);


//build to service provider
_serviceProvider = services.BuildServiceProvider(true);



Class1 class1 = new Class1(_serviceProvider);

