using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Shapes.Explicit
{
    public static class ExplicitShapeBuilder
    {
        public delegate void OutputPointDelegate(ref Vector3 point);

        public const int DefaultCircleSegmentsCount = 64;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void EnsureOutputPoint(OutputPointDelegate outputPoint)
        {
            if (outputPoint == null)
            {
                throw new ArgumentNullException(nameof(outputPoint));
            }
        }

        public static void BuildArc(OutputPointDelegate outputPoint, ref ArcF arc, float depth = 0f, int segmentsCount = DefaultCircleSegmentsCount)
        {
            EnsureOutputPoint(outputPoint);

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
                outputPoint(ref point);

                // Apply the rotation matrix
                var t = x;
                x = cos * x - sin * y;
                y = sin * t + cos * y;
            }
        }

        public static void BuildCircle(OutputPointDelegate outputPoint, ref CircleF circle, float depth = 0f, int circleSegmentsCount = DefaultCircleSegmentsCount)
        {
            EnsureOutputPoint(outputPoint);

            // www.slabode.exofire.net/circle_draw.shtml

            var theta = MathHelper.TwoPi / circleSegmentsCount;
            var cos = (float)Math.Cos(theta); // Pre-calculate the sine and cosine
            var sin = (float)Math.Sin(theta);
            var x = circle.Radius; // Start at angle = 0 
            var y = 0f;

            var position = circle.Center;

            for (var i = 0; i < circleSegmentsCount; i++)
            {
                var point = new Vector3(x + position.X, y + position.Y, depth);
                outputPoint(ref point);

                // Apply the rotation matrix
                var t = x;
                x = cos * x - sin * y;
                y = sin * t + cos * y;
            }
        }

        public static void BuildEllipse(OutputPointDelegate outputPoint, Vector2 position, Vector2 radius, float depth = 0f, int circleSegmentsCount = DefaultCircleSegmentsCount)
        {
            EnsureOutputPoint(outputPoint);
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
                outputPoint(ref point);

                // Apply the rotation matrix
                var t = x;
                x = cos * x * sx - sin * y * sy;
                y = sin * t * sy + cos * y * sx;
            }
        }

        public static void BuildRectangle(OutputPointDelegate outputPoint, ref RectangleF rectangle, float rotation = 0f, Vector2? origin = null, float depth = 0f)
        {
            EnsureOutputPoint(outputPoint);

            var origin1 = origin ?? Vector2.Zero;
            var sin = (float)Math.Sin(rotation);
            var cos = (float)Math.Cos(rotation);
            // ReSharper disable once UseObjectOrCollectionInitializer
            var point = new Vector3(0, 0, depth);
            var size = rectangle.Size;

            var rx = -origin1.X;
            var ry = -origin1.Y;
            point.X = rectangle.X + rx * cos - ry * sin;
            point.Y = rectangle.Y + rx * sin + ry * cos;
            outputPoint(ref point);

            rx = -origin1.X + size.Width;
            ry = -origin1.Y;
            point.X = rectangle.X + rx * cos - ry * sin;
            point.Y = rectangle.Y + rx * sin + ry * cos;
            outputPoint(ref point);

            rx = -origin1.X;
            ry = -origin1.Y + size.Height;
            point.X = rectangle.X + rx * cos - ry * sin;
            point.Y = rectangle.Y + rx * sin + ry * cos;
            outputPoint(ref point);

            rx = -origin1.X + size.Width;
            ry = -origin1.Y + size.Height;
            point.X = rectangle.X + rx * cos - ry * sin;
            point.Y = rectangle.Y + rx * sin + ry * cos;
            outputPoint(ref point);
        }
    }
}