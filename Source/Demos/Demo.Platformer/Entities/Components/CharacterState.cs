using MonoGame.Extended.Entities.Components;

namespace Demo.Platformer.Entities.Components
{
    public class CharacterState : EntityComponent
    {
        public CharacterState()
        {
            HealthPoints = 3;
            IsJumping = false;
        }

        public int HealthPoints { get; set; }
        public bool IsJumping { get; set; }
        public bool IsAlive => HealthPoints > 0;
    }
}