
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
    }
}
