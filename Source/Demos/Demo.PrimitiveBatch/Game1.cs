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

        private EffectDrawContext<BasicEffect> _basicEffectDrawContext;
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
            // we have the bottom of the box as 0, the top as the viewport height (so +Y is pointing up), left as 0 and right as the viewport width
            // we also flip the Z axis by setting the near plane to 0 and the far plane to -1. (by default -Z is into the screen, +Z is popping out of the screen)
            // this gives us an origin of (0,0,0) for the bottom left of the box, and thus the origin for drawing primitives on the screen is the bottom left aswell
            _cartesianProjection2D = Matrix.CreateTranslation(-0.5f, -0.5f, 0) * Matrix.CreateOrthographicOffCenter(0, viewport.Width, 0, viewport.Height, 0, -1);

            // view matrix: the camera; use to transform primitives from World space to View (or Camera) space
            // here we translate (move) the origin, the point (0,0,0), from the bottom left of our 3D projection box to (width/2, height/2, 0)
            // this allows to "plot" points onto the graph starting at the centre just like we learned in math class
            _cartesianCamera2D = Matrix.CreateTranslation(new Vector3(viewport.Width * 0.5f, viewport.Height * 0.5f, 0));

            // world matrix: the coordinate system of the world or universe used to transform primitives from their own Local space to the World space
            // here we scale the x, y and z axis by 100 units
            _cartesianWorld = Matrix.CreateScale(new Vector3(100, 100, 100));

            // projection matrix: the mapping from View or Camera space to Projection space so the GPU knows what information from the scene is to be rendered 
            // here we create an orthographic projection; a 3D box in screen space (one side is the screen) where any primitives outside this box is not rendered
            // have the bottom of the box as viewport height, the top as 0 (so +Y is pointing down), left as 0 and right as the viewport width
            // we also flip the Z axis by setting the near plane to 0 and the far plane to -1. (by default -Z is into the screen, +Z is popping out of the screen)
            // this gives us an origin of (0,0,0) for the top left of the box, and thus the origin for drawing primitives on the screen is the top left aswell
            _spriteBatchProjection = Matrix.CreateTranslation(-0.5f, -0.5f, 0) * Matrix.CreateOrthographicOffCenter(0, viewport.Width, viewport.Height, 0, 0, -1);

            // view matrix: the camera; use to transform primitives from World space to View (or Camera) space
            // here we don't do anything by using the identity matrix
            _spriteBatchCamera = Matrix.Identity;

            // world matrix: the coordinate system of the world or universe used to transform primitives from their own Local space to the World space
            // here we don't do anything by using the identity matrix leaving screen pixel units as world units
            _spriteBatchWorld = Matrix.Identity;

            // use a stock vertex and pixel shader
            var basicEffect = new BasicEffect(graphicsDevice);
            // create a context for drawing with PrimitiveBatch
            _basicEffectDrawContext = new EffectDrawContext<BasicEffect>(basicEffect);

            // create the VertexPositionColor PrimitiveBatch using our context we just created as the default
            _primitiveBatchPositionColor = new PrimitiveBatch<VertexPositionColor>(graphicsDevice, defaultDrawContext: _basicEffectDrawContext);
            // create the VertexPositionColorTexture PrimitiveBatch using our context we just created as the default
            _primitiveBatchPositionColorTexture = new PrimitiveBatch<VertexPositionColorTexture>(graphicsDevice, defaultDrawContext: _basicEffectDrawContext);

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
 
            // get a reference to the basic effect
            var basicEffect = _basicEffectDrawContext.Effect;

            // change the state the basic effect draw context for cartesian universe drawing
            basicEffect.VertexColorEnabled = true;
            basicEffect.TextureEnabled = false;
            basicEffect.World = _cartesianWorld;
            basicEffect.View = _cartesianCamera2D;
            basicEffect.Projection = _cartesianProjection2D;

            // draw the polygon mesh in the cartesian universe using the VertexPositionColor PrimitiveBatch
            _primitiveBatchPositionColor.Begin(BatchSortMode.Immediate);
            _primitiveBatchPositionColor.DrawPolygonMesh(_polygonMesh);
            _primitiveBatchPositionColor.End();

            // change the state the basic effect draw context for classic sprite drawing to screen universe just like SpriteBatch
            basicEffect.VertexColorEnabled = true;
            basicEffect.TextureEnabled = true;
            basicEffect.World = _spriteBatchWorld;
            basicEffect.View = _spriteBatchCamera;
            basicEffect.Projection = _spriteBatchProjection;
            basicEffect.Texture = _texture;

            // draw the sprite in the screen universe using the VertexPositionColorTexture PrimitiveBatch
            _primitiveBatchPositionColorTexture.Begin(BatchSortMode.Immediate);
            var spriteColor = new SpriteColor(Color.White, Color.Black, Color.White, Color.White);
            _primitiveBatchPositionColorTexture.DrawSprite(_texture, Vector3.Zero, color: spriteColor);
            _primitiveBatchPositionColorTexture.End();

            base.Draw(gameTime);
        }
    }
}
