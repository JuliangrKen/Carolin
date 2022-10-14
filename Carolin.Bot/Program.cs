using Carolin.Bot.Configuration;
using Carolin.Bot.Interfaces;
using Carolin.Bot.Services;
using Carolin.Bot.Utilites;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

var botConfig = ConfigGettter<DiscordBotConfig>.GetFromJsonFile();

var socketConfig = new DiscordSocketConfig()
{
    AlwaysDownloadUsers = true,
    ConnectionTimeout = 10_000,
    MessageCacheSize = 100,
    GatewayIntents = GatewayIntents.All
};
var socketClient = new DiscordSocketClient(socketConfig);

var commandServiceConfig = new CommandServiceConfig()
{
    DefaultRunMode = RunMode.Async,
};
var commandService = new CommandService(commandServiceConfig);

var services = new ServiceCollection()
    .AddSingleton(botConfig)
    .AddSingleton(socketClient)
    .AddSingleton(commandService)
    .AddSingleton<DiscordBotService>()
    .AddSingleton<ILogger, ConsoleLoggerService>();

var provider = services.BuildServiceProvider();
services.AddSingleton(services.BuildServiceProvider());

if (provider.GetService<DiscordBotService>() is not DiscordBotService botService)
    throw new Exception();

await botService.RunAsync();