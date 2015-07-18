using Microsoft.Xna.Framework.Graphics;
using NSubstitute;

namespace MonoGame.Extended.Tests
{
    public static class TestHelper
    {
        public static GraphicsDevice CreateGraphicsDevice()
        {
            const GraphicsProfile profile = GraphicsProfile.HiDef;
            var adapter = GraphicsAdapter.DefaultAdapter;
            var presentationParameters = new PresentationParameters();
            return Substitute.For<GraphicsDevice>(adapter, profile, presentationParameters);
        }
    }
}