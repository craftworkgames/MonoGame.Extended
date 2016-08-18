namespace Demo.Platformer.Entities.Components
{
    public abstract class EntityComponent
    {
        protected EntityComponent()
        {
        }

        public Entity Entity { get; internal set; }
    }
}