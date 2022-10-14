using Carolin.Bot.Configuration;
using Carolin.Bot.Interfaces;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Reflection;

namespace Carolin.Bot.Services
{
    internal class DiscordBotService
    {
        private readonly DiscordSocketClient socketClient;
        private readonly DiscordBotConfig botConfig;
        private readonly CommandService commandService;
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger logger;

        public DiscordBotService(DiscordSocketClient socketClient, 
            DiscordBotConfig botConfig,
            CommandService commandService,
            IServiceProvider serviceProvider,
            ILogger logger)
        {
            this.socketClient = socketClient;
            this.botConfig = botConfig;
            this.commandService = commandService;
            this.serviceProvider = serviceProvider;
            this.logger = logger;
        }

        public async Task RunAsync()
        {
            socketClient.Log += Log;
            socketClient.MessageReceived += CommandHandleAsync;

            await commandService.AddModulesAsync(Assembly.GetEntryAssembly(), serviceProvider);
            await socketClient.LoginAsync(TokenType.Bot, botConfig.Token);
            await socketClient.StartAsync();
            await Task.Delay(-1);
        }

        private async Task CommandHandleAsync(SocketMessage msg)
        {
            if (msg is not SocketUserMessage message)
                return;

            int argPos = 0;
            if (message.Author.IsBot || !message.HasStringPrefix(botConfig.Prefix, ref argPos))
                return;

            var context = new SocketCommandContext(socketClient, message);
            var result = await commandService.ExecuteAsync(context, argPos, serviceProvider);

            if (!result.IsSuccess)
                await logger.Log(result.ErrorReason);

            if (result.Error.Equals(CommandError.UnmetPrecondition))
                await message.Channel.SendFileAsync(result.ErrorReason);
        }

        private Task Log(LogMessage arg) =>
            logger.Log(arg.Message);
    }
}