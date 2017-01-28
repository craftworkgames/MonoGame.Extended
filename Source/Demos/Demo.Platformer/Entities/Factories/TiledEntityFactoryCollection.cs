using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Tiled;
using System.Collections.Generic;

namespace Demo.Platformer.Entities.Factories
{
    public class TiledEntityFactoryCollection : Dictionary<string, ITiledEntityFactory>
    {
        public void LoadContent(ContentManager content)
        {
            foreach (var factory in Values)
                factory.LoadContent(content);
        }

        public void BuildFromMap(EntityComponentSystem ecs, TiledMap map)
        {
            foreach (var objectLayer in map.ObjectLayers)
            {
                foreach (var tiledObject in objectLayer.Objects)
                    ecs.CreateEntity(tiledObject.Type, e => this[tiledObject.Type].BuildEntity(e, tiledObject));
            }
        }

        #region Unimplemented

        //public Entity CreateBloodExplosion(Vector2 position, float totalSeconds = 1.0f)
        //{
        //    var random = new FastRandom();
        //    var textureRegion = _characterTextureAtlas[0];
        //    var entity = _entityComponentSystem.CreateEntity(position);
        //    var profile = Profile.Spray(new Vector2(0, -1), MathHelper.Pi);
        //    var term = TimeSpan.FromSeconds(totalSeconds);
        //    var particleEmitter = new ParticleEmitter(textureRegion, 32, term, profile)
        //    {
        //        Parameters = new ParticleReleaseParameters
        //        {
        //            Speed = new Range<float>(140, 200),
        //            Quantity = new Range<int>(32, 64),
        //            Rotation = new Range<float>(-MathHelper.TwoPi, MathHelper.TwoPi)
        //        },
        //        Modifiers = new IModifier[]
        //        {
        //            new LinearGravityModifier { Direction = Vector2.UnitY, Strength = 350 },
        //            new OpacityFastFadeModifier(),
        //            new RotationModifier { RotationRate = random.NextSingle(-MathHelper.TwoPi, MathHelper.TwoPi) }
        //        }
        //    };
        //    entity.AttachComponent(particleEmitter);
        //    entity.Destroy(delaySeconds: totalSeconds);
        //    return entity;
        //}

        #endregion
    }
}
