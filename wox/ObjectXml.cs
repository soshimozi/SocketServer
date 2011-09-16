using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace wox
{
    [XmlRootAttribute("object", IsNullable=false)]
    public class ObjectXml
    {
        private const string ArrayTypeName = "array";

        [XmlElement("field", IsNullable=false)]
        public List<ObjectField> Fields
        {
            get;
            set;
        }


        [XmlAttribute("elementType")]
        public string ElementType
        {
            get;
            set;
        }

        [XmlAttribute("idref")]
        public string IdRef
        {
            get;
            set;
        }

        [XmlAttribute("id")]
        public string Id
        {
            get;
            set;
        }

        [XmlAttribute("type")]
        public string Type
        {
            get;
            set;
        }
    }
}
