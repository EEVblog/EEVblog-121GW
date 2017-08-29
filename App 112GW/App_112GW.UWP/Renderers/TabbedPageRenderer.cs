using rMultiplatform;
using rMultiplatform.BLE;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.PlatformConfiguration.WindowsSpecific;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(Xamarin.Forms.TabbedPage), typeof(CustomTabbedPageRenderer))]
namespace rMultiplatform
{
    public class CustomTabbedPageRenderer : TabbedPageRenderer
    {

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            if (Control != null)
                Control.Loaded += Control_Loaded;

            if (e.OldElement == null)
                return;

            // Unhook when disposing control
            if (Control != null)
                Control.Loaded -= Control_Loaded;
        }

        private void Control_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            // Disable UWP swipe gesture on tabbled page
            if (Control.ItemsPanelRoot != null)
                Control.ItemsPanelRoot.ManipulationMode = Windows.UI.Xaml.Input.ManipulationModes.None;
        }
    }
}