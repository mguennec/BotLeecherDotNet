using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotLeecher.Model
{
    public class PackList
    {
        public IList<Pack> Packs { get { return new List<Pack>(Packs); } private set { Packs = value; } }
        public IList<string> Messages { get { return new List<string>(Messages); } private set { Messages = value; } }

        private readonly IDictionary<int, Pack> PackByNumber = new ConcurrentDictionary<int, Pack>();
        private readonly IDictionary<string, Pack> PackByName = new ConcurrentDictionary<string, Pack>();


        public PackList(IList<Pack> packs, IList<string> messages) {
            this.Packs = packs;
            this.Messages = messages;
            foreach (Pack pack in packs) {
                PackByName.Add(pack.Name, pack);
                PackByNumber.Add(pack.Id, pack);
            }
        }
        
        public Pack GetByName(string name) {
            return PackByName[name.Trim('\"')];
        }

        public Pack GetByNumber(int number) {
            return PackByNumber[number];
        }
    }
}
