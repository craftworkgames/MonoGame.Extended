using System;
using System.Linq;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Gui.Controls
{
    public class GuiUniformGrid : GuiLayoutControl
    {
        public GuiUniformGrid()
            : base(null)
        {
        }

        public GuiUniformGrid(TextureRegion2D backgroundRegion)
            : base(backgroundRegion)
        {
        }

        public int Columns { get; set; }
        public int Rows { get; set; }

        protected override Size2 CalculateDesiredSize(IGuiContext context, Size2 availableSize)
        {
            var columns = CalculateColumns();
            var rows = CalculateRows(columns, Controls.Count);
            var maxCellWidth = availableSize.Width / columns;
            var maxCellHeight = availableSize.Height / rows;
            var sizes = Controls
                .Select(control => GuiLayoutHelper.GetSizeWithMargins(control, context, new Size2(maxCellWidth, maxCellHeight)))
                .ToArray();
            var cellWidth = Math.Min(sizes.Max(s => s.Width), maxCellWidth);
            var cellHeight = Math.Min(sizes.Max(s => s.Height), maxCellHeight);

            return new Size2(cellWidth * columns, cellHeight * rows);
        }

        public override void Layout(IGuiContext context, RectangleF rectangle)
        {
            var columns = CalculateColumns();
            var rows = CalculateRows(columns, Controls.Count);
            var cellWidth = (float)(rectangle.Width / columns);
            var cellHeight = (float)(rectangle.Height / rows);
            var columnIndex = 0;
            var rowIndex = 0;

            foreach (var control in Controls)
            {
                var x = columnIndex * cellWidth;
                var y = rowIndex * cellHeight;
                PlaceControl(context, control, x, y, cellWidth, cellHeight);
                columnIndex++;

                if (columnIndex > columns - 1)
                {
                    columnIndex = 0;
                    rowIndex++;
                }
            }
        }

        private int CalculateRows(int columns, int controlCount)
        {
            return Rows == 0 ? (int)Math.Ceiling((float)controlCount / columns) : Rows;
        }

        private int CalculateColumns()
        {
            return Columns == 0 ? (int)Math.Ceiling(Math.Sqrt(Controls.Count)) : Columns;
        }
    }
}