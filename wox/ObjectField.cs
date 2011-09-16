using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace wox
{
    [XmlRootAttribute("field", IsNullable = false)]
    public class ObjectField
    {

        [XmlAttribute("type")]
        public string Type
        {
            get;
            set;
        }

        [XmlAttribute("name")]
        public string Name
        {
            get;
            set;
        }

        [XmlAttribute("value")]
        public string Value
        {
            get;
            set;
        }
    }
}
