using System.Text.Json;

namespace Carolin.Bot.Utilites
{
    internal static class ConfigGettter<T> where T : class
    {
        public static T GetFromJsonFile()
        {
            var path = $@"{Environment.CurrentDirectory}\Configuration\{typeof(T).Name}.json";
            return GetFromJsonFile(path);
        }

        public static T GetFromJsonFile(string path)
        {
            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<T>(json) ?? throw new ArgumentNullException(nameof(T));
        }
    }
}