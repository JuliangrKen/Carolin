using Carolin.Bot.Interfaces;

namespace Carolin.Bot.Services
{
    public class ConsoleLoggerService : ILogger
    {
        public Task Log(string message)
        {
            Console.WriteLine(message);
            return Task.CompletedTask;
        }
    }
}