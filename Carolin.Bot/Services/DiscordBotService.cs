using Carolin.Bot.Configuration;
using Carolin.Bot.Interfaces;
using Discord;
using Discord.WebSocket;

namespace Carolin.Bot.Services
{
    internal class DiscordBotService
    {
        private readonly DiscordSocketClient socketClient;
        private readonly DiscordBotConfig botConfig;
        private readonly ILogger logger;

        public DiscordBotService(DiscordSocketClient socketClient, 
            DiscordBotConfig botConfig,
            ILogger logger)
        {
            this.socketClient = socketClient;
            this.botConfig = botConfig;
            this.logger = logger;
        }

        public async Task RunAsync()
        {
            socketClient.Log += Log;

            await socketClient.LoginAsync(TokenType.Bot, botConfig.Token);
            await socketClient.StartAsync();
            await Task.Delay(-1);
        }

        private Task Log(LogMessage arg) =>
            logger.Log(arg.Message);
    }
}