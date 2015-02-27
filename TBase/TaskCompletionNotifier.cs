using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TBase
{
    public sealed class TaskCompletionNotifier<T> : INotifyPropertyChanged
    {
        public TaskCompletionNotifier(Task<T> task)
        {
            Task = task;
            if (!task.IsCompleted)
            {
                var scheduler = (SynchronizationContext.Current == null) ? TaskScheduler.Current : TaskScheduler.FromCurrentSynchronizationContext();
                task.ContinueWith(t =>
                {
                    var propertyChanged = PropertyChanged;
                    if (propertyChanged != null)
                    {
                        propertyChanged(this, new PropertyChangedEventArgs("IsCompleted"));
                        if (t.IsCanceled)
                        {
                            propertyChanged(this, new PropertyChangedEventArgs("IsCanceled"));
                        }
                        else if (t.IsFaulted)
                        {
                            propertyChanged(this, new PropertyChangedEventArgs("IsFaulted"));
                            propertyChanged(this, new PropertyChangedEventArgs("ErrorMessage"));
                        }
                        else
                        {
                            propertyChanged(this, new PropertyChangedEventArgs("IsSuccessfullyCompleted"));
                            propertyChanged(this, new PropertyChangedEventArgs("Result"));
                        }
                    }
                },
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously,
                scheduler);
            }
        }

        // Gets the task being watched. This property never changes and is never <c>null</c>.
        public Task<T> Task { get; private set; }


        // Gets the result of the task. Returns the default value of TResult if the task has not completed successfully.
        public T Result { get { return (Task.Status == TaskStatus.RanToCompletion) ? Task.Result : default(T); } }

        // Gets whether the task has completed.
        public bool IsCompleted { get { return Task.IsCompleted; } }

        // Gets whether the task has completed successfully.
        public bool IsSuccessfullyCompleted { get { return Task.Status == TaskStatus.RanToCompletion; } }

        // Gets whether the task has been canceled.
        public bool IsCanceled { get { return Task.IsCanceled; } }

        // Gets whether the task has faulted.
        public bool IsFaulted { get { return Task.IsFaulted; } }

        // Gets the error message for the original faulting exception for the task. Returns <c>null</c> if the task is not faulted.
        //public string ErrorMessage { get { return (InnerException == null) ? null : InnerException.Message; } }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
