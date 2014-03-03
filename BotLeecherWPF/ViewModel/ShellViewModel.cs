using BotLeecher.Tools;
using BotLeecherWPF.Links;
using FirstFloor.ModernUI.Presentation;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFGenerics;

namespace BotLeecherWPF.ViewModel
{
    [Export]
    public class ShellViewModel : ViewModelBase
    {
        #region Members

        /// <summary>
        /// Logger
        /// </summary>
        private ILogger _logger;

        #endregion
        #region Properties

        #region TitleLinks

        private LinkCollection _titleLinks = new LinkCollection();

        /// <summary>
        /// Sets and gets the TitleLinks property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public LinkCollection TitleLinks
        {
            get
            {
                return _titleLinks;
            }

            set
            {
                if (_titleLinks == value)
                {
                    return;
                }

                _titleLinks = value;
                OnPropertyChanged();
            }
        }

        #endregion
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        [ImportingConstructor]
        public ShellViewModel(ILogger logger)
        {

            this._logger = logger;
            this._logger.Init();

            this.InitializeMenus();
        }

        #endregion

        /// <summary>
        /// Add the default menus
        /// </summary>
        private void InitializeMenus()
        {
            AddTitleLink(ServiceLocator.Current.GetInstance<SettingsTitleLinks>());
        }

        /// <summary>
        /// Add a title Link.
        /// </summary>
        /// <param name="link">Link to add</param>
        private void AddTitleLink(Link link) 
        {
            AddLinkToMenu(TitleLinks, link);
        }

        /// <summary>
        /// Add a Link to a LinkCollection.
        /// </summary>
        /// <param name="menu">LinkCollection to modify</param>
        /// <param name="link">Link to add</param>
        private void AddLinkToMenu(LinkCollection menu, Link link)
        {
            var nextLink = menu.FirstOrDefault();

            if (nextLink == null)
            {
                menu.Add(link);
            }
            else
            {
                menu.Insert(menu.IndexOf(nextLink), link);
            }
        }
    }
}