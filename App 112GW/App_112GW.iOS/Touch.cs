﻿using System;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Forms;

using CoreGraphics;
using Foundation;
using UIKit;
using Xamarin.Forms.Platform.iOS;

[assembly: ResolutionGroupName("rMultiplatform")]
[assembly: ExportEffect(typeof(rMultiplatform.iOS.Touch), "Touch")]

namespace rMultiplatform.iOS
{
    class TouchRecognizer : UIGestureRecognizer
    {
        Element element;        // Forms element for firing events
        UIView view;            // iOS UIView 
        rMultiplatform.Touch touchEffect;
        bool capture;

        static Dictionary<UIView, TouchRecognizer> viewDictionary =
            new Dictionary<UIView, TouchRecognizer>();

        static Dictionary<long, TouchRecognizer> idToTouchDictionary =
            new Dictionary<long, TouchRecognizer>();

        public TouchRecognizer(Element element, UIView view, rMultiplatform.Touch touchEffect)
        {
            this.element = element;
            this.view = view;
            this.touchEffect = touchEffect;

            viewDictionary.Add(view, this);
        }

        public void Detach()
        {
            viewDictionary.Remove(view);
        }

        // touches = touches of interest; evt = all touches of type UITouch
        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);

            foreach (UITouch touch in touches.Cast<UITouch>())
            {
                long id = touch.Handle.ToInt64();
                FireEvent(this, id, TouchActionType.Pressed, touch);

                idToTouchDictionary.Add(id, this);
            }

            // Save the setting of the Capture property
            capture = touchEffect.Capture;
        }

        public override void TouchesMoved(NSSet touches, UIEvent evt)
        {
            base.TouchesMoved(touches, evt);

            foreach (UITouch touch in touches.Cast<UITouch>())
            {
                long id = touch.Handle.ToInt64();

                if (capture)
                {
                    FireEvent(this, id, TouchActionType.Moved, touch);
                }
                else
                {
                    CheckForBoundaryHop(touch);

                    if (idToTouchDictionary[id] != null)
                    {
                        FireEvent(idToTouchDictionary[id], id, TouchActionType.Moved, touch);
                    }
                }
            }
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            base.TouchesEnded(touches, evt);

            foreach (UITouch touch in touches.Cast<UITouch>())
            {
                long id = touch.Handle.ToInt64();

                if (capture)
                {
                    FireEvent(this, id, TouchActionType.Released, touch);
                }
                else
                {
                    CheckForBoundaryHop(touch);

                    if (idToTouchDictionary[id] != null)
                    {
                        FireEvent(idToTouchDictionary[id], id, TouchActionType.Released, touch);
                    }
                }
                idToTouchDictionary.Remove(id);
            }
        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            base.TouchesCancelled(touches, evt);

            foreach (UITouch touch in touches.Cast<UITouch>())
            {
                long id = touch.Handle.ToInt64();

                if (capture)
                {
                    FireEvent(this, id, TouchActionType.Cancelled, touch);
                }
                else if (idToTouchDictionary[id] != null)
                {
                    FireEvent(idToTouchDictionary[id], id, TouchActionType.Cancelled, touch);
                }
                idToTouchDictionary.Remove(id);
            }
        }

        void CheckForBoundaryHop(UITouch touch)
        {
            long id = touch.Handle.ToInt64();

            // TODO: Might require converting to a List for multiple hits
            TouchRecognizer recognizerHit = null;

            foreach (UIView view in viewDictionary.Keys)
            {
                CGPoint location = touch.LocationInView(view);

                if (new CGRect(new CGPoint(), view.Frame.Size).Contains(location))
                {
                    recognizerHit = viewDictionary[view];
                }
            }
            if (recognizerHit != idToTouchDictionary[id])
            {
                if (idToTouchDictionary[id] != null)
                {
                    FireEvent(idToTouchDictionary[id], id, TouchActionType.Exited, touch);
                }
                if (recognizerHit != null)
                {
                    FireEvent(recognizerHit, id, TouchActionType.Entered, touch);
                }
                idToTouchDictionary[id] = recognizerHit;
            }
        }

        void FireEvent(TouchRecognizer recognizer, long id, TouchActionType actionType, UITouch touch)
        {
            // Convert touch location to Xamarin.Forms Point value
            CGPoint cgPoint = touch.LocationInView(recognizer.View);
            Point xfPoint = new Point(cgPoint.X, cgPoint.Y);

            // Get the method to call for firing events
            Action<Element, TouchActionEventArgs> onTouchAction = recognizer.touchEffect.OnTouchAction;

            // Call that method
            onTouchAction(recognizer.element,
                new rMultiplatform.TouchActionEventArgs(id, actionType, xfPoint));
        }
    }


    class Touch : PlatformEffect
    {
        UIView          view;
        TouchRecognizer touchRecognizer;

        protected override void OnAttached()
        {
            // Get the iOS UIView corresponding to the Element that the effect is attached to
            view = Control == null ? Container : Control;

            // Get access to the TouchEffect class in the PCL
            rMultiplatform.Touch effect = (rMultiplatform.Touch)Element.Effects.FirstOrDefault(e => e is rMultiplatform.Touch);

            if (effect != null && view != null)
            {
                // Create a TouchRecognizer for this UIView
                touchRecognizer = new TouchRecognizer(Element, view, effect);
                view.AddGestureRecognizer(touchRecognizer);
            }
        }

        protected override void OnDetached()
        {
            if (touchRecognizer != null)
            {
                // Clean up the TouchRecognizer object
                touchRecognizer.Detach();

                // Remove the TouchRecognizer from the UIView
                view.RemoveGestureRecognizer(touchRecognizer);
            }
        }
    }
}