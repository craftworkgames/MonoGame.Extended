using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGame.Extended
{
    public class SimpleSimProjection
    {
        private Matrix _simProjection;

        public Matrix SimProjection
        {
            get
            {
                return _simProjection;
            }
        }
        
        public SimpleSimProjection(GraphicsDeviceManager graphics, bool reverseWidth = false, bool reverseHeight = false) :
            this(graphics, ConvertSimUnits.DisplayToSimRatio, reverseWidth, reverseHeight)
        {
        }
        
        public SimpleSimProjection(GraphicsDeviceManager graphics, float ratio, bool reverseWidth = false, bool reverseHeight = false)
        {
            float width = graphics.GraphicsDevice.Viewport.Width * ratio;
            float height = graphics.GraphicsDevice.Viewport.Height * ratio;
            width = reverseWidth ? -width : width;
            height = reverseHeight ? -height : height;

            _simProjection = Matrix.CreateOrthographic(width, height, 0, 1);
        }

        public void MoveProjection(float directionX, float directionY)
        {
            MoveProjection(new Vector2(directionX, directionY), ConvertSimUnits.DisplayToSimRatio);
        }

        public void MoveProjection(float directionX, float directionY, float ratio)
        {
            MoveProjection(new Vector2(directionX, directionY), ratio);
        }

        public void MoveProjection(Vector2 direction)
        {
            MoveProjection(direction, ConvertSimUnits.DisplayToSimRatio);
        }

        public void MoveProjection(Vector2 direction, float ratio)
        {
            direction *= ratio;
            _simProjection = Matrix.CreateTranslation(direction.X, direction.Y, 0) * _simProjection;
        }
    }
}
