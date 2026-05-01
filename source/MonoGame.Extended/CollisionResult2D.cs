using Microsoft.Xna.Framework;

namespace MonoGame.Extended;

/// <summary>
/// Represents the result of a two-dimensional collision query.
/// </summary>
/// <remarks>
/// This type is the shared result model for collision APIs that report resolution data in addition to intersection state.
/// For static <see cref="Collision2D"/> methods, <see cref="MinimumTranslationVector"/> is the translation that moves
/// the first queried shape out of the second queried shape.
/// For instance methods on bounding types, <see cref="MinimumTranslationVector"/> is the translation that moves the
/// receiver out of the argument.
/// </remarks>
public readonly struct CollisionResult2D
{
    /// <summary>
    /// A collision result that represents no intersection.
    /// </summary>
    /// <value>
    /// A result whose <see cref="Intersects"/> value is <see langword="false"/> and whose direction data is zero.
    /// </value>
    public static readonly CollisionResult2D None;

    /// <summary>
    /// A value indicating whether the queried shapes intersect.
    /// </summary>
    public readonly bool Intersects;

    /// <summary>
    /// The collision normal used to resolve the intersection.
    /// </summary>
    /// <value>
    /// A unit-length direction when collision-producing code provides one.
    /// For <see cref="None"/>, this value is <see cref="Vector2.Zero"/>.
    /// </value>
    public readonly Vector2 Normal;

    /// <summary>
    /// The distance the queried shapes overlap along the collision normal, measured in world units.
    /// </summary>
    public readonly float PenetrationDepth;

    /// <summary>
    /// The translation vector that resolves the collision using the result direction convention.
    /// </summary>
    /// <value>
    /// The translation that resolves the overlap according to the static or instance direction convention documented on this type.
    /// For <see cref="None"/>, this value is <see cref="Vector2.Zero"/>.
    /// </value>
    public readonly Vector2 MinimumTranslationVector;

    /// <summary>
    /// Initializes a new instance of the <see cref="CollisionResult2D"/> struct.
    /// </summary>
    /// <param name="intersects">A value indicating whether the queried shapes intersect.</param>
    /// <param name="normal">The collision normal used to resolve the intersection.</param>
    /// <param name="penetrationDepth">The overlap distance along the collision normal, measured in world units.</param>
    /// <param name="minimumTranslationVector">The translation vector that resolves the collision.</param>
    public CollisionResult2D(bool intersects, Vector2 normal, float penetrationDepth, Vector2 minimumTranslationVector)
    {
        Intersects = intersects;
        Normal = normal;
        PenetrationDepth = penetrationDepth;
        MinimumTranslationVector = minimumTranslationVector;
    }

    internal readonly CollisionResult2D Invert()
    {
        return new CollisionResult2D(
            Intersects,
            -Normal,
            PenetrationDepth,
            -MinimumTranslationVector);
    }
}
