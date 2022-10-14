using Carolin.Bot.Configuration;
using Discord;
using Discord.Commands;

namespace Carolin.Bot.Modules
{
    public class HollowsModule : ModuleBase<SocketCommandContext>
    {
        private readonly DiscordBotConfig botConfig;

        public HollowsModule(DiscordBotConfig botConfig)
        {
            this.botConfig = botConfig;
        }

        [Command("пустой")]
        public async Task HollowCommandAsync()
        {
            var embed = new EmbedBuilder()
                .WithTitle("Убийство Пустого")
                .WithDescription("Вы вступили в бой с Пустым!..")
                .WithColor(Color.DarkGreen);

            var msg = await Context.Channel.SendMessageAsync(embed: embed.Build());

            Thread.Sleep(10_000);

            var result = await GetRandomResultAsync();

            embed.Description = result;

            await msg.ModifyAsync((msgProperties) =>
            {
                msgProperties.Embed = embed.Build();
            });
        }

        private async Task<string> GetRandomResultAsync()
        {
            var result = new Random().Next(1, 100);

            string? msg;
            if (result <= 90)
            {
                msg = "Вы успешно победили Пустого!";
                await ((IGuildUser)Context.User).AddRoleAsync(botConfig.HollowsMaskRoleID);

            }
            else if (result <= 95)
            {
                msg = "Вы проиграли Пустому...!";
            }
            else
            {
                msg = "Вы успешно победили Пустого, с него вам выпала маска. Поздравляю!";
            }

            return msg; 
        }
    }
}