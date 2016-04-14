using FarseerPhysics.DebugView;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;

namespace Demo.SimCamera
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private World _world;
        private DebugViewXNA _debugView;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            _world = new World(new Vector2(0, 0));
            _debugView = new DebugViewXNA(_world);
            _debugView.LoadContent(GraphicsDevice, Content);
            Body body = BodyFactory.CreateRectangle(_world, 2f, 2f, 1f);
            body.BodyType = BodyType.Dynamic;
            body.FixedRotation = true;
            body.Position = new Vector2(0,0);
            body.ApplyTorque(100);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _world.Step(Math.Min((float)gameTime.ElapsedGameTime.TotalSeconds, (1f / 60f)));
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            Matrix projection = Matrix.CreateOrthographicOffCenter(
                0f, 
                ConvertSimUnits.ToSimUnits(graphics.GraphicsDevice.Viewport.Width),
                ConvertSimUnits.ToSimUnits(graphics.GraphicsDevice.Viewport.Height), 
                0f, 
                0f, 
            1f);

            _debugView.RenderDebugData(ref projection);

            base.Draw(gameTime);
        }
    }
}
