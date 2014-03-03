using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using log4net.Config;

namespace BotLeecherWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        protected void OnStartup(object sender, StartupEventArgs e)
        {
            // Configure Log4Net
            XmlConfigurator.Configure();

            Bootstrapper.Bootstrapper bootstrapper = new Bootstrapper.Bootstrapper();
            bootstrapper.Run();
        }
    }
}
