using BotLeecher.Tools;
using FirstFloor.ModernUI.Presentation;
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

namespace BotLeecherWPF.Links
{
    /// <summary>
    /// Interaction logic for SettingsTitleLinks.xaml
    /// </summary>
    [Export]
    public partial class SettingsTitleLinks : Link
    {
        public SettingsTitleLinks()
        {
            InitializeComponent();
            SetBindings();
        }

        private void SetBindings()
        {
            var assembly = typeof(SettingsTitleLinks).Assembly.FullName;
            var dictionary = "Resources";

            var LinkSettingsLoc = new LocExtension(string.Concat(assembly, ":", dictionary, ":", "Settings"));
            var lDisplayNamepi = PropertyHelper<Link>.GetProperty(x => x.DisplayName);
            LinkSettingsLoc.SetBinding(linkSettings, lDisplayNamepi);
        }
    }
}
