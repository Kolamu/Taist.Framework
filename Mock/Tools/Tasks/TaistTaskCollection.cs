using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace Mock.Tools.Tasks
{
    using System.Reflection;

    using Mock.Tools.Exception;

    using Exception = System.Exception;
    
    public class TaistTaskCollection : List<TaistTaskUnit>, IDisposable
    {
        public int MaxRunCount { get; set; }

        private int RunCount
        {
            get
            {
                return MaxRunCount > 0 ? MaxRunCount : Count;
            }
        }

        ~TaistTaskCollection()
        {
            Dispose();
        }

        private int timeout = 0;
        private readonly object taskLock = new object();
        private int currentTaskIndex = 0;
        private AutoResetEvent reset = new AutoResetEvent(false);
        private bool wRet = false;
        private Exception taskException = null;

        enum TaskState
        {
            RUNNING,
            STOPPED
        }

        private TaskState state = TaskState.STOPPED;

        public void Run(TaistTaskType type, int timeout = 0)
        {
            if (this.Count == 0) return;
            try
            {
                this.timeout = timeout;
                currentTaskIndex = 0;
                Type t = typeof(TaistTaskCollection);
                MethodInfo m = t.GetMethod(type.ToString(), BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic);
                if (m == null)
                {
                    throw new InvalidTypeException(type.ToString());
                }
                reset.Reset();
                taskException = null;
                m.Invoke(this, null);

                if (!wRet)
                {
                    throw new TimeOutException(string.Format("Run {0} task", type));
                }

                if (taskException != null)
                {
                    throw taskException;
                }
            }
            finally
            {
                Close();
            }
        }

        public void Add(TaistTask task)
        {
            base.Add(new TaistTaskUnit(task, null));
        }

        private List<TaistTaskUnit> concurrencyTaskList = new List<TaistTaskUnit>();
        /// <summary>
        /// 并行执行
        /// </summary>
        private void Concurrency()
        {
            concurrencyTaskList = new List<TaistTaskUnit>();
            currentTaskIndex = 0;
            state = TaskState.RUNNING;
            for (int i = 0; i < RunCount; i++)
            {
                ConcurrencyRunTask(PopTask());
            }

            if (timeout > 0)
            {
                wRet = reset.WaitOne(timeout);
            }
            else
            {
                wRet = reset.WaitOne();
            }
        }

        private void ConcurrencyCallback(TaistTaskUnit task)
        {
            concurrencyTaskList.Remove(task);
            ConcurrencyRunTask(PopTask());
            if (concurrencyTaskList.Count == 0)
            {
                state = TaskState.STOPPED;
                reset.Set();
            }
            Callback(task);
        }

        private void ConcurrencyRunTask(TaistTaskUnit task)
        {
            if (task == null) return;
            concurrencyTaskList.Add(task);
            task.Run(ConcurrencyCallback);
        }

        private void Compete()
        {
            state = TaskState.RUNNING;
            competeTask = null;
            foreach (TaistTaskUnit task in this)
            {
                task.Run(CompeteCallback);
            }

            if (timeout > 0)
            {
                wRet = reset.WaitOne(timeout);
            }
            else
            {
                wRet = reset.WaitOne();
            }

            Callback(competeTask);
        }

        private void Callback(TaistTaskUnit completeTask)
        {
            if (completeTask == null)
            {
                taskException = new RunTaskException("complete task is null");
            }
            else
            {
                if (completeTask.RuntimeException == null)
                {
                    completeTask.Callback();
                }
                else
                {
                    taskException = completeTask.RuntimeException;
                }
            }
        }

        private TaistTaskUnit competeTask = null;
        private void CompeteCallback(TaistTaskUnit task)
        {
            if (state == TaskState.STOPPED)
            {
                return;
            }
            state = TaskState.STOPPED;
            competeTask = task;
            reset.Set();
        }

        private TaistTaskUnit PopTask()
        {
            lock (taskLock)
            {
                if (currentTaskIndex >= Count)
                {
                    return null;
                }
                else
                {
                    return this[currentTaskIndex++];
                }
            }
        }

        public void Close()
        {
            Dispose();
        }

        public void Dispose()
        {
            currentTaskIndex = 0;
            foreach (TaistTaskUnit task in this)
            {
                task.Stop();
            }
        }
    }
}
