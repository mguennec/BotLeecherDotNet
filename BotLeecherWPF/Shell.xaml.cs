using BotLeecherWPF.ViewModel;
using FirstFloor.ModernUI.Windows.Controls;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Export]
    public partial class Shell : ModernWindow
    {
        [ImportingConstructor]
        public Shell(MainViewModel vm)
        {
            this.DataContext = vm;
        }

        public Shell()
        {
            this.DataContext = ServiceLocator.Current.GetInstance<MainViewModel>();
            Application.Current.MainWindow = this;
            this.Closed += Exit;
        }

        private void Exit(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
        
    }

    
}
