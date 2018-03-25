using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AvtTest1Bot.Model;

namespace AvtTest1Bot.Services
{
    public interface IUserService
    {
        User Insert(User user);
        User GetUserByChatId(long id);
        void RemoveByChatId(long id);
    }
}
