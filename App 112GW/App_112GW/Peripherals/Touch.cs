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
using Xamarin.Forms.Platform;
using System.Diagnostics;

namespace rMultiplatform
{
    public class TouchPoint
    {
        public enum eTouchType
        {
            eMoved,
            eHover,
            ePressed,
            eReleased
        };

        public Point            Position;
        public eTouchType       TouchType
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
        public static TouchPoint Moved(Point pInput)
        {
            return new TouchPoint(pInput, TouchPoint.eTouchType.eMoved);
        }
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
        public TouchActionEventArgs(TouchPoint pLocation, uint pID)
        {
            Location = pLocation;
            ID = pID;
        }
        public uint ID
        {
            private set; get;
        }
        public TouchPoint Location
        {
            private set; get;
        }
    }
    public class Touch : RoutingEffect
    {
        public delegate void    TouchActionEventHandler(object sender, TouchActionEventArgs args);
        public event            TouchActionEventHandler     Pressed;
        public event            TouchActionEventHandler     Released;
        public event            TouchActionEventHandler     Hover;
        public event            TouchActionEventHandler     PressedMoved;

        public bool             Capture { set; get; }

        
        //Triggers a threadsafe event
        private void SafeEvent(TouchActionEventHandler EventFunction, Element element, TouchActionEventArgs args)
        {
            if (EventFunction != null)
                Device.BeginInvokeOnMainThread(() =>
                {
                    EventFunction(element, args);
                });
        }

        //Calls the safe event function for the event
        private void SafeHover(Element element, TouchActionEventArgs args)
        {
            SafeEvent(Hover, element, args);
        }
        private void SafePressed(Element element, TouchActionEventArgs args)
        {
            SafeEvent(Pressed, element, args);
        }
        private void SafeReleased(Element element, TouchActionEventArgs args)
        {
            SafeEvent(Released, element, args);
        }
        private void SafePressedMove(Element element, TouchActionEventArgs args)
        {
            SafeEvent(PressedMoved, element, args);
        }

        //Processes platform agnostic input
        private TouchPoint.eTouchType PreviousType;
        public void             OnTouchAction(Element e, TouchActionEventArgs a)
        {
            var type = a.Location.TouchType;
            var change = type != PreviousType;
            switch (type)
            {
                case TouchPoint.eTouchType.eMoved:
                    switch (PreviousType)
                    {
                        case TouchPoint.eTouchType.eReleased:
                            SafeHover(e, a);
                            break;
                        case TouchPoint.eTouchType.ePressed:
                            SafePressedMove(e, a);
                            break;
                        default:
                            throw (new Exception("Cannot determine previous state. It must be pressed or released"));
                    }
                    break;
                case TouchPoint.eTouchType.ePressed:
                    if (change)
                        SafePressed(e, a);
                    break;
                case TouchPoint.eTouchType.eReleased:
                    SafeReleased(e, a);
                    break;
                default:
                    throw (new Exception("Do not issue commands other than pressed, released and move from a device."));
            }

            //Moved is not a state change
            if (type != TouchPoint.eTouchType.eMoved)
                PreviousType = type;
        }

        //Initialise class and base
        public                  Touch() : base("rMultiplatform.Touch")
        {
            PreviousType = TouchPoint.eTouchType.eReleased;
        }

        //Common handlers
        public void RaiseAction(TouchActionEventArgs Action)
        {
            Debug.WriteLine(Action.Location.TouchType.ToString() + " event detected from " + Action.ID.ToString() + " at " + Action.Location.Position + ".");
            OnTouchAction(Element, Action);
        }
        public void ReleasedHandler(object sender, Point pt, uint ID)
        {
            RaiseAction(new TouchActionEventArgs(TouchPointFactory.Released(pt), ID));
        }
        public void MoveHandler(object sender, Point pt, uint ID)
        {
            RaiseAction(new TouchActionEventArgs(TouchPointFactory.Moved(pt), ID));
        }
        public void PressedHandler(object sender, Point pt, uint ID)
        {
            RaiseAction(new TouchActionEventArgs(TouchPointFactory.Pressed(pt), ID));
        }
    }
}
