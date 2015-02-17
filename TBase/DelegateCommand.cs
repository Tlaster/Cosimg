using System;
using System.Windows.Input;

namespace TBase
{
    public class DelegateCommand : ICommand
    {
        //用来执行无参数的命令的委托
        private Action action;
        //用来执行有参数的命令的委托
        private Action<object> paction;
        //是否可执行当前的方法
        private bool _can = true;
        public DelegateCommand(Action action)
        {
            this.action = action;
        }
        public DelegateCommand(Action<object> paction)
        {
            this.paction = paction;
        }
        public bool CanExecute(object parameter)
        {
            return this._can;
        }
        ///   <summary>
        ///  更改方法可执行状态
        ///   </summary>
        ///   <param name="can"></param>
        public void OnCanExecuteChanged(bool can)
        {
            this._can = can;
            if (this.CanExecuteChanged != null)
                this.CanExecuteChanged(this, new EventArgs());
        }
        public event EventHandler CanExecuteChanged;
        public void Execute(object parameter)
        {
            if (this.action != null)
                this.action();
            if (this.paction != null)
                this.paction(parameter);
        }
    }
    public class DelegateCommand<T> : ICommand
    {
        //用来执行有参数的命令的委托
        private Action<T> paction;
        //是否可执行当前的方法
        private bool _can = true;
        public DelegateCommand(Action<T> paction)
        {
            this.paction = paction;
        }
        public bool CanExecute(object parameter)
        {
            return this._can;
        }
        ///   <summary>
        ///  更改方法可执行状态
        ///   </summary>
        ///   <param name="can"></param>
        public void OnCanExecuteChanged(bool can)
        {
            this._can = can;
            if (this.CanExecuteChanged != null)
                this.CanExecuteChanged(this, new EventArgs());
        }
        public event EventHandler CanExecuteChanged;
        public void Execute(object parameter)
        {
            if (this.paction != null)
                this.paction((T)parameter);
        }
    }
}
