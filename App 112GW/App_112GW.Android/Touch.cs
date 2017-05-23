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
        Android.Views.View view;
        Element formsElement;
        rMultiplatform.Touch pclTouch;

        protected override void OnAttached()
        {
            // Get the Android View corresponding to the Element that the effect is attached to
            if (Control == null)
            {
                if (Container == null)
                    return;
                else
                    view = Container;
            }
            else            view = Control;

            // Get access to the Touch class in the PCL
            rMultiplatform.Touch Touch = (rMultiplatform.Touch)Element.Effects.FirstOrDefault(e => e is rMultiplatform.Touch);

            if (Touch != null && view != null)
            {
                formsElement = Element;
                pclTouch = Touch;

                // Set event handler on View
                view.Touch += OnTouch;
            }
        }
        protected override void OnDetached()
        {
            
        }

        Point GetPointer(Android.Views.View.TouchEventArgs args)
        {
            int index = args.Event.ActionIndex;
            MotionEvent.PointerCoords temp = new MotionEvent.PointerCoords();
            args.Event.GetPointerCoords(index, temp);
            return new Point(temp.X, temp.Y);
        }


        void OnTouch(object sender, Android.Views.View.TouchEventArgs args)
        {
            // Get the id that identifies a finger over the course of its progress
            int id = args.Event.GetPointerId(args.Event.ActionIndex);

            // Use ActionMasked here rather than Action to reduce the number of possibilities
            switch (args.Event.ActionMasked)
            {
                case MotionEventActions.Down:
                case MotionEventActions.PointerDown:
                    pclTouch.OnTouchAction(formsElement, new TouchActionEventArgs(id, TouchActionType.Pressed, GetPointer(args)));
                    break;
            }
        }
    }
}
