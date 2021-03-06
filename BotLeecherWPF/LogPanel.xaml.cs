﻿using BotLeecherWPF.ViewModel;
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
    /// Interaction logic for LogPanel.xaml
    /// </summary>
    [Export]
    public partial class LogPanel : UserControl
    {
        [ImportingConstructor]
        public LogPanel(MainViewModel vm)
        {
            this.DataContext = vm;
            this.InitializeComponent();
        }

        public LogPanel()
            : this(ServiceLocator.Current.GetInstance<MainViewModel>())
        {
            
        }
    }
}
