using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AvtTest1Bot.Services
{
    public class UpdateService : IUpdateService
    {
        private readonly IBotService _botService;
        private readonly ILogger<UpdateService> _logger;

        private ICommandManager _commandManager;

        public UpdateService(IBotService botService, ILogger<UpdateService> logger, ICommandManager commandManager)
        {
            _logger = logger;

            _botService = botService;

            _commandManager = commandManager;
        }

        public async Task EchoAsync(Update update)
        {
            if (update.Type != UpdateType.MessageUpdate)
            {
                return;
            }

            var message = update.Message;

            _logger.LogInformation("Received Message from {0}", message.Chat.Id);

            if (message.Type == MessageType.TextMessage)
            {
                _commandManager.HandleMessage(message);
            }
        }
    }
}
