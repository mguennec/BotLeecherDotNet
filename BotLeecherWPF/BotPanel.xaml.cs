using BotLeecherWPF.ViewModel;
using FirstFloor.ModernUI.Windows;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BotLeecherWPF
{
    /// <summary>
    /// Interaction logic for BotPanel.xaml
    /// </summary>
    /// 
    public partial class BotPanel : UserControl, IContent
    {
        public BotPanel()
        {

        }

        public void OnFragmentNavigation(FirstFloor.ModernUI.Windows.Navigation.FragmentNavigationEventArgs e)
        {
        }

        public void OnNavigatedFrom(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
        }

        public void OnNavigatedTo(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
            var source = Uri.UnescapeDataString(e.Source.ToString());
            if (source.Contains("Name="))
            {
                var name = source.Substring(source.IndexOf("Name=") + 5);
                this.DataContext = ServiceLocator.Current.GetInstance<MainViewModel>().GetItem(name);
            }
        }

        public void OnNavigatingFrom(FirstFloor.ModernUI.Windows.Navigation.NavigatingCancelEventArgs e)
        {
        }
    }
}
