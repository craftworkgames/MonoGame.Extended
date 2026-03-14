namespace MonoGame.Extended.Tilemaps.Rendering;

/// <summary>
/// Specifies how ungrouped layers are rendered.
/// </summary>
public enum RenderMode
{
    /// <summary>
    /// All ungrouped layers are merged into a single draw call.
    /// </summary>
    /// <remarks>
    /// Typically faster for maps with multiple layers due to reduced draw call overhead.
    /// </remarks>
    Merged,

    /// <summary>
    /// Each ungrouped layer is drawn individually.
    /// </summary>
    /// <remarks>
    /// May be faster for maps with frequently changing tiles, as only modified layers
    /// need rebuilding. Use benchmarks to determine best mode for your use case.
    /// </remarks>
    Separate
}
