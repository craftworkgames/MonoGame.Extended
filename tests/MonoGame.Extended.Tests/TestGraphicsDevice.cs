using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Tests;

public class TestGraphicsDeviceFixture : IDisposable
{
    public TestGame TestGame;
    public GraphicsDeviceManager GraphicsDeviceManager;
    public GraphicsDevice GraphicsDevice;

    public TestGraphicsDeviceFixture()
    {
        TestGame = new TestGame();
        GraphicsDeviceManager = new GraphicsDeviceManager(TestGame);
        GraphicsDeviceManager.GraphicsProfile = GraphicsProfile.HiDef;
        ((IGraphicsDeviceManager)TestGame.Services.GetService(typeof(IGraphicsDeviceManager))).CreateDevice();
        GraphicsDevice = TestGame.GraphicsDevice;
    }

    public void Dispose()
    {
        TestGame.Dispose();
        TestGame = null;
        GraphicsDeviceManager = null;
        GraphicsDevice = null;
    }
}

//[CollectionDefinition("Test Graphics Device")]
//public class TestGraphicsDeviceCollection : ICollectionFixture<TestGraphicsDeviceFixture> { }



//public class TestGraphicsDevice : GraphicsDevice
//{
//    public TestGame TestGame;
//    public GraphicsDeviceManager GraphicsDeviceManager;
//    public GraphicsDevice GraphicsDevice;

//    public TestGraphicsDevice()
//        : base(GraphicsAdapter.DefaultAdapter, GraphicsProfile.Reach, new PresentationParameters())
//    {
//    }
//}
