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
    }
}
