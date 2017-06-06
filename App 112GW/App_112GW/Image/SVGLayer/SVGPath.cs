using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace App_112GW
{
    public class SVGPath
    {
        [XmlAttribute]
        public string id;

        [XmlAttribute]
        public string d;

        [XmlAttribute]
        public string style;

        public override string ToString()
        {
            return "<path style = \""+ style + "\" d = \""+ d + "\" id = \""+ id + "\" />";
        }
    }
}
