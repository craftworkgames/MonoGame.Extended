using System;
using System.Linq;
using MonoGame.Extended.Collections;
using NUnit.Framework;

namespace MonoGame.Extended.Tests.Collections
{
    [TestFixture]
    public class DequeTests
    {
        private class TestDequeElement
        {
            public int Value { get; set; }
        }

        private Random _random;

        [TestFixtureSetUp]
        public void Initialize()
        {
            _random = new Random();
        }

        [TestFixtureTearDown]
        public void Dispose()
        {
        }

        [TestCase]
        public void Deque_Constructor_Default()
        {
            var deque = new Deque<object>();
            Assert.IsTrue(deque.Count == 0);
            Assert.IsTrue(deque.Capacity == 0);
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(50)]
        public void Deque_Constructor_Collection(int count)
        {
            var elements = new TestDequeElement[count];
            for (var i = 0; i < count; i++)
            {
                elements[i] = new TestDequeElement
                {
                    Value =  i
                };
            }
            var deque = new Deque<TestDequeElement>(elements);
            Assert.IsTrue(deque.Count == count);
            Assert.IsTrue(deque.Capacity == count);
            for (var index = 0; index < deque.Count; index++)
            {
                Assert.IsTrue(deque[index].Value == index); 
            }
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(50)]
        public void Deque_Constructor_Capacity(int capacity)
        {
            var deque = new Deque<TestDequeElement>(capacity);
            Assert.IsTrue(deque.Count == 0);
            Assert.IsTrue(deque.Capacity == capacity);
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(50)]
        public void Deque_Clear(int count)
        {
            var elements = new TestDequeElement[count];
            for (var i = 0; i < count; i++)
            {
                elements[i] = new TestDequeElement
                {
                    Value = i
                };
            }
            var deque = new Deque<TestDequeElement>(elements);
            deque.Clear();
            Assert.IsTrue(deque.Count == 0);
            Assert.IsTrue(deque.Capacity >= count);
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(50)]
        public void Deque_Trim_And_Clear(int count)
        {
            var elements = new TestDequeElement[count];
            for (var i = 0; i < count; i++)
            {
                elements[i] = new TestDequeElement
                {
                    Value = i
                };
            }
            var deque = new Deque<TestDequeElement>(elements);
            deque.Clear();
            deque.TrimExcess();
            Assert.IsTrue(deque.Count == 0);
            Assert.IsTrue(deque.Capacity == 0);
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(50)]
        public void Deque_Trim_Front(int count)
        {
            var elements = new TestDequeElement[count];
            for (var i = 0; i < count; i++)
            {
                elements[i] = new TestDequeElement
                {
                    Value = i
                };
            }
            var deque = new Deque<TestDequeElement>(elements);

            for (var i = 0; i < count; i++)
            {
                TestDequeElement element;
                deque.RemoveFromFront(out element);
                deque.Capacity = deque.Count;
                Assert.IsTrue(deque.Count == count - 1 - i);
                Assert.IsTrue(deque.Capacity == count - 1 - i);
            }
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(50)]
        public void Deque_Trim_Back(int count)
        {
            var elements = new TestDequeElement[count];
            for (var i = 0; i < count; i++)
            {
                elements[i] = new TestDequeElement
                {
                    Value = i
                };
            }
            var deque = new Deque<TestDequeElement>(elements);

            for (var i = 0; i < count; i++)
            {
                TestDequeElement element;
                deque.RemoveFromBack(out element);
                deque.Capacity = deque.Count;
                Assert.IsTrue(deque.Count == count - 1 - i);
                Assert.IsTrue(deque.Capacity == count - 1 - i);
            }
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(50)]
        public void Deque_Add_Front(int count)
        {
            var deque = new Deque<TestDequeElement>();
            for (var i = 0; i < count; i++)
            {
                deque.AddToFront(new TestDequeElement
                {
                    Value = i
                });
            }
            Assert.IsTrue(deque.Count == count);
            Assert.IsTrue(deque.Capacity >= count);
            for (var index = 0; index < deque.Count; index++)
            {
                var element = deque[index];
                Assert.IsTrue(element.Value == deque.Count - 1 - index);
            }
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(50)]
        public void Deque_Add_Back(int count)
        {
            var deque = new Deque<TestDequeElement>();
            for (var i = 0; i < count; i++)
            {
                deque.AddToBack(new TestDequeElement
                {
                    Value = i
                });
            }
            Assert.IsTrue(deque.Count == count);
            Assert.IsTrue(deque.Capacity >= count);
            for (var index = 0; index < deque.Count; index++)
            {
                var element = deque[index];
                Assert.IsTrue(element.Value == index);
            }
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(50)]
        public void Deque_Remove_Front(int count)
        {
            var elements = new TestDequeElement[count];
            for (var i = 0; i < count; i++)
            {
                elements[i] = new TestDequeElement
                {
                    Value = i
                };
            }
            var deque = new Deque<TestDequeElement>(elements);

            TestDequeElement element;
            var index = 0;
            while (deque.RemoveFromFront(out element))
            {
                Assert.IsTrue(element.Value == index);
                index++;
            }
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(50)]
        public void Deque_Remove_Back(int count)
        {
            var elements = new TestDequeElement[count];
            for (var i = 0; i < count; i++)
            {
                elements[i] = new TestDequeElement
                {
                    Value = i
                };
            }
            var deque = new Deque<TestDequeElement>(elements);

            TestDequeElement element;
            var index = 0;
            while (deque.RemoveFromBack(out element))
            {
                Assert.IsTrue(element.Value == elements.Length - 1 - index);
                index++;
            }
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(50)]
        public void Deque_Get_Front(int count)
        {
            var elements = new TestDequeElement[count];
            for (var i = 0; i < count; i++)
            {
                elements[i] = new TestDequeElement
                {
                    Value = i
                };
            }
            var deque = new Deque<TestDequeElement>(elements);
            var indices = Enumerable.Range(0, count);
            foreach (var index in indices)
            {
                TestDequeElement element;
                deque.GetFront(out element);
                deque.RemoveFromFront();
                Assert.IsTrue(element.Value == index);
            }
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(50)]
        public void Deque_Get_Back(int count)
        {
            var elements = new TestDequeElement[count];
            for (var i = 0; i < count; i++)
            {
                elements[i] = new TestDequeElement
                {
                    Value = i
                };
            }
            var deque = new Deque<TestDequeElement>(elements);
            var indices = Enumerable.Range(0, count);
            foreach (var index in indices)
            {
                TestDequeElement element;
                deque.GetBack(out element);
                deque.RemoveFromBack();
                Assert.IsTrue(element.Value == count - 1 - index);
            }
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(50)]
        public void Deque_Get_Index(int count)
        {
            var elements = new TestDequeElement[count];
            for (var i = 0; i < count; i++)
            {
                elements[i] = new TestDequeElement
                {
                    Value = i
                };
            }
            var deque = new Deque<TestDequeElement>(elements);
            var indices = Enumerable.Range(0, count).ToList().Shuffle(_random);
            foreach (var index in indices)
            {
                TestDequeElement element;
                deque.Get(index, out element);
                Assert.IsTrue(element.Value == index);
            }
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(50)]
        public void Deque_ForEach_Iteration(int count)
        {
            var elements = new TestDequeElement[count];
            for (var i = 0; i < count; i++)
            {
                elements[i] = new TestDequeElement
                {
                    Value = i
                };
            }
            var deque = new Deque<TestDequeElement>(elements);
            var counter = 0;
            foreach (var element in deque)
            {
                Assert.IsTrue(element.Value == counter);
                counter++;
            }
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(50)]
        public void Deque_ForEach_Iteration_Modified(int count)
        {
            var elements = new TestDequeElement[count];
            for (var i = 0; i < count; i++)
            {
                elements[i] = new TestDequeElement
                {
                    Value = i
                };
            }
            var deque = new Deque<TestDequeElement>(elements);
            var counter = 0;
            foreach (var element in deque)
            {
                Assert.IsTrue(element.Value == counter);
                counter++;
                deque.RemoveFromFront();
            }
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(50)]
        public void Deque_Remove(int count)
        {
            var elements = new TestDequeElement[count];
            for (var i = 0; i < count; i++)
            {
                elements[i] = new TestDequeElement
                {
                    Value = i
                };
            }
            var deque = new Deque<TestDequeElement>(elements);
            var counter = count;
            while (deque.Count > 0)
            {
                var index = _random.Next(0, deque.Count - 1);
                deque.RemoveAt(index);
                counter--;
                Assert.IsTrue(deque.Count == counter);
            }
        }
    }
}
