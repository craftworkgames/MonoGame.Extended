using System;
using System.Linq;

namespace MonoGame.Extended.Gui.Controls
{
    public class GuiUniformGrid : GuiLayoutControl
    {
        public GuiUniformGrid()
            : base(null)
        {
        }

        public GuiUniformGrid(GuiSkin skin) 
            : base(skin)
        {
        }

        public int Columns { get; set; }
        public int Rows { get; set; }

        protected override Size2 CalculateDesiredSize(IGuiContext context, Size2 availableSize)
        {
            var desiredSize = CalculateGridInfo(context, availableSize).MinCellSize;
            return desiredSize;
        }

        public override void Layout(IGuiContext context, RectangleF rectangle)
        {
            var gridInfo = CalculateGridInfo(context, rectangle.Size);
            var columnIndex = 0;
            var rowIndex = 0;
            var cellWidth = HorizontalAlignment == HorizontalAlignment.Stretch ? gridInfo.MaxCellWidth : gridInfo.MinCellWidth;
            var cellHeight = VerticalAlignment == VerticalAlignment.Stretch ? gridInfo.MaxCellHeight : gridInfo.MinCellHeight;

            foreach (var control in Controls)
            {
                var x = columnIndex * cellWidth;
                var y = rowIndex * cellHeight;

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
            var columns = Columns == 0 ? (int)Math.Ceiling(Math.Sqrt(Controls.Count)) : Columns;
            var rows = Rows == 0 ? (int)Math.Ceiling((float)Controls.Count / columns) : Rows;
            var maxCellWidth = availableSize.Width / columns;
            var maxCellHeight = availableSize.Height / rows;
            var sizes = Controls
                .Select(control => GuiLayoutHelper.GetSizeWithMargins(control, context, new Size2(maxCellWidth, maxCellHeight)))
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