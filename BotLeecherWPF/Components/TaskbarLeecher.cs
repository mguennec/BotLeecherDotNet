using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hardcodet.Wpf.TaskbarNotification;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Input;
using WPFGenerics;
using System.Windows.Controls;
using System.Windows;
using System.Diagnostics;
using System.ComponentModel.Composition;

namespace BotLeecherWPF.Components
{
    [Export]
    public class TaskbarLeecher : TaskbarIcon
    {
        [DllImport("User32.dll")]
        internal static extern IntPtr SetForegroundWindow(IntPtr hWnd);

        public TaskbarLeecher() : base()
        {
            Icon = new Icon(System.Reflection.Assembly.GetEntryAssembly().GetManifestResourceStream("BotLeecherWPF.Resources.PerfCenterCpl.ico"));
            Visibility = System.Windows.Visibility.Visible;
            System.Windows.Threading.Dispatcher.CurrentDispatcher.ShutdownStarted += DisposeTask;
            DoubleClickCommand = new CommandHandler(FocusWindow, true);
            ContextMenu = new System.Windows.Controls.ContextMenu();
            var item = new MenuItem();
            item.Header = "Exit";
            item.Click += ExitWindow;
            ContextMenu.Items.Add(item);
        }

        private void ExitWindow(object sender, System.Windows.RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void FocusWindow()
        {
            var handle = Process.GetCurrentProcess().MainWindowHandle;
            SetForegroundWindow(handle);
        }

        private void DisposeTask(object sender, EventArgs e)
        {
            if (!IsDisposed)
            {
                Dispose();
            }
        }

    }
}
