using System;
using MonoGame.Extended.Tilemaps;

namespace MonoGame.Extended.Tests;

public sealed class TilemapTileAnimationTests
{
    [Fact]
    public void TotalDuration_SumsAllFrameDurations()
    {
        TilemapTileAnimationFrame[] frames =
        [
            new TilemapTileAnimationFrame(0, 0.1f),
                new TilemapTileAnimationFrame(1, 0.2f),
                new TilemapTileAnimationFrame(2, 0.3f)
        ];
        TilemapTileAnimation animation = new TilemapTileAnimation(frames);

        Assert.Equal(0.6f, animation.TotalDuration, 0.001f);
    }

    [Fact]
    public void Update_WithSmallDelta_StaysOnSameFrame()
    {
        TilemapTileAnimationFrame[] frames =
        [
            new TilemapTileAnimationFrame(0, 0.5f),
                new TilemapTileAnimationFrame(1, 0.5f)
        ];
        TilemapTileAnimation animation = new TilemapTileAnimation(frames);

        animation.Update(0.1f);

        Assert.Equal(0, animation.CurrentFrameIndex);
        Assert.Equal(0, animation.CurrentFrame.TileId);
    }

    [Fact]
    public void Update_WithExactFrameDuration_AdvancesToNextFrame()
    {
        TilemapTileAnimationFrame[] frames =
        [
            new TilemapTileAnimationFrame (0, 0.5f),
                new TilemapTileAnimationFrame (1, 0.5f)
        ];
        TilemapTileAnimation animation = new TilemapTileAnimation(frames);

        animation.Update(0.5f);

        Assert.Equal(1, animation.CurrentFrameIndex);
        Assert.Equal(1, animation.CurrentFrame.TileId);
    }

    [Fact]
    public void Update_WithLargeDelta_AdvancesMultipleFrames()
    {
        TilemapTileAnimationFrame[] frames =
        [
            new(0, 0.1f),
                new(1, 0.1f),
                new(2, 0.1f)
        ];
        TilemapTileAnimation animation = new TilemapTileAnimation(frames);

        // 0.25s: crosses frame 0 (0.1s) and frame 1 (0.1s), lands in frame 2
        animation.Update(0.25f);

        Assert.Equal(2, animation.CurrentFrameIndex);
        Assert.Equal(2, animation.CurrentFrame.TileId);
    }

    [Fact]
    public void Update_WhenReachingEnd_WrapsToBeginning()
    {
        TilemapTileAnimationFrame[] frames =
        [
            new(0, 0.1f),
                new(1, 0.1f)
        ];
        TilemapTileAnimation animation = new TilemapTileAnimation(frames);

        // 0.25s exceeds total duration of 0.2s, wrapping 0.05s into the first frame
        animation.Update(0.25f);

        Assert.Equal(0, animation.CurrentFrameIndex);
        Assert.Equal(0, animation.CurrentFrame.TileId);
    }

    [Fact]
    public void Reset_ResetsToFirstFrame()
    {
        TilemapTileAnimationFrame[] frames =
        [
            new TilemapTileAnimationFrame(0, 0.1f),
                new TilemapTileAnimationFrame(1, 0.1f)
        ];
        TilemapTileAnimation animation = new TilemapTileAnimation(frames);
        animation.Update(0.15f);

        animation.Reset();

        Assert.Equal(0, animation.CurrentFrameIndex);
    }

    [Fact]
    public void GetFrameAtTime_WithinFirstFrame_ReturnsFirstFrame()
    {
        TilemapTileAnimationFrame[] frames =
        [
            new TilemapTileAnimationFrame(10, 0.1f),
                new TilemapTileAnimationFrame(11, 0.2f)
        ];
        TilemapTileAnimation animation = new TilemapTileAnimation(frames);

        Assert.Equal(10, animation.GetFrameAtTime(0.05f).TileId);
    }

    [Fact]
    public void GetFrameAtTime_InSecondFrame_ReturnsSecondFrame()
    {
        TilemapTileAnimationFrame[] frames =
        [
            new TilemapTileAnimationFrame(10, 0.1f),
                new TilemapTileAnimationFrame(11, 0.2f),
                new TilemapTileAnimationFrame(12, 0.3f)
        ];
        TilemapTileAnimation animation = new TilemapTileAnimation(frames);

        Assert.Equal(11, animation.GetFrameAtTime(0.15f).TileId);
    }

    [Fact]
    public void GetFrameAtTime_BeyondTotalDuration_Wraps()
    {
        TilemapTileAnimationFrame[] frames =
        [
            new TilemapTileAnimationFrame(10, 0.1f),
                new TilemapTileAnimationFrame(11, 0.2f)
        ];
        TilemapTileAnimation animation = new TilemapTileAnimation(frames);

        // 0.35s exceeds total duration of 0.3s, wrapping 0.05s into the first frame
        Assert.Equal(10, animation.GetFrameAtTime(0.35f).TileId);
    }

    [Fact]
    public void GetFrameAtTime_WithNegativeTime_WrapsFromEnd()
    {
        TilemapTileAnimationFrame[] frames =
        [
            new TilemapTileAnimationFrame(10, 0.1f),
                new TilemapTileAnimationFrame(11, 0.2f)
        ];
        TilemapTileAnimation animation = new TilemapTileAnimation(frames);

        Assert.Equal(11, animation.GetFrameAtTime(-0.1f).TileId);
    }

    [Fact]
    public void GetFrameAtTime_WithEmptyFrames_ThrowsInvalidOperationException()
    {
        TilemapTileAnimation animation = new TilemapTileAnimation(Array.Empty<TilemapTileAnimationFrame>());

        Assert.Throws<InvalidOperationException>(() => animation.GetFrameAtTime(0f));
    }

    [Fact]
    public void GetFrameAtTime_DoesNotAffectCurrentState()
    {
        TilemapTileAnimationFrame[] frames =
        [
            new TilemapTileAnimationFrame(10, 0.1f),
                new TilemapTileAnimationFrame(11, 0.2f)
        ];
        TilemapTileAnimation animation = new TilemapTileAnimation(frames);

        TilemapTileAnimationFrame frame1 = animation.GetFrameAtTime(0.15f);
        TilemapTileAnimationFrame frame2 = animation.GetFrameAtTime(0.25f);

        Assert.Equal(11, frame1.TileId);
        Assert.Equal(11, frame2.TileId);
        Assert.Equal(0, animation.CurrentFrameIndex);
        Assert.Equal(10, animation.CurrentFrame.TileId);
    }
}
