using System;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

using Android.Views;

[assembly: ResolutionGroupName("rMultiplatform")]
[assembly: ExportEffect(typeof(rMultiplatform.Droid.Touch), "Touch")]

namespace rMultiplatform.Droid
{
    public class Touch : PlatformEffect
    {
        Android.Views.View      view;
        rMultiplatform.Touch    effect;

        protected override void OnAttached()
        {
            // Get the Android View corresponding to the Element that the effect is attached to
            view = Control == null ? Container : Control;

            // Get access to the Touch class in the PCL
            effect = (rMultiplatform.Touch)Element.Effects.FirstOrDefault(e => e is rMultiplatform.Touch);

            //If view is valid add touch event
            if (view != null)
                view.Touch += CommonHandler;
        }
        protected override void OnDetached()
        {
            //view.Touch -= CommonHandler;
        }

        void FinishHandler(object sender, Android.Views.View.TouchEventArgs args)
        {
            effect.OnTouchAction(Element, new TouchActionEventArgs(TouchPointFactory.Released(GetPointer(args))));
        }
        void CommonHandler(object sender, Android.Views.View.TouchEventArgs args)
        {
            // Use ActionMasked here rather than Action to reduce the number of possibilities
            switch (args.Event.ActionMasked)
            {
                case MotionEventActions.ButtonPress:
                case MotionEventActions.Down:
                case MotionEventActions.Move:
                case MotionEventActions.PointerDown:
                    effect.OnTouchAction(Element, new TouchActionEventArgs(TouchPointFactory.Pressed(GetPointer(args))));
                    break;

                case MotionEventActions.HoverMove:
                case MotionEventActions.HoverEnter:
                    //Add support for this
                    effect.OnTouchAction(Element, new TouchActionEventArgs(TouchPointFactory.Hover(GetPointer(args))));
                    break;

                case MotionEventActions.Outside:
                case MotionEventActions.HoverExit:
                case MotionEventActions.PointerUp:
                case MotionEventActions.Up:
                case MotionEventActions.Cancel:
                case MotionEventActions.ButtonRelease:
                    FinishHandler(sender, args);
                    break;

                default:
                    break;
            }
            
        }

        //Android only
        private Point GetPointer(Android.Views.View.TouchEventArgs args)
        {
            int index = args.Event.ActionIndex;
            MotionEvent.PointerCoords temp = new MotionEvent.PointerCoords();
            args.Event.GetPointerCoords(index, temp);
            return new Point(temp.X, temp.Y);
        }
    }
}
