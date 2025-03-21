﻿using System;
using System.Windows.Input;
using Builder.Presentation.Properties;


namespace Builder.Presentation.Commands.Settings
{
    internal abstract class SettingsCommand : ICommand
    {
        protected Properties.Settings Settings { get; }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        protected SettingsCommand()
        {
            Settings = Properties.Settings.Default;
        }

        public abstract bool CanExecute(object parameter);

        public abstract void Execute(object parameter);
    }
}
