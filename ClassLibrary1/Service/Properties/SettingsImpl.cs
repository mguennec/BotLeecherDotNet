using BotLeecher.Entities;
using BotLeecher.Enums;
using BotLeecher.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotLeecher.Service.Properties
{
    [Export(typeof(Settings))]
    public class SettingsImpl : Settings {

        private const char SEPARATOR = ',';
        private static readonly char[] SEPARATORS = new char[1];
        private PropertiesLoader Loader = PropertiesLoader.GetInstance();
        private Tools.Properties ConfigFile;

        public SettingsImpl() {
            this.ConfigFile = Loader.ConfigFile;
            SEPARATORS[0] = SEPARATOR;
        }

        private String GetPropertyFromList(IList<string> list) {
            StringBuilder builder = new StringBuilder();
            bool first = true;
            foreach (string str in list) {
                if (first) {
                    first = false;
                } else {
                    builder.Append(SEPARATOR);
                }
                builder.Append(str);
            }
            return builder.ToString();
        }

        public Setting Get(SettingProperty property) {
            Setting setting = new Setting(property);
            string value = ConfigFile.Get(property.PropertyName);
            if (value == null || value.Length == 0) {
                setting.Value = property.DefaultValue;
                Save(setting);
            } else {
                string[] split = value.Split(SEPARATORS);
                setting.Value = new List<string>(split);
            }
            return setting;
        }

        public void Save(Setting setting) {
            ConfigFile.Set(setting.Key.PropertyName, GetPropertyFromList(setting.Value));
            Loader.SaveConfig();
        }
    }
}
