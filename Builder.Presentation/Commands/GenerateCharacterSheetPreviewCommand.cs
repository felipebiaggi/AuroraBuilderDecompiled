using System;
using System.IO;
using System.Windows.Input;
using Builder.Core.Events;
using Builder.Presentation;
using Builder.Presentation.Services;
using Builder.Presentation.Views;

namespace Builder.Presentation.Commands
{
    public class GenerateCharacterSheetPreviewCommand : ICommand
    {
        private IEventAggregator _eventAggregator;

        private readonly CharacterSheetGenerator _generator;

        public string DestinationPath { get; set; }

        public event EventHandler CanExecuteChanged;

        public GenerateCharacterSheetPreviewCommand()
        {
            _eventAggregator = ApplicationManager.Current.EventAggregator;
            _generator = new CharacterSheetGenerator(CharacterManager.Current);
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            FileInfo file = _generator.GenerateNewSheet(DestinationPath, generateForPreview: true);
            _eventAggregator.Send(new CharacterSheetPreviewEvent(file));
        }
    }
}
