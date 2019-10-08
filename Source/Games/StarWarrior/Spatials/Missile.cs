// Original code dervied from:
// https://github.com/thelinuxlich/starwarrior_CSharp/blob/master/StarWarrior/StarWarrior/Spatials/Missile.cs

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace StarWarrior.Spatials
{
    internal static class Missile
    {
        private static Texture2D _bullet;

        public static void Render(SpriteBatch spriteBatch, ContentManager contentManager, Transform2 transform)
        {
            if (_bullet == null)
                _bullet = contentManager.Load<Texture2D>("bullet");

            var worldPosition = transform.WorldPosition;
            var renderPosition = new Vector2(worldPosition.X - _bullet.Width * 0.5f, worldPosition.Y - _bullet.Height * 0.5f);
            spriteBatch.Draw(_bullet, renderPosition, _bullet.Bounds, Color.Red);
        }
    }
}