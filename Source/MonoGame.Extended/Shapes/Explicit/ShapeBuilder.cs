using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Shapes.Explicit
{
    public class ShapeBuilder : IDisposable
    {
        private Vector3[] _buffer;
        private int _capacity;
        private int _count;

        public IReadOnlyList<Vector3> Buffer
        {
            get { return _buffer; }
        }

        public int Count
        {
            get { return _count; }
        }

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
            _count = 0;
        }

        public void Append(Vector3 point)
        {
            EnsureCapacity(_count + 1);
            _buffer[_count++] = point;
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

        public void CreateCircle(Vector2 position, float radius, int circleSegmentsCount)
        {
            if (circleSegmentsCount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(circleSegmentsCount), circleSegmentsCount, "The circle segments count has to be greater or equal to one.");
            }

            Clear();

            // slabode.exofire.net/circle_draw.shtml

            var theta = MathHelper.TwoPi / circleSegmentsCount;
            var cos = (float)Math.Cos(theta); // Pre-calculate the sine and cosine
            var sin = (float)Math.Sin(theta);
            var x = radius; // Start at angle = 0 
            var y = 0f;

            for (var i = 0; i < circleSegmentsCount; i++)
            {
                Append(new Vector3(x + position.X, y + position.Y, 0));

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