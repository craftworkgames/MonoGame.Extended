using System;

namespace MonoGame.Extended.Tilemaps;

/// <summary>
/// Represents a frame-based animation for a tile.
/// </summary>
public class TilemapTileAnimation
{
    private float _elapsedTime;

    /// <summary>
    /// Gets the animation frames.
    /// </summary>
    public TilemapTileAnimationFrame[] Frames { get; }

    /// <summary>
    /// Gets the total duration of the animation in seconds.
    /// </summary>
    public float TotalDuration { get; }

    /// <summary>
    /// Gets the current frame index.
    /// </summary>
    public int CurrentFrameIndex { get; private set; }

    /// <summary>
    /// Gets the current frame.
    /// </summary>
    public TilemapTileAnimationFrame CurrentFrame
    {
        get
        {
            return Frames[CurrentFrameIndex];
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TilemapTileAnimation"/> class.
    /// </summary>
    /// <param name="frames">The animation frames.</param>
    public TilemapTileAnimation(TilemapTileAnimationFrame[] frames)
    {
        Frames = frames;

        float total = 0;
        foreach (TilemapTileAnimationFrame frame in frames)
        {
            total += frame.Duration;
        }

        TotalDuration = total;
    }

    /// <summary>
    /// Updates the animation.
    /// </summary>
    /// <param name="deltaTime">The time elapsed since the last update, in seconds.</param>
    public void Update(float deltaTime)
    {
        if (Frames.Length == 0)
        {
            return;
        }

        _elapsedTime += deltaTime;

        while (_elapsedTime >= Frames[CurrentFrameIndex].Duration)
        {
            _elapsedTime -= Frames[CurrentFrameIndex].Duration;
            CurrentFrameIndex = (CurrentFrameIndex + 1) % Frames.Length;
        }
    }

    /// <summary>
    /// Returns the animation frame that would be active at the specified playback time.
    /// </summary>
    /// <param name="time">The playback position in seconds. Values outside [0, TotalDuration) are wrapped.</param>
    /// <returns>The frame active at the specified time.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the animation has no frames.</exception>
    public TilemapTileAnimationFrame GetFrameAtTime(float time)
    {
        if (Frames.Length == 0)
        {
            throw new InvalidOperationException("Animation has no frames");
        }

        float totalDuration = TotalDuration;
        if (totalDuration <= 0)
        {
            return Frames[0];
        }

        time %= totalDuration;
        if (time < 0)
        {
            time += totalDuration;
        }

        float accumulated = 0;
        foreach (TilemapTileAnimationFrame frame in Frames)
        {
            accumulated += frame.Duration;
            if (time < accumulated)
            {
                return frame;
            }
        }

        // Should never reach here, but return last frame as fallback
        return Frames[Frames.Length - 1];
    }

    /// <summary>
    /// Resets the animation to the first frame.
    /// </summary>
    public void Reset()
    {
        CurrentFrameIndex = 0;
        _elapsedTime = 0;
    }
}
