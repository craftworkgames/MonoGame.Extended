using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.TextureAtlases;

namespace Demo.PrimitiveBatch
{
    public class Game1 : Game
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly GraphicsDeviceManager _graphicsDeviceManager;

        // dynamic batcher
        private GeometryBatch2D _batch;

        // texture used by the sprite
        private Texture2D _spriteTexture;
        private TextureRegion2D _spriteTextureRegion;

//        // the polygon
//        private VertexPositionColor[] _polygonVertices;
//        private short[] _polygonIndices;
//        // the curve (continous line segements)
//        private VertexPositionColor[] _curveVertices;

        // the rotation angle of the sprite
        private float _spriteRotation;
//
//        // primitives matrix transformation chain
//        private Matrix _primitivesModelToWorld;
//        private Matrix _primitivesWorldToView;
//        private Matrix _primitivesViewToProjetion;

        public Game1()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(game: this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
        }

        protected override void LoadContent()
        {
            var graphicsDevice = GraphicsDevice;

            _batch = new GeometryBatch2D(graphicsDevice);

//            var viewport = graphicsDevice.Viewport;

//            // the transformation used to transform primitives from their model space to the world space
//            // here we scale the x, y and z axes by 100 units
//            _primitivesModelToWorld = Matrix.CreateScale(scales: new Vector3(x: 100, y: 100, z: 100));
//
//            // the camera transformation used to transform primitives from world space to a view space
//            // here we don't do anything by using the identity matrix
//            _primitivesWorldToView = Matrix.Identity;
//
//            // the transformation used to transform primitives from view space to projection space
//            // here we create an orthographic projection; a 3D box where any primitives outside this box are not rendered
//            // here the box is created off an origin point (0,0,0) which is the centre of the screen's surface
//            _primitivesViewToProjetion = Matrix.CreateOrthographicOffCenter(left: viewport.Width * -0.5f, right: viewport.Width * 0.5f, bottom: viewport.Height * -0.5f, top: viewport.Height * 0.5f, zNearPlane: 0, zFarPlane: 1);

            // load the texture for the sprites
            _spriteTexture = Content.Load<Texture2D>(assetName: "logo-square-128");
            _spriteTextureRegion = new TextureRegion2D(_spriteTexture);
           
//            // create our polygon mesh; vertices are in Local space; indices are index references to the vertices to draw 
//            // indices have to multiple of 3 for PrimitiveType.TriangleList which says to draw a collection of triangles each with 3 vertices (different triangles can share vertices) 
//            // here we have 2 triangles in the list to form a quad or rectangle: http://wiki.lwjgl.org/images/a/a8/QuadVertices.png
//            // TriangleList is the most common scenario to have polygon vertices layed out in memory for uploading to the GPU
//            _polygonVertices = new[]
//            {
//                new VertexPositionColor(position: new Vector3(x: 0, y: 0, z: 0), color: Color.Red),
//                new VertexPositionColor(position: new Vector3(x: 2, y: 0, z: 0), color: Color.Blue),
//                new VertexPositionColor(position: new Vector3(x: 1, y: 2, z: 0), color: Color.Green),
//                new VertexPositionColor(position: new Vector3(x: 3, y: 2, z: 0), color: Color.White)
//            };
//            _polygonIndices = new short[]
//            {
//                1,
//                0,
//                2,
//                1,
//                2,
//                3
//            };
//
//            // create our curve as an approximation by a series of line segments; vertices are in Local space; no indices
//            // LineStrip joins the vertices given in order into a continuous series of line segments
//            var curveVertices = new List<VertexPositionColor>();
//            var angleStep = MathHelper.ToRadians(degrees: 1);
//            const int circlesCount = 3;
//            for (var angle = 0.0f; angle <= MathHelper.TwoPi * circlesCount; angle += angleStep)
//            {
//                var vertexPosition = new Vector3(x: (float)Math.Sin(angle) - angle / 10, y: (float)Math.Cos(angle) - angle / 15, z: 0);
//                var vertex = new VertexPositionColor(vertexPosition, Color.White);
//                curveVertices.Add(vertex);
//            }
//            _curveVertices = curveVertices.ToArray();
        }

        protected override void Update(GameTime gameTime)
        {
            if ((GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // clear the (pixel) buffer to a specific color
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // set the states for rendering
            // this could be moved outside the render loop if it doesn't change frame per frame 
            // however, it's left here indicating it's possible and common to change the state between frames
            // use alphablend so the transparent part of the texture is blended with the color behind it
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

            // draw the polygon and curve in the cartesian coordinate system using the VertexPositionColor PrimitiveBatch
            //            _primitiveBatchPositionColor.Begin();
            //            _primitiveBatchPositionColor.Draw(_primitiveMaterial, PrimitiveType.TriangleList, _polygonVertices, _polygonIndices);
            //            _primitiveBatchPositionColor.Draw(_primitiveMaterial, PrimitiveType.LineStrip, _curveVertices);
            //            _primitiveBatchPositionColor.End();

            // draw the sprite
            _batch.Begin();
            var spriteOrigin = new Vector2(x: _spriteTextureRegion.Texture.Width * 0.5f, y: _spriteTextureRegion.Texture.Height * 0.5f);
            var spritePosition = new Vector2(x: 150, y: 150);
            _spriteRotation += MathHelper.ToRadians(degrees: 1);
            var matrix = Matrix2D.CreateScale(Vector2.One) * Matrix2D.CreateRotationZ(-_spriteRotation) * Matrix2D.CreateTranslation(spritePosition);
            _batch.DrawSprite(_spriteTextureRegion, ref matrix, origin: spriteOrigin);
            _batch.End();

            base.Draw(gameTime);
        }
    }
}
