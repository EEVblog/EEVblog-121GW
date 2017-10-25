using System;
using System.Collections;
using System.Text;
using Xamarin.Forms;
using System.Diagnostics;
using App_112GW;
using SkiaSharp;
using Xamarin.Forms.PlatformConfiguration;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Collections.Specialized;

namespace rMultiplatform
{
	class MathChart : AutoGrid
	{
		SmartChart Chart;
		SmartChartMenu Menu = new SmartChartMenu();

		public class EventMonitor<T>
		{
			public event NotifyCollectionChangedEventHandler CollectionChanged;
			IObservableList<T> Data;
			bool AddedEvent;

			private void Data_Changed(object sender, NotifyCollectionChangedEventArgs e)
			{
				CollectionChanged?.Invoke(sender, e);
			}

			public void SetupEvent(IObservableList<T> pData)
			{
				if (pData == null)
					return;

				if (Data != null)
					if (AddedEvent)
						Data.CollectionChanged -= Data_Changed;

				Data = pData;
				Data.CollectionChanged += Data_Changed;
				AddedEvent = true;
			}

			public EventMonitor()
			{
				Data = null;
				AddedEvent = false;
			}
		}


		public string VerticalLabel
		{
			set
			{
				if (Chart != null)
					Chart.Title = value;
			}
		}

		public delegate float Operation(float A, float B);

		public class OperationItem
		{
			public string Label { get; set; }
			public Operation Function { get; set; }
			public Color Color { get; set; }

			public OperationItem(string pLabel, Operation pFunction)
			{
				Label = pLabel;
				Function = pFunction;
				Color = Globals.BackgroundColor;
			}
		}
		static List<OperationItem> Operations = new List<OperationItem>()
		{
			new OperationItem( "A + B",			  (A, B) => A + B					),
			new OperationItem( "A - B",			  (A, B) => A - B					),
			new OperationItem( "A x B",			  (A, B) => A * B					),
			new OperationItem( "A / B",			  (A, B) => A / B					),
			new OperationItem( "A\u00b2 x B",		(A, B) => (float)Math.Sqrt(A) * B  ),
			new OperationItem( "A\u00b2 / B",		(A, B) => (float)Math.Sqrt(A) / B  ),
			new OperationItem( "A / B\u00b2",		(A, B) => A / (float)Math.Sqrt(A)  )
		};

		Picker A_List		 = new Picker();
		Picker B_List		 = new Picker();
		Picker Operation_List = new Picker();

		public IList SourceA
		{
			set		 {   A_List.ItemsSource = value; }
			private get {   return A_List.ItemsSource;  }
		}
		public IList SourceB
		{
			set		 {   B_List.ItemsSource = value;}
			private get {   return B_List.ItemsSource; }
		}

		SKPoint Interpolate(SKPoint A, SKPoint B, float X)
		{
			var mx = (B.X - A.X);
			var my = (B.Y - A.Y);
			var bx = A.X;
			var by = A.Y;

			var dx = (X - A.X);
			var rx = dx / mx;

			var valx = mx * rx + bx;
			var valy = my * rx + by;
			return new SKPoint(valx, valy);
		}

		private bool Fullscreen = true;
		private int i1 = 1, i2 = 1;
		private float x_val, y_val1, y_val2;
		void Resample(List<SKPoint> L1, List<SKPoint> L2)
		{
			if (RenderReady())
			{
				var modified = false;
				var l1_count = L1.Count;
				var l2_count = L2.Count;

				//
				if ((i1 >= l1_count) || (i2 >= l2_count))
					if (!PreparePlot()) return;

				if ((l1_count > 1) && (l2_count > 1))
				{
					while ((i1 < l1_count) && (i2 < l2_count))
					{
						var p1 = L1[i1];
						var p2 = L2[i2];
						var x1 = p1.X;
						var x2 = p2.X;
						var y1 = p1.Y;
						var y2 = p2.Y;

						if (x1 == x2 && y1 == y2)
						{
							if (x1 > x2)
							{
								x_val = x2;
								y_val2 = p2.Y;

								//Interpolate a point for L1
								var P1 = L1[i1 - 1];
								var P2 = L1[i1];
								var pt = Interpolate(P1, P2, x2);
								y_val1 = pt.Y;
								++i2;
							}
							else
							{
								x_val = x1;
								y_val1 = p1.Y;

								//Interpolate a point for L2
								var P1 = L2[i2 - 1];
								var P2 = L2[i2];
								var pt = Interpolate(P1, P2, x1);
								y_val2 = pt.Y;
								++i1;
							}
						}
						else
						{
							x_val = x1;
							y_val1 = y1;
							y_val2 = y2;
							++i1;
							++i2;
						}

						var op_result = Current_Operation(y_val1, y_val2);
						modified = true;
						Data.Add(new SKPoint(x_val, op_result));
					}

					if (modified)   Data.Trigger();
				}
			}
		}

		Mutex mutex = new Mutex();
		TriggerList<SKPoint> Data = new TriggerList<SKPoint>();
		private void Data_Changed(object sender, NotifyCollectionChangedEventArgs e)
		{
			var emp = DeviceA.Logger.Data.ToList();
			var amp = DeviceB.Logger.Data.ToList();
			Resample(emp, amp);
		}
		
		//Returns true only when nothing is null
		private bool RenderReady() => ((DeviceA != null) && (DeviceB != null) && (Current_Operation != null));

		private void AddEvents()
		{
			if (DeviceA != null) DeviceAEvent.SetupEvent(DeviceA.Logger.Data);
			if (DeviceB != null) DeviceBEvent.SetupEvent(DeviceB.Logger.Data);
		}

		private bool PreparePlot()
		{
			if (RenderReady())
			{
				i1 = 1; i2 = 1;
				Data?.Clear();
				AddEvents();
				return true;
			}
			return false;
		}

		Operation Current_Operation = null;

		EventMonitor<SKPoint> DeviceAEvent = new EventMonitor<SKPoint>();
		EventMonitor<SKPoint> DeviceBEvent = new EventMonitor<SKPoint>();
		Multimeter DeviceA = null;
		Multimeter DeviceB = null;

		private void List_ItemSelected(ref Multimeter Device, object sender, EventArgs e)
		{
			var item = (sender as Picker).SelectedItem;
			if (item != null)
			{
				Device = item as Multimeter;
				PreparePlot();
			}
		}
		
		private void Operation_List_ItemSelected(object sender, EventArgs e)
		{
			var sel_item = (sender as Picker).SelectedItem as OperationItem;
			Current_Operation = sel_item.Function;
			VerticalLabel = "(" + sel_item.Label + ")";
			PreparePlot();
		}

		static LayoutOptions ColumnLayout = LayoutOptions.Fill;
		static Picker MakePicker( EventHandler SelectedHandler, string Title, string BindText)
		{
			var output = new Picker();
			output.Title = Title;
			output.VerticalOptions = LayoutOptions.StartAndExpand;
			output.HorizontalOptions = ColumnLayout;
			output.SelectedIndexChanged += SelectedHandler;
			output.ItemDisplayBinding = new Binding(BindText);

			//TODO : This is a bug in xamarin, row height need to be made automatic
			return output;
		}


		public MathChart()
		{
			DeviceAEvent.CollectionChanged += Data_Changed;
			DeviceBEvent.CollectionChanged += Data_Changed;

			//Setup listviews
			Menu = new SmartChartMenu(true, false);
			Menu.SaveClicked += (o, e) => { Chart.SaveCSV(); };

			//
			A_List		  = MakePicker((o, e) => { List_ItemSelected(ref DeviceA, o, e); }, "Device A", "ShortId");
			B_List		  = MakePicker((o, e) => { List_ItemSelected(ref DeviceB, o, e); }, "Device B", "ShortId");
			Operation_List  = MakePicker(Operation_List_ItemSelected, "Operation", "Label");
			Operation_List.ItemsSource = Operations;

			//
			Chart = new SmartChart(
								new SmartData(
									new SmartAxisPair(
										new SmartAxisHorizontal ("Horizontal",  +0, 1),
										new SmartAxisVertical   ("Vertical",	-1, 1)), Data));
			Chart.Clicked += Plot_FullscreenClicked;

			//
			DefineGrid(3, 3);

			//RowDefinition 1
			AutoAdd(A_List);
			AutoAdd(Operation_List);
			AutoAdd(B_List);	FormatCurrentRow(GridUnitType.Auto);
			AutoAdd(Chart, 3);  FormatCurrentRow(GridUnitType.Star);
			AutoAdd(Menu, 3);   FormatCurrentRow(GridUnitType.Auto);
		}
		private void Plot_FullscreenClicked(object sender, EventArgs e)
		{
			if (Fullscreen) MaximiseItem(Chart);
			else			RestoreItems();
			Fullscreen = !Fullscreen;
		}
	}
}