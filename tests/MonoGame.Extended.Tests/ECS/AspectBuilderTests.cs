using System;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;
using Xunit;

namespace MonoGame.Extended.ECS.Tests
{
    public class AspectBuilderTests
    {
        [Fact]
        public void MatchAllTypes()
        {
            var builder = new AspectBuilder()
                .All(typeof(Transform2), typeof(Sprite));

            Assert.Equal(2, builder.AllTypes.Count);
            Assert.Contains(typeof(Transform2), builder.AllTypes);
            Assert.Contains(typeof(Sprite), builder.AllTypes);
        }

        [Fact]
        public void MatchAllTypesIsEmpty()
        {
            var builder = new AspectBuilder()
                .All();

            Assert.Empty(builder.AllTypes);
            Assert.Empty(builder.OneTypes);
            Assert.Empty(builder.ExclusionTypes);
        }

        [Fact]
        public void MatchOneOfType()
        {
            var builder = new AspectBuilder()
                .One(typeof(Transform2), typeof(Sprite));

            Assert.Equal(2, builder.OneTypes.Count);
            Assert.Contains(typeof(Transform2), builder.OneTypes);
            Assert.Contains(typeof(Sprite), builder.OneTypes);
        }
        
        [Fact]
        public void ExcludeTypes()
        {
            var builder = new AspectBuilder()
                .Exclude(typeof(Transform2), typeof(Sprite));

            Assert.Equal(2, builder.ExclusionTypes.Count);
            Assert.Contains(typeof(Transform2), builder.ExclusionTypes);
            Assert.Contains(typeof(Sprite), builder.ExclusionTypes);
        }

        [Fact]
        public void BuildAspect()
        {
            var componentManager = new ComponentManager();
            var builder = new AspectBuilder()
                .All(typeof(Transform2), typeof(Sprite))
                .One(typeof(string))
                .Exclude(typeof(Texture2D));

            var aspect = builder.Build(componentManager);

            Assert.True(aspect.AllSet.Data != 0);
            Assert.True(aspect.OneSet.Data != 0);
            Assert.True(aspect.ExclusionSet.Data != 0);
        }
    }
}