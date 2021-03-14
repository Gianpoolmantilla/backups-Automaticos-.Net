using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace backupsAutomaticos
{
    public class BaseConfigSection : ConfigurationSection 
    {
        [ConfigurationProperty("base")]
        public BaseCollection BaseItems
        {
            get { return ((BaseCollection) (base["base"]));}
        }
    }

    [ConfigurationCollection(typeof( baseElement))]
    public class BaseCollection : ConfigurationElementCollection
    {
        public baseElement this[int idx]
        {
            get { return(baseElement) BaseGet(idx); }
        }
        protected override ConfigurationElement CreateNewElement()
        {
            return new baseElement();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((baseElement)(element)).nombreBase;
        }
    } 

    public class baseElement: ConfigurationElement
    {
        [ConfigurationProperty("nombreBase", DefaultValue ="",IsKey =false, IsRequired =false)]
        public string nombreBase
        {
            get { return((string) (base["nombreBase"]));}
            set { base["nombreBase"] = value; }
        }
    }
}
