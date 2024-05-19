using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using Xunit;

namespace MonoGame.Extended.Tests.Sprites
{
    public class SpriteSheetAnimationTests
    {
        [Theory]
        [InlineData(0, 0)]
        [InlineData(0, 0.9f)]
        [InlineData(1, 1f)]
        [InlineData(1, 1.9f)]
        [InlineData(0, 2f)]
        [InlineData(0, 2.9f)]
        [InlineData(1, 3f)]
        [InlineData(0, 4f)]
        [InlineData(1, 5f)]
        public void Looping_SpriteSheetAnimation_Should_Return_Correct_Frame_And_Not_Complete(int expectedTextureRegionIndex, float time)
        {
            var textureRegion2D1 = new TextureRegion2D("Region 1", null, new Rectangle());
            var textureRegion2D2 = new TextureRegion2D("Region 2", null, new Rectangle());

            var textureRegions = new[] { textureRegion2D1, textureRegion2D2 };

            var spriteSheetAnimationData = new SpriteSheetAnimationData(
                new[] { 0, 1 },
                1,
                true
            );

            var spriteSheetAnimation = new SpriteSheetAnimation("Test", textureRegions, spriteSheetAnimationData);
            var isCompleteFired = false;
            spriteSheetAnimation.OnCompleted += () => isCompleteFired = true;

            spriteSheetAnimation.Play();
            var gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(time));

            spriteSheetAnimation.Update(gameTime);

            Assert.Equal(textureRegions[expectedTextureRegionIndex], spriteSheetAnimation.CurrentFrame);
            Assert.False(spriteSheetAnimation.IsComplete);
            Assert.False(isCompleteFired);
        }

        [Fact]
        public void Looping_SpriteSheetAnimation_Should_Return_Correct_Frame_And_Not_Complete_Over_Multiple_Updates()
        {
            var textureRegion2D1 = new TextureRegion2D("Region 1", null, new Rectangle());
            var textureRegion2D2 = new TextureRegion2D("Region 2", null, new Rectangle());

            var textureRegions = new[] { textureRegion2D1, textureRegion2D2 };

            var spriteSheetAnimationData = new SpriteSheetAnimationData(
                new[] { 0, 1 },
                1,
                true
            );

            var spriteSheetAnimation = new SpriteSheetAnimation("Test", textureRegions, spriteSheetAnimationData);
            var isCompleteFired = false;
            spriteSheetAnimation.OnCompleted += () => isCompleteFired = true;

            spriteSheetAnimation.Play();
            var gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(0));
            spriteSheetAnimation.Update(gameTime);

            Assert.Equal(textureRegions[0], spriteSheetAnimation.CurrentFrame);
            Assert.False(spriteSheetAnimation.IsComplete);
            Assert.False(isCompleteFired);

            gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1));
            spriteSheetAnimation.Update(gameTime);

            Assert.Equal(textureRegions[1], spriteSheetAnimation.CurrentFrame);
            Assert.False(spriteSheetAnimation.IsComplete);
            Assert.False(isCompleteFired);

            gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1));
            spriteSheetAnimation.Update(gameTime);

            Assert.Equal(textureRegions[0], spriteSheetAnimation.CurrentFrame);
            Assert.False(spriteSheetAnimation.IsComplete);
            Assert.False(isCompleteFired);

            gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1));
            spriteSheetAnimation.Update(gameTime);

            Assert.Equal(textureRegions[1], spriteSheetAnimation.CurrentFrame);
            Assert.False(spriteSheetAnimation.IsComplete);
            Assert.False(isCompleteFired);
        }

        [Theory]
        [InlineData(0, 0.9f)]
        [InlineData(1, 1f)]
        [InlineData(1, 1.1f)]
        [InlineData(1, 1.9f)]
        public void Non_Looping_SpriteSheetAnimation_Should_Return_Correct_Frame_And_Not_Complete(int expectedTextureRegionIndex, float time)
        {
            var textureRegion2D1 = new TextureRegion2D("Region 1", null, new Rectangle());
            var textureRegion2D2 = new TextureRegion2D("Region 2", null, new Rectangle());

            var textureRegions = new[] { textureRegion2D1, textureRegion2D2 };

            var spriteSheetAnimationData = new SpriteSheetAnimationData(
                new[] { 0, 1 },
                1,
                false
            );

            var spriteSheetAnimation = new SpriteSheetAnimation("Test", textureRegions, spriteSheetAnimationData);
            var isCompleteFired = false;
            spriteSheetAnimation.OnCompleted += () => isCompleteFired = true;

            spriteSheetAnimation.Play();
            var gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(time));

            spriteSheetAnimation.Update(gameTime);

            Assert.Equal(textureRegions[expectedTextureRegionIndex], spriteSheetAnimation.CurrentFrame);
            Assert.False(spriteSheetAnimation.IsComplete);
            Assert.False(isCompleteFired);
        }

        [Theory]
        [InlineData(1, 2f)]
        [InlineData(1, 3f)]
        [InlineData(1, 4f)]
        public void Non_Looping_SpriteSheetAnimation_Should_Return_Correct_Frame_And_Complete_When_AnimationDuration_Is_Reached(int expectedTextureRegionIndex, float time)
        {
            var textureRegion2D1 = new TextureRegion2D("Region 1", null, new Rectangle());
            var textureRegion2D2 = new TextureRegion2D("Region 2", null, new Rectangle());

            var textureRegions = new[] { textureRegion2D1, textureRegion2D2 };

            var spriteSheetAnimationData = new SpriteSheetAnimationData(
                new[] { 0, 1 },
                1,
                false
            );

            var spriteSheetAnimation = new SpriteSheetAnimation("Test", textureRegions, spriteSheetAnimationData);
            var isCompleteFired = false;
            spriteSheetAnimation.OnCompleted += () => isCompleteFired = true;

            spriteSheetAnimation.Play();
            var gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(time));

            spriteSheetAnimation.Update(gameTime);

            Assert.Equal(textureRegions[expectedTextureRegionIndex], spriteSheetAnimation.CurrentFrame);
            Assert.True(spriteSheetAnimation.IsComplete);
            Assert.True(isCompleteFired);
        }

        [Fact]
        public void Non_Looping_SpriteSheetAnimation_Should_Return_Correct_Frame_And_Complete_When_AnimationDuration_Is_Reached_Over_Multiple_Updates()
        {
            var textureRegion2D1 = new TextureRegion2D("Region 1", null, new Rectangle());
            var textureRegion2D2 = new TextureRegion2D("Region 2", null, new Rectangle());

            var textureRegions = new[] { textureRegion2D1, textureRegion2D2 };

            var spriteSheetAnimationData = new SpriteSheetAnimationData(
                new[] { 0, 1 },
                1,
                false
            );

            var spriteSheetAnimation = new SpriteSheetAnimation("Test", textureRegions, spriteSheetAnimationData);
            var isCompleteFired = false;
            spriteSheetAnimation.OnCompleted += () => isCompleteFired = true;

            spriteSheetAnimation.Play();

            var gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(0));
            spriteSheetAnimation.Update(gameTime);

            Assert.Equal(textureRegions[0], spriteSheetAnimation.CurrentFrame);
            Assert.False(spriteSheetAnimation.IsComplete);
            Assert.False(isCompleteFired);

            gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1));
            spriteSheetAnimation.Update(gameTime);

            Assert.Equal(textureRegions[1], spriteSheetAnimation.CurrentFrame);
            Assert.False(spriteSheetAnimation.IsComplete);
            Assert.False(isCompleteFired);

            gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1));
            spriteSheetAnimation.Update(gameTime);

            Assert.Equal(textureRegions[1], spriteSheetAnimation.CurrentFrame);
            Assert.True(spriteSheetAnimation.IsComplete);
            Assert.True(isCompleteFired);

            isCompleteFired = false; // Reset isCompleteFired for next execution
            gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1));
            spriteSheetAnimation.Update(gameTime);

            Assert.Equal(textureRegions[1], spriteSheetAnimation.CurrentFrame);
            Assert.True(spriteSheetAnimation.IsComplete);
            Assert.False(isCompleteFired); // Event is not fired again as animation was already completed
        }

        [Theory]
        [InlineData(1, 0)]
        [InlineData(1, 0.9f)]
        [InlineData(0, 1f)]
        [InlineData(0, 1.9f)]
        [InlineData(1, 2f)]
        [InlineData(1, 2.9f)]
        [InlineData(0, 3f)]
        [InlineData(1, 4f)]
        [InlineData(0, 5f)]
        public void Reversed_Looping_SpriteSheetAnimation_Should_Return_Correct_Frame_And_Not_Complete(int expectedTextureRegionIndex, float time)
        {
            var textureRegion2D1 = new TextureRegion2D("Region 1", null, new Rectangle());
            var textureRegion2D2 = new TextureRegion2D("Region 2", null, new Rectangle());

            var textureRegions = new[] { textureRegion2D1, textureRegion2D2 };

            var spriteSheetAnimationData = new SpriteSheetAnimationData(
                new[] { 0, 1 },
                1,
                true,
                true
            );

            var spriteSheetAnimation = new SpriteSheetAnimation("Test", textureRegions, spriteSheetAnimationData);
            var isCompleteFired = false;
            spriteSheetAnimation.OnCompleted += () => isCompleteFired = true;

            spriteSheetAnimation.Play();
            var gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(time));

            spriteSheetAnimation.Update(gameTime);

            Assert.Equal(textureRegions[expectedTextureRegionIndex], spriteSheetAnimation.CurrentFrame);
            Assert.False(spriteSheetAnimation.IsComplete);
            Assert.False(isCompleteFired);
        }

        [Fact]
        public void Reversed_Looping_SpriteSheetAnimation_Should_Return_Correct_Frame_And_Not_Complete_Over_Multiple_Updates()
        {
            var textureRegion2D1 = new TextureRegion2D("Region 1", null, new Rectangle());
            var textureRegion2D2 = new TextureRegion2D("Region 2", null, new Rectangle());

            var textureRegions = new[] { textureRegion2D1, textureRegion2D2 };

            var spriteSheetAnimationData = new SpriteSheetAnimationData(
                new[] { 0, 1 },
                1,
                true,
                true
            );

            var spriteSheetAnimation = new SpriteSheetAnimation("Test", textureRegions, spriteSheetAnimationData);
            var isCompleteFired = false;
            spriteSheetAnimation.OnCompleted += () => isCompleteFired = true;

            spriteSheetAnimation.Play();
            var gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(0));
            spriteSheetAnimation.Update(gameTime);

            Assert.Equal(textureRegions[1], spriteSheetAnimation.CurrentFrame);
            Assert.False(spriteSheetAnimation.IsComplete);
            Assert.False(isCompleteFired);

            gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1));
            spriteSheetAnimation.Update(gameTime);

            Assert.Equal(textureRegions[0], spriteSheetAnimation.CurrentFrame);
            Assert.False(spriteSheetAnimation.IsComplete);
            Assert.False(isCompleteFired);

            gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1));
            spriteSheetAnimation.Update(gameTime);

            Assert.Equal(textureRegions[1], spriteSheetAnimation.CurrentFrame);
            Assert.False(spriteSheetAnimation.IsComplete);
            Assert.False(isCompleteFired);

            gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1));
            spriteSheetAnimation.Update(gameTime);

            Assert.Equal(textureRegions[0], spriteSheetAnimation.CurrentFrame);
            Assert.False(spriteSheetAnimation.IsComplete);
            Assert.False(isCompleteFired);
        }

        [Theory]
        [InlineData(1, 0.9f)]
        [InlineData(0, 1f)]
        [InlineData(0, 1.1f)]
        [InlineData(0, 1.9f)]
        public void Reversed_Non_Looping_SpriteSheetAnimation_Should_Return_Correct_Frame_And_Not_Complete(int expectedTextureRegionIndex, float time)
        {
            var textureRegion2D1 = new TextureRegion2D("Region 1", null, new Rectangle());
            var textureRegion2D2 = new TextureRegion2D("Region 2", null, new Rectangle());

            var textureRegions = new[] { textureRegion2D1, textureRegion2D2 };

            var spriteSheetAnimationData = new SpriteSheetAnimationData(
                new[] { 0, 1 },
                1,
                false,
                true
            );

            var spriteSheetAnimation = new SpriteSheetAnimation("Test", textureRegions, spriteSheetAnimationData);
            var isCompleteFired = false;
            spriteSheetAnimation.OnCompleted += () => isCompleteFired = true;

            spriteSheetAnimation.Play();
            var gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(time));

            spriteSheetAnimation.Update(gameTime);

            Assert.Equal(textureRegions[expectedTextureRegionIndex], spriteSheetAnimation.CurrentFrame);
            Assert.False(spriteSheetAnimation.IsComplete);
            Assert.False(isCompleteFired);
        }

        [Theory]
        [InlineData(0, 2f)]
        [InlineData(0, 3f)]
        [InlineData(0, 4f)]
        public void Reversed_Non_Looping_SpriteSheetAnimation_Should_Return_Correct_Frame_And_Complete_When_AnimationDuration_Is_Reached(int expectedTextureRegionIndex, float time)
        {
            var textureRegion2D1 = new TextureRegion2D("Region 1", null, new Rectangle());
            var textureRegion2D2 = new TextureRegion2D("Region 2", null, new Rectangle());

            var textureRegions = new[] { textureRegion2D1, textureRegion2D2 };

            var spriteSheetAnimationData = new SpriteSheetAnimationData(
                new[] { 0, 1 },
                1,
                false,
                true
            );

            var spriteSheetAnimation = new SpriteSheetAnimation("Test", textureRegions, spriteSheetAnimationData);
            var isCompleteFired = false;
            spriteSheetAnimation.OnCompleted += () => isCompleteFired = true;

            spriteSheetAnimation.Play();
            var gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(time));

            spriteSheetAnimation.Update(gameTime);

            Assert.Equal(textureRegions[expectedTextureRegionIndex], spriteSheetAnimation.CurrentFrame);
            Assert.True(spriteSheetAnimation.IsComplete);
            Assert.True(isCompleteFired);
        }

        [Fact]
        public void Reversed_Non_Looping_SpriteSheetAnimation_Should_Return_Correct_Frame_And_Complete_When_AnimationDuration_Is_Reached_Over_Multiple_Updates()
        {
            var textureRegion2D1 = new TextureRegion2D("Region 1", null, new Rectangle());
            var textureRegion2D2 = new TextureRegion2D("Region 2", null, new Rectangle());

            var textureRegions = new[] { textureRegion2D1, textureRegion2D2 };

            var spriteSheetAnimationData = new SpriteSheetAnimationData(
                new[] { 0, 1 },
                1,
                false,
                true
            );

            var spriteSheetAnimation = new SpriteSheetAnimation("Test", textureRegions, spriteSheetAnimationData);
            var isCompleteFired = false;
            spriteSheetAnimation.OnCompleted += () => isCompleteFired = true;

            spriteSheetAnimation.Play();

            var gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(0));
            spriteSheetAnimation.Update(gameTime);

            Assert.Equal(textureRegions[1], spriteSheetAnimation.CurrentFrame);
            Assert.False(spriteSheetAnimation.IsComplete);
            Assert.False(isCompleteFired);

            gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1));
            spriteSheetAnimation.Update(gameTime);

            Assert.Equal(textureRegions[0], spriteSheetAnimation.CurrentFrame);
            Assert.False(spriteSheetAnimation.IsComplete);
            Assert.False(isCompleteFired);

            gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1));
            spriteSheetAnimation.Update(gameTime);

            Assert.Equal(textureRegions[0], spriteSheetAnimation.CurrentFrame);
            Assert.True(spriteSheetAnimation.IsComplete);
            Assert.True(isCompleteFired);

            isCompleteFired = false; // Reset isCompleteFired for next execution
            gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1));
            spriteSheetAnimation.Update(gameTime);

            Assert.Equal(textureRegions[0], spriteSheetAnimation.CurrentFrame);
            Assert.True(spriteSheetAnimation.IsComplete);
            Assert.False(isCompleteFired);  // Event is not fired again as animation was already completed;
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(0, 0.9f)]
        [InlineData(1, 1f)]
        [InlineData(1, 1.9f)]
        [InlineData(0, 2f)]
        [InlineData(0, 2.9f)]
        [InlineData(1, 3f)]
        [InlineData(0, 4f)]
        [InlineData(1, 5f)]
        [InlineData(0, 6f)]
        [InlineData(1, 7f)]
        [InlineData(0, 8f)]
        public void PingPong_Looping_SpriteSheetAnimation_Should_Return_Correct_Frame_And_Not_Complete(int expectedTextureRegionIndex, float time)
        {
            var textureRegion2D1 = new TextureRegion2D("Region 1", null, new Rectangle());
            var textureRegion2D2 = new TextureRegion2D("Region 2", null, new Rectangle());

            var textureRegions = new[] { textureRegion2D1, textureRegion2D2 };

            var spriteSheetAnimationData = new SpriteSheetAnimationData(
                new[] { 0, 1 },
                1,
                true,
                false,
                true
            );

            var spriteSheetAnimation = new SpriteSheetAnimation("Test", textureRegions, spriteSheetAnimationData);
            var isCompleteFired = false;
            spriteSheetAnimation.OnCompleted += () => isCompleteFired = true;

            spriteSheetAnimation.Play();
            var gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(time));

            spriteSheetAnimation.Update(gameTime);

            Assert.Equal(textureRegions[expectedTextureRegionIndex], spriteSheetAnimation.CurrentFrame);
            Assert.False(spriteSheetAnimation.IsComplete);
            Assert.False(isCompleteFired);
        }

        [Fact]
        public void PingPong_Looping_SpriteSheetAnimation_Should_Return_Correct_Frame_And_Not_Complete_Over_Multiple_Updates()
        {
            var textureRegion2D1 = new TextureRegion2D("Region 1", null, new Rectangle());
            var textureRegion2D2 = new TextureRegion2D("Region 2", null, new Rectangle());

            var textureRegions = new[] { textureRegion2D1, textureRegion2D2 };

            var spriteSheetAnimationData = new SpriteSheetAnimationData(
                new[] { 0, 1 },
                1,
                true,
                false,
                true
            );

            var spriteSheetAnimation = new SpriteSheetAnimation("Test", textureRegions, spriteSheetAnimationData);
            var isCompleteFired = false;
            spriteSheetAnimation.OnCompleted += () => isCompleteFired = true;

            spriteSheetAnimation.Play();
            var gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(0));
            spriteSheetAnimation.Update(gameTime);

            Assert.Equal(textureRegions[0], spriteSheetAnimation.CurrentFrame);
            Assert.False(spriteSheetAnimation.IsComplete);
            Assert.False(isCompleteFired);

            gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1));
            spriteSheetAnimation.Update(gameTime);

            Assert.Equal(textureRegions[1], spriteSheetAnimation.CurrentFrame);
            Assert.False(spriteSheetAnimation.IsComplete);
            Assert.False(isCompleteFired);

            gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1));
            spriteSheetAnimation.Update(gameTime);

            Assert.Equal(textureRegions[0], spriteSheetAnimation.CurrentFrame);
            Assert.False(spriteSheetAnimation.IsComplete);
            Assert.False(isCompleteFired);

            gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1));
            spriteSheetAnimation.Update(gameTime);

            Assert.Equal(textureRegions[1], spriteSheetAnimation.CurrentFrame);
            Assert.False(spriteSheetAnimation.IsComplete);
            Assert.False(isCompleteFired);

            gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1));
            spriteSheetAnimation.Update(gameTime);

            Assert.Equal(textureRegions[0], spriteSheetAnimation.CurrentFrame);
            Assert.False(spriteSheetAnimation.IsComplete);
            Assert.False(isCompleteFired);

            gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1));
            spriteSheetAnimation.Update(gameTime);

            Assert.Equal(textureRegions[1], spriteSheetAnimation.CurrentFrame);
            Assert.False(spriteSheetAnimation.IsComplete);
            Assert.False(isCompleteFired);
        }

        [Theory]
        [InlineData(0, 0.9f)]
        [InlineData(1, 1f)]
        [InlineData(1, 1.9f)]
        [InlineData(0, 2f)]
        [InlineData(0, 2.9f)]
        public void PingPong_Non_Looping_SpriteSheetAnimation_Should_Return_Correct_Frame_And_Not_Complete(int expectedTextureRegionIndex, float time)
        {
            var textureRegion2D1 = new TextureRegion2D("Region 1", null, new Rectangle());
            var textureRegion2D2 = new TextureRegion2D("Region 2", null, new Rectangle());

            var textureRegions = new[] { textureRegion2D1, textureRegion2D2 };

            var spriteSheetAnimationData = new SpriteSheetAnimationData(
                new[] { 0, 1 },
                1,
                false,
                false,
                true
            );

            var spriteSheetAnimation = new SpriteSheetAnimation("Test", textureRegions, spriteSheetAnimationData);
            var isCompleteFired = false;
            spriteSheetAnimation.OnCompleted += () => isCompleteFired = true;

            spriteSheetAnimation.Play();
            var gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(time));

            spriteSheetAnimation.Update(gameTime);

            Assert.Equal(textureRegions[expectedTextureRegionIndex], spriteSheetAnimation.CurrentFrame);
            Assert.False(spriteSheetAnimation.IsComplete);
            Assert.False(isCompleteFired);
        }

        [Theory]
        [InlineData(0, 3f)]
        [InlineData(0, 4f)]
        [InlineData(0, 5f)]
        public void PingPong_Non_Looping_SpriteSheetAnimation_Should_Return_Correct_Frame_And_Complete_When_AnimationDuration_Is_Reached(int expectedTextureRegionIndex, float time)
        {
            var textureRegion2D1 = new TextureRegion2D("Region 1", null, new Rectangle());
            var textureRegion2D2 = new TextureRegion2D("Region 2", null, new Rectangle());

            var textureRegions = new[] { textureRegion2D1, textureRegion2D2 };

            var spriteSheetAnimationData = new SpriteSheetAnimationData(
                new[] { 0, 1 },
                1,
                false,
                false,
                true
            );

            var spriteSheetAnimation = new SpriteSheetAnimation("Test", textureRegions, spriteSheetAnimationData);
            var isCompleteFired = false;
            spriteSheetAnimation.OnCompleted += () => isCompleteFired = true;

            spriteSheetAnimation.Play();
            var gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(time));

            spriteSheetAnimation.Update(gameTime);

            Assert.Equal(textureRegions[expectedTextureRegionIndex], spriteSheetAnimation.CurrentFrame);
            Assert.True(spriteSheetAnimation.IsComplete);
            Assert.True(isCompleteFired);
        }

        [Fact]
        public void PingPong_Non_Looping_SpriteSheetAnimation_Should_Return_Correct_Frame_And_Complete_When_AnimationDuration_Is_Reached_Over_Multiple_Updates()
        {
            var textureRegion2D1 = new TextureRegion2D("Region 1", null, new Rectangle());
            var textureRegion2D2 = new TextureRegion2D("Region 2", null, new Rectangle());
            var textureRegion2D3 = new TextureRegion2D("Region 3", null, new Rectangle());

            var textureRegions = new[] { textureRegion2D1, textureRegion2D2, textureRegion2D3 };

            var spriteSheetAnimationData = new SpriteSheetAnimationData(
                new[] { 0, 1, 2 },
                1,
                false,
                false,
                true
            );

            var spriteSheetAnimation = new SpriteSheetAnimation("Test", textureRegions, spriteSheetAnimationData);

            var isCompleteFired = false;
            spriteSheetAnimation.OnCompleted += () => isCompleteFired = true;

            spriteSheetAnimation.Play();

            var gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(0));
            spriteSheetAnimation.Update(gameTime);

            Assert.Equal(textureRegions[0], spriteSheetAnimation.CurrentFrame);
            Assert.False(spriteSheetAnimation.IsComplete);
            Assert.False(isCompleteFired);

            gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1));
            spriteSheetAnimation.Update(gameTime);

            Assert.Equal(textureRegions[1], spriteSheetAnimation.CurrentFrame);
            Assert.False(spriteSheetAnimation.IsComplete);
            Assert.False(isCompleteFired);

            gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1));
            spriteSheetAnimation.Update(gameTime);

            Assert.Equal(textureRegions[2], spriteSheetAnimation.CurrentFrame);
            Assert.False(spriteSheetAnimation.IsComplete);
            Assert.False(isCompleteFired);

            gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1));
            spriteSheetAnimation.Update(gameTime);

            Assert.Equal(textureRegions[1], spriteSheetAnimation.CurrentFrame);
            Assert.False(spriteSheetAnimation.IsComplete);
            Assert.False(isCompleteFired);

            gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1));
            spriteSheetAnimation.Update(gameTime);

            Assert.Equal(textureRegions[0], spriteSheetAnimation.CurrentFrame);
            Assert.False(spriteSheetAnimation.IsComplete);
            Assert.False(isCompleteFired);

            gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1));
            spriteSheetAnimation.Update(gameTime);

            Assert.Equal(textureRegions[0], spriteSheetAnimation.CurrentFrame);
            Assert.True(spriteSheetAnimation.IsComplete);
            Assert.True(isCompleteFired);

            isCompleteFired = false; // Reset isCompleteFired for next execution
            gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1));
            spriteSheetAnimation.Update(gameTime);

            Assert.Equal(textureRegions[0], spriteSheetAnimation.CurrentFrame);
            Assert.True(spriteSheetAnimation.IsComplete);
            Assert.False(isCompleteFired); // Event is not fired again as animation was already completed
        }

        [Theory]
        [InlineData(1, 0)]
        [InlineData(1, 0.9f)]
        [InlineData(0, 1f)]
        [InlineData(0, 1.9f)]
        [InlineData(1, 2f)]
        [InlineData(1, 2.9f)]
        [InlineData(0, 3f)]
        [InlineData(1, 4f)]
        [InlineData(0, 5f)]
        [InlineData(1, 6f)]
        [InlineData(0, 7f)]
        [InlineData(1, 8f)]
        public void Reversed_PingPong_Looping_SpriteSheetAnimation_Should_Return_Correct_Frame_And_Not_Complete(int expectedTextureRegionIndex, float time)
        {
            var textureRegion2D1 = new TextureRegion2D("Region 1", null, new Rectangle());
            var textureRegion2D2 = new TextureRegion2D("Region 2", null, new Rectangle());

            var textureRegions = new[] { textureRegion2D1, textureRegion2D2 };

            var spriteSheetAnimationData = new SpriteSheetAnimationData(
                new[] { 0, 1 },
                1,
                true,
                true,
                true
            );

            var spriteSheetAnimation = new SpriteSheetAnimation("Test", textureRegions, spriteSheetAnimationData);
            var isCompleteFired = false;
            spriteSheetAnimation.OnCompleted += () => isCompleteFired = true;

            spriteSheetAnimation.Play();
            var gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(time));

            spriteSheetAnimation.Update(gameTime);

            Assert.Equal(textureRegions[expectedTextureRegionIndex], spriteSheetAnimation.CurrentFrame);
            Assert.False(spriteSheetAnimation.IsComplete);
            Assert.False(isCompleteFired);
        }

        [Fact]
        public void Reversed_PingPong_Looping_SpriteSheetAnimation_Should_Return_Correct_Frame_And_Not_Complete_Over_Multiple_Updates()
        {
            var textureRegion2D1 = new TextureRegion2D("Region 1", null, new Rectangle());
            var textureRegion2D2 = new TextureRegion2D("Region 2", null, new Rectangle());

            var textureRegions = new[] { textureRegion2D1, textureRegion2D2 };

            var spriteSheetAnimationData = new SpriteSheetAnimationData(
                new[] { 0, 1 },
                1,
                true,
                true,
                true
            );

            var spriteSheetAnimation = new SpriteSheetAnimation("Test", textureRegions, spriteSheetAnimationData);
            var isCompleteFired = false;
            spriteSheetAnimation.OnCompleted += () => isCompleteFired = true;

            spriteSheetAnimation.Play();
            var gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(0));
            spriteSheetAnimation.Update(gameTime);

            Assert.Equal(textureRegions[1], spriteSheetAnimation.CurrentFrame);
            Assert.False(spriteSheetAnimation.IsComplete);
            Assert.False(isCompleteFired);

            gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1));
            spriteSheetAnimation.Update(gameTime);

            Assert.Equal(textureRegions[0], spriteSheetAnimation.CurrentFrame);
            Assert.False(spriteSheetAnimation.IsComplete);
            Assert.False(isCompleteFired);

            gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1));
            spriteSheetAnimation.Update(gameTime);

            Assert.Equal(textureRegions[1], spriteSheetAnimation.CurrentFrame);
            Assert.False(spriteSheetAnimation.IsComplete);
            Assert.False(isCompleteFired);

            gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1));
            spriteSheetAnimation.Update(gameTime);

            Assert.Equal(textureRegions[0], spriteSheetAnimation.CurrentFrame);
            Assert.False(spriteSheetAnimation.IsComplete);
            Assert.False(isCompleteFired);

            gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1));
            spriteSheetAnimation.Update(gameTime);

            Assert.Equal(textureRegions[1], spriteSheetAnimation.CurrentFrame);
            Assert.False(spriteSheetAnimation.IsComplete);
            Assert.False(isCompleteFired);

            gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1));
            spriteSheetAnimation.Update(gameTime);

            Assert.Equal(textureRegions[0], spriteSheetAnimation.CurrentFrame);
            Assert.False(spriteSheetAnimation.IsComplete);
            Assert.False(isCompleteFired);
        }

        [Theory]
        [InlineData(1, 0.9f)]
        [InlineData(0, 1f)]
        [InlineData(0, 1.9f)]
        [InlineData(1, 2f)]
        [InlineData(1, 2.9f)]
        public void Reversed_PingPong_Non_Looping_SpriteSheetAnimation_Should_Return_Correct_Frame_And_Not_Complete(int expectedTextureRegionIndex, float time)
        {
            var textureRegion2D1 = new TextureRegion2D("Region 1", null, new Rectangle());
            var textureRegion2D2 = new TextureRegion2D("Region 2", null, new Rectangle());

            var textureRegions = new[] { textureRegion2D1, textureRegion2D2 };

            var spriteSheetAnimationData = new SpriteSheetAnimationData(
                new[] { 0, 1 },
                1,
                false,
                true,
                true
            );

            var spriteSheetAnimation = new SpriteSheetAnimation("Test", textureRegions, spriteSheetAnimationData);
            var isCompleteFired = false;
            spriteSheetAnimation.OnCompleted += () => isCompleteFired = true;

            spriteSheetAnimation.Play();
            var gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(time));

            spriteSheetAnimation.Update(gameTime);

            Assert.Equal(textureRegions[expectedTextureRegionIndex], spriteSheetAnimation.CurrentFrame);
            Assert.False(spriteSheetAnimation.IsComplete);
            Assert.False(isCompleteFired);
        }

        [Theory]
        [InlineData(1, 3f)]
        [InlineData(1, 4f)]
        [InlineData(1, 5f)]
        public void Reversed_PingPong_Non_Looping_SpriteSheetAnimation_Should_Return_Correct_Frame_And_Complete_When_AnimationDuration_Is_Reached(int expectedTextureRegionIndex, float time)
        {
            var textureRegion2D1 = new TextureRegion2D("Region 1", null, new Rectangle());
            var textureRegion2D2 = new TextureRegion2D("Region 2", null, new Rectangle());

            var textureRegions = new[] { textureRegion2D1, textureRegion2D2 };

            var spriteSheetAnimationData = new SpriteSheetAnimationData(
                new[] { 0, 1 },
                1,
                false,
                true,
                true
            );

            var spriteSheetAnimation = new SpriteSheetAnimation("Test", textureRegions, spriteSheetAnimationData);
            var isCompleteFired = false;
            spriteSheetAnimation.OnCompleted += () => isCompleteFired = true;

            spriteSheetAnimation.Play();
            var gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(time));

            spriteSheetAnimation.Update(gameTime);

            Assert.Equal(textureRegions[expectedTextureRegionIndex], spriteSheetAnimation.CurrentFrame);
            Assert.True(spriteSheetAnimation.IsComplete);
            Assert.True(isCompleteFired);
        }

        [Fact]
        public void Reversed_PingPong_Non_Looping_SpriteSheetAnimation_Should_Return_Correct_Frame_And_Complete_When_AnimationDuration_Is_Reached_Over_Multiple_Updates()
        {
            var textureRegion2D1 = new TextureRegion2D("Region 1", null, new Rectangle());
            var textureRegion2D2 = new TextureRegion2D("Region 2", null, new Rectangle());
            var textureRegion2D3 = new TextureRegion2D("Region 3", null, new Rectangle());

            var textureRegions = new[] { textureRegion2D1, textureRegion2D2, textureRegion2D3 };

            var spriteSheetAnimationData = new SpriteSheetAnimationData(
                new[] { 0, 1, 2 },
                1,
                false,
                true,
                true
            );

            var spriteSheetAnimation = new SpriteSheetAnimation("Test", textureRegions, spriteSheetAnimationData);

            var isCompleteFired = false;
            spriteSheetAnimation.OnCompleted += () => isCompleteFired = true;

            spriteSheetAnimation.Play();

            var gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(0));
            spriteSheetAnimation.Update(gameTime);

            Assert.Equal(textureRegions[2], spriteSheetAnimation.CurrentFrame);
            Assert.False(spriteSheetAnimation.IsComplete);
            Assert.False(isCompleteFired);

            gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1));
            spriteSheetAnimation.Update(gameTime);

            Assert.Equal(textureRegions[1], spriteSheetAnimation.CurrentFrame);
            Assert.False(spriteSheetAnimation.IsComplete);
            Assert.False(isCompleteFired);

            gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1));
            spriteSheetAnimation.Update(gameTime);

            Assert.Equal(textureRegions[0], spriteSheetAnimation.CurrentFrame);
            Assert.False(spriteSheetAnimation.IsComplete);
            Assert.False(isCompleteFired);

            gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1));
            spriteSheetAnimation.Update(gameTime);

            Assert.Equal(textureRegions[1], spriteSheetAnimation.CurrentFrame);
            Assert.False(spriteSheetAnimation.IsComplete);
            Assert.False(isCompleteFired);

            gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1));
            spriteSheetAnimation.Update(gameTime);

            Assert.Equal(textureRegions[2], spriteSheetAnimation.CurrentFrame);
            Assert.False(spriteSheetAnimation.IsComplete);
            Assert.False(isCompleteFired);

            gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1));
            spriteSheetAnimation.Update(gameTime);

            Assert.Equal(textureRegions[2], spriteSheetAnimation.CurrentFrame);
            Assert.True(spriteSheetAnimation.IsComplete);
            Assert.True(isCompleteFired);

            isCompleteFired = false; // Reset isCompleteFired for next execution
            gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1));
            spriteSheetAnimation.Update(gameTime);

            Assert.Equal(textureRegions[2], spriteSheetAnimation.CurrentFrame);
            Assert.True(spriteSheetAnimation.IsComplete);
            Assert.False(isCompleteFired); // Event is not fired again as animation was already completed
        }
    }
}
