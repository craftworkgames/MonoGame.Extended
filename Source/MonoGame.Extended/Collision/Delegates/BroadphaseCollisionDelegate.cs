namespace MonoGame.Extended.Collision
{
    public delegate void BroadphaseCollisionDelegate(Collider2D firstCollider, Collider2D secondCollider, out bool cancel);
}
