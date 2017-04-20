using System;
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

        public override void Layout(RectangleF rectangle)
        {
            var columns = Columns == 0 ? Math.Ceiling(Math.Sqrt(Controls.Count)) : Columns;
            var rows = Rows == 0 ? columns : Rows;
            var cellWidth = (float)(rectangle.Width / columns);
            var cellHeight = (float)(rectangle.Height / rows);
            var columnIndex = 0;
            var rowIndex = 0;

            foreach (var control in Controls)
            {
                var x = columnIndex * cellWidth;
                var y = rowIndex * cellHeight;
                PlaceControl(control, x, y, cellWidth, cellHeight);
                columnIndex++;

                if (columnIndex > columns - 1)
                {
                    columnIndex = 0;
                    rowIndex++;
                }
            }
        }
    }
}