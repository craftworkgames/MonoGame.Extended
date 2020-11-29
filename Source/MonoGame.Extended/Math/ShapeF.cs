using System;
using System.Collections;
using System.Collections.Generic;

namespace MonoGame.Extended
{
    /// <summary>
    ///     Base class for shapes.
    /// </summary>
    /// <remakarks>
    ///     Created to allow checking intersection between shapes of different types.
    /// </remakarks>
    public interface IShapeF
    {
        /// <summary>
        /// Gets or sets the position of the shape.
        /// </summary>
        Point2 Position { get; set; }
    }


    public readonly struct ShapeCollectionContent<T>
    {
        private readonly List<T> _shapes;

        public ShapeCollectionContent(List<T> shapes)
        {
            _shapes = shapes;
        }


        public List<T>.Enumerator GetEnumerator() => _shapes?.GetEnumerator() ?? default;

        public T this[int index] { get => _shapes != null ? _shapes[index] : throw new IndexOutOfRangeException(); }

        public int Count => _shapes?.Count ?? 0;


        public bool Contains(T item) => _shapes?.Contains(item) ?? false;

        public void CopyTo(T[] array, int arrayIndex) => _shapes?.CopyTo(array, arrayIndex);


        public int IndexOf(T item) => _shapes?.IndexOf(item) ?? -1;

    }
    public readonly struct ShapeAssoativeListContent<TKey, TShape>
        where TShape : struct, IShapeF
    {


        private readonly Dictionary<TKey, TShape> _shapes;

        public ShapeAssoativeListContent(Dictionary<TKey, TShape> shapes)
        {
            _shapes = shapes;
        }


        public Dictionary<TKey, TShape>.Enumerator GetEnumerator() => _shapes?.GetEnumerator() ?? default;

        public Dictionary<TKey, TShape>.KeyCollection Keys => _shapes.Keys;

        public TShape this[TKey index] { get => _shapes != null ? _shapes[index] : throw new IndexOutOfRangeException(); }

        public int Count => _shapes?.Count ?? 0;

        public bool Contains(TKey item) => _shapes?.ContainsKey(item) ?? false;

    }

    public class ShapeCollection
    {
        public const int DEFAULT_CAPACITY = 128;
        private readonly List<RectangleF> _recList;
        private readonly List<CircleF> _circleList;


        public ShapeCollection(int capacity) : this(capacity, capacity)
        { }
        public ShapeCollection(int capacityRect = DEFAULT_CAPACITY, int capacityCircle = DEFAULT_CAPACITY)
        {
            _recList = new List<RectangleF>(capacityRect);
            _circleList = new List<CircleF>(capacityCircle);
        }

        public ShapeCollectionContent<RectangleF> Rectangles => new ShapeCollectionContent<RectangleF>(_recList);
        public ShapeCollectionContent<CircleF> Circles => new ShapeCollectionContent<CircleF>(_circleList);

        public void Add<TShape>(TShape shape)
            where TShape : struct, IShapeF
        {
            if (shape is RectangleF rec)
                _recList.Add(rec);
            else if (shape is CircleF circle)
                _circleList.Add(circle);
            else
                throw new NotSupportedException();
        }

        public void Remove<TShape>(TShape shape)
            where TShape : struct, IShapeF
        {
            if (shape is RectangleF rec)
                _recList.Remove(rec);
            else if (shape is CircleF circle)
                _circleList.Remove(circle);
            else
                throw new NotSupportedException();
        }
        public bool Contains<TShape>(TShape shape)
            where TShape : struct, IShapeF
        {
            if (shape is RectangleF rec)
                return _recList.Contains(rec);
            else if (shape is CircleF circle)
                return _circleList.Contains(circle);
            else
                throw new NotSupportedException();
        }

        public bool Collides<TShape>(TShape colideWith)
         where TShape : struct, IShapeF
        {

            var _rect = _recList.GetEnumerator();
            var _circle = _circleList.GetEnumerator();
            while (_rect.MoveNext())
            {
                var c = _rect.Current;
                if (Shape.Intersects(colideWith, c))
                    return true;
            }
            while (_circle.MoveNext())
            {
                var c = _circle.Current;
                if (Shape.Intersects(colideWith, c))
                    return true;
            }
            return false;
        }
    }


    public class ShapeAssociativList<T>
    {
        public const int DEFAULT_CAPACITY = 128;
        private readonly Dictionary<T, RectangleF> _recList;
        private readonly Dictionary<T, CircleF> _circleList;


        public ShapeAssociativList(int capacity) : this(capacity, capacity)
        { }
        public ShapeAssociativList(int capacityRect = DEFAULT_CAPACITY, int capacityCircle = DEFAULT_CAPACITY)
        {
            _recList = new Dictionary<T, RectangleF>(capacityRect);
            _circleList = new Dictionary<T, CircleF>(capacityCircle);
            // those classes will be creted lazy. 
            _ = _recList.Keys;
            _ = _circleList.Keys;
        }

        public ShapeAssoativeListContent<T, RectangleF> Rectangles => new ShapeAssoativeListContent<T, RectangleF>();
        public ShapeAssoativeListContent<T, CircleF> Circles => new ShapeAssoativeListContent<T, CircleF>();

        public void Add<TShape>(T item, TShape shape)
            where TShape : struct, IShapeF
        {
            if (shape is RectangleF rec)
            {
                if (_circleList.ContainsKey(item))
                    throw new ArgumentException($"key {item} already added with a circle", nameof(item));
                _recList.Add(item, rec);
            }
            else if (shape is CircleF circle)
            {
                if (_recList.ContainsKey(item))
                    throw new ArgumentException($"key {item} already added with a rectangle", nameof(item));
                _circleList.Add(item, circle);
            }
            else
                throw new NotSupportedException();
        }

        public void Set<TShape>(T item, TShape shape)
            where TShape : struct, IShapeF
        {
            if (shape is RectangleF rec)
            {
                _circleList.Remove(item);
                _recList.Add(item, rec);
            }
            else if (shape is CircleF circle)
            {
                _recList.Remove(item);
                _circleList.Add(item, circle);
            }
            else
                throw new NotSupportedException();
        }

        public void Remove<TShape>(T item)
            where TShape : struct, IShapeF
        {
            if (!_recList.Remove(item))
                _circleList.Remove(item);
        }
        public bool Contains<TShape>(T item)
            where TShape : struct, IShapeF
        {
            return _recList.ContainsKey(item)
                || _circleList.ContainsKey(item);
        }

        public bool Collides<TShape>(TShape colideWith)
            where TShape : struct, IShapeF
        {
            using (var r = new ColideEnumerator<TShape>(_recList.GetEnumerator(), _circleList.GetEnumerator(), colideWith))
                return r.MoveNext();
        }

        public ColideEnumerator<TShape> Collisions<TShape>(TShape colideWith)
            where TShape : struct, IShapeF
        {
            return new ColideEnumerator<TShape>(_recList.GetEnumerator(), _circleList.GetEnumerator(), colideWith);
        }

        public struct ColideEnumerator<TShape> : IEnumerator<T>
            where TShape : struct, IShapeF
        {
            private readonly Dictionary<T, RectangleF>.Enumerator _rect;
            private readonly Dictionary<T, CircleF>.Enumerator _circle;

            private readonly TShape _collision;

            internal ColideEnumerator(Dictionary<T, RectangleF>.Enumerator rect, Dictionary<T, CircleF>.Enumerator circle, TShape collision)
            {
                _rect = rect;
                _circle = circle;
                _collision = collision;
                Current = default;
            }

            public T Current { get; private set; }


            public void Dispose()
            {
                _rect.Dispose();
                _circle.Dispose();
            }

            public bool MoveNext()
            {
                while (_rect.MoveNext())
                {
                    var c = _rect.Current;
                    if (Shape.Intersects(_collision, c.Value))
                    {
                        Current = c.Key;
                        return true;
                    }
                }
                while (_circle.MoveNext())
                {
                    var c = _circle.Current;
                    if (Shape.Intersects(_collision, c.Value))
                    {
                        Current = c.Key;
                        return true;
                    }
                }
                return false;

            }

            object IEnumerator.Current => Current;


            void IEnumerator.Reset()
            {
                (_rect as IEnumerator).Reset();
                (_circle as IEnumerator).Reset();
            }

        }
    }




    /// <summary>
    ///     Class that implements methods for shared <see cref="IShapeF" /> methods.
    /// </summary>
    public static class Shape
    {
        /// <summary>
        ///     Check if two shapes intersect.
        /// </summary>
        /// <param name="shapeA">The first shape.</param>
        /// <param name="shapeB">The second shape.</param>
        /// <returns>True if the two shapes intersect.</returns>
        public static bool Intersects<TShapeA, TShapeB>(this TShapeA shapeA, TShapeB shapeB)
            where TShapeA : struct, IShapeF
            where TShapeB : struct, IShapeF
        {
            var intersects = false;

            if (shapeA is RectangleF rectangleA && shapeB is RectangleF rectangleB)
            {
                intersects = rectangleA.Intersects(rectangleB);
            }
            else if (shapeA is CircleF circleA && shapeB is CircleF circleB)
            {
                intersects = circleA.Intersects(circleB);
            }
            else if (shapeA is RectangleF rect1 && shapeB is CircleF circ1)
            {
                return Intersects(circ1, rect1);
            }
            else if (shapeA is CircleF circ2 && shapeB is RectangleF rect2)
            {
                return Intersects(circ2, rect2);
            }

            return intersects;
        }

        public static RectangleF GetEnclosingRectangle<TShape>(this TShape shape)
            where TShape : struct, IShapeF
        {
            if (shape is RectangleF rectangleA)
            {
                return rectangleA;
            }
            else if (shape is CircleF circleA)
            {
                return circleA.ToRectangleF();
            }
            else
            {
                throw new NotImplementedException();
            }
        }



        /// <summary>
        ///     Checks if a circle and rectangle intersect.
        /// </summary>
        /// <param name="circle">Circle to check intersection with rectangle.</param>
        /// <param name="rectangle">Rectangle to check intersection with circle.</param>
        /// <returns>True if the circle and rectangle intersect.</returns>
        public static bool Intersects(CircleF circle, RectangleF rectangle)
        {
            var closestPoint = rectangle.ClosestPointTo(circle.Center);
            return circle.Contains(closestPoint);
        }
    }
}

