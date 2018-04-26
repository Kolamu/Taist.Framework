namespace Mock.Tools.Tasks
{
    using System;
    using System.Threading;
    using Mock.Data;
    public delegate void TaistTask();

    public delegate void TaistTaskCallback(params object[] callbackParameters);

    public delegate void TaistTaskComplete(TaistTaskUnit task);

    public enum TaistTaskType
    {
        /// <summary>
        /// 并行执行
        /// </summary>
        CONCURRENCY,

        /// <summary>
        /// 重复执行
        /// </summary>
        REPEAT,

        /// <summary>
        /// 尝试执行
        /// </summary>
        ATTEMP,

        /// <summary>
        /// 竞争执行
        /// </summary>
        COMPETE,
    }

    public class TaistTaskUnit
    {
        private TaistTask _task = null;
        private TaistTaskCallback _callback = null;
        private bool _runComplete = false;
        private Thread _runThread = null;
        private RunTaskException taskException = null;
        private object[] _callbackParameters = null;

        public bool Complete
        {
            get
            {
                return _runComplete;
            }
        }

        public RunTaskException RuntimeException
        {
            get
            {
                return taskException;
            }
        }

        public void Run(TaistTaskComplete taskCompleteEventHandler, params object[] callBackParameters)
        {
            taskException = null;
            _runComplete = true;
            if (_runThread != null)
            {
                throw new TaskIsRunningException();
            }

            _callbackParameters = callBackParameters;
            _runThread = new Thread(Invoke);
            CaseManager.SetIdentification(_runThread.ManagedThreadId);
            _runThread.IsBackground = true;
            _runThread.Start(taskCompleteEventHandler);
        }

        public void Stop()
        {
            _runComplete = false;
            if (_runThread != null)
            {
                CaseManager.RemoveIdentification(_runThread.ManagedThreadId);
                try
                {
                    _runThread.Abort();
                }
                catch { }
                _runThread = null;
            }
        }

        public void Callback()
        {
            Stop();
            if (taskException != null)
            {
                return;
            }

            if (_callback != null)
            {
                _callback(_callbackParameters);
            }
        }

        private string CaseId
        {
            get
            {
                try
                {
                    return CaseManager.GetIdentification();
                }
                catch
                {
                    return "DefaultTask";
                }
            }
        }

        private void Invoke(object invokeArgs)
        {
            if (_task == null) return;
            taskException = null;
            try
            {
                _task();
            }
            catch (ThreadAbortException)
            {
                throw;
            }
            catch (Exception ex)
            {
                taskException = new RunTaskException(ex);
            }

            if (invokeArgs != null)
            {
                TaistTaskComplete taskCompleteEventHandler = invokeArgs as TaistTaskComplete;
                if (taskCompleteEventHandler != null)
                {
                    taskCompleteEventHandler(this);
                }
            }
        }

        public TaistTaskUnit(TaistTask task, TaistTaskCallback callback)
        {
            _task = task;
            _callback = callback;
        }
    }
}
