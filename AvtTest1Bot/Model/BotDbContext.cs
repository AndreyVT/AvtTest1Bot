using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AvtTest1Bot.Model
{
    public class BotDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public BotDbContext(DbContextOptions<BotDbContext> options)
           : base(options)
        {
        }
    }
}
