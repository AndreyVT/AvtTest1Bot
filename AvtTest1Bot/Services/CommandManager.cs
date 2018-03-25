using System;
using System.Collections.Generic;
using AvtTest1Bot.Model;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace AvtTest1Bot.Services
{
    public class CommandManager : ICommandManager
    {
        private ILogger<CommandManager> _logger;
        private Dictionary<long, string> _commandCache;

        private Dictionary<string, MessageHandler> _handlers;
        delegate void MessageHandler(Message message);

        private IBotService _botService;
        private IMapper _mapper;
        private IUserService _userService;

        public CommandManager(ILogger<CommandManager> logger, IBotService botService, IMapper mapper, IUserService userService)
        {
            _logger = logger;
            _botService = botService;

            _mapper = mapper;
            _userService = userService;

            _commandCache = new Dictionary<long, string>();
            _handlers = new Dictionary<string, MessageHandler>();

            _handlers[Common.Help] = HelpHandler;
            _handlers[Common.Register] = RegisterHandler;
            _handlers[Common.UserInfo] = UserInfoHandler;
            _handlers[Common.Delete] = DeleteHandler;
        }

        /// <summary>
        /// Обработчик поступающих сообщений
        /// </summary>
        /// <param name="message"></param>
        public async void HandleMessage(Message message)
        {
            _logger.LogTrace($"[CommandManager.HandleMessage START] :: ChatId: {message.Chat.Id} Message: {message.Text}");

            string normalizedMessage = message.Text.ToLower().Trim();
            // если не начали обрабатывать комманду 
            if (!_commandCache.ContainsKey(message.Chat.Id))
            {
                // впроверим если нет хендлера для обработки такого сообщения, выведем подсказку
                if (!_handlers.ContainsKey(normalizedMessage))
                {
                    await _botService.Client.SendTextMessageAsync(message.Chat.Id, $"{message.Text} :: наберите /help - что бы просмотреть список доступных комманд.");
                }
                else
                {
                    _handlers[normalizedMessage].Invoke(message);
                }
            }
            else
            // если начали обрабатывать комманду, продолжим
            {
                _handlers[_commandCache[message.Chat.Id]].Invoke(message);
            }

            _logger.LogTrace($"[CommandManager.HandleMessage FINISH] :: ChatId: {message.Chat.Id} Message: {message.Text}");
        }

        /// <summary>
        /// Обработчки команды регистрации
        /// </summary>
        /// <param name="message"></param>
        private async void RegisterHandler(Message message)
        {
            // сначла проверим, возможно пользовательуже зарегистрирован
            Model.User user = _userService.GetUserByChatId(message.Chat.Id);
            if (user != null)
            {
                await _botService.Client.SendTextMessageAsync(message.Chat.Id, $"Пользователь уже зарегистрирован.");
                SendUserInfo(message, user);
                return;
            }

            // если не начинали регистрацию значит начнем
            if (!_commandCache.ContainsKey(message.Chat.Id))
            {
                _commandCache.Add(message.Chat.Id, Common.Register);
                await _botService.Client.SendTextMessageAsync(message.Chat.Id, "Хорошо. Вы хотите зарегистрироваться. Напишите информацию о нем в формате: Фамилия Имя Отчество ДатаРождения. Например: Иванов Иван Иванович 21.11.1982 ");
            }
            else // если начинали продолжим
            {
                user = _mapper.MapUser(message.Text, message.From.Id);
                if (user != null)
                {
                    user = _userService.Insert(user);
                    await _botService.Client.SendTextMessageAsync(message.Chat.Id, $"Отлично! Пользователь {user.Name} успешно зарегистрирован.");
                }
                else
                {
                    await _botService.Client.SendTextMessageAsync(message.Chat.Id, "Некорректная информация. Данные нужно вводить в формате: Фамилия Имя Отчество ДатаРождения. Например: Иванов Иван Иванович 21.11.1982");
                }
                _commandCache.Remove(message.Chat.Id);
            }
        }

        /// <summary>
        /// Обработчки команды помощи
        /// </summary>
        /// <param name="message"></param>
        private async void HelpHandler(Message message)
        {
            await _botService.Client.SendTextMessageAsync(message.Chat.Id, "Список доступных комманд. /register - зарегистрироваться. /userInfo - информация о пользователе /delete - удалить зарегистрированного пользователя ");
        }

        /// <summary>
        /// Обработчки команды информации о пользователе
        /// </summary>
        /// <param name="message"></param>
        private async void UserInfoHandler(Message message)
        {
            Model.User user = _userService.GetUserByChatId(message.Chat.Id);
            if (user == null)
            {
                await _botService.Client.SendTextMessageAsync(message.Chat.Id, "Информации не найдено. Возможно вы еще не зарегистрированы. Для регистрации используйте команду /register или /help для подсказки.");
            }
            else
            {
                SendUserInfo(message, user);
            }
        }

        /// <summary>
        /// Отправка в чат информации о пользователе
        /// </summary>
        /// <param name="message"></param>
        /// <param name="user"></param>
        private async void SendUserInfo(Message message, Model.User user)
        {
            await _botService.Client.SendTextMessageAsync(message.Chat.Id, $"Пользователь: {user.Surname} {user.Name} {user.Patronymic} Дата рождения: {user.Bibirthday} Дата регистрации: {user.RegisterDate}");
        }

        /// <summary>
        /// Обработчки команды удаления регистрации
        /// </summary>
        /// <param name="message"></param>
        private async void DeleteHandler(Message message)
        {
            // если не начинали удаление значит начнем
            if (!_commandCache.ContainsKey(message.Chat.Id))
            {
                _commandCache.Add(message.Chat.Id, Common.Delete);
                await _botService.Client.SendTextMessageAsync(message.Chat.Id, "Внимание!. Вы хотите удалить регистрацию. Для подтверждения введите: ДА");
            }
            else // если начинали продолжим
            {
                if (message.Text.ToUpper().Trim() == "ДА")
                { // значит без сожаления удаляем
                    _userService.RemoveByChatId(message.Chat.Id);
                    await _botService.Client.SendTextMessageAsync(message.Chat.Id, $"Регистрация удалена");
                    _commandCache.Remove(message.Chat.Id);
                }
                else
                {
                    await _botService.Client.SendTextMessageAsync(message.Chat.Id, $"Регистрация НЕ удалена");
                    _commandCache.Remove(message.Chat.Id);
                }
            }
        }
    }
}
