using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Shapes.Explicit
{
    public static class ShapeBuilder
    {
        public delegate void PointDelegate(ref Vector3 point);

        internal const int DefaultCircleSegmentsCount = 32;

        public static void CreateArc(PointDelegate result, Vector2 position, float radius, float startAngle, float endAngle, float depth = 0f, int circleSegmentsCount = DefaultCircleSegmentsCount)
        {
            // www.slabode.exofire.net/circle_draw.shtml

            if (startAngle > MathHelper.TwoPi)
            {
                startAngle = startAngle % MathHelper.TwoPi;
            }

            if (endAngle > MathHelper.TwoPi)
            {
                endAngle = endAngle % MathHelper.TwoPi;
            }

            var theta = endAngle / (circleSegmentsCount - 1); // The - 1 bit comes from the fact that the arc is open
            var cos = (float)Math.Cos(theta); // Pre-calculate the sine and cosine
            var sin = (float)Math.Sin(theta);
            var x = radius * (float)Math.Cos(startAngle);
            var y = 0f;

            for (var i = 0; i < circleSegmentsCount; i++)
            {
                var point = new Vector3(x + position.X, y + position.Y, depth);
                result(ref point);

                // Apply the rotation matrix
                var t = x;
                x = cos * x - sin * y;
                y = sin * t + cos * y;
            }
        }

        public static void CreateCircle(PointDelegate result, Vector2 position, float radius, float depth = 0f, int circleSegmentsCount = DefaultCircleSegmentsCount)
        {
            // www.slabode.exofire.net/circle_draw.shtml

            var theta = MathHelper.TwoPi / circleSegmentsCount;
            var cos = (float)Math.Cos(theta); // Pre-calculate the sine and cosine
            var sin = (float)Math.Sin(theta);
            var x = radius; // Start at angle = 0 
            var y = 0f;

            for (var i = 0; i < circleSegmentsCount; i++)
            {
                var point = new Vector3(x + position.X, y + position.Y, depth);
                result(ref point);

                // Apply the rotation matrix
                var t = x;
                x = cos * x - sin * y;
                y = sin * t + cos * y;
            }
        }

        public static void CreateRectangleFromCenter(PointDelegate result, Vector2 position, SizeF size, float rotation, float depth = 0f)
        {
            var sin = (float)Math.Sin(rotation);
            var cos = (float)Math.Cos(rotation);
            // ReSharper disable once UseObjectOrCollectionInitializer
            var point = new Vector3(0, 0, depth);
            var halfSize = size * 0.5f;

            var rx = -halfSize.Width;
            var ry = -halfSize.Height;
            point.X = position.X + rx * cos - ry * sin;
            point.Y = position.Y + rx * sin + ry * cos;
            result(ref point);

            rx = halfSize.Width;
            ry = -halfSize.Height;
            point.X = position.X + rx * cos - ry * sin;
            point.Y = position.Y + rx * sin + ry * cos;
            result(ref point);

            rx = -halfSize.Width;
            ry = halfSize.Height;
            point.X = position.X + rx * cos - ry * sin;
            point.Y = position.Y + rx * sin + ry * cos;
            result(ref point);

            rx = halfSize.Width;
            ry = halfSize.Height;
            point.X = position.X + rx * cos - ry * sin;
            point.Y = position.Y + rx * sin + ry * cos;
            result(ref point);
        }

        public static void CreateRectangleFromTopLeft(PointDelegate result, Vector2 position, SizeF size, float rotation, Vector2? origin = null, float depth = 0f)
        {
            var origin1 = origin ?? Vector2.Zero;
            var sin = (float)Math.Sin(rotation);
            var cos = (float)Math.Cos(rotation);
            // ReSharper disable once UseObjectOrCollectionInitializer
            var point = new Vector3(0, 0, depth);

            var rx = -origin1.X;
            var ry = -origin1.Y;
            point.X = position.X + rx * cos - ry * sin;
            point.Y = position.Y + rx * sin + ry * cos;
            result(ref point);

            rx = -origin1.X + size.Width;
            ry = -origin1.Y;
            point.X = position.X + rx * cos - ry * sin;
            point.Y = position.Y + rx * sin + ry * cos;
            result(ref point);

            rx = -origin1.X;
            ry = -origin1.Y + size.Height;
            point.X = position.X + rx * cos - ry * sin;
            point.Y = position.Y + rx * sin + ry * cos;
            result(ref point);

            rx = -origin1.X + size.Width;
            ry = -origin1.Y + size.Height;
            point.X = position.X + rx * cos - ry * sin;
            point.Y = position.Y + rx * sin + ry * cos;
            result(ref point);
        }
    }
}