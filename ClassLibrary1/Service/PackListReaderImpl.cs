using BotLeecher.Enums;
using BotLeecher.Model;
using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BotLeecher.Service
{
    [Export(typeof(PackListReader))]
    public class PackListReaderImpl : PackListReader {

        private static readonly ILog LOGGER = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const string PATTERN = "#([0-9]+) *([0-9]+)x \\[ *<? *([0-9\\.]*)(.)\\] (.+)";
        private Settings settings;

        [ImportingConstructor]
        public PackListReaderImpl(Settings settings) {
            this.settings = settings;
        }

        public PackList ReadPacks(string listFile)
        {
            IList<Pack> packs = new List<Pack>();
            IList<string> messages = new List<string>();
            IList<string> files = new List<string>(Directory.GetFiles(settings.Get(SettingProperty.PROP_SAVEFOLDER).GetFirstValue()));
            StreamReader reader = null;
            try
            {
                reader = new StreamReader(listFile);
                string str;
                while ((str = reader.ReadLine()) != null)
                {
                    var matches = Regex.Matches(str, PATTERN);
                    if (matches.Count > 0)
                    {
                        Pack pack = readPackLine(matches[0]);
                        checkExists(pack, files);
                        packs.Add(pack);
                    }
                }
            }
            catch (Exception ex)
            {
                LOGGER.Error("Could not read packet file!", ex);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }

            return new PackList(packs, messages);
        }

        public PackList ReadPacks(StringBuilder sb)
        {
            IList<Pack> packs = new List<Pack>();
            IList<string> messages = new List<string>();
            IList<string> files = new List<string>(Directory.GetFiles(settings.Get(SettingProperty.PROP_SAVEFOLDER).GetFirstValue()));
            StringReader reader = null;
            try
            {
                reader = new StringReader(sb.ToString());
                string str;
                while ((str = reader.ReadLine()) != null)
                {
                    var matches = Regex.Matches(str, PATTERN);
                    if (matches.Count > 0)
                    {
                        Pack pack = readPackLine(matches[0]);
                        checkExists(pack, files);
                        packs.Add(pack);
                    }
                }
            }
            catch (Exception ex)
            {
                LOGGER.Error("Could not read packet file!", ex);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }

            return new PackList(packs, messages);
        }

        private void checkExists(Pack pack, IList<string> files) {
            if (files.Contains(pack.Name)) {
                pack.Status = new PackStatus(PackStatus.Status.DOWNLOADED);
            }
        }

        private int calcSize(string size, string unit) {
            int multiplier;
            switch (unit) {
                case "M":
                    multiplier = 1024;
                    break;
                case "K":
                    multiplier = 1;
                    break;
                case "G":
                    multiplier = 1024 * 1024;
                    break;
                default:
                    multiplier = 1;
                    break;
            }
            double value = 0;
            double.TryParse(size, out value);
            return (int) (value * multiplier);
        }

        private Pack readPackLine(Match matcher) {
            Pack pack = new Pack();
            int id;
            if (int.TryParse(matcher.Groups[1].Value, out id)) {
                pack.Id = id;
            }
            pack.Status = new PackStatus(PackStatus.Status.AVAILABLE);
            pack.Name = matcher.Groups[5].Value;
            int dls;
            if (int.TryParse(matcher.Groups[2].Value, out dls)) {
                pack.Downloads = dls;
            }
            pack.Size = calcSize(matcher.Groups[3].Value, matcher.Groups[4].Value);

            return pack;
        }

    }
}
