using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotLeecher.Enums
{
    public class PackStatus
    {
        public Status CurrentStatus { get; set; }
        
        public enum Status { 
            AVAILABLE, QUEUED, DOWNLOADED, DOWNLOADING
        }

        public PackStatus(Status status)
        {
            this.CurrentStatus = status;
        }

        public override string ToString()
        {
            return Enum.GetName(typeof(Status), CurrentStatus);
        }
    }
}
