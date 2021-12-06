using Livet.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Test.Utilities
{
    /// <summary>
    /// 任意の型の引数を1つ受け付けるRelayCommand
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RelayCommand<T> : ICommand
    {
        Action<T?> ExecuteAction { get; }
        Func<T?, bool>? CanExecuteFunc { get; }

        /// <summary>
        /// RaiseCanExecuteChanged が呼び出されたときに生成されます。
        /// </summary>
        public event EventHandler? CanExecuteChanged;

        /// <summary>
        /// 常に実行可能な新しいコマンドを作成します。
        /// </summary>
        /// <param name="execute">実行ロジック。</param>
        public RelayCommand(Action<T?> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// 新しいコマンドを作成します。
        /// </summary>
        /// <param name="execute">実行ロジック。</param>
        /// <param name="canExecute">実行ステータス ロジック。</param>
        public RelayCommand(Action<T?> execute, Func<T?, bool>? canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            ExecuteAction = execute;
            CanExecuteFunc = canExecute;
        }

        /// <summary>
        /// 現在の状態でこの <see cref="RelayCommand"/> が実行できるかどうかを判定します。
        /// </summary>
        /// <param name="parameter">
        /// コマンドによって使用されるデータ。コマンドが、データの引き渡しを必要としない場合、このオブジェクトを null に設定できます。
        /// </param>
        /// <returns>このコマンドが実行可能な場合は true、それ以外の場合は false。</returns>
        public bool CanExecute(object? parameter)
        {
            return CanExecuteFunc == null || CanExecuteFunc((T?)parameter);
        }

        /// <summary>
        /// 現在のコマンド ターゲットに対して <see cref="RelayCommand"/> を実行します。
        /// </summary>
        /// <param name="parameter">
        /// コマンドによって使用されるデータ。コマンドが、データの引き渡しを必要としない場合、このオブジェクトを null に設定できます。
        /// </param>
        public void Execute(object? parameter)
        {
            ExecuteAction((T?)parameter);
        }

        /// <summary>
        /// <see cref="CanExecuteChanged"/> イベントを発生させるために使用されるメソッド
        /// <see cref="CanExecute"/> の戻り値を表すために
        /// メソッドが変更されました。
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public class ViewModelCommandHandler
    {
        public ViewModelCommand Get(Action execute, Func<bool>? canExecute = null)
        {
            if (_Command == null)
            {
                _Command = new ViewModelCommand(execute, canExecute);
            }
            return _Command;
        }

        ViewModelCommand? _Command = null;
    }

    public class ViewModelCommandHandler<T>
    {
        // CanExecuteがないやつは、とりあえずListenerCommandに任せる
        public ListenerCommand<T> Get(Action<T> execute)
        {
            if (_ListenerCommand == null)
            {
                _ListenerCommand = new ListenerCommand<T>(execute, null);
            }
            return _ListenerCommand;
        }

        public RelayCommand<T> Get(Action<T?> execute, Func<T?, bool> canExecute)
        {
            if (_RelayCommand == null)
            {
                _RelayCommand = new RelayCommand<T>(execute, canExecute);
            }
            return _RelayCommand;
        }

        public ListenerCommand<T> Get(Action<T> execute, Func<bool> canExecute)
        {
            if (_ListenerCommand == null)
            {
                _ListenerCommand = new ListenerCommand<T>(execute, canExecute);
            }
            return _ListenerCommand;
        }

        RelayCommand<T>? _RelayCommand = null;
        ListenerCommand<T>? _ListenerCommand = null;
    }
}
