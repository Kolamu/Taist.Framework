namespace Mock
{
    using System;
    using System.Reflection;
    using System.Threading;
    using System.Diagnostics;
    using System.Collections.Generic;

    using Mock.Tools.Controls;

    /// <summary>
    /// 自动化测试框架日志管理类
    /// </summary>
    public class LogManager
    {
        private static bool flag = true;
        public static void Start()
        {
            //string message = "Start";
            //try
            //{
            //    StackTrace st = new StackTrace(true);
            //    string methodName = st.GetFrame(1).GetMethod().Name;
            //    string className = st.GetFrame(1).GetMethod().DeclaringType.Name;

            //    message = string.Format("{0}.{1} {2}", methodName, className, message);
            //    Robot.Note(message, NoteType.DEBUG);
            //}
            //catch (Exception ex)
            //{
            //    Robot.Note(string.Format("Log [{0}] error : {1}", message, ex.Message), NoteType.ERROR);
            //}
        }

        public static void End()
        {
            //string message = "End";
            //try
            //{
            //    StackTrace st = new StackTrace(true);
            //    string methodName = st.GetFrame(1).GetMethod().Name;
            //    string className = st.GetFrame(1).GetMethod().DeclaringType.Name;

            //    message = string.Format("{0}.{1} {2}", methodName, className, message);
            //    Robot.Note(message, NoteType.DEBUG);
            //}
            //catch (Exception ex)
            //{
            //    Robot.Note(string.Format("Log [{0}] error : {1}", message, ex.Message), NoteType.ERROR);
            //}
        }

        /// <summary>
        /// 打印调试日志
        /// </summary>
        /// <param name="message"></param>
        public static void Debug(string message)
        {
            try
            {
                StackTrace st = new StackTrace(true);
                string methodName = st.GetFrame(1).GetMethod().Name;
                string className = st.GetFrame(1).GetMethod().DeclaringType.Name;

                message = string.Format("{0}.{1} => {2}", methodName, className, message);
                if(flag)
                Robot.Note(message, NoteType.DEBUG);
            }
            catch (Exception ex)
            {
                if (flag) Robot.Note(string.Format("Log [{0}] error : {1}", message, ex.Message), NoteType.ERROR);
            }
        }

        private static List<string> writeLog = new List<string>();
        public static void DebugTimer(string stringPattern, int second, params object[] objs)
        {
            StackTrace st = new StackTrace(true);
            string message = string.Format(stringPattern, objs);
            if (!writeLog.Contains(st.ToString()))
            {
                writeLog.Add(st.ToString());
                new Thread(() =>
                {
                    Robot.Recess(second * 1000);
                    writeLog.Remove(st.ToString());
                })
                {
                    IsBackground = true
                }.Start();
                Debug(message);
            }
        }

        /// <summary>
        /// 打印调试日志
        /// </summary>
        /// <param name="stringPattern"></param>
        /// <param name="objs"></param>
        public static void DebugFormat(string stringPattern, params object[] objs)
        {
            string message = string.Empty;
            try
            {
                StackTrace st = new StackTrace(true);
                string methodName = st.GetFrame(1).GetMethod().Name;
                string className = st.GetFrame(1).GetMethod().DeclaringType.Name;

                try
                {
                    message = string.Format(stringPattern, objs);
                }
                catch (Exception ex)
                {
                    if (flag) Robot.Note(string.Format("Log {0} message format error : {1}", stringPattern, ex.Message), NoteType.ERROR);
                    return;
                }
                message = string.Format("{0}.{1} => {2}", methodName, className, message);
                if (flag) Robot.Note(message, NoteType.DEBUG);
            }
            catch (Exception ex)
            {
                if (flag) Robot.Note(string.Format("Log [{0}] error : {1}", message, ex.Message), NoteType.ERROR);
            }
        }

        /// <summary>
        /// 打印调试日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="depth"></param>
        public static void Debug(string message, int depth = 1)
        {
            try
            {
                StackTrace st = new StackTrace(true);
                string methodName = st.GetFrame(depth).GetMethod().Name;
                string className = st.GetFrame(depth).GetMethod().DeclaringType.Name;

                message = string.Format("{0}.{1} => {2}", methodName, className, message);
                if (flag) Robot.Note(message, NoteType.DEBUG);
            }
            catch(Exception ex)
            {
                if (flag) Robot.Note(string.Format("Log [{0}] depth[{2}] error : {1}", message, ex.Message, depth), NoteType.ERROR);
            }
        }

        /// <summary>
        /// 打印调试日志
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="message"></param>
        public static void Debug(WindowsUnit unit, string message)
        {
            try
            {
                StackTrace st = new StackTrace(true);
                string methodName = st.GetFrame(1).GetMethod().Name;
                string className = st.GetFrame(1).GetMethod().DeclaringType.Name;

                message = string.Format("{0}.{1} => %%%{3}.{4}%%%{2}", methodName, className, message, RobotContext.WindowName, RobotContext.ElementName);
                if (flag) Robot.Note(message, NoteType.DEBUG);
            }
            catch (Exception ex)
            {
                if (flag) Robot.Note(string.Format("Log [{0}] error : {1}", message, ex.Message), NoteType.ERROR);
            }
        }

        /// <summary>
        /// 打印错误日志
        /// </summary>
        /// <param name="ex"></param>
        public static void Error(TaistException ex)
        {
            try
            {
                StackTrace st = new StackTrace(true);
                string methodName = st.GetFrame(1).GetMethod().Name;
                string className = st.GetFrame(1).GetMethod().DeclaringType.Name;

                string message = string.Format("{0}.{1} => {2}\n{3}", methodName, className, ex.Message, ex.StackTrace);
                if (flag) Robot.Note(message, NoteType.EXCEPTION);
            }
            catch (Exception e)
            {
                if (flag) Robot.Note(string.Format("Log [{0}] error : {1}", ex.Message, e.Message), NoteType.ERROR);
            }
        }

        /// <summary>
        /// 打印错误日志
        /// </summary>
        /// <param name="ex"></param>
        public static void Error(System.Exception ex)
        {
            try
            {
                Exception error = ex;

                while (error is TargetInvocationException)
                {
                    error = error.InnerException;
                }
                StackTrace st = new StackTrace(true);
                string methodName = st.GetFrame(1).GetMethod().Name;
                string className = st.GetFrame(1).GetMethod().DeclaringType.Name;
                
                string message = null;
                if (ex is TaistException)
                {
                    message = string.Format("{0}.{1} => {2}\n{3}", methodName, className, ex.Message, ex.StackTrace);
                }
                else
                {
                    message = string.Format("{0}.{1} => UnHandledException {2}\n{3}", methodName, className, ex.Message, ex.StackTrace);
                }
                if (flag) Robot.Note(message, NoteType.EXCEPTION);
            }
            catch(Exception e)
            {
                if (flag) Robot.Note(string.Format("Log [{0}] error : {1}", ex.Message, e.Message), NoteType.ERROR);
            }
        }

        /// <summary>
        /// 打印错误日志
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="args"></param>
        public static void Error(string pattern, params object[] args)
        {
            try
            {
                StackTrace st = new StackTrace(true);
                string methodName = st.GetFrame(1).GetMethod().Name;
                string className = st.GetFrame(1).GetMethod().DeclaringType.Name;

                string message = string.Format(pattern, args);
                message = string.Format("{0}.{1} => {2}", methodName, className, message);
                if (flag) Robot.Note(message, NoteType.EXCEPTION);
            }
            catch (Exception ex)
            {
                if (flag) Robot.Note(string.Format("Log [{0}] error : {1}", pattern, ex.Message), NoteType.ERROR);
            }
        }

        /// <summary>
        /// 打印错误日志
        /// </summary>
        /// <param name="printException"></param>
        public static void ErrorOnlyPrint(System.Exception printException)
        {

            if (printException is System.Threading.ThreadAbortException)
            {
                return;
            }

            System.Exception ex = printException;
            while (ex is TargetInvocationException)
            {
                ex = ex.InnerException;
            }

            try
            {
                StackTrace st = new StackTrace(true);
                string methodName = st.GetFrame(1).GetMethod().Name;
                string className = st.GetFrame(1).GetMethod().DeclaringType.Name;

                string message = string.Format("OnlyPrint {0}.{1} => {2}\r\n{3}", methodName, className, ex.Message, ex.StackTrace);
                if (flag) Robot.Note(message, NoteType.EXCEPTION);
            }
            catch (ThreadAbortException)
            {
            }
            catch (Exception e)
            {
                if (flag) Robot.Note(string.Format("Log [{0}] error : {1}", ex.Message, e.Message), NoteType.ERROR);
            }
        }

        /// <summary>
        /// 打印警告日志
        /// </summary>
        /// <param name="message"></param>
        public static void Warning(string message)
        {
            try
            {
                StackTrace st = new StackTrace(true);
                string methodName = st.GetFrame(1).GetMethod().Name;
                string className = st.GetFrame(1).GetMethod().DeclaringType.Name;

                message = string.Format("{0}.{1} => {2}", methodName, className, message);
                if (flag) Robot.Note(message, NoteType.WARNING);
            }
            catch (Exception ex)
            {
                if (flag) Robot.Note(string.Format("Log [{0}] error : {1}", message, ex.Message), NoteType.ERROR);
            }
        }


        /// <summary>
        /// 打印消息日志
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="args"></param>
        public static void Message(string pattern, params object[] args)
        {
            try
            {
                StackTrace st = new StackTrace(true);
                string methodName = st.GetFrame(1).GetMethod().Name;
                string className = st.GetFrame(1).GetMethod().DeclaringType.Name;

                string message = string.Format(pattern, args);
                message = string.Format("{0}.{1} => {2}", methodName, className, message);
                if (flag) Robot.Note(message, NoteType.MESSAGE);
            }
            catch (Exception ex)
            {
                if (flag) Robot.Note(string.Format("Log [{0}] error : {1}", pattern, ex.Message), NoteType.ERROR);
            }
        }


        /// <summary>
        /// 打印调试日志
        /// </summary>
        /// <param name="stringPattern"></param>
        /// <param name="objs"></param>
        public static void DebugX(string stringPattern, params object[] objs)
        {
            string message = string.Empty;
            try
            {
                StackTrace st = new StackTrace(true);
                string methodName = st.GetFrame(1).GetMethod().Name;
                string className = st.GetFrame(1).GetMethod().DeclaringType.Name;

                try
                {
                    message = string.Format(stringPattern, objs);
                }
                catch (Exception ex)
                {
                    if (!flag) Robot.Note(string.Format("Log {0} message format error : {1}", stringPattern, ex.Message), NoteType.ERROR);
                    return;
                }
                message = string.Format("{0}.{1} => {2}", methodName, className, message);
                if (!flag) Robot.Note(message, NoteType.DEBUG);
            }
            catch (Exception ex)
            {
                if (!flag) Robot.Note(string.Format("Log [{0}] error : {1}", message, ex.Message), NoteType.ERROR);
            }
        }

        public static void Atom(string stringPattern, params object[] args)
        {
            try
            {
                StackTrace st = new StackTrace(true);
                string methodName = st.GetFrame(1).GetMethod().Name;
                string className = st.GetFrame(1).GetMethod().DeclaringType.Name;

                string message = string.Format(stringPattern, args);
                message = string.Format("{0}.{1} => {2}", methodName, className, message);
                if (flag) Robot.Note(message, NoteType.MESSAGE, ignoreConfig: true);
            }
            catch (Exception ex)
            {
                if (flag) Robot.Note(string.Format("Log [{0}] error : {1}", stringPattern, ex.Message), NoteType.ERROR, ignoreConfig: true);
            }
        }
    }
}
