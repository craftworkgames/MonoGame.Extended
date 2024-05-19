using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Tests
{
    public class TestGraphicsDevice : GraphicsDevice
    {
        public TestGraphicsDevice() 
            : base(GraphicsAdapter.DefaultAdapter, GraphicsProfile.Reach, new PresentationParameters())
        {
        }
    }
}