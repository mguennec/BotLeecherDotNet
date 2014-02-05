using BotLeecher.Model;
using ircsharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotLeecher
{
   public interface BotListener
   {
       void Failure(User botName, string fileName);

       void Complete(User botName, string fileName);

       void Beginning(User botName, string fileName);

       void PackListLoaded(User botName, IList<Pack> packList);

       void UpdateStatus(User botName, string fileName, int completion);
    }
}
