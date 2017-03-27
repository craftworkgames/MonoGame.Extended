using System;
using MonoGame.Extended.Entities;

namespace Demo.Platformer.Entities.Components
{
    [Component]
    [ComponentPool(InitialSize = 1)]
    public class PlayerComponent : Component
    {
        public float WalkSpeed { get; set; }
        public float JumpSpeed { get; set; }
        public bool IsJumping { get; set; }

        public override void Reset()
        {
            base.Reset();

            WalkSpeed = 0;
            JumpSpeed = 0;
            IsJumping = false;
        }
    }
}
