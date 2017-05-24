using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.ComponentModel;
using Xamarin.Forms.PlatformConfiguration;

namespace rMultiplatform
{
    public class TouchPoint
    {
        public enum eTouchType
        {
            eHover,
            ePressed,
            eReleased
        };

        protected Point       Position;
        public eTouchType     TouchType
        {
            private set;
            get;
        }

        public TouchPoint(Point pInput, eTouchType pTouchType)
        {
            Position = pInput;
            TouchType = pTouchType;
        }
    }
    public static class TouchPointFactory
    {
        public static TouchPoint Hover(Point pInput)
        {
            return new TouchPoint(pInput, TouchPoint.eTouchType.eHover);
        }
        public static TouchPoint Pressed(Point pInput)
        {
            return new TouchPoint(pInput, TouchPoint.eTouchType.ePressed);
        }
        public static TouchPoint Released(Point pInput)
        {
            return new TouchPoint(pInput, TouchPoint.eTouchType.eReleased);
        }
    }

    public class TouchActionEventArgs : EventArgs
    {
        public TouchActionEventArgs(TouchPoint location)
        {
            Location = location;
        }
        public TouchPoint Location
        {
            private set; get;
        }
    }
    class Touch: RoutingEffect
    {
        public delegate void    TouchActionEventHandler(object sender, TouchActionEventArgs args);
        public event            TouchActionEventHandler Press;
        public event            TouchActionEventHandler Release;
        public event            TouchActionEventHandler Hover;

        public bool             Capture { set; get; }

        TouchPoint.eTouchType   prevType;
        public void             OnTouchAction(Element element, TouchActionEventArgs args)
        {
            var type = args.Location.TouchType;
            switch (type)
            {
                case TouchPoint.eTouchType.eHover:
                    if (type != prevType)
                        if (Hover != null)
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                Hover(element, args);
                            });
                    break;
                case TouchPoint.eTouchType.ePressed:
                    if (prevType != TouchPoint.eTouchType.eHover)
                        if (type != prevType)
                            if (Press != null)
                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    Press(element, args);
                                });
                    break;
                case TouchPoint.eTouchType.eReleased:
                    if (type != prevType)
                        if (Release != null)
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                Release(element, args);
                            });
                    break;
                default:
                    break;
            }
            prevType = type;
        }
        public                  Touch() : base("rMultiplatform.Touch")
        {
            prevType = TouchPoint.eTouchType.eReleased;
        }
    }
}
