using BotLeecherWPF.ViewModel;
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
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : UserControl
    {
        
        [ImportingConstructor]
        public Settings(SettingsViewModel vm)
        {
            this.DataContext = vm;
            this.InitializeComponent();
        }

        public Settings()
            : this(ServiceLocator.Current.GetInstance<SettingsViewModel>())
        {
            
        }
    }
}
