using BotLeecher.Enums;
using BotLeecher.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotLeecher.Model
{
   public class Pack : ModelBase
    {
        public int Id { get; set; }
        private PackStatus status { get; set; }
        private string name { get; set; }
        public int Size { get; set; }
        public int Downloads { get; set; }


        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                NotifyChange(() => Name);
            }
        }

        public PackStatus Status
        {
            get { return status; }
            set
            {
                status = value;
                NotifyChange(() => Status);
            }
        }

        public string toString()
        {
            return "Pack #" + Id + ", " + Size + "K, " + Downloads + " downloads -> " + Name;
        }


    }
}
