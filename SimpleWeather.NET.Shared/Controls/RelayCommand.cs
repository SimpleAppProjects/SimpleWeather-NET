﻿using System;
using System.Windows.Input;

namespace SimpleWeather.NET.Controls
{
    public class RelayCommand : ICommand
    {
        private Action RelayAction;

        public RelayCommand(Action relay)
        {
            RelayAction = relay;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            RelayAction?.Invoke();
        }
    }
}
