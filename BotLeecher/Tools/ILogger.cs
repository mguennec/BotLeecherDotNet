using Microsoft.Practices.Prism.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotLeecher.Tools
{

    public interface ILogger : ILoggerFacade
    {
        /// <summary>
        /// Initiate the logger
        /// </summary>
        void Init();

        /// <summary>
        /// Name of the Logger
        /// </summary>
        string LoggerName { get; }

        /// <summary>
        /// Writes a log message with a priority of "None"
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="category">The message category.</param>
        void Log(string message, Category category);

        /// <summary>
        /// Writes a log message with a priority of "None"
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="ex">An exception to write</param>
        /// <param name="category">The message category.</param>
        void Log(string message, Exception ex, Category category);

        /// <summary>
        /// Writes a log message.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="ex">An exception to write</param>
        /// <param name="category">The message category.</param>
        /// <param name="priority">Not used by Log4Net; pass Priority.None.</param>
        void Log(string message, Exception ex, Category category, Priority priority);

        /// <summary>
        /// Writes a "Debug" log message with a priority of "None"
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="category">The message category.</param>
        void Debug(string message);

        /// <summary>
        /// Writes a "Debug" log message with a priority of "None"
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="ex">An exception to write</param>
        /// <param name="category">The message category.</param>
        void Debug(string message, Exception ex);

        /// <summary>
        /// Writes a "Info" log message with a priority of "None"
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="category">The message category.</param>
        void Info(string message);

        /// <summary>
        /// Writes a "Info" log message with a priority of "None"
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="ex">An exception to write</param>
        /// <param name="category">The message category.</param>
        void Info(string message, Exception ex);

        /// <summary>
        /// Writes a "warn" log message with a priority of "None"
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="category">The message category.</param>
        void Warn(string message);

        /// <summary>
        /// Writes a "warn" log message with a priority of "None"
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="ex">An exception to write</param>
        /// <param name="category">The message category.</param>
        void Warn(string message, Exception ex);

        /// <summary>
        /// Writes a "Exception" log message with a priority of "None"
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="category">The message category.</param>
        void Exception(string message);

        /// <summary>
        /// Writes a "Exception" log message with a priority of "None"
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="ex">An exception to write</param>
        /// <param name="category">The message category.</param>
        void Exception(string message, Exception ex);
    }
}
