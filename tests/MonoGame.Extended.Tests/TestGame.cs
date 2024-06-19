using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Tests
{
    public class TestGame : Game
    {
        public TestGame()
        {
            GraphicsAdapter.UseReferenceDevice = true;
            GraphicsAdapter.UseDriverType = GraphicsAdapter.DriverType.FastSoftware;
        }
    }
}
