
using System;
using System.Collections.Generic;
using System.Text;

using Xamarin.Forms;
using rMultiplatform;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.Runtime.CompilerServices;
using App_112GW;
namespace rMultiplatform
{
	public class AutoGrid : Grid
	{
		private void AddView(View pInput, int pX, int pY, int pXSpan = 1, int pYSpan = 1)
		{
			pInput.Margin = 0;
			Children.Add(pInput, pX, pX + pXSpan, pY, pY + pYSpan);
		}
		private void DefineRows(int Count)
		{
			for (var i = 0; i < Count; ++i)
				RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
		}
		private void DefineColumns(int Count)
		{
			for (var i = 0; i < Count; ++i)
				ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
		}
		private int current_row = 0;
		private int current_column = 0;

		public void FormatRow(int x, GridUnitType Format)
		{
			RowDefinitions[x].Height = new GridLength(1, Format);
		}
		public void FormatColumn(int y, GridUnitType Format)
		{
			ColumnDefinitions[y].Width = new GridLength(1, Format);
		}

		public void FormatFirstRow(GridUnitType Format)
		{
			FormatRow(0, Format);
		}
		public void FormatLastRow(GridUnitType Format)
		{
			FormatRow(RowDefinitions.Count - 1, Format);
		}
		public void FormatCurrentRow(GridUnitType Format)
		{
			FormatRow(current_row - 1, Format);
		}
		public void AutoAdd(View Item, int Width = 1)
		{
			if (current_row == RowDefinitions.Count)
				throw new Exception("Adding too many items to multimeter menu.");

			AddView(Item, current_column, current_row, Width);
			current_column += Width;
			if (current_column == ColumnDefinitions.Count)
			{
				current_column = 0;
				current_row++;
			}
		}

		public struct ItemState
		{
			public View Item;
			public int row, column, row_span, column_span;
			public bool visibility;

			public ItemState(View pItem)
			{
				Item			= pItem;

				visibility	  = Item.IsVisible;
				row			 = Grid.GetRow(Item);
				column		  = Grid.GetColumn(Item);
				row_span		= Grid.GetRowSpan(Item);
				column_span	 = Grid.GetColumnSpan(Item);
			}

			public void SetVisibility(View pItem, bool state)
			{
				Type item_type = pItem.GetType();

				if (item_type == typeof(MultimeterScreen))
					(pItem as MultimeterScreen).IsVisible = state;
				else
					pItem.IsVisible = state;
			}
			public void Restore()
			{
				SetVisibility(Item, visibility);
				Grid.SetRow		 (Item, row);
				Grid.SetColumn	  (Item, column);
				Grid.SetRowSpan	 (Item, row_span);
				Grid.SetColumnSpan  (Item, column_span);
			}
		}

		private List<ItemState> RestoreList  = null;
		public void MaximiseItem(View pItem)
		{
			if (RestoreList == null)
			{
				RestoreList = new List<ItemState>();

				foreach (var child in Children)
				{
					var restoreitem = new ItemState(child);
					RestoreList.Add(restoreitem);
					if (!child.Equals(pItem))
						restoreitem.SetVisibility(child, false);
				}
				Grid.SetRow(pItem, 0);
				Grid.SetColumn(pItem, 0);
				Grid.SetRowSpan(pItem, RowDefinitions.Count);
				Grid.SetColumnSpan(pItem, ColumnDefinitions.Count);
			}
		}
		public void RestoreItems()
		{
			if (RestoreList != null)
			{
                foreach (var item in RestoreList)
                    item.Restore();
				RestoreList = null;
			}
		}

		public void DefineGrid(int Width, int Height)
		{
			DefineColumns(Width);
			DefineRows(Height);
		}
		public ContentView MergedCell(int x, int y, int width, int height)
		{
			var view = new ContentView();
			AddView(view, x, y, width, height);
			return view;
		}

		public AutoGrid()
		{
			BackgroundColor = Globals.BackgroundColor;
			HorizontalOptions = LayoutOptions.Fill;
			VerticalOptions = LayoutOptions.Fill;
			Padding = Globals.Padding;
		}
	}
}
