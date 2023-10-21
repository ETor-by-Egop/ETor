using ETor.App;
using ETor.App.Services;
using ETor.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Core;

var scheduler = TaskScheduler.Current;

var services = new ServiceCollection();

services.AddLogging(
    builder => builder
        .ClearProviders()
        .AddSerilog(
            logger: new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger(),
            dispose: true
        )
);

var configuration = new ConfigurationBuilder()
    .AddInMemoryCollection(
        new Dictionary<string, string?>()
        {
            [nameof(NetworkConfig) + ":" + nameof(NetworkConfig.Port)] = "6969",
        }
    )
    .Build();

services.AddSingleton<IConfiguration>(configuration);

services.Configure<NetworkConfig>(configuration.GetSection(nameof(NetworkConfig)));

services.AddSingleton<Application>();
services.AddSingleton<IManifestLoader, ManifestLoader>();
services.AddSingleton<IUdpConnector, UdpConnector>();
services.AddSingleton<ITrackerManager, TrackerManager>();

var serviceProvider = services.BuildServiceProvider();

var application = serviceProvider.GetRequiredService<Application>();
var config = serviceProvider.GetRequiredService<IOptions<NetworkConfig>>()
    .Value;

await application.Initialize();

await application.AddDownload("C:\\Users\\Admin\\Downloads\\17_13_the_hobbit_the_battle_of_the_five_armies-201.torrent");

await application.WaitForExit();