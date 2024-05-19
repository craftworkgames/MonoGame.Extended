using System;
using System.Linq;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Gui.Controls
{
    public class UniformGrid : LayoutControl
    {
        public UniformGrid()
        {
        }

        public int Columns { get; set; }
        public int Rows { get; set; }

        public override Size GetContentSize(IGuiContext context)
        {
            var columns = Columns == 0 ? (int)Math.Ceiling(Math.Sqrt(Items.Count)) : Columns;
            var rows = Rows == 0 ? (int)Math.Ceiling((float)Items.Count / columns) : Rows;
            var sizes = Items
                .Select(control => control.CalculateActualSize(context))
                .ToArray();
            var minCellWidth = sizes.Max(s => s.Width);
            var minCellHeight = sizes.Max(s => s.Height);
            return new Size(minCellWidth * columns, minCellHeight * rows);
        }

        protected override void Layout(IGuiContext context, Rectangle rectangle)
        {
            var gridInfo = CalculateGridInfo(context, rectangle.Size);
            var columnIndex = 0;
            var rowIndex = 0;
            var cellWidth = HorizontalAlignment == HorizontalAlignment.Stretch ? gridInfo.MaxCellWidth : gridInfo.MinCellWidth;
            var cellHeight = VerticalAlignment == VerticalAlignment.Stretch ? gridInfo.MaxCellHeight : gridInfo.MinCellHeight;

            foreach (var control in Items)
            {
                var x = columnIndex * cellWidth + rectangle.X;
                var y = rowIndex * cellHeight + rectangle.Y;

                PlaceControl(context, control, x, y, cellWidth, cellHeight);
                columnIndex++;

                if (columnIndex > gridInfo.Columns - 1)
                {
                    columnIndex = 0;
                    rowIndex++;
                }
            }
        }

        private struct GridInfo
        {
            public float MinCellWidth;
            public float MinCellHeight;
            public float MaxCellWidth;
            public float MaxCellHeight;
            public float Columns;
            public float Rows;
            public Size2 MinCellSize => new Size2(MinCellWidth * Columns, MinCellHeight * Rows);
        }
        
        private GridInfo CalculateGridInfo(IGuiContext context, Size2 availableSize)
        {
            var columns = Columns == 0 ? (int)Math.Ceiling(Math.Sqrt(Items.Count)) : Columns;
            var rows = Rows == 0 ? (int)Math.Ceiling((float)Items.Count / columns) : Rows;
            var maxCellWidth = availableSize.Width / columns;
            var maxCellHeight = availableSize.Height / rows;
            var sizes = Items
                .Select(control => control.CalculateActualSize(context)) // LayoutHelper.GetSizeWithMargins(control, context, new Size2(maxCellWidth, maxCellHeight)))
                .ToArray();
            var maxControlWidth = sizes.Length == 0 ? 0 : sizes.Max(s => s.Width);
            var maxControlHeight = sizes.Length == 0 ? 0 : sizes.Max(s => s.Height);

            return new GridInfo
            {
                Columns = columns,
                Rows = rows,
                MinCellWidth = Math.Min(maxControlWidth, maxCellWidth),
                MinCellHeight = Math.Min(maxControlHeight, maxCellHeight),
                MaxCellWidth = maxCellWidth,
                MaxCellHeight =  maxCellHeight
            };
        }
    }
}