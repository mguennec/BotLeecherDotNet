using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotLeecher.Tools
{
    public class PropertiesLoader
    {
        private static readonly ILog LOGGER = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const string CONFIG_FILE_NAME = "botleecher.properties";
        private static PropertiesLoader instance;
        public Properties ConfigFile { get; set; }

        public static PropertiesLoader GetInstance() {
            if (instance == null) {
                lock(typeof(PropertiesLoader)) {
                    if (instance == null) {
                        instance = new PropertiesLoader();
                    }
                }
            }
            return instance;
        }

        private PropertiesLoader() {
            ConfigFile = LoadConfig();
        }


        private string GetConfigFilePath() {
            string path = (Environment.OSVersion.Platform == PlatformID.Unix || 
                   Environment.OSVersion.Platform == PlatformID.MacOSX)
                    ? Environment.GetEnvironmentVariable("HOME")
                    : Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
            path += Path.DirectorySeparatorChar + CONFIG_FILE_NAME;
            return path;
        }

        private Properties LoadConfig() {
            Properties configFile = new Properties(GetConfigFilePath());

            return configFile;
        }
        
        public string GetProperty(string property, string defaultValue) {
            return ConfigFile.Get(property, defaultValue);
        }

        public void SaveConfig() {
            ConfigFile.Save();
        }
    }
}
