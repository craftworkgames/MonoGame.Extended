
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.SimCamera
{
    public class Block
    {
        private Texture2D _texture;
        private Body _body;

        private float _blockVelocity = 3;

        public Block(World world, Vector2 position, Texture2D texture)
        {
            _body = BodyFactory.CreateRectangle(world, 1f, 1f, 1f);
            _body.BodyType = BodyType.Dynamic;
            _body.Position = position;

            _texture = texture;
        }

        public void Update(GameTime time)
        {
            bool anyKeyPressed = false;

            var vectorVelocity = new Vector2();

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                vectorVelocity.X -= _blockVelocity;
                anyKeyPressed = true;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                vectorVelocity.X += _blockVelocity;
                anyKeyPressed = true;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                vectorVelocity.Y -= _blockVelocity;
                anyKeyPressed = true;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                vectorVelocity.Y += _blockVelocity;
                anyKeyPressed = true;
            }

            if (anyKeyPressed)
            {
                _body.LinearVelocity = vectorVelocity;
            }
            else
            {
                _body.LinearVelocity = new Vector2(0, 0);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                _texture,
                ConvertSimUnits.ToDisplayUnits(_body.Position),
                null,
                Color.White,
                _body.Rotation,
                new Vector2(50, 50),
                1f,
                SpriteEffects.None,
                0f
            );
        }
    }
}
