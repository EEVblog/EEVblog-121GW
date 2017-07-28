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

using System.Collections;

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
            set;
            get;
        }

        public TouchPoint(Point pInput, eTouchType pTouchType)
        {
            Position    = pInput;
            TouchType   = pTouchType;
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

    public class TouchPinch
    {
        private Point _PointA, _PointB;
        public Point PointA
        {
            get
            {
                return _PointA;
            }
            private set
            {
                _PointA = value;
            }
        }
        public Point PointB
        {
            get
            {
                return _PointB;
            }
            private set
            {
                _PointB = value;
            }
        }

        public double Angle
        {
            get;
            private set;
        }

        private double _Distance;
        public double Distance
        {
            get { return _Distance; }
        }

        public Point Center
        {
            get
            {
                var cx = (PointA.X + PointB.X) / 2;
                var cy = (PointA.Y + PointB.Y) / 2;

                Debug.WriteLine("cx=" + cx + " cy=" + cy);

                return new Point(cx, cy);
            }
        }
        public double Zoom
        {
            get
            {
                return 1 + (DistanceDelta / Distance) * 0.5;
            }
        }

        public double DistanceDelta;
        public double AngleDelta;

        public bool Set(Point A, Point B)
        {
            PointA = A;
            PointB = B;

            var x1 = A.X;
            var y1 = A.Y;
            var x2 = B.X;
            var y2 = B.Y;

            var deltax = x2 - x1;
            var deltay = y2 - y1;
            var distance = Math.Sqrt(deltax * deltax + deltay * deltay);
            var angle = Math.Atan(deltay / deltax);

            if (distance > 0)
            {
                if (Distance == 0)
                {
                    _Distance = distance;
                    Angle = angle;


                    DistanceDelta = 0;
                    AngleDelta = 0;
                }
                else
                {
                    DistanceDelta = distance - Distance;
                    _Distance = distance;
                    AngleDelta = angle - Angle;
                    Angle = angle;

                    if (DistanceDelta != 0)
                        return true;
                }
            }
            return false;
        }
        public void Clear()
        {
            _Distance = 0;
        }



        public TouchPinch ()
        {}
    }

    public class TouchPinchActionEventArgs : EventArgs
    {
        public TouchPinchActionEventArgs(TouchPinch pPinch)
        {
            Pinch = pPinch;
        }
        public TouchPinch Pinch
        {
            private set; get;
        }
    }

    public class TouchCursor
    {
        private TouchPoint Point;
        
        public Point Position
        {
            get
            {
                return Point.Position;
            }
            set
            {
                var NewVal = value;
                var OldVal = Point.Position;
                var DeltaX = NewVal.X - OldVal.X;
                var DeltaY = NewVal.Y - OldVal.Y;
                _Delta = Math.Sqrt(DeltaX * DeltaX + DeltaY * DeltaY);
                Point.Position = value;
            }
        }
        public TouchPoint.eTouchType TouchType
        {
            get
            {
                return Point.TouchType;
            }
            set
            {
                Point.TouchType = value;
            }
        }
        private double _Delta;
        public double Delta
        {
            get
            {
                return _Delta;   
            }
            set
            {
                _Delta = value;
            }
        }
        
        public TouchCursor (TouchPoint pPoint)
        {
            Point = pPoint;
        }
    }

    public class Touch : RoutingEffect
    {
        public delegate void    TouchActionEventHandler(object sender, TouchActionEventArgs args);
        public delegate void    TouchPinchActionEventHandler(object sender, TouchPinchActionEventArgs args);
        public event            TouchActionEventHandler     Pressed;
        public event            TouchActionEventHandler     Released;
        public event            TouchActionEventHandler     Hover;
        public event            TouchActionEventHandler     PressedMoved;

        public event TouchPinchActionEventHandler Pinch;
        public event TouchActionEventHandler Swipe;

        public double MultitouchThreshold = 5;
        public bool             Capture { set; get; }

        Dictionary<uint, TouchCursor> Cursors;
        TouchPinch PinchProcessor = new TouchPinch();

        //Adds a cursor to the map if it doesn't already exist
        bool PinchMode = false;
        void ProcessCursor(TouchActionEventArgs args)
        {
            if (!Cursors.ContainsKey(args.ID))
                Cursors.Add(args.ID, new TouchCursor(args.Location));
            else
            {
                var item = Cursors[args.ID];
                item.Position = args.Location.Position;
                switch (Cursors.Count)
                {
                    case 1:
                        if (item.Delta > MultitouchThreshold)
                        {

                        }
                        break;
                    case 2:
                        PinchMode = true;
                        if (PinchProcessor.Set(Cursors.ElementAt(0).Value.Position, Cursors.ElementAt(1).Value.Position))
                        {
                            Pinch?.Invoke(this, new TouchPinchActionEventArgs(PinchProcessor));
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        void RemoveCursor(TouchActionEventArgs args)
        {
            PinchProcessor.Clear();
            Cursors.Remove(args.ID);
        }


        //Removes a cursor from the map


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
           // ProcessCursor(args);
            SafeEvent(Hover, element, args);
        }
        private void SafePressed(Element element, TouchActionEventArgs args)
        {
            ProcessCursor(args);
            SafeEvent(Pressed, element, args);
        }
        private void SafePressedMove(Element element, TouchActionEventArgs args)
        {
            ProcessCursor(args);
            SafeEvent(PressedMoved, element, args);
        }
        private void SafeReleased(Element element, TouchActionEventArgs args)
        {
            if (Cursors.Count == 1 && !PinchMode)
                SafeEvent(Released, element, args);

            RemoveCursor(args);
            if (Cursors.Count == 0)
                PinchMode = false;
        }

        //Processes platform agnostic input
        private TouchPoint.eTouchType PreviousType;
        public void OnTouchAction(Element e, TouchActionEventArgs a)
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

        //Common handlers
        public void RaiseAction(TouchActionEventArgs Action)
        {
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

        //Initialise class and base
        public                  Touch() : base("rMultiplatform.Touch")
        {
            PreviousType = TouchPoint.eTouchType.eReleased;
            Cursors = new Dictionary<uint, TouchCursor>();
        }
    }
}
