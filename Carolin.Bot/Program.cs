using Carolin.Bot.Configuration;
using Carolin.Bot.Interfaces;
using Carolin.Bot.Services;
using Carolin.Bot.Utilites;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

var botConfig = ConfigGettter<DiscordBotConfig>.GetFromJsonFile();
var socketClient = new DiscordSocketClient();

var services = new ServiceCollection()
    .AddSingleton(botConfig)
    .AddSingleton(socketClient)
    .AddSingleton<DiscordBotService>()
    .AddSingleton<MessageCommandsService>()
    .AddSingleton<ILogger, ConsoleLoggerService>();

var provider = services.BuildServiceProvider();
services.AddSingleton(services.BuildServiceProvider());

if (provider.GetService<DiscordBotService>() is not DiscordBotService botService)
    throw new Exception();

await botService.RunAsync();