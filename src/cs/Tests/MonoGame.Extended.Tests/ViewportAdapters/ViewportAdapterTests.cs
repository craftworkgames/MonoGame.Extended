namespace MonoGame.Extended.Tests.ViewportAdapters;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.ViewportAdapters;
using MonoGame.Extended.ViewportAdapters.Exceptions;
using Xunit;

public class ViewportAdapterTests
{
    [Fact]
    public void Constructor_ShouldThrowNullGraphicsDeviceException_IfANullGraphicsDeviceIsPassed()
    {
        var act = () => new TestViewportAdapter(null);

        Assert.Throws<NullGraphicsDeviceException>(act);
    }

    class TestViewportAdapter : ViewportAdapter
    {
        public TestViewportAdapter(GraphicsDevice graphicsDevice) : base(graphicsDevice)
        {
        }

        public override int VirtualWidth { get; }

        public override int VirtualHeight { get; }

        public override int ViewportWidth { get; }

        public override int ViewportHeight { get; }

        public override Matrix GetScaleMatrix() => throw new System.NotImplementedException();
    }
}
