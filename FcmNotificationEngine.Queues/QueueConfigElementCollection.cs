using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FcmNotificationEngine.Queues
{
    public class QueueConfigElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new QueueConfigElement();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((QueueConfigElement)element).Name;
        }
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.BasicMap;
            }
        }
        protected override string ElementName
        {
            get
            {
                return "queue";
            }
        }
    }
}
