using AvtTest1Bot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AvtTest1Bot.Services
{
    public interface IMapper
    {
        User MapUser(string text, long userId);
    }
}
