using MonoGame.Extended.Entities;

namespace Demo.Platformer.Entities.Components
{
    [EntityComponent]
    [EntityComponentPool(InitialSize = 100)]
    public class CharacterComponent : EntityComponent
    {
        public int HealthPoints { get; set; }
        public bool IsAlive => HealthPoints > 0;

        public override void Reset()
        {
            HealthPoints = 3;
        }
    }
}