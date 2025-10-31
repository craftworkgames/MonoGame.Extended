using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tests.Fixtures;
using MonoGame.Extended.ViewportAdapters;

namespace MonoGame.Extended.Tests;

[Collection("GraphicsTest")]
public sealed class OrthographicCameraTests
{
    private readonly GraphicsTestFixture _graphicsFixture;

    public OrthographicCameraTests(GraphicsTestFixture graphicsTestFixture)
    {
        _graphicsFixture = graphicsTestFixture;
    }

    [Fact]
    public void SetPosition_WorldBoundsDisabled_SetsValueWithoutClamping()
    {
        OrthographicCamera camera = new OrthographicCamera(_graphicsFixture.GraphicsDevice);
        camera.DisableWorldBounds();
        Vector2 expectedPosition = new Vector2(100, 100);

        camera.Position = expectedPosition;

        Assert.Equal(expectedPosition, camera.Position);
    }

    [Fact]
    public void SetPosition_WorldBoundsEnabled_ClampsToMinimumBounds()
    {
        OrthographicCamera camera = new OrthographicCamera(_graphicsFixture.GraphicsDevice);
        Viewport viewport = _graphicsFixture.GraphicsDevice.Viewport;
        Rectangle worldBounds = new Rectangle(0, 0, viewport.Width * 2, viewport.Height * 2);
        camera.EnableWorldBounds(worldBounds);

        camera.Position = new Vector2(-100, -100);

        Vector2 expectedPosition = new Vector2(0, 0);
        Assert.Equal(expectedPosition, camera.Position);
    }

    [Fact]
    public void SetPosition_WorldBoundsEnabled_ClampsToMaximumBounds()
    {
        OrthographicCamera camera = new OrthographicCamera(_graphicsFixture.GraphicsDevice);
        Viewport viewport = _graphicsFixture.GraphicsDevice.Viewport;
        Rectangle worldBounds = new Rectangle(0, 0, viewport.Width * 2, viewport.Height * 2);
        camera.EnableWorldBounds(worldBounds);

        camera.Position = new Vector2(viewport.Width, viewport.Height) * 3;

        Vector2 expectedPosition = new Vector2(worldBounds.Right - viewport.Width, worldBounds.Bottom - viewport.Height);
        Assert.Equal(expectedPosition, camera.Position);
    }

    [Fact]
    public void SetPosition_WorldBoundsEnabled_DoesNotClampWhenWithinBounds()
    {
        OrthographicCamera camera = new OrthographicCamera(_graphicsFixture.GraphicsDevice);
        Viewport viewport = _graphicsFixture.GraphicsDevice.Viewport;
        Rectangle worldBounds = new Rectangle(0, 0, viewport.Width * 2, viewport.Height * 2);
        camera.EnableWorldBounds(worldBounds);

        Vector2 expectedPosition = new Vector2(viewport.Width, viewport.Height);
        camera.Position = expectedPosition;

        Assert.Equal(expectedPosition, camera.Position);
    }

    [Fact]
    public void SetPosition_WorldBoundsSmallerThanCamera_CentersOnWorldBounds()
    {
        OrthographicCamera camera = new OrthographicCamera(_graphicsFixture.GraphicsDevice);
        Rectangle worldBounds = new Rectangle(100, 200, 50, 50);
        camera.EnableWorldBounds(worldBounds);

        camera.Position = new Vector2(1000, 1000);

        Vector2 expectedCenter = worldBounds.Center.ToVector2();
        Assert.Equal(expectedCenter, camera.Center);
    }

    [Fact]
    public void SetZoom_DefaultLimits_SetsValueWithoutClamping()
    {
        OrthographicCamera camera = new OrthographicCamera(_graphicsFixture.GraphicsDevice);
        camera.DisableWorldBounds();
        float expectedZoom = 2.0f;

        camera.Zoom = expectedZoom;

        Assert.Equal(expectedZoom, camera.Zoom);
    }

    [Fact]
    public void SetZoom_BelowMinimumZoom_ClampsToMinimum()
    {
        OrthographicCamera camera = new OrthographicCamera(_graphicsFixture.GraphicsDevice);
        camera.DisableWorldBounds();
        camera.MinimumZoom = 1.0f;

        camera.Zoom = 0.9f;

        Assert.Equal(camera.MinimumZoom, camera.Zoom);
    }

    [Fact]
    public void SetZoom_AboveMaximumZoom_ClampsToMaximum()
    {
        OrthographicCamera camera = new OrthographicCamera(_graphicsFixture.GraphicsDevice);
        camera.DisableWorldBounds();
        camera.MaximumZoom = 1.0f;

        camera.Zoom = 1.1f;

        Assert.Equal(camera.MaximumZoom, camera.Zoom);
    }

    [Fact]
    public void SetZoom_WorldBoundsEnabled_BelowMinimumWorldBoundsZoom_ClampsToWorldBounds()
    {
        OrthographicCamera camera = new OrthographicCamera(_graphicsFixture.GraphicsDevice);
        Viewport viewport = _graphicsFixture.GraphicsDevice.Viewport;
        Rectangle worldBounds = new Rectangle(0, 0, viewport.Width * 2, viewport.Height * 2);
        camera.EnableWorldBounds(worldBounds);
        camera.IsZoomClampedToWorldBounds = true;

        // Viewport for testing is 800x480, so world bounds are 1600x960
        // Minimum zoom to keep view within bounds: max(800/1600, 480/960) = max(0.5, 0.5) = 0.5
        // So a zoom at 0.5 is at the world bounds minium, so we set lower than that to check clamping.
        camera.Zoom = 0.3f;

        Assert.Equal(0.5f, camera.Zoom);
    }

    [Fact]
    public void SetZoom_WorldBoundsEnabled_AboveMaximumWorldBoundsZoom_ClampsToWorldBounds()
    {
        OrthographicCamera camera = new OrthographicCamera(_graphicsFixture.GraphicsDevice);
        Viewport viewport = _graphicsFixture.GraphicsDevice.Viewport;
        Rectangle worldBounds = new Rectangle(0, 0, viewport.Width * 2, viewport.Height * 2);
        camera.EnableWorldBounds(worldBounds);
        camera.IsZoomClampedToWorldBounds = true;

        // Viewport for testing is 800x480, so world bounds are 1600x960
        // Minimum zoom to keep view within bounds: max(800/1600, 480/960) = max(0.5, 0.5) = 0.5
        // So a zoom at 0.5 is at the world bounds minium, so we set lower than that to check clamping.
        camera.Zoom = 0.3f;

        Assert.Equal(0.5f, camera.Zoom);
    }

    [Fact]
    public void SetZoom_ExplicitMaximumZoom_TakesPrecedenceOverWorldBoundsMinimum()
    {
        OrthographicCamera camera = new OrthographicCamera(_graphicsFixture.GraphicsDevice);
        Viewport viewport = _graphicsFixture.GraphicsDevice.Viewport;
        Rectangle worldBounds = new Rectangle(0, 0, viewport.Width * 2, viewport.Height * 2);
        camera.EnableWorldBounds(worldBounds);
        camera.IsZoomClampedToWorldBounds = true;

        // Set explicit maximum BELOW what world bounds minimum requires (0.5)
        camera.MaximumZoom = 0.4f;

        // Try to set zoom to world bounds minimum
        camera.Zoom = 0.5f;

        // Explicit MaximumZoom should take precedence
        Assert.Equal(0.4f, camera.Zoom);
    }

    [Fact]
    public void SetZoom_WorldBoundsEnabled_ClampsPositionAfterZoomChange()
    {
        OrthographicCamera camera = new OrthographicCamera(_graphicsFixture.GraphicsDevice);
        Viewport viewport = _graphicsFixture.GraphicsDevice.Viewport;
        Rectangle worldBounds = new Rectangle(0, 0, viewport.Width, viewport.Height);

        // Position camera at edge of world bounds
        camera.Position = new Vector2(viewport.Width, viewport.Height);

        camera.EnableWorldBounds(worldBounds);
        camera.IsZoomClampedToWorldBounds = true;

        // Zoom out
        // this should force position adjustment to keep view in bounds
        camera.Zoom = 0.5f;

        // Zoom clamped to 1.0 (camera sees 800×480, same as world bounds)
        Assert.Equal(1.0f, camera.Zoom);

        // With zoom 1.0 and world bounds = viewport size, only valid position is (0, 0)
        Assert.Equal(0f, camera.Position.X, 2);
        Assert.Equal(0f, camera.Position.Y, 2);
    }

    [Fact]
    public void SetMinimumZoom_AboveCurrentZoom_ClampsCurrentZoom()
    {
        OrthographicCamera camera = new OrthographicCamera(_graphicsFixture.GraphicsDevice);
        camera.Zoom = 0.5f;

        camera.MinimumZoom = 1.0f;

        Assert.Equal(1.0f, camera.Zoom);
    }

    [Fact]
    public void SetMaximumZoom_BelowCurrentZoom_ClampsCurrentZoom()
    {
        OrthographicCamera camera = new OrthographicCamera(_graphicsFixture.GraphicsDevice);
        camera.Zoom = 2.0f;

        camera.MaximumZoom = 1.0f;

        Assert.Equal(1.0f, camera.Zoom);
    }

    [Fact]
    public void SetMinimumZoom_Negative_ThrowsArgumentOutOfRangeException()
    {
        OrthographicCamera camera = new OrthographicCamera(_graphicsFixture.GraphicsDevice);

        Assert.Throws<ArgumentOutOfRangeException>(() => camera.MinimumZoom = -1.0f);
    }

    [Fact]
    public void SetMaximumZoom_Negative_ThrowsArgumentOutOfRangeException()
    {
        OrthographicCamera camera = new OrthographicCamera(_graphicsFixture.GraphicsDevice);

        Assert.Throws<ArgumentOutOfRangeException>(() => camera.MaximumZoom = -1.0f);
    }

    [Fact]
    public void SetPitch_BelowMinimum_ClampsToMinimum()
    {
        OrthographicCamera camera = new OrthographicCamera(_graphicsFixture.GraphicsDevice);
        camera.MinimumPitch = 1.0f;

        camera.Pitch = 0.9f;

        Assert.Equal(camera.MinimumPitch, camera.Pitch);
    }

    [Fact]
    public void SetPitch_AboveMaximum_ClampsToMaximum()
    {
        OrthographicCamera camera = new OrthographicCamera(_graphicsFixture.GraphicsDevice);
        camera.MaximumPitch = 1.0f;

        camera.Pitch = 1.1f;

        Assert.Equal(camera.MaximumPitch, camera.Pitch);
    }

    [Fact]
    public void SetMinimumPitch_Negative_ThrowsArgumentOutOfRangeException()
    {
        OrthographicCamera camera = new OrthographicCamera(_graphicsFixture.GraphicsDevice);
        Assert.Throws<ArgumentOutOfRangeException>(() => camera.MinimumPitch = -0.01f);
    }

    [Fact]
    public void SetMaximumPitch_Negative_ThrowsArgumentOutOfRangeException()
    {
        OrthographicCamera camera = new OrthographicCamera(_graphicsFixture.GraphicsDevice);
        Assert.Throws<ArgumentOutOfRangeException>(() => camera.MaximumPitch = -0.01f);
    }

    [Fact]
    public void BoundingRectangle_WithMovement_ReturnsCorrectBounds()
    {
        DefaultViewportAdapter viewportAdapter = new DefaultViewportAdapter(_graphicsFixture.GraphicsDevice);
        OrthographicCamera camera = new OrthographicCamera(viewportAdapter);

        // Move right 2, then down 3
        Vector2 movement = new Vector2(2, 3);
        camera.Move(new Vector2(movement.X, 0));
        camera.Move(new Vector2(0, movement.Y));

        RectangleF boundingRectangle = camera.BoundingRectangle;

        Assert.Equal(movement.X, boundingRectangle.Left, 2);
        Assert.Equal(movement.Y, boundingRectangle.Top, 2);
        Assert.Equal(movement.X + viewportAdapter.VirtualWidth, boundingRectangle.Right, 2);
        Assert.Equal(movement.Y + viewportAdapter.VirtualHeight, boundingRectangle.Bottom, 2);
    }

    [Fact]
    public void BoundingRectangle_WithZoom_ReturnsCorrectBounds()
    {
        OrthographicCamera camera = new OrthographicCamera(_graphicsFixture.GraphicsDevice);

        camera.Zoom = 2f;

        RectangleF boundingRectangle = camera.BoundingRectangle;

        // With 2x zoom on 800x480 viewport, camera sees 400x240 area
        Assert.Equal(400f, boundingRectangle.Width, 2);
        Assert.Equal(240f, boundingRectangle.Height, 2);
    }

    [Fact]
    public void ContainsPoint_WithDefaultCamera_ReturnsCorrectContainment()
    {
        OrthographicCamera camera = new OrthographicCamera(_graphicsFixture.GraphicsDevice);
        var viewport = _graphicsFixture.GraphicsDevice.Viewport;

        Assert.Equal(ContainmentType.Contains, camera.Contains(new Point(1, 1)));
        Assert.Equal(ContainmentType.Contains, camera.Contains(new Point(viewport.Width - 1, viewport.Height - 1)));
        Assert.Equal(ContainmentType.Disjoint, camera.Contains(new Point(-1, -1)));
        Assert.Equal(ContainmentType.Disjoint, camera.Contains(new Point(viewport.Width + 1, viewport.Height + 1)));
    }

    [Fact]
    public void ContainsVector2_WithDefaultCamera_ReturnsCorrectContainment()
    {
        OrthographicCamera camera = new OrthographicCamera(_graphicsFixture.GraphicsDevice);
        var viewport = _graphicsFixture.GraphicsDevice.Viewport;

        Assert.Equal(ContainmentType.Contains, camera.Contains(new Vector2(viewport.Width - 0.5f, viewport.Height - 0.5f)));
        Assert.Equal(ContainmentType.Contains, camera.Contains(new Vector2(0.5f, 0.5f)));
        Assert.Equal(ContainmentType.Disjoint, camera.Contains(new Vector2(-0.5f, -0.5f)));
        Assert.Equal(ContainmentType.Disjoint, camera.Contains(new Vector2(viewport.Width + 0.5f, viewport.Height + 0.5f)));
        Assert.Equal(ContainmentType.Disjoint, camera.Contains(new Vector2(-0.5f, viewport.Height / 2f)));
        Assert.Equal(ContainmentType.Contains, camera.Contains(new Vector2(0.5f, viewport.Height / 2f)));
        Assert.Equal(ContainmentType.Contains, camera.Contains(new Vector2(viewport.Width - 0.5f, viewport.Height / 2f)));
        Assert.Equal(ContainmentType.Disjoint, camera.Contains(new Vector2(viewport.Width + 0.5f, viewport.Height / 2f)));
    }

    [Fact]
    public void ContainsRectangle_WithDefaultCamera_ReturnsCorrectContainment()
    {
        OrthographicCamera camera = new OrthographicCamera(_graphicsFixture.GraphicsDevice);

        Assert.Equal(ContainmentType.Intersects, camera.Contains(new Rectangle(-50, -50, 100, 100)));
        Assert.Equal(ContainmentType.Contains, camera.Contains(new Rectangle(50, 50, 100, 100)));
        Assert.Equal(ContainmentType.Disjoint, camera.Contains(new Rectangle(850, 500, 100, 100)));
    }

    [Fact]
    public void ContainsRectangle_FullyContained_ReturnsContains()
    {
        OrthographicCamera camera = new OrthographicCamera(_graphicsFixture.GraphicsDevice);
        Rectangle fullyContainedRect = new Rectangle(100, 100, 200, 200);

        ContainmentType result = camera.Contains(fullyContainedRect);

        Assert.Equal(ContainmentType.Contains, result);
    }

    [Fact]
    public void ContainsRectangle_PartiallyContained_ReturnsIntersects()
    {
        OrthographicCamera camera = new OrthographicCamera(_graphicsFixture.GraphicsDevice);
        Rectangle partiallyContainedRect = new Rectangle(-50, -50, 100, 100);

        ContainmentType result = camera.Contains(partiallyContainedRect);

        Assert.Equal(ContainmentType.Intersects, result);
    }

    [Fact]
    public void EnableWorldBounds_SetsWorldBoundsAndFlag()
    {
        OrthographicCamera camera = new OrthographicCamera(_graphicsFixture.GraphicsDevice);
        Rectangle worldBounds = new Rectangle(0, 0, 800, 600);

        camera.EnableWorldBounds(worldBounds);

        Assert.Equal(worldBounds, camera.WorldBounds);
        Assert.True(camera.IsClampedToWorldBounds);
    }

    [Fact]
    public void DisableWorldBounds_ClearsWorldBoundsAndFlag()
    {
        OrthographicCamera camera = new OrthographicCamera(_graphicsFixture.GraphicsDevice);
        camera.EnableWorldBounds(new Rectangle(0, 0, 800, 600));

        camera.DisableWorldBounds();

        Assert.Equal(Rectangle.Empty, camera.WorldBounds);
        Assert.False(camera.IsClampedToWorldBounds);
    }

    [Fact]
    public void Move_WithoutRotation_TranslatesPosition()
    {
        OrthographicCamera camera = new OrthographicCamera(_graphicsFixture.GraphicsDevice);
        Vector2 originalPosition = camera.Position;
        Vector2 movement = new Vector2(10, 20);

        camera.Move(movement);

        Assert.Equal(originalPosition + movement, camera.Position);
    }

    [Fact]
    public void Move_WithRotation_TranslatesPositionRelativeToRotation()
    {
        OrthographicCamera camera = new OrthographicCamera(_graphicsFixture.GraphicsDevice);

        // 90 degrees
        camera.Rotation = MathHelper.PiOver2;

        // Move right in world space
        Vector2 movement = new Vector2(10, 0);

        camera.Move(movement);

        // With 90 degree rotation, moving "right" should actually move "up" in screen space
        // The movement is transformed by the inverse rotation
        Vector2 expectedMovement = Vector2.Transform(movement, Matrix.CreateRotationZ(-camera.Rotation));

        Assert.Equal(expectedMovement.X, camera.Position.X, 3);
        Assert.Equal(expectedMovement.Y, camera.Position.Y, 3);
    }

    [Fact]
    public void Rotate_IncreasesRotation()
    {
        OrthographicCamera camera = new OrthographicCamera(_graphicsFixture.GraphicsDevice);
        var deltaRotation = MathHelper.PiOver4;

        camera.Rotate(deltaRotation);

        Assert.Equal(deltaRotation, camera.Rotation, 5);
    }

    [Fact]
    public void ZoomIn_IncreasesZoom()
    {
        OrthographicCamera camera = new OrthographicCamera(_graphicsFixture.GraphicsDevice);
        float originalZoom = camera.Zoom;

        camera.ZoomIn(1);

        Assert.Equal(originalZoom + 1, camera.Zoom);
    }

    [Fact]
    public void ZoomOut_DecreasesZoom()
    {
        OrthographicCamera camera = new OrthographicCamera(_graphicsFixture.GraphicsDevice);
        float originalZoom = camera.Zoom;

        camera.ZoomOut(1);

        Assert.Equal(originalZoom - 1, camera.Zoom);
    }

    [Fact]
    public void PitchUp_IncreasesPitch()
    {
        OrthographicCamera camera = new OrthographicCamera(_graphicsFixture.GraphicsDevice);
        float originalPitch = camera.Pitch;

        camera.PitchUp(1);

        Assert.Equal(originalPitch + 1, camera.Pitch);
    }

    [Fact]
    public void PitchDown_DecreasesPitch()
    {
        OrthographicCamera camera = new OrthographicCamera(_graphicsFixture.GraphicsDevice);
        float originalPitch = camera.Pitch;

        camera.PitchDown(1);

        Assert.Equal(originalPitch - 1, camera.Pitch);
    }

    [Fact]
    public void LookAt_SetsPositionCorrectly()
    {
        DefaultViewportAdapter viewportAdapter = new DefaultViewportAdapter(_graphicsFixture.GraphicsDevice);
        OrthographicCamera camera = new OrthographicCamera(viewportAdapter);
        Vector2 targetPosition = new Vector2(100, 200);

        camera.LookAt(targetPosition);

        Vector2 expectedPosition = targetPosition - new Vector2(viewportAdapter.VirtualWidth, viewportAdapter.VirtualHeight) * 0.5f;
        Assert.Equal(expectedPosition, camera.Position);
    }

    [Fact]
    public void ScreenToWorld_WithCameraMovement_TransformsCorrectly()
    {
        OrthographicCamera camera = new OrthographicCamera(_graphicsFixture.GraphicsDevice);
        camera.Position = new Vector2(100, 200);

        // Screen position at origin
        Vector2 screenPosition = Vector2.Zero;
        Vector2 worldPosition = camera.ScreenToWorld(screenPosition);

        // World position should account for camera offset
        Assert.Equal(100, worldPosition.X, 2);
        Assert.Equal(200, worldPosition.Y, 2);
    }

    [Fact]
    public void WorldToScreen_WithCameraMovement_TransformsCorrectly()
    {
        OrthographicCamera camera = new OrthographicCamera(_graphicsFixture.GraphicsDevice);
        camera.Position = new Vector2(100, 200);

        // World position at camera position
        Vector2 worldPosition = new Vector2(100, 200);
        Vector2 screenPosition = camera.WorldToScreen(worldPosition);

        // Should appear at screen origin
        Assert.Equal(0, screenPosition.X, 2);
        Assert.Equal(0, screenPosition.Y, 2);
    }

    [Fact]
    public void ScreenToWorld_WithZoom_TransformsCorrectly()
    {
        OrthographicCamera camera = new OrthographicCamera(_graphicsFixture.GraphicsDevice);
        camera.Zoom = 2.0f;

        Vector2 screenPosition = new Vector2(100, 100);
        Vector2 worldPosition = camera.ScreenToWorld(screenPosition);

        // With default camera:
        // - Origin is at (400, 240) - the viewport center
        // - Position is at (0, 0)
        // - Screen (100, 100) is 300px left and 140px up from Origin
        // - With 2x zoom: world offset is (300/2, 140/2) = (150, 70) from Origin
        // - World position: Origin - offset = (400 - 150, 240 - 70) = (250, 170)
        Assert.Equal(250, worldPosition.X, 2);
        Assert.Equal(170, worldPosition.Y, 2);
    }

    [Fact]
    public void WorldToScreen_WithZoom_TransformsCorrectly()
    {
        OrthographicCamera camera = new OrthographicCamera(_graphicsFixture.GraphicsDevice);
        camera.Zoom = 2.0f;

        Vector2 worldPosition = new Vector2(100, 100);
        Vector2 screenPosition = camera.WorldToScreen(worldPosition);

        // With default camera:
        // - Origin is at (400, 240) - the viewport center
        // - Position is at (0, 0)
        // - Camera.Center is at (400, 240)
        // - World (100, 100) is 300px left and 140px up from Camera.Center
        // - With 2x zoom: screen offset is (300 * 2, 140 * 2) = (600, 280) from Origin
        // - Screen position: Origin - offset = (400 - 600, 240 - 280) = (-200, -40)
        Assert.Equal(-200, screenPosition.X, 2);
        Assert.Equal(-40, screenPosition.Y, 2);
    }

    [Fact]
    public void WorldToScreen_RoundTrip_ReturnsOriginalPosition()
    {
        OrthographicCamera camera = new OrthographicCamera(_graphicsFixture.GraphicsDevice);
        camera.Position = new Vector2(100, 200);
        camera.Zoom = 1.5f;

        Vector2 originalWorld = new Vector2(250, 350);

        Vector2 screen = camera.WorldToScreen(originalWorld);
        Vector2 backToWorld = camera.ScreenToWorld(screen);

        Assert.Equal(originalWorld.X, backToWorld.X, 2);
        Assert.Equal(originalWorld.Y, backToWorld.Y, 2);
    }

    [Fact]
    public void GetViewMatrix_ReturnsValidMatrix()
    {
        OrthographicCamera camera = new OrthographicCamera(_graphicsFixture.GraphicsDevice);

        Matrix viewMatrix = camera.GetViewMatrix();

        Assert.Equal(Matrix.Identity, viewMatrix);
    }

    [Fact]
    public void GetInverseViewMatrix_IsInverseOfViewMatrix()
    {
        OrthographicCamera camera = new OrthographicCamera(_graphicsFixture.GraphicsDevice);
        camera.Position = new Vector2(10, 20);
        camera.Rotation = MathHelper.PiOver4;
        camera.Zoom = 2f;

        Matrix viewMatrix = camera.GetViewMatrix();
        Matrix inverseViewMatrix = camera.GetInverseViewMatrix();
        Matrix shouldBeIdentity = Matrix.Multiply(viewMatrix, inverseViewMatrix);

        // Check if the result is close to identity matrix
        AssertExtensions.Equal(Matrix.Identity, shouldBeIdentity, 3);
    }

    [Fact]
    public void GetViewMatrix_WithParallaxFactor_ReturnsValidMatrix()
    {
        OrthographicCamera camera = new OrthographicCamera(_graphicsFixture.GraphicsDevice);

        // Set non-zero position to make parallax effect visible
        camera.Position = new Vector2(100, 50);
        Vector2 parallaxFactor = new Vector2(0.5f, 0.5f);

        Matrix viewMatrix = camera.GetViewMatrix(parallaxFactor);

        Assert.NotEqual(Matrix.Identity, viewMatrix);
    }

    [Fact]
    public void GetViewMatrix_WithParallaxFactor_AppliesCorrectTransformation()
    {
        OrthographicCamera camera = new OrthographicCamera(_graphicsFixture.GraphicsDevice);
        camera.Position = new Vector2(100, 60);
        Vector2 parallaxFactor = new Vector2(0.5f, 0.25f);

        Matrix parallaxMatrix = camera.GetViewMatrix(parallaxFactor);

        // Default parallax factor of (1,1)
        Matrix normalMatrix = camera.GetViewMatrix();

        // The matrices should be different when parallax factor is not (1,1) and position is not zero
        Assert.NotEqual(normalMatrix, parallaxMatrix);
        Assert.NotEqual(Matrix.Identity, parallaxMatrix);
        Assert.NotEqual(Matrix.Identity, normalMatrix);

        // With position (100, 60) and parallax (0.5, 0.25):
        // Expected translation = -(100 * 0.5, 60 * 0.25) = (-50, -15)
        // This should be reflected in the view matrix M41, M42 values
        Assert.Equal(-50f, parallaxMatrix.M41, 1);
        Assert.Equal(-15f, parallaxMatrix.M42, 1);
    }

    [Fact]
    public void GetBoundingFrustum_ReturnsValidFrustum()
    {
        var camera = new OrthographicCamera(_graphicsFixture.GraphicsDevice);
        var viewport = _graphicsFixture.GraphicsDevice.Viewport;

        var boundingFrustum = camera.GetBoundingFrustum();
        var corners = boundingFrustum.GetCorners();

        // Verify we have 8 corners (standard frustum)
        Assert.Equal(8, corners.Length);

        // Check near plane corners (Z = 1)
        Assert.Equal(0, corners[0].X, 2);
        Assert.Equal(0, corners[0].Y, 2);
        Assert.Equal(1, corners[0].Z, 2);

        Assert.Equal(viewport.Width, corners[1].X, 2);
        Assert.Equal(0, corners[1].Y, 2);
        Assert.Equal(1, corners[1].Z, 2);

        Assert.Equal(viewport.Width, corners[2].X, 2);
        Assert.Equal(viewport.Height, corners[2].Y, 2);
        Assert.Equal(1, corners[2].Z, 2);

        Assert.Equal(0, corners[3].X, 2);
        Assert.Equal(viewport.Height, corners[3].Y, 2);
        Assert.Equal(1, corners[3].Z, 2);

        // Check far plane corners (Z = 0)
        Assert.Equal(0, corners[4].X, 2);
        Assert.Equal(0, corners[4].Y, 2);
        Assert.Equal(0, corners[4].Z, 2);

        Assert.Equal(viewport.Width, corners[5].X, 2);
        Assert.Equal(0, corners[5].Y, 2);
        Assert.Equal(0, corners[5].Z, 2);

        Assert.Equal(viewport.Width, corners[6].X, 2);
        Assert.Equal(viewport.Height, corners[6].Y, 2);
        Assert.Equal(0, corners[6].Z, 2);

        Assert.Equal(0, corners[7].X, 2);
        Assert.Equal(viewport.Height, corners[7].Y, 2);
        Assert.Equal(0, corners[7].Z, 2);
    }

}
