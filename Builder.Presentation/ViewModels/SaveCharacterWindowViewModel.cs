using Builder.Core;
using Builder.Core.Logging;
using Builder.Presentation.Models;
using Builder.Presentation.Services.Data;
using Builder.Presentation.ViewModels.Base;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Builder.Presentation.ViewModels
{
    public sealed class SaveCharacterWindowViewModel : ViewModelBase
    {
        private string _filename;

        private CharacterFile _characterFile;

        public Character Character => CharacterManager.Current.Character;

        public string Filename
        {
            get
            {
                return _filename;
            }
            set
            {
                SetProperty(ref _filename, value, "Filename");
                SanitizeFilename();
            }
        }

        public CharacterFile CharacterFile
        {
            get
            {
                return _characterFile;
            }
            set
            {
                SetProperty(ref _characterFile, value, "CharacterFile");
            }
        }

        public ICommand SaveCharacterCommand => new RelayCommand(SaveCharacter);

        public SaveCharacterWindowViewModel()
        {
            if (base.IsInDesignMode)
            {
                _characterFile = new CharacterFile("path")
                {
                    DisplayName = "Jalan Melthrohe",
                    DisplayLevel = "3",
                    DisplayRace = "Human",
                    DisplayClass = "Fighter",
                    DisplayPortraitFilePath = ""
                };
                _filename = "jalan-3";
            }
        }

        public override Task InitializeAsync(InitializationArguments args)
        {
            if (!(args.Argument is CharacterFile characterFile))
            {
                throw new NullReferenceException("file");
            }
            CharacterFile = characterFile;
            Filename = (CharacterFile.IsNew ? CharacterFile.DisplayName : new FileInfo(CharacterFile.FilePath).Name.Replace(".dnd5e", ""));
            return base.InitializeAsync(args);
        }

        private void SaveCharacter()
        {
            if (string.IsNullOrWhiteSpace(Filename))
            {
                Filename = CharacterFile.DisplayName.ToLower();
            }
            if (File.Exists(Filename))
            {
                Logger.Info(Filename + " exists, overwriting file.");
            }
            CharacterFile.Save();
        }

        private void SanitizeFilename()
        {
            string filename = Filename;
            filename = Path.GetInvalidFileNameChars().Aggregate(filename, (string current, char invalidChar) => current.Replace(invalidChar.ToString(), ""));
            CharacterFile.FilePath = Path.Combine(DataManager.Current.GetCombinedCharacterFilePath(filename));
        }
    }
}
