using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace rMultiplatform
{
    public enum TouchActionType
    {
        Entered,
        Pressed,
        Moved,
        Released,
        Exited,
        Cancelled
    }
    public class TouchActionEventArgs : EventArgs
    {
        public TouchActionEventArgs(long id, TouchActionType type, Point location)
        {
            Id = id;
            Type = type;
            Location = location;
        }

        public long             Id { private set; get; }

        public TouchActionType  Type { private set; get; }

        public Point            Location { private set; get; }
    }
    class Touch: RoutingEffect
    {
        public delegate void    TouchActionEventHandler(object sender, TouchActionEventArgs args);
        public event            TouchActionEventHandler TouchAction;

        public bool             Capture { set; get; }
        public void             OnTouchAction(Element element, TouchActionEventArgs args)
        {
            TouchAction?.Invoke(element, args);
        }

        public Touch() : base("rMultiplatform.Touch")
        {
        }
    }
}
