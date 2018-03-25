using AvtTest1Bot.Model;
using Telegram.Bot.Types;

namespace AvtTest1Bot.Services
{
    public interface ICommandManager
    {
        void HandleMessage(Message message);
    }
}
