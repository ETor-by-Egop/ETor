using System.Security.Cryptography;
using ETor.App.Services;
using ETor.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Serilog;

namespace ETor.App;

public static class Extensions
{
    public static JsonSerializer WithMyConverter(this JsonSerializer serializer)
    {
        serializer.Converters.Insert(0, new BEncodeJsonConverter());

        return serializer;
    }

    public static JsonSerializer WithEnumAsString(this JsonSerializer serializer)
    {
        serializer.Converters.Insert(0, new StringEnumConverter());

        return serializer;
    }

    public static IServiceCollection RegisterApplication(this IServiceCollection services)
    {
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
                    [nameof(FilePickerConfig) + ":" + nameof(FilePickerConfig.DefaultPath)] = "C:\\Users\\Admin\\Downloads",
                    [nameof(FileManagerConfig) + ":" + nameof(FileManagerConfig.DownloadPath)] = "C:\\Users\\Admin\\Downloads\\ETorDownloads",
                }
            )
            .Build();

        services.AddSingleton<IConfiguration>(configuration);

        services.Configure<NetworkConfig>(configuration.GetSection(nameof(NetworkConfig)));
        services.Configure<FilePickerConfig>(configuration.GetSection(nameof(FilePickerConfig)));
        services.Configure<FileManagerConfig>(configuration.GetSection(nameof(FileManagerConfig)));

        services.AddSingleton<Application>();
        services.AddSingleton<IUdpSender, UdpSender>();
        services.AddSingleton<ITrackerManager, TrackerManager>();
        services.AddSingleton<IFileManager, FileManager>();
        services.AddSingleton<IPieceManager, PieceManager>();

        return services;
    }
}