using MonoGame.Extended.Entities.Components;

namespace Demo.Platformer.Entities.Components
{
    public class PlayerState : EntityComponent
    {
        public bool IsOnGround { get; set; }
    }
}