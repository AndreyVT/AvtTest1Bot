using Telegram.Bot;

namespace AvtTest1Bot.Services
{
    public interface IBotService
    {
        TelegramBotClient Client { get; }
    }
}
