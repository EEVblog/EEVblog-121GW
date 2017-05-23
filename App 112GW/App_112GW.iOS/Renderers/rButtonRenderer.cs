using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(rMultiplatform.Button), typeof(rMultiplatform.iOS.Renderers.ButtonRenderer))]
namespace rMultiplatform.iOS.Renderers
{
    class ButtonRenderer : ButtonRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);

            var button = this.Control;
            if (button != null)
            {
            }
        }
    }
}