using System.Linq;
using AvtTest1Bot.Model;

namespace AvtTest1Bot.Services
{
    public class UserService : IUserService
    {
        private BotDbContext _dbContext;

        public UserService(BotDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public User GetUserByChatId(long id)
        {
            return _dbContext.Users.Where(c => c.TelegramId == id).FirstOrDefault();
        }

        public User Insert(User user)
        {
            user = _dbContext.Users.Add(user).Entity;
            _dbContext.SaveChanges();
            
            return user;
        }

        public void RemoveByChatId(long id)
        {
            User user = _dbContext.Users.Where(c => c.TelegramId == id).FirstOrDefault();
            if (user != null)
            {
                _dbContext.Users.Remove(user);
                _dbContext.SaveChanges();
            }
        }
    }
}
