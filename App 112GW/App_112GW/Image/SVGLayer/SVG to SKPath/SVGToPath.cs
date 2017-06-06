using System.Reflection;
using System.IO;
using SkiaSharp;
using System.Xml;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;

using Xamarin;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml.Internals;
using System.Xml.Serialization;

namespace App_112GW
{

    public class SVGToPath
    {
        List<SVGPath> Paths;
        bool ProcessSVG(string Name, Stream Stream)
        {
            using (var reader = new System.IO.StreamReader(Stream))
            {
                var serializer = new XmlSerializer(typeof(List<SVGPath>));
                Paths = (List<SVGPath>)serializer.Deserialize(reader);
            }
            return true;
        }


        enum Coordinate
        {
            Absolute,
            Relative
        }
        public  SVGToPath(string pInput)
        {
            var loader = new GeneralLoader(ProcessSVG, "svg");
        }
    }
}
