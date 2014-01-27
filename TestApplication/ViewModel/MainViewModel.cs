using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFGenerics;

namespace TestApplication.ViewModel
{
    class MainViewModel : ViewModelBase
    {
        private string mySuperText;
        private List<string> mySuperList;

        public string MySuperText
        {
            get { return mySuperText;}
            private set { SetProperty(ref mySuperText, value); }
        }

        public List<string> MySuperList
        {
            get { return mySuperList; }
            private set { SetProperty(ref mySuperList, value); }
        }

        public MainViewModel()
        {
            // Set text
            MySuperText = "Hello i'm a text!";
            // Set values
            FillList();
        }

        private void FillList()
        {
            var list = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                list.Add(string.Format("Item{0}", i));
            }
            // Set item list
            MySuperList = list;
        }
    }
}
