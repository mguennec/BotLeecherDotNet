using BotLeecher.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotLeecher.Service
{
    public interface PackListReader {
        PackList ReadPacks(string listFile);
        PackList ReadPacks(StringBuilder sb);
    }
}
