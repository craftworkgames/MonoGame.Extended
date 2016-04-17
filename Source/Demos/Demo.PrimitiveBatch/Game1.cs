using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Graphics.Batching;

namespace Demo.PrimitiveBatch
{
    public class Game1 : Game
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly GraphicsDeviceManager _graphicsDeviceManager;

        private BasicEffect _basicEffect;
        private PrimitiveBatch<VertexPositionColor> _primitiveBatchPositionColor;
        private PrimitiveBatch<VertexPositionColorTexture> _primitiveBatchPositionColorTexture;
        private Texture2D _texture;
        private FaceVertexPolygonMesh<VertexPositionColor> _polygonMesh;
        private Matrix _cartesianProjection2D;
        private Matrix _cartesianCamera2D;
        private Matrix _cartesianWorld;
        private Matrix _spriteBatchProjection;
        private Matrix _spriteBatchCamera;
        private Matrix _spriteBatchWorld;
        private float _spriteRotation;

        public Game1()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            Window.Position = Point.Zero;
        }

        protected override void LoadContent()
        {
            // load the texture for drawing later
            _texture = Content.Load<Texture2D>("logo-square-128");
            
            // get a reference to the graphics device
            var graphicsDevice = GraphicsDevice;

            // set the state of the graphics device once; we are not going to change it again in this demo
            graphicsDevice.BlendState = BlendState.AlphaBlend;
            graphicsDevice.RasterizerState = RasterizerState.CullNone;
            graphicsDevice.DepthStencilState = DepthStencilState.None;
            graphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

            // viewport: the dimensions and properties of the drawable surface
            var viewport = graphicsDevice.Viewport;

            // projection matrix: the mapping from View or Camera space to Projection space so the GPU knows what information from the scene is to be rendered 
            // here we create an orthographic projection; a 3D box in screen space (one side is the screen) where any primitives outside this box is not rendered
            // here the box is set so the origin (0,0,0) is the centre of the box's surface, thus the origin for drawing primitives on the screen is the centre aswell
            _cartesianProjection2D = Matrix.CreateTranslation(-0.5f, -0.5f, 0) * Matrix.CreateOrthographicOffCenter(-viewport.Width * 0.5f, viewport.Width * 0.5f, -viewport.Height * 0.5f, viewport.Height * 0.5f, 0, 1);

            // view matrix: the camera; use to transform primitives from World space to View (or Camera) space
            // here we don't do anything by using the identity matrix
            _cartesianCamera2D = Matrix.Identity;

            // world matrix: the coordinate system of the world or universe used to transform primitives from their own Local space to the World space
            // here we scale the x, y and z axis by 100 units
            _cartesianWorld = Matrix.CreateScale(new Vector3(100, 100, 100));

            // projection matrix: the mapping from View or Camera space to Projection space so the GPU knows what information from the scene is to be rendered 
            // here we create an orthographic projection; a 3D box in screen space (one side is the screen) where any primitives outside this box is not rendered
            // here the box is set so the origin (0,0,0) is the top-left of the box's surface, thus the origin for drawing primitives on the screen is the top-left aswell
            // the Z axis is also flipped by setting the near plane to 0 and the far plane to -1. (by default -Z is into the screen, +Z is popping out of the screen)
            _spriteBatchProjection = Matrix.CreateTranslation(-0.5f, -0.5f, 0) * Matrix.CreateOrthographicOffCenter(0, viewport.Width, viewport.Height, 0, 0, -1);

            // view matrix: the camera; use to transform primitives from World space to View (or Camera) space
            // here we don't do anything by using the identity matrix
            _spriteBatchCamera = Matrix.Identity;

            // world matrix: the coordinate system of the world or universe used to transform primitives from their own Local space to the World space
            // here we don't do anything by using the identity matrix leaving screen pixel units as world units
            _spriteBatchWorld = Matrix.Identity;

            // use a stock vertex and pixel shader
            _basicEffect = new BasicEffect(graphicsDevice);

            // create the VertexPositionColor PrimitiveBatch
            _primitiveBatchPositionColor = new PrimitiveBatch<VertexPositionColor>(graphicsDevice, Array.Sort);
            // create the VertexPositionColorTexture PrimitiveBatch
            _primitiveBatchPositionColorTexture = new PrimitiveBatch<VertexPositionColorTexture>(graphicsDevice, Array.Sort);

            // create our polygon mesh; vertices are in Local space; indices are index references to the vertices to draw 
            // indices have to multiple of 3 for PrimitiveType.TriangleList which says to draw a collection of triangles each with 3 vertices (triangles can share vertices)
            // TriangleList is the recommended way to have vertices layed out in memory for uploading to the GPU these days
            var vertices = new[]
            {
                new VertexPositionColor(new Vector3(0, 0, 0), Color.Red),
                new VertexPositionColor(new Vector3(2, 0, 0), Color.Blue),
                new VertexPositionColor(new Vector3(1, 2, 0), Color.Green),
                new VertexPositionColor(new Vector3(3, 2, 0), Color.White)
            };
            var indices = new short[]
            {
                0,
                1,
                2,
                2,
                1,
                3
            };
            _polygonMesh = new FaceVertexPolygonMesh<VertexPositionColor>(PrimitiveType.TriangleList, vertices, indices);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // clear the (pixel) buffers to a specific color
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // change the state of the basic effect for cartesian universe drawing
            _basicEffect.VertexColorEnabled = true;
            _basicEffect.TextureEnabled = false;
            _basicEffect.World = _cartesianWorld;
            _basicEffect.View = _cartesianCamera2D;
            _basicEffect.Projection = _cartesianProjection2D;

            // draw the polygon mesh in the cartesian universe using the VertexPositionColor PrimitiveBatch
            _primitiveBatchPositionColor.Begin(BatchSortMode.Immediate, _basicEffect);
            _primitiveBatchPositionColor.DrawPolygonMesh(_polygonMesh);
            _primitiveBatchPositionColor.End();

            // change the state of the basic effect for classic sprite drawing to screen universe just like SpriteBatch
            _basicEffect.VertexColorEnabled = true;
            _basicEffect.TextureEnabled = true;
            _basicEffect.World = _spriteBatchWorld;
            _basicEffect.View = _spriteBatchCamera;
            _basicEffect.Projection = _spriteBatchProjection;
            _basicEffect.Texture = _texture;

            // draw the sprite in the screen universe using the VertexPositionColorTexture PrimitiveBatch
            _primitiveBatchPositionColorTexture.Begin(BatchSortMode.Immediate, _basicEffect);
            var viewport = GraphicsDevice.Viewport;
            var textureRegion = new TextureRegion<Texture2D>(_texture, null);
            var spriteColor = new SpriteColor(Color.Black, Color.Black, Color.White, Color.White);
            var spriteOrigin = new Vector2(_texture.Width * 0.5f, _texture.Height * 0.5f);
            var spritePosition = new Vector2(viewport.Width * 0.25f, viewport.Height * 0.25f);
            var spriteDepth = 0f;
            _spriteRotation += MathHelper.ToRadians(1);
            _primitiveBatchPositionColorTexture.DrawSprite(textureRegion, new Vector3(spritePosition, spriteDepth), color: spriteColor, rotation: _spriteRotation, origin: spriteOrigin);
            _primitiveBatchPositionColorTexture.End();

            base.Draw(gameTime);
        }
    }
}
