using BotLeecher.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using WPFGenerics;

namespace BotLeecherWPF.ViewModel
{
    [Export]
    public class SettingsViewModel : ViewModelBase
    {
        private BotMediator _mediator;
        private ICommand _chooseSaveDirCommand;
        private ICommand _choosePlayerCommand;

        [ImportingConstructor]
        public SettingsViewModel(BotMediator mediator)
        {
            this._mediator = mediator;
        }

        public ICommand ChooseSaveDirCommand
        {
            get
            {
                return _chooseSaveDirCommand ?? (_chooseSaveDirCommand = new CommandHandler(ChooseSaveDir, true));
            }
        }

        public ICommand ChoosePlayerCommand
        {
            get
            {
                return _choosePlayerCommand ?? (_choosePlayerCommand = new CommandHandler(ChoosePlayer, true));
            }
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
                OnPropertyChanged("SaveFolder");
            }
        }

        public string Player
        {
            get
            {
                return _mediator.GetPlayer();
            }
            set
            {
                _mediator.SetPlayer(value);
                OnPropertyChanged("Player");
            }
        }

        public string PlayerOptions
        {
            get
            {
                return _mediator.GetPlayerOptions();
            }
            set
            {
                _mediator.SetPlayerOptions(value);
                OnPropertyChanged("PlayerOptions");
            }
        }

        private void ChooseSaveDir()
        {
            var dialog = new FolderBrowserDialog();
            dialog.SelectedPath = SaveFolder;
            var result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                SaveFolder = dialog.SelectedPath;
            }
        }

        private void ChoosePlayer()
        {
            var dialog = new OpenFileDialog();
            var result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                Player = dialog.FileName;
            }
        }
    }
}
