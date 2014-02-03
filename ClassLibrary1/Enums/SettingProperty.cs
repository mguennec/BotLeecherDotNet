using ClassLibrary1.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotLeecher.Enums
{
    public sealed class SettingProperty
    {
        public String PropertyName {get; set;}
        public IList<string> DefaultValue {get; set;}

        public static readonly SettingProperty PROP_SAVEFOLDER = new SettingProperty("savefolder", "X:\\Anime");
        public static readonly SettingProperty PROP_SERVER = new SettingProperty("servers", "irc.rizon.net");
        public static readonly SettingProperty PROP_CHANNEL = new SettingProperty("channels", "#exiled-destiny");
        public static readonly SettingProperty PROP_NICKS = new SettingProperty("nicks", "namekman", "namekmin", "namekman22");
        public static readonly SettingProperty PROP_KEYWORDS = new SettingProperty("keywords", "added");
        public static readonly SettingProperty PROP_STORAGETYPE = new SettingProperty("db.type", StorageType.FILES.Type);
        public static readonly SettingProperty PROP_STORAGEPATH = new SettingProperty("db.path", "F:/botleecher/db");


        private SettingProperty(string name, params string[] defaultValue) {
            this.PropertyName = name;
            this.DefaultValue = new List<string>(defaultValue);
        }

        public static SettingProperty GetByPropertyName(string name) {
            SettingProperty settingNames = null;
            foreach(SettingProperty setting in EnumsUtils.GetValues<SettingProperty>()) {
                if (setting.PropertyName == name)
                {
                    settingNames = setting;
                    break;
                }
            }
            return settingNames;
        }

        private static IList<SettingProperty> GetValues()
        {
            IList<SettingProperty> retVal = new List<SettingProperty>();
            var vals = typeof(SettingProperty).GetProperties();
            foreach (var val in vals) {
                if (val.GetValue(null, null) is SettingProperty)
                {
                    retVal.Add((SettingProperty) val.GetValue(null, null));
                }
            }
            return retVal;
        }
    }
}
