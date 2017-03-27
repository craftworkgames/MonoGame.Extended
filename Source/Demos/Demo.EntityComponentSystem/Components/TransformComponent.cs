using MonoGame.Extended.Entities;

namespace Demo.EntityComponentSystem.Components
{
    [Component]
    //[ComponentPool(InitialSize = 5, CanResize = true, ResizeSize = 20, IsThreadSafe = false)]
    public class TransformComponent : TransformComponent2D
    {
    }
}
