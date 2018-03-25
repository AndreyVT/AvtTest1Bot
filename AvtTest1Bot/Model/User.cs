using System;

namespace AvtTest1Bot.Model
{
    public class User
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string Patronymic { get; set; }

        public DateTime Bibirthday { get; set; }

        public DateTime RegisterDate { get; set; }

        public long TelegramId { get; set; }
    }
}
