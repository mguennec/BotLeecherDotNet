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
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFGenerics;

namespace BotLeecherWPF
{
    /// <summary>
    /// Interaction logic for BotPanel.xaml
    /// </summary>
    ///
    [Content("/BotPanel")]
    public partial class BotPanel : UserControl, IContent
    {
        public BotPanel()
        {
            InitializeComponent();
        }

        public void OnFragmentNavigation(FirstFloor.ModernUI.Windows.Navigation.FragmentNavigationEventArgs e)
        {
        }

        public void OnNavigatedFrom(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
        }

        public void OnNavigatedTo(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
            var name = HttpUtility.ParseQueryString(e.Source.OriginalString.Substring(e.Source.OriginalString.IndexOf('?') + 1)).Get("Name");
            if (!string.IsNullOrWhiteSpace(name))
            {
                this.DataContext = ServiceLocator.Current.GetInstance<MainViewModel>().GetItem(name);
                dataPager.ItemsSource = ((ItemViewModel)this.DataContext).Packs;
            }
        }

        public void OnNavigatingFrom(FirstFloor.ModernUI.Windows.Navigation.NavigatingCancelEventArgs e)
        {
        }

        private void packGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            scroll.ScrollToVerticalOffset(scroll.VerticalOffset - e.Delta / 3);
        }
    }
}
