using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Shapes.Explicit
{
    public static class ShapeBuilder
    {
        public delegate void PointDelegate(ref Vector3 point);

        internal const int DefaultSegmentsCount = 32;

        public static void Arc(PointDelegate result, ref ArcF arc, float depth = 0f, int segmentsCount = DefaultSegmentsCount)
        {
            // www.slabode.exofire.net/circle_draw.shtml

            var startAngle = arc.StartAngle;
            var endAngle = arc.EndAngle;

            if (startAngle > MathHelper.TwoPi)
            {
                startAngle = startAngle % MathHelper.TwoPi;
            }

            if (endAngle > MathHelper.TwoPi)
            {
                endAngle = endAngle % MathHelper.TwoPi;
            }

            var theta = endAngle / (segmentsCount - 1); // The - 1 bit comes from the fact that the arc is open
            var cos = (float)Math.Cos(theta); // Pre-calculate the sine and cosine
            var sin = (float)Math.Sin(theta);
            var x = arc.Radius.Width * (float)Math.Cos(startAngle);
            var y = 0f;
            var sx = arc.Radius.Width / arc.Radius.Height;
            var sy = arc.Radius.Height / arc.Radius.Width;

            var point = new Vector3(0, 0, depth);
            var centre = arc.Centre;

            for (var i = 0; i < segmentsCount; i++)
            {
                point.X = sx * x + centre.X;
                point.Y = sy * y + centre.Y;
                result(ref point);

                // Apply the rotation matrix
                var t = x;
                x = cos * x - sin * y;
                y = sin * t + cos * y;
            }
        }

        public static void CreateCircle(PointDelegate result, Vector2 position, float radius, float depth = 0f, int circleSegmentsCount = DefaultSegmentsCount)
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

        public static void CreateEllipse(PointDelegate result, Vector2 position, Vector2 radius, float depth = 0f, int circleSegmentsCount = DefaultSegmentsCount)
        {
            // www.slabode.exofire.net/circle_draw.shtml

            var theta = MathHelper.TwoPi / circleSegmentsCount;
            var cos = (float)Math.Cos(theta); // Pre-calculate the sine and cosine
            var sin = (float)Math.Sin(theta);
            var x = radius.X; // Start at angle = 0 
            var y = 0f;
            var sx = radius.X / radius.Y;
            var sy = radius.Y / radius.X;

            for (var i = 0; i < circleSegmentsCount; i++)
            {
                var point = new Vector3(x + position.X, y + position.Y, depth);
                result(ref point);

                // Apply the rotation matrix
                var t = x;
                x = cos * x * sx - sin * y * sy;
                y = sin * t * sy + cos * y * sx;
            }
        }

        public static void Rectangle(PointDelegate result, Point2F position, SizeF size, float depth = 0f)
        {
            var point = new Vector3(0, 0, depth);
            var halfSize = size * 0.5f;

            point.X = position.X - halfSize.Width;
            point.Y = position.Y - halfSize.Height;
            result(ref point);

            point.X = position.X + halfSize.Width;
            point.Y = position.Y - halfSize.Height;
            result(ref point);

            point.X = position.X - halfSize.Width;
            point.Y = position.Y + halfSize.Height;
            result(ref point);

            point.X = position.X + halfSize.Width;
            point.Y = position.Y + halfSize.Height;
            result(ref point);
        }
    }
}