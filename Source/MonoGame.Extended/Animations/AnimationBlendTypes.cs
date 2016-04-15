namespace MonoGame.Extended.Animations
{
    public enum AnimationBlendType
    {
        /// <summary>
        /// Will override similar tracks on first animation
        /// </summary>
        OverrideFirst,
        /// <summary>
        /// Will override similar tracks on second animation
        /// </summary>
        OverrideLast,
        /// <summary>
        /// Will completely remove tracks of first animation
        /// </summary>
        DeleteFirst,
        /// <summary>
        /// Will combine tracks of first animation
        /// </summary>
        Insert
    }
}