using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Eminent.Service.Helper;
using log4net;
using System.ServiceProcess;

namespace FcmNotificationEngine
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            if (args.Length > 0 && args[0].ToLower().Equals("/debug"))
            {
                Application.Run(new ServiceRunner(new NotificationSender()));
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
       {
                 new NotificationSender()
       };
                ServiceBase.Run(ServicesToRun);
            }
        }

        public static void LogException(Exception ex)
        {
            LogException(ex, true);
        }

        public static void LogException(Exception ex, bool rethrow)
        {
            LogException(null, ex, rethrow);
        }

        public static void LogException(string Message, Exception ex, bool rethrow)
        {
            ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            if (String.IsNullOrEmpty(Message))
                log.Error(ex);
            else
                log.Error(Message, ex);

            if (rethrow)
            {
                throw ex;
            }
        }



        public static void LogInfo(string Message)
        {
            ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            log.Info(Message);

        }

        public static void LogDebug(string Message)
        {
            ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            log.Debug(Message);

        }
    }


}
