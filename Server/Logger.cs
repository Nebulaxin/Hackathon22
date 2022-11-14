using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server
{
    public static class Logger
    {
        #region Log
        public static void Log(string s)
        {
            Console.WriteLine(s);
        }

        public static void Log(object o)
        {
            Console.WriteLine(o.ToString());
        }

        public static async Task LogAsync(string s)
        {
            await Console.Out.WriteLineAsync(s);
        }

        public static async Task LogAsync(object o)
        {
            await Console.Out.WriteLineAsync(o.ToString());
        }
        #endregion

        #region Leveled Log
        private static void SetColor(Level l)
        {
            Console.ForegroundColor = l switch
            {
                Level.Warning => ConsoleColor.Yellow,
                Level.Error => ConsoleColor.Red,
                _ => ConsoleColor.Gray,
            };
        }

        public static void Log(string s, Level l)
        {
            SetColor(l);
            Console.WriteLine(s);
            Console.ResetColor();
        }

        public static void Log(object o, Level l)
        {
            SetColor(l);
            Console.WriteLine(o.ToString());
            Console.ResetColor();
        }

        public static async Task LogAsync(string s, Level l)
        {
            SetColor(l);
            await Console.Out.WriteLineAsync(s);
            Console.ResetColor();
        }

        public static async Task LogAsync(object o, Level l)
        {
            SetColor(l);
            await Console.Out.WriteLineAsync(o.ToString());
            Console.ResetColor();
        }
        #endregion

        public enum Level
        {
            Log,
            Warning,
            Error
        }
    }
}
