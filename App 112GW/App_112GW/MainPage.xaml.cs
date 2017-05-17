using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace App_112GW
{
	public partial class MainPage : ContentPage
	{
		SKSize dimen;
		float aspect;
		float padding = 20;
		Random randy = new Random();
		const string MultimeterLayer = "./Layers";
		List<Multimeter> Devices = new List<Multimeter>();


		private static Color ColorText		= Color.FromRgb(0xAA, 0xAA, 0xAA);
		private static Color BorderColor	= Color.FromRgb(0x33, 0x33, 0x33);
		private static double FontSize		= Device.GetNamedSize(NamedSize.Medium, typeof(Button));
		private static Style ButtonStyle	= new Style(typeof(Button))
		{
			Setters =
			{
				new Setter{ Property = Button.TextColorProperty, Value = ColorText},
				new Setter{ Property = Button.BorderColorProperty, Value = BorderColor},
				new Setter{ Property = Button.FontSizeProperty, Value = FontSize}
			}
		};
		private Button		ButtonAddDevice		= new Button { Text = "Add Device", Style = ButtonStyle};
		private Button		ButtonStartLogging	= new Button { Text = "Start Logging", Style = ButtonStyle };


		private View		RenderView;
		private Grid		UserGrid			= new Grid {HorizontalOptions=LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill, RowSpacing = 1, ColumnSpacing = 1, Padding = 1};

		void InitSurface()
		{
			if (Device.RuntimePlatform == "Windows" && Device.Idiom == TargetIdiom.Phone)
			{
				SKCanvasView canvasView = new SKCanvasView();
				canvasView.PaintSurface += OnCanvasViewPaintSurface;
				RenderView = canvasView;
			}
			else
			{
				SKGLView canvasView = new SKGLView();
				canvasView.PaintSurface += OnGLViewPaintSurface;
				RenderView = canvasView;
			}
			RenderView.SizeChanged	+= GLRescale;

			UserGrid.BackgroundColor = BackgroundColor;
			UserGrid.RowDefinitions.Add(	new RowDefinition		{ Height	= new GridLength(1, GridUnitType.Star)		});
			UserGrid.RowDefinitions.Add(	new RowDefinition		{ Height	= new GridLength(50, GridUnitType.Absolute)	});
			UserGrid.ColumnDefinitions.Add(	new ColumnDefinition	{ Width		= new GridLength(1, GridUnitType.Star)		});
			UserGrid.ColumnDefinitions.Add(	new ColumnDefinition	{ Width		= new GridLength(1, GridUnitType.Star)		});
		
			UserGrid.Children.Add	(RenderView);
			Grid.SetRow				(RenderView, 0);
			Grid.SetColumn			(RenderView, 0);
			Grid.SetRowSpan			(RenderView, 1);
			Grid.SetColumnSpan		(RenderView, 2);

			UserGrid.Children.Add	(ButtonAddDevice,		0, 1);
			UserGrid.Children.Add	(ButtonStartLogging,	1, 1);
			Grid.SetColumnSpan		(ButtonAddDevice,		1);
			Grid.SetColumnSpan		(ButtonStartLogging,	1);
			
			ButtonAddDevice.Clicked		+= AddDevice;
			ButtonStartLogging.Clicked	+= StartLogging;
			
			Content = UserGrid;
		}

		int a = 0;
		public void TapCallback(object sender, EventArgs args)
		{
			if (a % 2 == 0)
				Devices.Last().Clicked();
			else
				Devices.Last().Deselect();

			Devices.Last().SetBargraph(a++);
			Devices.Last().SetLargeSegments(a.ToString());
			Devices.Last().SetSmallSegments((30 - a).ToString());

			if (a > 30)
				a = 0;

			Invalidate();
		}

		public MainPage ()
		{
			InitializeComponent();
			BackgroundColor = Color.FromRgb(38, 38, 38);

			InitSurface();
		}
		
		void GLRescale (object sender, EventArgs args)
		{
			dimen.Width = (float)RenderView.Width;
			dimen.Height = dimen.Width * aspect;

			Invalidate();
		}
		
		void Invalidate()
		{
			if (RenderView is SKCanvasView)
				(RenderView as SKCanvasView).InvalidateSurface();
			else
				(RenderView as SKGLView).InvalidateSurface();
		}

		void PaintSurface(SKSurface pInput)
		{
			if (Devices.Count > 0)
			{
				// then we get the canvas that we can draw on
				var canvas = pInput.Canvas;
				float Y = 0;

				canvas.Clear(BackgroundColor.ToSKColor());
				foreach (Multimeter Device in Devices)
				{
					Device.Render(ref canvas, new SKRect(0, Y, dimen.Width, Y + dimen.Height));
					Y += padding + dimen.Height;
				}
			}
		}
		void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs e)
		{
			PaintSurface(e.Surface);
		}
		void OnGLViewPaintSurface (object sender, SKPaintGLSurfaceEventArgs e)
		{
			PaintSurface(e.Surface);
		}

		void AddDevice(object sender, EventArgs args)
		{
			Devices.Add(new Multimeter(MultimeterLayer));
			Devices.Last().SetLargeSegments("0.000");
			Devices.Last().SetBargraph(0);

			var TapRecogniser = new TapGestureRecognizer();
			TapRecogniser.Tapped += TapCallback;
			Devices.Last().GestureRecognizers.Add(TapRecogniser);
			//UserGrid.GestureRecognizers.Add(TapRecogniser);
			UserGrid.Children.First().GestureRecognizers.Add(TapRecogniser);

			//Calculate ratios
			dimen = Devices.Last().Dimensions().ToSKSize();
			aspect = dimen.Height / dimen.Width;

			//
			Invalidate();
		}
		void StartLogging(object sender, EventArgs args)
		{
			
		}
	}
}
