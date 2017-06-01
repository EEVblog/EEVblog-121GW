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
        private Android.Views.View      view;
        private rMultiplatform.Touch    effect;

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
        }

        //Android only, routed args
        void CommonHandler(object sender, Android.Views.View.TouchEventArgs args)
        {
            switch (args.Event.ActionMasked)
            {
                case MotionEventActions.ButtonPress:
                case MotionEventActions.Down:
                case MotionEventActions.PointerDown:
                    effect.PressedHandler(sender, GetPoint(args));
                    break;

                case MotionEventActions.Move:
                case MotionEventActions.HoverMove:
                case MotionEventActions.HoverEnter:
                    effect.MoveHandler(sender, GetPoint(args));
                    break;

                case MotionEventActions.HoverExit:
                case MotionEventActions.PointerUp:
                case MotionEventActions.Up:
                case MotionEventActions.Cancel:
                case MotionEventActions.ButtonRelease:
                    effect.ReleasedHandler(sender, GetPoint(args));
                    break;

                default:
                    break;
            }
        }

        //Shared handler functions
        private Point GetPoint(Android.Views.View.TouchEventArgs args)
        {
            int index = args.Event.ActionIndex;
            MotionEvent.PointerCoords temp = new MotionEvent.PointerCoords();
            args.Event.GetPointerCoords(index, temp);
            return new Point(temp.X, temp.Y);
        }
    }
}
