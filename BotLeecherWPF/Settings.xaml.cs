﻿using BotLeecherWPF.ViewModel;
using FirstFloor.ModernUI.Windows;
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
using WPFGenerics;

namespace BotLeecherWPF
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    [Content("/Settings")]
    public partial class Settings : UserControl, IContent
    {
        
        [ImportingConstructor]
        public Settings(SettingsViewModel vm)
        {
            this.DataContext = vm;
            this.InitializeComponent();
        }

        public void OnFragmentNavigation(FirstFloor.ModernUI.Windows.Navigation.FragmentNavigationEventArgs e)
        {
        }

        public void OnNavigatedFrom(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
        }

        public void OnNavigatedTo(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
        }

        public void OnNavigatingFrom(FirstFloor.ModernUI.Windows.Navigation.NavigatingCancelEventArgs e)
        {
        }
    }
}
