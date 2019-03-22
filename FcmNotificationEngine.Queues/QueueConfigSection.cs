using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FcmNotificationEngine.Queues
{
    public class QueueConfigSection : ConfigurationSection
    {
        public QueueConfigSection()
        {

        }

        [ConfigurationProperty("", IsDefaultCollection = true)]
        public QueueConfigElementCollection Queues
        {
            get
            {
                return (QueueConfigElementCollection)base[""];
            }
        }
    }
}
