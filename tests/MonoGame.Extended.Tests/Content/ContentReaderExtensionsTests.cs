using System;
using System.IO;
using System.Linq;
using System.Reflection;
using MonoGame.Extended.Collections;
using MonoGame.Extended.Content;
using Xunit;

namespace MonoGame.Extended.Tests.Content
{
    public class ContentReaderExtensionsTests
    {
        [Theory]
        [InlineData("testsuperdir/testsubdir1/testsubdir2/resource", "testsuperdir/testsubdir1/testsubdir2/resource")]
        [InlineData("testsuperdir/testsubdir1/../testsubdir2/resource", "testsuperdir/testsubdir2/resource")]
        [InlineData("testsuperdir/../resource", "resource")]
        [InlineData("../testsuperdir/testsubdir1/../testsubdir2/resource", "../testsuperdir/testsubdir2/resource")]
        [InlineData("testsuperdir/testsubdir1/testsubdir2/../../testsubdir3/resource", "testsuperdir/testsubdir3/resource")]
        [InlineData("testsuperdir/testsubdir1/../testsubdir2/../testsubdir3/resource", "testsuperdir/testsubdir3/resource")]
        public void ContentReaderExtensions_ShortenRelativePath(string input, string expectedoutput)
        {
            Assert.True(ContentReaderExtensions.ShortenRelativePath(input) == expectedoutput);
        }

    }
}
