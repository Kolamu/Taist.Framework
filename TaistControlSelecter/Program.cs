using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace TaistControlSelecter
{
    static class Program
    {
        public static EventWaitHandle ProgramStarted;
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool createNew;
            ProgramStarted = new EventWaitHandle(false, EventResetMode.AutoReset, "ControlSelecter", out createNew);
            if (!createNew)
            {
                ProgramStarted.Set();
                return;
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SelecterMainForm());
        }
    }
}
