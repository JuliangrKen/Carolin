using Carolin.Bot.Configuration;
using Carolin.Bot.Interfaces;
using Discord.Commands;
using Discord.WebSocket;
using System.Windows.Input;

namespace Carolin.Bot.Services
{
    public class MessageCommandsService
    {
        private readonly DiscordSocketClient socketClient;
        private readonly CommandService commandService;
        private readonly DiscordBotConfig config;
        private readonly ILogger logger;
        private readonly IServiceProvider serviceProvider;

        public MessageCommandsService(DiscordSocketClient socketClient,
            CommandService commandService,
            DiscordBotConfig config,
            ILogger logger,
            IServiceProvider serviceProvider)
        {
            this.socketClient = socketClient;
            this.commandService = commandService;
            this.config = config;
            this.logger = logger;
            this.serviceProvider = serviceProvider;

            this.socketClient.MessageReceived += HandleAsync;
        }

        private async Task HandleAsync(SocketMessage msg)
        {
            if (msg is not SocketUserMessage message)
                return;

            int argPos = default;

            if (message.Author.IsBot || message.HasStringPrefix(config.Prefix, ref argPos))
                return;

            var context = new SocketCommandContext(socketClient, message);

            var result = await commandService.ExecuteAsync(context, argPos, serviceProvider);

            if (!result.IsSuccess)
                await logger.Log(result.ErrorReason);

            if (result.Error.Equals(CommandError.UnmetPrecondition))
                await message.Channel.SendFileAsync(result.ErrorReason);
        }
    }
}