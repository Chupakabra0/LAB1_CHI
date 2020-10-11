using System;
using System.ServiceModel.Dispatcher;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RKMApp.Basic {
    public interface IAsyncCommand : ICommand {
        public Task ExecuteAsync();
        public bool CanExecute();
    }

    public class AsyncCommand : IAsyncCommand {
        public event EventHandler CanExecuteChanged;

        public AsyncCommand(Func<Task> execute, Func<bool> canExecute = null, IErrorHandler errorHandler = null) {
            this.execute      = execute;
            this.canExecute   = canExecute;
            this.errorHandler = errorHandler;
        }

        public bool CanExecute() {
            return !this.isExecuting && (this.canExecute?.Invoke() ?? true);
        }

        public async Task ExecuteAsync() {
            if (this.CanExecute()) {
                try {
                    this.IsExecuting = true;
                    await this.execute();
                }
                finally {
                    this.IsExecuting = false;
                }
            }

            this.RaiseCanExecuteChanged();
        }

        public void RaiseCanExecuteChanged() {
            this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        bool ICommand.CanExecute(object parameter) => this.CanExecute();
        void ICommand.Execute(object parameter)    => this.ExecuteAsync().FireAndForgetSafeAsync(this.errorHandler);

        private bool isExecuting;
        public bool IsExecuting {
            get => this.isExecuting;
            set {
                if (value == this.isExecuting) return;
                this.isExecuting = value;
                this.CanExecuteChanged?.Invoke(this, new EventArgs());
            }
        }
        private readonly Func<Task>    execute;
        private readonly Func<bool>    canExecute;
        private readonly IErrorHandler errorHandler;
    }
}