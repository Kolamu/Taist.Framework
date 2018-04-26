namespace Runner
{
    using System;
    using System.IO;

    using Mock;

    public class Log
    {
        private static string logPath = Path.Combine(Config.WorkingDirectory, "Log\\Runner.log");
        public static void Warning(string warningMessage)
        {
            string message = string.Format("{0}", warningMessage);
            Robot.Note(message, NoteType.WARNING, logPath);
        }

        public static void Debug(string debugMessage)
        {
            string message = string.Format("{0}", debugMessage);
            Robot.Note(message, NoteType.DEBUG, logPath);
        }

    }
}
