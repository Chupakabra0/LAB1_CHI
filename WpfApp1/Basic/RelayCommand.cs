using System;
using System.Windows.Input;

namespace WpfApp1.Basic {
    public class RelayCommand : ICommand {

        public RelayCommand(Action<object> function, Func<object, bool> canExecute = null) {
            this.canExecute = canExecute;
            this.function = function;
        }

        public bool CanExecute(object parameter) => this.canExecute?.Invoke(parameter) ?? true;

        public void Execute(object parameter) => this.function(parameter);

        public event EventHandler CanExecuteChanged;

        protected Func<object, bool> canExecute;
        protected Action<object> function;
    }
}