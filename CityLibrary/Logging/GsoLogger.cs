/*
 * www.gso-koeln.de 2020
 */
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gso.FS.EFCore.Logging
{
    public class GsoLogger : ILogger
    {
        private readonly LogLevel logLevel = LogLevel.Trace;
        private readonly List<int> eventIDs = null;

        public GsoLogger(LogLevel logLevel = LogLevel.Trace, List<int> eventIDs = null) {
            this.logLevel = logLevel;
            this.eventIDs = eventIDs;
        }

        public static bool LoggingEnabled = true;

        private static void ConsoleWriteLineLevel(object text, LogLevel l) {
            if (l >= LogLevel.Error)
                ConsoleWriteLineColor(text, ConsoleColor.Red);
            else if (l >= LogLevel.Warning)
                ConsoleWriteLineColor(text, ConsoleColor.Yellow);
            else if (l >= LogLevel.Information)
                ConsoleWriteLineColor(text, ConsoleColor.DarkGreen);
            else
                ConsoleWriteLineColor(text, ConsoleColor.Cyan);
        }

        private static void ConsoleWriteLineColor(object s, ConsoleColor color) {
            var memo = Console.ForegroundColor;
            try {
                Console.ForegroundColor = color;
                Console.WriteLine(s);
            } finally {
                Console.ForegroundColor = memo;
            }
        }

        #region ILogger implementation
        public bool IsEnabled(LogLevel logLevel) {
            return logLevel >= this.logLevel;
        }
        //=> true; // this.logLevel > 0 && logLevel >= this.logLevel;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) {
            if (!LoggingEnabled) return;
            if (eventIDs != null && !eventIDs.Contains(eventId.Id)) return;

            //var msg = state.ToString();

            //// scan duration info eg. (10ms) for coloring
            //var re = new System.Text.RegularExpressions.Regex(@"\(([0-9]*)ms\)");
            //var m = re.Match(msg);
            //if (m != null && m.Groups.Count == 2) {
            //    var g = m.Groups[1];
            //    int ms = 0;
            //    if (Int32.TryParse(g.ToString(), out ms)) {
            //        if (ms > 50 && logLevel < LogLevel.Error) logLevel = LogLevel.Error;
            //        else if (ms > 10 && logLevel < LogLevel.Warning) logLevel = LogLevel.Warning;
            //    }
            //}

            //// only keep part after ']'
            //var parts = msg.Split(']');
            //msg = parts[parts.Length - 1].Trim();

            //if (exception == null) {

            //} else {

            //}

            ////state = default(TState);
            //exception = new ApplicationException("EXC");
            //string text = $"{DateTime.Now:HH:mm:ss.fff} {logLevel} #{eventId.Id} {eventId.Name}:\n{formatter(state, exception)}";
            string text = $"{logLevel} #{eventId.Id} {eventId.Name}: {formatter(state, exception)}";
            //string text = msg;

            // Call log method now
            ConsoleWriteLineLevel(text, logLevel);
        }

        public IDisposable BeginScope<TState>(TState state) { return null; }
        #endregion
    }
}
