﻿using BotLeecher.Tools;
using BotLeecherWPF.ViewModel;
using FirstFloor.ModernUI.Presentation;
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
using WPFLocalizeExtension.Extensions;

namespace BotLeecherWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Export]
    public partial class Shell : ModernWindow
    {
        [ImportingConstructor]
        public Shell(ShellViewModel vm)
        {
            this.DataContext = vm;
            InitializeComponent();

            SetBindings();
        }

        private void SetBindings()
        {
            var assembly = typeof(Shell).Assembly.FullName;
            var dictionary = "Resources";

            var loc = new LocExtension(string.Concat(assembly, ":", dictionary, ":", "Main"));
            var lDisplayNamepi = PropertyHelper<Link>.GetProperty(x => x.DisplayName);
            loc.SetBinding(Main, lDisplayNamepi);
        }
    }

    
}
