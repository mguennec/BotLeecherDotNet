using BotLeecher.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotLeecher.Entities
{
    public class Setting
    {
        public string Id { get; set; }

        public SettingProperty Key { get; set; }

        public IList<string> Value { get; set; }

        public Setting() { }
        public Setting(SettingProperty key)
        {
            this.Key = key;
        }
        public Setting(SettingProperty key, IList<string> value)
            : this(key)
        {
            this.Value = new List<string>(value);
        }

        public Setting(SettingProperty key, params string[] value)
            : this(key, new List<string>(value)) { }

        public String GetFirstValue()
        {
            return (Value == null || Value.Count == 0) ? null : Value[0];
        }
    }
}
