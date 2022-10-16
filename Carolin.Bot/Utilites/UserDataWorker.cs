using Carolin.Bot.Models;
using System.Text.Json;

namespace Carolin.Bot.Utilites
{
    internal static class UserDataWorker
    {
        public static string UserDateDirPath => $@"{Environment.CurrentDirectory}/Database/UsersData";

        public static async Task<UserData> GetUserDataAsync(ulong discordID)
        {
            using var fs = File.OpenRead($@"{UserDateDirPath}/{discordID}.json");
            var data = await JsonSerializer.DeserializeAsync<UserData>(fs);
            return data ?? throw new Exception();
        }

        public static async Task UpdateOrCreateUserDataAsync(ulong discordID, UserData? data = null)
        {
            Directory.CreateDirectory(UserDateDirPath);

            var userData = data ?? new UserData()
            {
                HollowsKills = 1,
                LastUsingCommand = DateTime.Now
            };

            using var fs = File.Open($@"{UserDateDirPath}/{discordID}.json", FileMode.OpenOrCreate);
            await JsonSerializer.SerializeAsync(fs, userData);
        }
    }
}