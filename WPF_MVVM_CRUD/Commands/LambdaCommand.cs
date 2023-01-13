using System;
using WPF_MVVM_CRUD.Commands.Base;

namespace WPF_MVVM_CRUD.Commands
{
    public class LambdaCommand : Command
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public LambdaCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public override bool CanExecute(object? parameter)
        {
            return _canExecute.Invoke(parameter);
        }

        public override void Execute(object? parameter)
        {
            _execute(parameter);
        }
    }
}
