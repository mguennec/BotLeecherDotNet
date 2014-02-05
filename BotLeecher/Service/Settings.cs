using BotLeecher.Entities;
using BotLeecher.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotLeecher.Service
{
    public interface Settings {

        Setting Get(SettingProperty property);

        void Save(Setting setting);
    }
}
