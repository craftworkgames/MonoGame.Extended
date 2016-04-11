using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Animations;

namespace MonoGame.Extended.Entities
{
    public class AnimatorComponent : EntityComponent
    {
        public List<Animation> Animations { get; set; }
        public override void Update(GameTime gameTime) {
            base.Update(gameTime);
            foreach (var animation in Animations) {
                animation.Update(gameTime.ElapsedGameTime.TotalMilliseconds);
            }
        }
    }
}