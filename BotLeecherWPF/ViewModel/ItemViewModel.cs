using BotLeecher.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WPFGenerics;
using BotLeecherWPF.Model;
using BotLeecher.Service;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Input;
using FirstFloor.ModernUI.Windows.Controls;
using System.Windows;
using FirstFloor.ModernUI.Presentation;
using System.Threading;
using System.Windows.Threading;
using BotLeecher.Enums;
using System.Windows.Data;
using System.Text.RegularExpressions;
using System.ComponentModel;

namespace BotLeecherWPF.ViewModel
{
    public class ItemViewModel : ViewModelBase
    {
        private string name;
        private string filter;
        private ObservableCollection<Pack> packs = new ObservableCollection<Pack>();
        private DownloadState state = new DownloadState();
        private BotMediator BotMediator;
        private EventMediatorService Service;
        private ICommand dlCommand;
        private ICommand refreshCommand;
        private ICommand cancelCommand;
        private ICollectionView _collectionView;

        public ICommand DlCommand
        {
            get
            {
                return dlCommand ?? (dlCommand = new CommandHandler<Pack>(GetPack, true));
            }
        }

        public ICommand RefreshCommand
        {
            get
            {
                return refreshCommand ?? (refreshCommand = new CommandHandler<Pack>(Refresh, true));
            }
        }

        public ICommand CancelCommand
        {
            get
            {
                return cancelCommand ?? (cancelCommand = new CommandHandler<Pack>(Cancel, true));
            }
        }

        public ItemViewModel(string tabName, BotMediator mediator, EventMediatorService service)
        {
            this.TabName = tabName;
            this.name = tabName;
            this.BotMediator = mediator;
            this.Service = service;
            _collectionView = CollectionViewSource.GetDefaultView(Packs);
            _collectionView.Filter = this.CollectionViewSourceFilter;
            _collectionView.SortDescriptions.Add(new SortDescription("Id", ListSortDirection.Descending));
            AddTab();
        }

        private bool CollectionViewSourceFilter(object obj)
        {
            return Regex.IsMatch(((Pack)obj).Name, Filter + ".*");
        }

        private void AddTab()
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(new ThreadStart(() =>
            {
                var wnd = Application.Current.MainWindow as ModernWindow;
                if (wnd.MenuLinkGroups.Count == 0)
                {
                    wnd.MenuLinkGroups.Add(new LinkGroup
                    {
                        DisplayName = "Bots",
                        GroupName = "Bots"
                    });
                }
                wnd.MenuLinkGroups.First<LinkGroup>().Links.Add(new Link
                {
                    DisplayName = this.Name,
                    Source = new Uri(Uri.EscapeUriString("/BotPanel.xaml?Name=" + this.Name), UriKind.Relative)
                });
            }));
            //AddMockData();
        }

        private void AddMockData()
        {
            for (int i = 1; i < 900; i++)
            {
                packs.Add(new Pack
                {
                    Downloads = i + 8,
                    Id = i,
                    Name = "Test" + i,
                    Size = i * 1024,
                    Status = new PackStatus(BotLeecher.Enums.PackStatus.Status.DOWNLOADED)
                });
            }
        }

        public void SetPacks(IList<Pack> packs)
        {
            var uiContext = SynchronizationContext.Current;
            uiContext.Send(x =>
            {
                var packList = (IList<Pack>) x;
                this.packs.Clear();
                foreach (var pack in packList)
                {
                    this.packs.Add(pack);
                }
            }, packs);
        }

        public void GetPack(Pack pack)
        {
            BotMediator.GetPack(name, pack.Id);
        }

        private void Cancel(Pack obj)
        {
            BotMediator.Cancel(this.Name);
        }
        private void Refresh(Pack obj)
        {
            GetList(true);
        }


        public void GetList(bool refresh)
        {
            BotMediator.GetList(name, refresh);
        }

        public string TabName
        {
            get;
            private set;
        }

        public ObservableCollection<Pack> Packs
        {
            get
            {
                return packs;
            }
            set
            {
                packs = value;
                OnPropertyChanged("Packs");
            }
        }

        public DownloadState State
        {
            get
            {
                return state;
            }
            set
            {
                state = value;
                OnPropertyChanged("State");
            }
        }

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        public string Filter
        {
            get
            {
                return filter;
            }

            set
            {
                if (filter != value)
                {
                    filter = value;
                    OnPropertyChanged("Filter");
                    _collectionView.Refresh();
                }
            }
        }
    }
}
