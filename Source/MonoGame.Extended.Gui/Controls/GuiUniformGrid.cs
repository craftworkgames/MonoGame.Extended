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
            return CalculateGridInfo(context, availableSize).Size;
        }

        public override void Layout(IGuiContext context, RectangleF rectangle)
        {
            var cell = CalculateGridInfo(context, rectangle.Size);
            var columnIndex = 0;
            var rowIndex = 0;

            foreach (var control in Controls)
            {
                var x = columnIndex * cell.Width;
                var y = rowIndex * cell.Height;
                PlaceControl(context, control, x, y, cell.Width, cell.Height);
                columnIndex++;

                if (columnIndex > cell.Coloumns - 1)
                {
                    columnIndex = 0;
                    rowIndex++;
                }
            }
        }

        private struct GridInfo
        {
            public float Width;
            public float Height;
            public float Coloumns;
            public float Rows;
            public Size2 Size => new Size2(Width * Coloumns, Height * Rows);
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

            return new GridInfo
            {
                Coloumns = columns,
                Rows = rows,
                Width = HorizontalAlignment == HorizontalAlignment.Stretch ? maxCellWidth : Math.Min(sizes.Max(s => s.Width), maxCellWidth),
                Height = VerticalAlignment == VerticalAlignment.Stretch ? maxCellHeight : Math.Min(sizes.Max(s => s.Height), maxCellHeight)
            };
        }
    }
}