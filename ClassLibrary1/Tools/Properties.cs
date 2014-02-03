using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotLeecher.Tools
{
    public class Properties
    {
        private Dictionary<String, String> List;
        private String Filename;

        public Properties(String file)
        {
            Reload(file);
        }

        public String Get(String field, String defValue)
        {
            return (Get(field) == null) ? (defValue) : (Get(field));
        }
        public String Get(String field)
        {
            return (List.ContainsKey(field)) ? (List[field]) : (null);
        }

        public void Set(String field, Object value)
        {
            if (!List.ContainsKey(field))
                List.Add(field, value.ToString());
            else
                List[field] = value.ToString();
        }

        public void Save()
        {
            Save(this.Filename);
        }

        public void Save(string filename)
        {
            this.Filename = filename;

            if (!System.IO.File.Exists(filename))
                System.IO.File.Create(filename);

            System.IO.StreamWriter file = new System.IO.StreamWriter(filename);

            foreach (string prop in List.Keys.ToArray())
                if (!string.IsNullOrWhiteSpace(List[prop]))
                    file.WriteLine(prop + "=" + List[prop]);

            file.Close();
        }

        public void Reload()
        {
            Reload(this.Filename);
        }

        public void Reload(String filename)
        {
            this.Filename = filename;
            List = new Dictionary<String, String>();

            if (System.IO.File.Exists(filename))
                LoadFromFile(filename);
            else
                System.IO.File.Create(filename);
        }

        private void LoadFromFile(string file)
        {
            foreach (string line in System.IO.File.ReadAllLines(file))
            {
                if ((!string.IsNullOrEmpty(line)) &&
                    (!line.StartsWith(";")) &&
                    (!line.StartsWith("#")) &&
                    (!line.StartsWith("'")) &&
                    (line.Contains('=')))
                {
                    int index = line.IndexOf('=');
                    string key = line.Substring(0, index).Trim();
                    string value = line.Substring(index + 1).Trim();

                    if ((value.StartsWith("\"") && value.EndsWith("\"")) ||
                        (value.StartsWith("'") && value.EndsWith("'")))
                    {
                        value = value.Substring(1, value.Length - 2);
                    }

                    try
                    {
                        //ignore dublicates
                        List.Add(key, value);
                    }
                    catch { }
                }
            }
        }


    }
}
