using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Shapes.Explicit
{
    public class ShapeBuilder : IDisposable
    {
        internal const int DefaultCircleSegmentsCount = 32;

        private Vector3[] _buffer;
        private int _capacity;

        public IReadOnlyList<Vector3> Buffer
        {
            get { return _buffer; }
        }

        public int Count { get; private set; }

        public int Capacity
        {
            get { return _capacity; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The capacity has to be greater than or equal to zero.");
                }

                if (value > MaximumCapacity)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The capacity has to be less than or equal to the maximum capacity.");
                }

                if (_capacity == value)
                {
                    return;
                }

                _capacity = value;
                Array.Resize(ref _buffer, _capacity);
            }
        }

        public int MaximumCapacity { get; }

        public ShapeBuilder(int initialCapacity = 4, int maximumCapacity = int.MaxValue)
        {
            if (initialCapacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maximumCapacity), initialCapacity, "The initial capacity has to be greater than or equal to zero.");
            }

            if (maximumCapacity < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maximumCapacity), maximumCapacity, "The maximum capacity has to be greater than or equal to one.");
            }

            if (initialCapacity > maximumCapacity)
            {
                throw new ArgumentOutOfRangeException(nameof(maximumCapacity), initialCapacity, "The initial capacity has to be less than or equal to the maximum capacity.");
            }

            MaximumCapacity = maximumCapacity;
            _buffer = new Vector3[initialCapacity];
        
        }

        public void Clear()
        {
            Count = 0;
        }

        public void Append(Vector3 point)
        {
            EnsureCapacity(Count + 1);
            _buffer[Count++] = point;
        }

        public int EnsureCapacity(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), capacity, "The capacity has to be greater than or equal to zero.");
            }

            if (Capacity < capacity)
            {
                Capacity = capacity;
            }
     
            return Capacity;
        }

        public void AppendArc(Vector2 position, float radius, float startAngle, float endAngle, float depth = 0f, int circleSegmentsCount = DefaultCircleSegmentsCount)
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
                Append(point);

                // Apply the rotation matrix
                var t = x;
                x = cos * x - sin * y;
                y = sin * t + cos * y;
            }
        }

        public void AppendCircle(Vector2 position, float radius, float depth = 0f, int circleSegmentsCount = DefaultCircleSegmentsCount)
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
                Append(point);

                // Apply the rotation matrix
                var t = x;
                x = cos * x - sin * y;
                y = sin * t + cos * y;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _buffer = null;
            }
        }
    }
}