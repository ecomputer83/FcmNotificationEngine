using Eminent.Service.Helper;
using System;
using Newtonsoft.Json.Linq;
using PushSharp.Core;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Timers;
using System.Collections.Generic;
using PushSharp.Apple;
using PushSharp.Google;
using FcmNotificationEngine.Queues;
using System.Threading;
using FcmSharp.Settings;
using FcmSharp;
using FcmSharp.Requests;

namespace FcmNotificationEngine
{
    partial class NotificationSender : DebuggableService
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly System.Timers.Timer timer = new System.Timers.Timer();
        int numberOfDaysToProcess = 3;

        public NotificationSender()
        {
            log4net.Config.XmlConfigurator.Configure();
            InitializeComponent();

            timer.Enabled = false;
            timer.AutoReset = false;
            timer.Elapsed += new ElapsedEventHandler(TimerElapsed);
        }

        protected override void OnStart(string[] args)
        {
            int interval = 60000; //every minute

            if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["POLLING_INTERVAL_SECONDS"]))
            {
                try
                {
                    interval = Convert.ToInt32(ConfigurationManager.AppSettings["POLLING_INTERVAL_SECONDS"]) * 1000;
                    timer.Interval = interval;
                }
                catch { }
            }

            if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["MESSAGES_INTERVAL_DAYS"]))
            {
                try
                {
                    numberOfDaysToProcess = Convert.ToInt32(ConfigurationManager.AppSettings["MESSAGES_INTERVAL_DAYS"]);
                }
                catch { }
            }

            log.Debug(String.Format("Checking for outbound push notifications messages created in the last {0} days; every {1} ms", numberOfDaysToProcess, interval));

            StartPushBroker();
        }

        private void StartPushBroker()
        {
            try
            {
                ProcessMessages();
                timer.Start();

            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }


        protected override void OnStop()
        {
            try
            {
                log.Info("stopped");
                timer.Stop();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        protected override void OnContinue()
        {
            try
            {
                log.Info("continuing - checking for outbound push notifications");
                ProcessMessages();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

        }

        protected override void OnPause()
        {
            try
            {
                log.Info("pausing");
                timer.Stop();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            ProcessMessages();
        }

        private void ProcessMessages()
        {
            try
            {
                SendMessagesInQueue();

            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            finally
            {
                timer.Start();
            }

        }

        private void SendMessagesInQueue()
        {
            QueueConfigSection queueSetup = (QueueConfigSection)ConfigurationManager.GetSection("queues");

            foreach (QueueConfigElement queue in queueSetup.Queues)
            {
                MessageQueue mQueue = new MessageQueue(numberOfDaysToProcess);
                DataTable dtMessages = mQueue.GetPendingMessages(queue);

                foreach (DataRow row in dtMessages.Rows)
                {

                    try
                    {
                        SendMessage(queue, row["DeviceID"].ToString(), row);
                    }
                    catch (Exception ex)
                    {
                        log.Error(String.Format("Error processing message in {0} queue", queue.Name), ex);
                    }


                }
            }
        }

        private void SendMessage(QueueConfigElement queue, string pushNotificationID, DataRow msg)
        {
            try
            {
                // Read the Credentials from a File, which is not under Version Control:
                var settings = FileBasedFcmClientSettings.CreateFromFile(msg["ProjectID"].ToString(), queue.AccountKey);

                // Construct the Client:
                using (var client = new FcmClient(settings))
                {
                    // Construct the Data Payload to send:
                    var data = new Notification
                    {
                        Title = msg["Title"].ToString(),
                        Body = msg["Body"].ToString()
                    };

                    // The Message should be sent to the given token:
                    var message = new FcmMessage()
                    {
                        ValidateOnly = false,
                        Message = new Message
                        {
                            Token = pushNotificationID,
                            Notification = data
                        }
                    };

                    // Finally send the Message and wait for the Result:
                    CancellationTokenSource cts = new CancellationTokenSource();

                    // Send the Message and wait synchronously:
                    var result = client.SendAsync(message, cts.Token).GetAwaiter().GetResult();

                    // Print the Result to the Console:
                    if (!string.IsNullOrEmpty(result.Name))
                    {
                        MessageQueue.UpdateSentNotification(queue, pushNotificationID);
                    }

                }
            }
            catch (Exception ex)
            {
                log.Error(String.Format("Error sending message {0} in {1} queue", pushNotificationID, queue.Name), ex);

            }

        }








    }
}
