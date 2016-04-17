using FarseerPhysics.Common;
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
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private World _world;
        private DebugViewXNA _debugView;

        private Block _blockControlled;
        private Block _block;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
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
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Texture2D textureBlock = Content.Load<Texture2D>("block");

            _world = new World(new Vector2(0, 0));

            float simWidth = ConvertSimUnits.ToSimUnits(_graphics.GraphicsDevice.Viewport.Width);
            float simHeight = ConvertSimUnits.ToSimUnits(_graphics.GraphicsDevice.Viewport.Height);

            Vertices borders = new Vertices(3);
            borders.Add(new Vector2(0, 0));
            borders.Add(new Vector2(0, simHeight));
            borders.Add(new Vector2(simWidth, simHeight));
            borders.Add(new Vector2(simWidth, 0));

            BodyFactory.CreateLoopShape(_world, borders);

            _debugView = new DebugViewXNA(_world);
            _debugView.LoadContent(GraphicsDevice, Content);

            _blockControlled = new Block(
                _world, 
                new Vector2(
                    ConvertSimUnits.ToSimUnits(110), 
                    ConvertSimUnits.ToSimUnits(110)
                    ),
                textureBlock);

            _block = new Block(
                _world, 
                new Vector2(
                    ConvertSimUnits.ToSimUnits(410), 
                    ConvertSimUnits.ToSimUnits(110)
                    ), 
                textureBlock);

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

            _blockControlled.Update(gameTime);

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
                ConvertSimUnits.ToSimUnits(_graphics.GraphicsDevice.Viewport.Width),
                ConvertSimUnits.ToSimUnits(_graphics.GraphicsDevice.Viewport.Height), 
                0f, 
                0f, 
            1f);

            _spriteBatch.Begin();
            _blockControlled.Draw(_spriteBatch);
            _block.Draw(_spriteBatch);
            _spriteBatch.End();

            _debugView.RenderDebugData(ref projection);

            base.Draw(gameTime);
        }
    }
}
