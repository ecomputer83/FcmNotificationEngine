using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FcmNotificationEngine.Queues
{
    public class MessageQueue
    {
        private int numberOfDaysToProcess { get; set; }

        public MessageQueue(int days)
        {
            numberOfDaysToProcess = days;
        }

        public DataTable GetPendingMessages(QueueConfigElement queue)
        {

            return GetPendingOutboundMessages(queue);
        }

        private DataTable GetPendingOutboundMessages(QueueConfigElement queue)
        {
            DataTable retVal;
            Database db = DatabaseFactory.CreateDatabase(queue.ConnectionStringKey);
            string SQL = "";
            //string SQL = "SELECT * FROM PushNotification WHERE PushNotificationID='CC57B0B6-7876-4ABF-80E1-F7E187D0C29D'  AND AppID=@AppID AND CreatedOn > (GetUTCDate()-@NumDays) Order By CreatedOn";
            if (queue.DateMode == QueueConfigElement.dateMode.UniversalDate)
            {
                SQL = "SELECT * FROM Notification WHERE IsSent=0 AND ProjectID=@AppID AND CreatedOn > (GetUTCDate()-@NumDays) Order By CreatedOn";
            }
            else
            {
                SQL = "SELECT * FROM Notification WHERE IsSent=0 AND ProjectID=@AppID AND CreatedOn > (GetDate()-@NumDays) Order By CreatedOn";
            }
            //create command and specify stored procedure name
            DbCommand command = db.GetSqlStringCommand(SQL);

            db.AddInParameter(command, "@AppID", DbType.String, queue.Name);
            db.AddInParameter(command, "@NumDays", DbType.Double, numberOfDaysToProcess);



            //execute command
            retVal = ExecuteDataTable(db, null, command);

            return retVal;
        }

        public static int UpdateSentNotification(QueueConfigElement queue, string pushNotificationID)
        {
            int retVal;
            Database db = DatabaseFactory.CreateDatabase(queue.ConnectionStringKey);

            string SQL = "";
            if (queue.DateMode == QueueConfigElement.dateMode.UniversalDate)
            {
                SQL = @"UPDATE Notification SET IsSent=1, SentDate=GetUTCDate(), ModifiedOn=GetUTCDate(), ModifiedBy='SYSTEM' WHERE DeviceID=@PushNotificationID";
            }
            else
            {
                SQL = @"UPDATE Notification SET IsSent=1, SentDate=GetDate(), ModifiedOn=GetDate(), ModifiedBy='SYSTEM' WHERE DeviceID=@PushNotificationID";
            }
            //create command and specify stored procedure name
            DbCommand command = db.GetSqlStringCommand(SQL);

            // specify stored procedure parameters
            db.AddInParameter(command, "@PushNotificationID", DbType.String, pushNotificationID);

            //execute command
            retVal = ExecuteNonQuery(db, null, command);

            return retVal;
        }


        #region ExecuteDataSet
        public static DataSet ExecuteDataSet(Database db, DbTransaction tran, DbCommand cmd)
        {
            //declare variables
            DataSet ret = null;

            //check whether DBTransaction is null
            if (tran == null)
            {
                ret = db.ExecuteDataSet(cmd);
            }
            else
            {
                ret = db.ExecuteDataSet(cmd, tran);
            }

            return ret;

        }
        #endregion

        #region ExecuteDataTable
        public static DataTable ExecuteDataTable(Database db, DbTransaction tran, DbCommand cmd)
        {
            //declare variables
            DataTable ret = null;

            ret = ExecuteDataSet(db, tran, cmd).Tables[0];

            return ret;

        }
        #endregion

        #region ExecuteNonQuery
        public static int ExecuteNonQuery(Database db, DbTransaction tran, DbCommand cmd)
        {
            //declare variable
            int retVal = 0;

            if (tran == null)
            {
                retVal = db.ExecuteNonQuery(cmd);

            }
            else
            {
                retVal = db.ExecuteNonQuery(cmd, tran);
            }

            return retVal;
        }
        #endregion


    }
}
