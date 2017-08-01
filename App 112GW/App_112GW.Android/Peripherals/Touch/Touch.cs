using System;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

using Android.Views;
using System.Diagnostics;

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
            var evnt = args.Event;
            var actn = args.Event.Action;
            // Get the pointer index
            int pointer_count = evnt.PointerCount;

            // Get the id to identify a finger over the course of its progress
            switch (actn & MotionEventActions.Mask)
            {
                case MotionEventActions.Down:
                case MotionEventActions.PointerDown:
                    for (var i = 0; i < pointer_count; ++i)
                        effect.PressedHandler(sender, GetPoint(args, i), (uint)i);
                    break;
                case MotionEventActions.Move:
                case MotionEventActions.HoverMove:
                case MotionEventActions.HoverEnter:
                    for (var i = 0; i < pointer_count; ++i)
                        effect.MoveHandler(sender, GetPoint(args, i), (uint)i);
                    break;
                case MotionEventActions.HoverExit:
                case MotionEventActions.PointerUp:
                case MotionEventActions.Up:
                case MotionEventActions.Cancel:
                    for (var i = 0; i < pointer_count; ++i)
                        effect.ReleasedHandler(sender, GetPoint(args, i), (uint)i);
                    break;
            }
        }

        //Shared handler functions
        private Point GetPoint(Android.Views.View.TouchEventArgs args, int index)
        {
            MotionEvent.PointerCoords temp = new MotionEvent.PointerCoords();
            args.Event.GetPointerCoords(index, temp);
            return new Point(temp.X, temp.Y);
        }
    }
}
