using ETor.App;
using ETor.App.Services;
using ETor.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Core;

// var application = serviceProvider.GetRequiredService<Application>();
// var config = serviceProvider.GetRequiredService<IOptions<NetworkConfig>>()
//     .Value;
//
// await application.Initialize();
//
// await application.AddDownload("C:\\Users\\Admin\\Downloads\\17_13_the_hobbit_the_battle_of_the_five_armies-201.torrent");
//
// await application.WaitForExit();