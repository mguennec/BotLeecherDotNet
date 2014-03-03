using log4net;
using Microsoft.Practices.Prism.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotLeecher.Tools
{

    /// <summary>
    /// See more at: http://www.codeproject.com/Articles/165376/A-Prism-4-Application-Checklist
    /// See more at: http://www.codewrecks.com/blog/index.php/2012/07/03/little-trick-to-generate-ilogger-with-mef-based-on-type/#sthash.aE4HImxU.dpuf
    /// </summary>
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(ILogger)), Export()]
    public class Logger : ILogger
    {
        #region Properties

        /// <summary>
        /// Name of the logger
        /// </summary>
        public string LoggerName { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Logger that will perform the actual logs
        /// </summary>
        private ILog _realLogger;

        /// <summary>
        /// Property returning a single instance of a named logger.
        /// A new logger is created the first time the property is called.
        /// </summary>
        private ILog RealLogger
        {
            get
            {
                return _realLogger ?? (_realLogger = CreateLogger());
            }
        }

        /// <summary>
        /// Create a logger which name is the one of the calling class.
        /// </summary>
        /// <returns>A logger</returns>
        private ILog CreateLogger()
        {
            // 3 is because in the stack trace we have :
            //  - the CreateLogger function, 
            //  - then the Get_RealLogger getter, 
            //  - the wrapper Log function 
            //  - and finally the method of the class that is calling the Debug method. 
            var frame = new StackFrame(3, false);
            var className = frame.GetMethod().DeclaringType.FullName;
            LoggerName = className;
            return log4net.LogManager.GetLogger(className);
        }

        #endregion

        #region ILoggerFacade Members

        /// <summary>
        /// Writes a log message.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="category">The message category.</param>
        /// <param name="priority">Not used by Log4Net; pass Priority.None.</param>
        public void Log(string message, Category category, Priority priority)
        {
            switch (category)
            {
                case Category.Debug:
                    if (RealLogger.IsDebugEnabled)
                        RealLogger.Debug(message);
                    break;
                case Category.Warn:
                    if (RealLogger.IsWarnEnabled)
                        RealLogger.Warn(message);
                    break;
                case Category.Exception:
                    if (RealLogger.IsErrorEnabled)
                        RealLogger.Error(message);
                    break;
                case Category.Info:
                    if (RealLogger.IsInfoEnabled)
                        RealLogger.Info(message);
                    break;
            }
        }

        #endregion

        #region ILogger members

        /// <summary>
        /// Initiate the logger.
        /// </summary>
        public void Init()
        {
            var l = RealLogger;
        }

        /// <summary>
        /// Writes a log message with a priority of "None"
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="category">The message category.</param>
        public void Log(string message, Category category)
        {
            Log(message, category, Priority.None);
        }

        /// <summary>
        /// Writes a log message with a priority of "None"
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="ex">An exception to write</param>
        /// <param name="category">The message category.</param>
        public void Log(string message, Exception ex, Category category)
        {
            Log(message, ex, category, Priority.None);
        }

        /// <summary>
        /// Writes a log message.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="ex">An exception to write</param>
        /// <param name="category">The message category.</param>
        /// <param name="priority">Not used by Log4Net; pass Priority.None.</param>
        public void Log(string message, Exception ex, Category category, Priority priority)
        {
            switch (category)
            {
                case Category.Debug:
                    if (RealLogger.IsDebugEnabled)
                        RealLogger.Debug(message, ex);
                    break;
                case Category.Warn:
                    if (RealLogger.IsWarnEnabled)
                        RealLogger.Warn(message, ex);
                    break;
                case Category.Exception:
                    if (RealLogger.IsErrorEnabled)
                        RealLogger.Error(message, ex);
                    break;
                case Category.Info:
                    if (RealLogger.IsInfoEnabled)
                        RealLogger.Info(message, ex);
                    break;
            }
        }

        /// <summary>
        /// Writes a "Debug" log message with a priority of "None"
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="category">The message category.</param>
        public void Debug(string message)
        {
            Log(message, Category.Debug, Priority.None);
        }

        /// <summary>
        /// Writes a "Debug" log message with a priority of "None"
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="ex">An exception to write</param>
        /// <param name="category">The message category.</param>
        public void Debug(string message, Exception ex)
        {
            Log(message, ex, Category.Debug, Priority.None);
        }

        /// <summary>
        /// Writes a "Info" log message with a priority of "None"
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="category">The message category.</param>
        public void Info(string message)
        {
            Log(message, Category.Info, Priority.None);
        }

        /// <summary>
        /// Writes a "Info" log message with a priority of "None"
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="ex">An exception to write</param>
        /// <param name="category">The message category.</param>
        public void Info(string message, Exception ex)
        {
            Log(message, ex, Category.Info, Priority.None);
        }

        /// <summary>
        /// Writes a "warn" log message with a priority of "None"
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="category">The message category.</param>
        public void Warn(string message)
        {
            Log(message, Category.Warn, Priority.None);
        }

        /// <summary>
        /// Writes a "warn" log message with a priority of "None"
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="ex">An exception to write</param>
        /// <param name="category">The message category.</param>
        public void Warn(string message, Exception ex)
        {
            Log(message, ex, Category.Warn, Priority.None);
        }

        /// <summary>
        /// Writes a "Exception" log message with a priority of "None"
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="category">The message category.</param>
        public void Exception(string message)
        {
            Log(message, Category.Exception, Priority.None);
        }

        /// <summary>
        /// Writes a "Exception" log message with a priority of "None"
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="ex">An exception to write</param>
        /// <param name="category">The message category.</param>
        public void Exception(string message, Exception ex)
        {
            Log(message, ex, Category.Exception, Priority.None);
        }

        #endregion
    }
}
