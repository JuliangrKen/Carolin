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

        [Command("количество-пустых")]
        public async Task GetNumHollowCommandAsync(IUser? user = null)
        {
            var gamerID = Context.User.Id;

            if (user != null && Context.User.Id == botConfig.DevID)
                gamerID = user.Id;

            try
            {
                await ReplyAsync($"Количество убитых Пустых: **{await GetDatabaseContentAsync(gamerID)}**.");
            }
            catch
            {
                await Context.Channel.SendMessageAsync("`Пользователь не найден в базе данных!`");
            }
        }

        private async Task<string> GetRandomResultAsync()
        {
            var result = new Random().Next(1, 100);

            string? msg;
            if (result <= 90)
            {
                msg = "Вы успешно победили Пустого!";
                await CreateOrUpdateUserDate(Context.User.Id, 1);

            }
            else if (result <= 95)
            {
                msg = "Вы проиграли Пустому...!";
            }
            else
            {
                msg = "Вы успешно победили Пустого, с него вам выпала маска. Поздравляю!";
                await ((IGuildUser)Context.User).AddRoleAsync(botConfig.HollowsMaskRoleID);
                await CreateOrUpdateUserDate(Context.User.Id, 1);
            }

            return msg;
        }

        private async Task CreateOrUpdateUserDate(ulong discordID, int num)
        {
            var data = await GetDatabaseContentAsync(discordID);

            try
            {
                var newNum = Convert.ToInt32(data) + num;
                await File.WriteAllTextAsync(await GetDatabaseFilePath(discordID), newNum.ToString());
            }
            catch
            {
                await File.WriteAllTextAsync(await GetDatabaseFilePath(discordID), num.ToString());
            }
        }
        private async Task<string> GetDatabaseContentAsync(ulong discordID) =>
            await File.ReadAllTextAsync(await GetDatabaseFilePath(discordID));

        private Task<string> GetDatabaseFilePath(ulong discordID)
        {
            var dir = Directory.CreateDirectory($@"{Environment.CurrentDirectory}/Database/Hollows");
            
            var filePath = $@"{dir.FullName}/{discordID}.txt";

            if (!File.Exists(filePath))
                File.Create(filePath);

            return Task.FromResult(filePath);
        }
    }
}