using AvtTest1Bot.Model;
using System;

namespace AvtTest1Bot.Services
{
    public class Mapper : IMapper
    {
        public User MapUser(string text, long userId)
        {
            string[] userArray = text.Split();
            if (userArray.Length < 4)
                return null; // по хорошему передалать для выдачи юзеру корректных оповещений в чем он не прав

            User newUser = new User();
            newUser.Surname = userArray[0];
            newUser.Name = userArray[1];
            newUser.Patronymic = userArray[2];
            DateTime birthdayDate;
            if (!DateTime.TryParse(userArray[3], out birthdayDate))
            {
                return null; // по хорошему передалать для выдачи юзеру корректных оповещений в чем он не прав
            }

            newUser.Bibirthday = birthdayDate;
            newUser.RegisterDate = DateTime.Now;
            newUser.TelegramId = userId;

            return newUser;
        }
    }
}
