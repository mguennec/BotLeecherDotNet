using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WPFGenerics;

namespace BotLeecherWPF.Model
{
    public class DownloadState : ModelBase
    {
        private string name;
        private int progress;

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                NotifyChange(() => Name);
            }
        }

        public int Progress
        {
            get { return progress; }
            set
            {
                progress = value;
                NotifyChange(() => Progress);
            }
        }
    }
}
