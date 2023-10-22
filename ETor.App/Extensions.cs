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

    public static string ToHexString(this IList<byte> bytes)
    {
        return string.Create(
            bytes.Count * 2,
            bytes,
            (span, segment) =>
            {
                for (var index = 0; index < segment.Count; index++)
                {
                    var c = segment[index];
                    if (!c.TryFormat(span[(index * 2)..], out var written, "X2") || written < 2)
                    {
                        throw new InvalidOperationException("Failed to format bytes as hex");
                    }
                }
            }
        );
    }

    public static byte[] Sha1(this byte[] input)
    {
        return SHA1.HashData(input);
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
                }
            )
            .Build();

        services.AddSingleton<IConfiguration>(configuration);

        services.Configure<NetworkConfig>(configuration.GetSection(nameof(NetworkConfig)));
        services.Configure<FilePickerConfig>(configuration.GetSection(nameof(FilePickerConfig)));

        services.AddSingleton<Application>();
        services.AddSingleton<IUdpConnector, UdpConnector>();
        services.AddSingleton<ITrackerManager, TrackerManager>();

        return services;
    }
}