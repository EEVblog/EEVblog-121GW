using System;
using System.Linq;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

[assembly: ResolutionGroupName("rMultiplatform")]
[assembly: ExportEffect(typeof(rMultiplatform.UWP.Touch), "Touch")]

namespace rMultiplatform.UWP
{
    class Touch : PlatformEffect
    {
        FrameworkElement        view;
        rMultiplatform.Touch    effect;

        protected override void OnAttached()
        {
            // Get the Windows FrameworkElement corresponding to the Element that the effect is attached to
            view = Control == null ? Container : Control;

            // Get access to the TouchEffect class in the PCL
            effect = (rMultiplatform.Touch)Element.Effects.FirstOrDefault(e => e is rMultiplatform.Touch);

            if (effect != null && view != null)
            {
                // Set event handlers on FrameworkElement
                //view.PointerEntered     += CommonHandler;
                view.PointerPressed     += CommonHandler;
                //view.PointerMoved       += CommonHandler;
                view.PointerReleased    += FinishHandler;
                view.PointerExited      += FinishHandler;
                //view.PointerCanceled    += FinishHandler;
            }
        }
        protected override void OnDetached()
        {
            // Release event handlers on FrameworkElement
            view.PointerEntered     -= CommonHandler;
            view.PointerPressed     -= CommonHandler;
            view.PointerMoved       -= CommonHandler;

            view.PointerReleased    -= FinishHandler;
            view.PointerExited      -= FinishHandler;
            view.PointerCanceled    -= FinishHandler;
        }

        void FinishHandler(object sender, PointerRoutedEventArgs args)
        {
            var pp = args.GetCurrentPoint(sender as UIElement).Position;
            var point = new Point(pp.X, pp.Y);
            effect.OnTouchAction(Element, new TouchActionEventArgs(TouchPointFactory.Released(point)));
        }
        void CommonHandler(object sender, PointerRoutedEventArgs args)
        {
            var pp = args.GetCurrentPoint(sender as UIElement).Position;
            var point = new Point(pp.X, pp.Y);
            effect.OnTouchAction(Element, new TouchActionEventArgs(TouchPointFactory.Pressed(point)));
        }
    }
}
