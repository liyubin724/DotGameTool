using System;

namespace Game.Core.Tools.ExcelToData
{
    public static class Logger
    {
        public static bool IsDebug = true;
        public static void Log(string msg)
        {
            if(IsDebug)
                Console.WriteLine(msg);
        }

        public static void LogError(string msg)
        {
            ConsoleColor cc = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(msg);

            Console.ForegroundColor = cc;
        }

        public static void LogWarning(string msg)
        {
            ConsoleColor cc = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(msg);

            Console.ForegroundColor = cc;
        }
    }
}
