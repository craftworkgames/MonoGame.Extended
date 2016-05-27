using System;
using System.Collections.Generic;
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

        // primitive batch for geometric primitives such as convex polygons and lines
        private PrimitiveBatch<VertexPositionColor> _primitiveBatchPositionColor;
        // primitive batch for sprites (quads with texture)
        private PrimitiveBatch<VertexPositionColorTexture> _primitiveBatchPositionColorTexture;

        // a material for the geometric primitives
        private PrimitiveEffectMaterial _primitiveMaterial;
        // a material for the sprites 
        // a new material will be required for each texture
        private SpriteEffectMaterial _spriteMaterial; 

        // the polygon
        private VertexPositionColor[] _polygonVertices;
        private short[] _polygonIndices;
        // the curve (continous line segements)
        private VertexPositionColor[] _curveVertices;

        // the rotation angle of the sprite
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
            // get a reference to the graphics device
            var graphicsDevice = GraphicsDevice;

            // viewport: the dimensions and properties of the drawable surface
            var viewport = graphicsDevice.Viewport;

            // load the custom effect for the primitives
            var primitiveEffect = new PrimitiveEffect(Content.Load<Effect>("PrimitiveEffect"))
            {
                // world matrix: the coordinate system of the world or universe used to transform primitives from their own Local space to the World space
                // here we scale the x, y and z axes by 100 units
                World = Matrix.CreateScale(new Vector3(100, 100, 100)),
                // view matrix: the camera; use to transform primitives from World space to View (or Camera) space
                // here we don't do anything by using the identity matrix
                View = Matrix.Identity,
                // projection matrix: the mapping from View or Camera space to Projection space so the GPU knows what information from the scene is to be rendered 
                // here we create an orthographic projection; a 3D box in screen space (one side is the screen) where any primitives outside this box is not rendered
                // here the box is setup so the origin (0,0,0) is the centre of the screen's surface
                Projection = Matrix.CreateOrthographicOffCenter(viewport.Width * -0.5f, viewport.Width * 0.5f, viewport.Height * -0.5f, viewport.Height * 0.5f, 0, 1)
            };

            // create a material for rendering polygons
            _primitiveMaterial = new PrimitiveEffectMaterial(primitiveEffect);

            // load the custom effect for the sprites
            var spriteEffect = new SpriteEffect(Content.Load<Effect>("SpriteEffect"))
            {
                // world matrix: the coordinate system of the world or universe used to transform primitives from their own Local space to the World space
                // here we don't do anything by using the identity matrix leaving screen pixel units as world units
                World = Matrix.Identity,
                // view matrix: the camera; use to transform primitives from World space to View (or Camera) space
                // here we don't do anything by using the identity matrix
                View = Matrix.Identity,
                // projection matrix: the mapping from View or Camera space to Projection space so the GPU knows what information from the scene is to be rendered 
                // here we create an orthographic projection; a 3D box in screen space (one side is the screen) where any primitives outside this box is not rendered
                // here the box is set so the origin (0,0,0) is the top-left of the screen's surface
                // the Z axis is also flipped by setting the near plane to 0 and the far plane to -1. (by default -Z is into the screen, +Z is popping out of the screen)
                // here an adjustment by half a pixel is also added because there’s a discrepancy between how the centers of pixels and the centers of texels are computed
                Projection = Matrix.CreateTranslation(-0.5f, -0.5f, 0) * Matrix.CreateOrthographicOffCenter(0, viewport.Width, viewport.Height, 0, 0, -1)
            };
            // load the texture for the sprites
            var spriteTexture = Content.Load<Texture2D>("logo-square-128");
            // create a material for rendering sprites
            // each texture will need a seperate material
            _spriteMaterial = new SpriteEffectMaterial(spriteEffect, spriteTexture);

            // create the VertexPositionColor PrimitiveBatch for rendering the primitives
            _primitiveBatchPositionColor = new PrimitiveBatch<VertexPositionColor>(graphicsDevice, Array.Sort);
            // create the VertexPositionColorTexture PrimitiveBatch for rendering the sprites
            _primitiveBatchPositionColorTexture = new PrimitiveBatch<VertexPositionColorTexture>(graphicsDevice, Array.Sort);

            // create our polygon mesh; vertices are in Local space; indices are index references to the vertices to draw 
            // indices have to multiple of 3 for PrimitiveType.TriangleList which says to draw a collection of triangles each with 3 vertices (different triangles can share vertices) 
            // here we have 2 triangles in the list to form a quad or rectangle: http://wiki.lwjgl.org/images/a/a8/QuadVertices.png
            // TriangleList is the most common scenario to have polygon vertices layed out in memory for uploading to the GPU
            _polygonVertices = new[]
            {
                new VertexPositionColor(new Vector3(0, 0, 0), Color.Red),
                new VertexPositionColor(new Vector3(2, 0, 0), Color.Blue),
                new VertexPositionColor(new Vector3(1, 2, 0), Color.Green),
                new VertexPositionColor(new Vector3(3, 2, 0), Color.White)
            };
            _polygonIndices = new short[]
            {
                1,
                0,
                2,
                1,
                2,
                3,
            };

            // create our curve as an approximation by a series of line segments; vertices are in Local space; no indices
            // LineStrip joins the vertices given in order into a continuous series of line segments
            var curveVertices = new List<VertexPositionColor>();
            var angleStep = MathHelper.ToRadians(1);
            const int circlesCount = 3;
            for (var angle = 0.0f; angle <= MathHelper.TwoPi * circlesCount; angle += angleStep)
            {
                var vertexPosition = new Vector3((float)Math.Sin(angle) - angle / 10, (float)Math.Cos(angle) - angle / 15, 0);
                var vertex = new VertexPositionColor(vertexPosition, Color.White);
                curveVertices.Add(vertex);
            }
            _curveVertices = curveVertices.ToArray();
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

            // set the states for rendering
            // this could be moved outside the render loop if it doesn't change frame per frame 
            // however, it's left here indicating it's possible and common to change the state between frames
            // use alphablend so the transparent part of the texture is blended with the color behind it
            GraphicsDevice.BlendState = BlendState.AlphaBlend;

            // draw the polygon and curve in the cartesian coordinate system using the VertexPositionColor PrimitiveBatch
            _primitiveBatchPositionColor.Begin();
            _primitiveBatchPositionColor.Draw(_primitiveMaterial, PrimitiveType.TriangleList, _polygonVertices, _polygonIndices);
            _primitiveBatchPositionColor.Draw(_primitiveMaterial, PrimitiveType.LineStrip, _curveVertices);
            _primitiveBatchPositionColor.End();

            // draw the sprite in the screen coordinate system using the VertexPositionColorTexture PrimitiveBatch
            _primitiveBatchPositionColorTexture.Begin(BatchMode.Immediate);
            var spriteColor = Color.White;
            var spriteOrigin = new Vector2(_spriteMaterial.Texture.Width * 0.5f, _spriteMaterial.Texture.Height * 0.5f);
            var spritePosition = new Vector2(150, 150);
            var spriteDepth = 0f;
            _spriteRotation += MathHelper.ToRadians(1);
            _primitiveBatchPositionColorTexture.DrawSprite(_spriteMaterial, null, new Vector3(spritePosition, spriteDepth), color: spriteColor, rotation: _spriteRotation, origin: spriteOrigin);
            _primitiveBatchPositionColorTexture.End();

            base.Draw(gameTime);
        }
    }
}
