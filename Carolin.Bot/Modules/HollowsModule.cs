using Carolin.Bot.Configuration;
using Carolin.Bot.Models;
using Carolin.Bot.Utilites;
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
            try
            {
                var userData = await UserDataWorker.GetUserDataAsync(Context.User.Id);

                if (DateTime.Now - userData.LastUsingCommand <= new TimeSpan(0, 0, 20, 0))
                {
                    await ReplyAsync($"`КД на использование - 20 минут! Последнее использование - {userData.LastUsingCommand} (МСК)`");
                    return;
                }
            }
            catch
            {

            }

            var embed = new EmbedBuilder()
                .WithTitle("Убийство Пустого")
                .WithDescription("Вы вступили в бой с Пустым!..")
                .WithColor(Color.DarkGreen);

            var msg = await Context.Channel.SendMessageAsync(embed: embed.Build());

            var result = await GetRandomResultAsync();
            embed.Description = result;

            Thread.Sleep(10_000);

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
                await ReplyAsync($"Количество убитых Пустых: **{(await UserDataWorker.GetUserDataAsync(gamerID)).HollowsKills}**.");
            }
            catch
            {
                await Context.Channel.SendMessageAsync("`Пользователь не найден в базе данных!`");
            }
        }

        [Command("установить-пустых")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task SetNumHollowCommandAsync(IUser user, int value) =>
            await UpdateOrCreateUserDataAsync(user.Id, value, true);

        [Command("добавить-пустых")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task AddNumHollowCommandAsync(IUser user, int value) =>
            await UpdateOrCreateUserDataAsync(user.Id, value);

        private async Task<string> GetRandomResultAsync()
        {
            var result = new Random().Next(1, 100);

            string? msg;
            if (result <= 90)
            {
                msg = "Вы успешно победили Пустого!";
                await UpdateOrCreateUserDataAsync();
            }
            else if (result <= 95)
            {
                msg = "Вы проиграли Пустому...";
            }
            else
            {
                msg = "Вы успешно победили Пустого, с него вам выпала маска. Поздравляю!";

                await ((IGuildUser)Context.User).AddRoleAsync(botConfig.HollowsMaskRoleID);
                await UpdateOrCreateUserDataAsync();
            }

            return msg;
        }

        private async Task UpdateOrCreateUserDataAsync() =>
            await UpdateOrCreateUserDataAsync(Context.User.Id, 1, false, DateTime.Now);

        private async Task UpdateOrCreateUserDataAsync(ulong discrodId, int value, bool isSet = false, DateTime? date = null)
        {
            try
            {
                var data = await UserDataWorker.GetUserDataAsync(discrodId);

                var userData = new UserData()
                {
                    HollowsKills = isSet ? value: data.HollowsKills + value,
                    LastUsingCommand = date ?? data.LastUsingCommand
                };

                await UserDataWorker.UpdateOrCreateUserDataAsync(discrodId, userData);
            }
            catch
            {
                await UserDataWorker.UpdateOrCreateUserDataAsync(discrodId);
            }
        }
    }
}