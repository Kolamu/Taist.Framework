namespace Runner
{
    using System;
    using System.Reflection;
    using System.Threading;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    using Mock;
    using Mock.Data;
    using Mock.Data.Attributes;
    using Mock.Data.TaistDataCenter;
    using Mock.Data.Exception;
    using Mock.Tools.Tasks;
    class Program
    {
        static EventWaitHandle ProgramStarted;
        static ListenService ls = new ListenService();
        static AutoResetEvent closeEvent = new AutoResetEvent(false);
        static void Main(string[] args)
        {
            RunnerParameters parameter = new RunnerParameters(args);

            if (parameter.Singleton)
            {
                bool createNew;
                ProgramStarted = new EventWaitHandle(false, EventResetMode.AutoReset, "TaistRunner", out createNew);
                if (!createNew)
                {
                    ProgramStarted.Set();
                    if (parameter.HasUserInterface)
                    {
                        MessageBox.Show(null, "有程序正在运行", "自动化测试结果", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return;
                }
            }

            AppDomain.CurrentDomain.FirstChanceException += (object source, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e) =>
                {
                    //Robot.Note(string.Format("FirstChanceException: {0}\n{1}", e.Exception.Message, e.Exception.StackTrace), NoteType.EXCEPTION);
                };

            AppDomain.CurrentDomain.UnhandledException += (object obj, UnhandledExceptionEventArgs e) =>
            {
                Exception ex = (Exception)e.ExceptionObject;
                
                Robot.Note(string.Format("UnhandledException: {0}\n{1}", ex.Message, ex.StackTrace), NoteType.EXCEPTION, ".\\Log\\UnhandledException.log");
                if (e.IsTerminating)
                {
                    if (parameter.HasUserInterface)
                    {
                        MessageBox.Show(null, "未处理的异常\n" + ex.Message + "\n\n详情请查看Log\\UnhandledException.log", "自动化测试结果", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    Environment.Exit(0);
                }
            };

            //closeEvent.Reset();
            //ls.Start();
            //closeEvent.WaitOne();
            try
            {
                TestCasePool.Initilize(System.IO.Path.Combine(Config.WorkingDirectory, "TestCasePool.xml"));
                TestCasePool.Execute();
                if (parameter.HasUserInterface)
                {
                    MessageBox.Show(null, "测试完成", "自动化测试结果", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    Robot.Recess(2000);
                }
            }
            catch (RunTaskException ex)
            {
                Robot.Note(string.Format("UnhandledException: {0}\n{1}", ex.InnerException.Message, ex.InnerException.StackTrace), NoteType.EXCEPTION);
                if (parameter.HasUserInterface)
                {
                    MessageBox.Show(null, "未处理的异常\n\n" + ex.InnerException.Message + "\n\n详情请查看日志", "自动化测试结果", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    Robot.Recess(2000);
                }
            }
            catch (Exception ex)
            {
                Robot.Note(string.Format("UnhandledException: {0}\n{1}", ex.Message, ex.StackTrace), NoteType.EXCEPTION);
                if (parameter.HasUserInterface)
                {
                    MessageBox.Show(null, "未处理的异常\n\n" + ex.Message + "\n\n详情请查看日志", "自动化测试结果", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    Robot.Recess(2000);
                }
            }
            Environment.Exit(0);
        }
    }
}
