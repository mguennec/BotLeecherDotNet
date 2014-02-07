using BotLeecher.Service;
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
    public class SettingsViewModel : ViewModelBase
    {
        private BotMediator _mediator;

        [ImportingConstructor]
        public SettingsViewModel(BotMediator mediator)
        {
            this._mediator = mediator;
        }

        public string Nicks
        {
            get
            {
                return string.Join(",", _mediator.GetNicks());
            }
            set
            {
                var nicks = value.Split(',');
                _mediator.SetNicks(nicks.ToList());
            }
        }

        public string Keywords
        {
            get
            {
                return string.Join(",", _mediator.GetKeywords());
            }
            set
            {
                var keywords = value.Split(',');
                _mediator.SetKeywords(keywords.ToList());
            }
        }

        public string SaveFolder
        {
            get
            {
                return _mediator.GetSaveDir();
            }
            set
            {
                _mediator.SetSaveDir(value);
            }
        }
    }
}
