using BotLeecher.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotLeecher.Model
{
   public class Pack
    {
        public int Id { get; set; }
        public PackStatus Status { get; set; }
        public string Name { get; set; }
        public int Size { get; set; }
        public int Downloads { get; set; }


        public string toString()
        {
            return "Pack #" + Id + ", " + Size + "K, " + Downloads + " downloads -> " + Name;
        }
    }
}
