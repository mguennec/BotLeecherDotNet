using BotLeecher.Service.Properties;
using BotLeecher.Tools;
using BotLeecherWPF.ViewModel;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.MefExtensions;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace BotLeecherWPF.Bootstrapper
{
    public class Bootstrapper : MefBootstrapper
    {
        protected override System.Windows.DependencyObject CreateShell()
        {
            // retrieve the MefContentLoader export and assign to global resources (so {DynamicResource MefContentLoader} can be resolved)
            var contentLoader = this.Container.GetExportedValue<MefContentLoaderService>();
            System.Windows.Application.Current.Resources.Add("MefContentLoader", contentLoader);

            return this.Container.GetExportedValue<Shell>() as Window;
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();
            InitializeCultures();
            System.Windows.Application.Current.MainWindow = (Window) this.Shell;
            System.Windows.Application.Current.MainWindow.Show();
        }

        protected override void ConfigureAggregateCatalog()
        {
            base.ConfigureAggregateCatalog();
            this.AggregateCatalog.Catalogs.Add(
                new AssemblyCatalog(typeof(BotLeecher.Service.BotMediator).Assembly));
            this.AggregateCatalog.Catalogs.Add(
                new AssemblyCatalog(typeof(Bootstrapper).Assembly));
        }
        private void InitializeCultures()
        {
            if (!string.IsNullOrEmpty(BotLeecherWPF.Properties.Settings.Default.Culture))
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo(BotLeecherWPF.Properties.Settings.Default.Culture);
            }
            if (!string.IsNullOrEmpty(BotLeecherWPF.Properties.Settings.Default.UICulture))
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(BotLeecherWPF.Properties.Settings.Default.UICulture);
            }
        }

        protected override ILoggerFacade CreateLogger()
        {
            return new Logger();
        }

    }

}
