using System;
using System.Text;
using NUnit.Framework;

namespace MonoGame.Extended.Tests
{
    [TestFixture]
    public class UnsafeCodeTests
    {
        [Test]
        public void Unsafe_Copy_Test()
        {
            var a = new byte[100];
            var b = new byte[100];
            for (var i = 0; i < 100; ++i)
            {
                a[i] = (byte)i;
            }
            UnsafeCopy(a, 0, b, 0, 100);
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("The first 10 elements are: ");
            for (var i = 0; i < 10; ++i)
            {
                stringBuilder.Append(b[i] + " ");
            }

            Assert.AreEqual(stringBuilder.ToString(), "The first 10 elements are: 0 1 2 3 4 5 6 7 8 9 ");
        }

        // The unsafe keyword allows pointers to be used within the following method:
        private static unsafe void UnsafeCopy(byte[] src, int srcIndex, byte[] dst, int dstIndex, int count)
        {
            if (src == null || srcIndex < 0 || dst == null || dstIndex < 0 || count < 0)
            {
                throw new ArgumentException();
            }
            var srcLen = src.Length;
            var dstLen = dst.Length;
            if (srcLen - srcIndex < count || dstLen - dstIndex < count)
            {
                throw new ArgumentException();
            }

            // The following fixed statement pins the location of
            // the src and dst objects in memory so that they will
            // not be moved by garbage collection.          
            fixed (byte* pSrc = src, pDst = dst)
            {
                var ps = pSrc;
                var pd = pDst;

                // Loop over the count in blocks of 4 bytes, copying an
                // integer (4 bytes) at a time:
                for (var n = 0; n < count / 4; n++)
                {
                    *(int*)pd = *(int*)ps;
                    pd += 4;
                    ps += 4;
                }

                // Complete the copy by moving any bytes that weren't
                // moved in blocks of 4:
                for (var n = 0; n < count % 4; n++)
                {
                    *pd = *ps;
                    pd++;
                    ps++;
                }
            }
        }
    }
}
