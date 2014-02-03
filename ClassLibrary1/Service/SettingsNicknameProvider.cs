using BotLeecher.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotLeecher.Service
{
    [Export(typeof(NicknameProvider))]
    public class SettingsNicknameProvider : NicknameProvider {

        [ImportingConstructor]
        public SettingsNicknameProvider(Settings settings)
        {
            this.Settings = settings;
        }

        private Settings Settings;

        public string GetNickName() {
            IList<string> nicks = Settings.Get(SettingProperty.PROP_NICKS).Value;
            string nick;
            if (nicks != null && nicks.Count > 0) {
                nick = nicks[new Random().Next() % nicks.Count];
            } else {
                nick = "";
            }
            return nick;
        }
    }
}
