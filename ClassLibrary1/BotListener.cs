using BotLeecher.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotLeecher
{
   public interface BotListener
   {
       void PackListLoaded(string botName, IList<Pack> packList);

       void UpdateStatus(string botName, string fileName, int completion);
    }
}
