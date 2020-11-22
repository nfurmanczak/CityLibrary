/*
 * www.gso-koeln.de 2020
 */
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gso.FS.EFCore.Logging
{
    public class GsoLoggerProvider : ILoggerProvider
    {
        private LogLevel logLevel = LogLevel.Information;
        private ISet<int> eventIDs = null;
        private ISet<string> categories = null;

        public GsoLoggerProvider(LogLevel logLevel = LogLevel.Information) { SetLogLevel(logLevel); }

        public GsoLoggerProvider SetLogLevel(LogLevel logLevel) {
            this.logLevel = logLevel;
            return this;
        }

        public GsoLoggerProvider AddCategories(params string[] categories) {
            if (categories != null && categories.Length > 0) {
                if (this.categories == null) this.categories = new SortedSet<string>();
                this.categories.UnionWith(categories);
            }
            return this;
        }

        public GsoLoggerProvider AddEventIDs(params int[] eventIDs) {
            if (eventIDs != null && eventIDs.Length > 0) {
                if (this.eventIDs == null) this.eventIDs = new SortedSet<int>();
                this.eventIDs.UnionWith(eventIDs);
            }
            return this;
        }

        #region ***** ILoggerProvider implementation
        public ILogger CreateLogger(string categoryName) {
            // being called for each category
            categoryName = categoryName.Trim('.') + '.';
            bool match = categories == null || categories.Where(c => categoryName.StartsWith(c.Trim('.') + '.')).FirstOrDefault() != null;
            var effLogLevel = match ? logLevel : LogLevel.None;
            var logger = new GsoLogger(effLogLevel, eventIDs?.ToList());
            if (effLogLevel == LogLevel.Trace) logger.LogTrace("Logger created for categorie: " + categoryName);
            return logger;
        }
        #endregion

        public void Dispose() { }
    }
}
