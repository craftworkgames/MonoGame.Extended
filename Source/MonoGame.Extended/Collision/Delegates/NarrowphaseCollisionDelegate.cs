namespace MonoGame.Extended.Collision
{
    public delegate void NarrowphaseCollisionDelegate(Collider2D firstCollider, Collider2D secondCollider, out bool cancel);
}
