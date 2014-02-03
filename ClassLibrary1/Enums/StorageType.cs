using ClassLibrary1.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotLeecher.Enums
{
    public class StorageType
    {
        public static readonly StorageType FILES = new StorageType("files");
        public static readonly StorageType EMBEDDED_MONGO = new StorageType("embedded_mongo");

        public String Type {get; private set;}

        private StorageType(string type) {
            this.Type = type;
        }


        public static StorageType getByType(string type) {
            StorageType storageType = null;
            foreach(StorageType value in EnumsUtils.GetValues<StorageType>()) {
                if (type == value.Type)
                {
                    storageType = value;
                    break;
                }
            }
            return storageType;
        }
    }
}
