using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FcmNotificationEngine.Queues
{
    public class QueueConfigElement : ConfigurationElement
    {
        public enum dateMode
        {
            LocalDate,
            UniversalDate
        }
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get
            {
                return this["name"].ToString();
            }
            set
            {
                this["name"] = value;
            }
        }

        [ConfigurationProperty("connectionStringKey", IsRequired = true)]
        public string ConnectionStringKey
        {
            get
            {
                return this["connectionStringKey"].ToString();
            }
            set
            {
                this["connectionStringKey"] = value;
            }
        }

        [ConfigurationProperty("dateMode", IsRequired = false)]
        public dateMode DateMode
        {
            get
            {
                return (dateMode)this["dateMode"];
            }
            set
            {
                this["iosEnabled"] = value;
            }
        }

        [ConfigurationProperty("AccountKey", IsRequired = true)]
        public string AccountKey
        {
            get
            {
                return this["AccountKey"].ToString();
            }
            set
            {
                this["AccountKey"] = value;
            }
        }

        



    }
}
