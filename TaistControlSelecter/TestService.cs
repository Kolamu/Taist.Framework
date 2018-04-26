namespace Runner
{
    using System;
    using System.Reflection;
    using System.Threading;
    using System.Windows.Forms;

    using Mock;
    using Mock.Data;
    internal class TestService
    {
        private AutoResetEvent completeEvent;
        private bool busy = false;
        public TestService()
        {
            completeEvent = new AutoResetEvent(false);
            busy = false;
        }

        ~TestService()
        {
            Stop();
        }

        /// <summary>
        /// 获取当前系统是否正在执行用例
        /// </summary>
        public bool Busy
        {
            get
            {
                return busy;
            }
        }

        internal void Start()
        {
            busy = true;
            new Thread(() =>
                {
                    completeEvent.Reset();
                    Thread testThread = new Thread(() =>
                    {
                        try
                        {
                            try
                            {
                                string path = string.Format("{0}\\..\\TestCasePool.xml", Assembly.GetEntryAssembly().Location);
                                TestCasePool.Initilize(path);
                                TestCasePool.Execute();
                                //MessageBox.Show(null, "测试完毕", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            catch (Exception ex)
                            {
                                Robot.Note(ex.Message + "\n" + ex.StackTrace, NoteType.EXCEPTION);
                                if (ex.InnerException != null)
                                {
                                    Robot.Note(string.Format("InnerException > {0}\n{1}", ex.InnerException.Message, ex.InnerException.StackTrace), NoteType.EXCEPTION);
                                }
                                //MessageBox.Show(null, string.Format("{0}\n\n详情请查看报告或日志", ex.Message), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            completeEvent.Set();
                        }
                        catch { }
                    });
                    testThread.IsBackground = true;
                    testThread.Start();
                    completeEvent.WaitOne();

                    try
                    {
                        testThread.Abort();
                    }
                    catch { }
                }).Start();
            busy = false;
        }

        internal void Stop()
        {
            if (busy)
            {
                busy = false;
                completeEvent.Set();
            }
        }
    }
}
