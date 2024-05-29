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

        // Test added for issue #633
        // https://github.com/craftworkgames/MonoGame.Extended/issues/633
        //
        // Issue states that the input string from a json file is parsed in correctly by the content reader.
        // Adding this test to ensure that GetRelativeAssetName is parsed correctly.
        [Theory]
        [InlineData("test.tex.png", "test.tex")]
        public void ContentReaderExtensions_GetRelativeAssetName(string input, string expected)
        {
            var contentReaderType = typeof(Microsoft.Xna.Framework.Content.ContentReader).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).First();
            var contentReader = (Microsoft.Xna.Framework.Content.ContentReader)contentReaderType.Invoke(new object[]
            {
                null,                   // manager: parameter
                new MemoryStream(),     // stream: parameter
                $"{expected}.xnb",      // assetName: parameter
                0,                      // version: parameter
                null                    // recordDisposableObject: parameter
            });

            var actual = contentReader.GetRelativeAssetName(input);

            Assert.Equal(expected, actual);
        }
    }
}
