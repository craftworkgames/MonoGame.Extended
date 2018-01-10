using MonoGame.Extended.Entities;

namespace MonoGame.Extended.Gui.Tests.Implementation
{
    [EntityComponent]
    public class EntityComponentBasic : TransformComponent2D
    {
        public int Number { get; set; }
    }
}