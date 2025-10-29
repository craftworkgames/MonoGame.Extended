using System;
using Microsoft.Xna.Framework;
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
    public void Center_ReturnsExpected()
    {
        OrthographicCamera camera = new OrthographicCamera(_graphicsFixture.GraphicsDevice);
        camera.Position = new Vector2(10, 20);


        Vector2 expectedCenter = camera.Position + camera.Origin;
        Assert.Equal(expectedCenter, camera.Center);
    }

    [Fact]
    public void Zoom_WithinBounds_SetsValue()
    {
        OrthographicCamera camera = new OrthographicCamera(_graphicsFixture.GraphicsDevice);
        camera.MinimumZoom = 0.5f;
        camera.MaximumZoom = 2.0f;

        camera.Zoom = 1.5f;

        Assert.Equal(1.5f, camera.Zoom);
    }

    [Fact]
    public void Zoom_BelowMinimum_ThrowsException()
    {
        OrthographicCamera camera = new OrthographicCamera(_graphicsFixture.GraphicsDevice);
        camera.MinimumZoom = 0.5f;

        Assert.Throws<ArgumentException>(() => camera.Zoom = 0.3f);
    }

    [Fact]
    public void Zoom_AboveMaximum_ThrowsException()
    {
        OrthographicCamera camera = new OrthographicCamera(_graphicsFixture.GraphicsDevice);
        camera.MaximumZoom = 2.0f;

        Assert.Throws<ArgumentException>(() => camera.Zoom = 2.5f);
    }

    [Fact]
    public void MinimumZoom_NegativeValue_ThrowsException()
    {
        OrthographicCamera camera = new OrthographicCamera(_graphicsFixture.GraphicsDevice);

        Assert.Throws<ArgumentException>(() => camera.MinimumZoom = -1.0f);
    }

    [Fact]
    public void MaximumZoom_NegativeValue_ThrowsException()
    {
        OrthographicCamera camera = new OrthographicCamera(_graphicsFixture.GraphicsDevice);

        Assert.Throws<ArgumentException>(() => camera.MaximumZoom = -1.0f);
    }

    [Fact]
    public void ZoomIn_WithinBounds_IncreasesZoom()
    {
        OrthographicCamera camera = new OrthographicCamera(_graphicsFixture.GraphicsDevice);
        camera.MaximumZoom = 3.0f;
        float originalZoom = camera.Zoom;

        camera.ZoomIn(0.5f);

        Assert.Equal(originalZoom + 0.5f, camera.Zoom);
    }

    [Fact]
    public void ZoomIn_ExceedsMaximum_ClampsToMaximum()
    {
        OrthographicCamera camera = new OrthographicCamera(_graphicsFixture.GraphicsDevice);
        camera.MaximumZoom = 2.0f;
        camera.Zoom = 1.8f;

        camera.ZoomIn(0.5f);

        Assert.Equal(2.0f, camera.Zoom);
    }

    [Fact]
    public void ZoomOut_WithinBounds_DecreasesZoom()
    {
        OrthographicCamera camera = new OrthographicCamera(_graphicsFixture.GraphicsDevice);
        camera.Zoom = 2.0f;

        camera.ZoomOut(0.5f);

        Assert.Equal(1.5f, camera.Zoom);
    }

    [Fact]
    public void ZoomOut_BelowMinimum_ClampsToMinimum()
    {
        OrthographicCamera camera = new OrthographicCamera(_graphicsFixture.GraphicsDevice);
        camera.MinimumZoom = 0.5f;
        camera.Zoom = 0.7f;

        camera.ZoomOut(0.5f);

        Assert.Equal(0.5f, camera.Zoom);
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
    public void Rotate_IncreasesRotation()
    {
        OrthographicCamera camera = new OrthographicCamera(_graphicsFixture.GraphicsDevice);
        var deltaRotation = MathHelper.PiOver4;

        camera.Rotate(deltaRotation);

        Assert.Equal(deltaRotation, camera.Rotation, 5);
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

        Assert.True(Vector2.Distance(expectedMovement, camera.Position) < 0.001f,
            $"Expected position {expectedMovement}, but got {camera.Position}");
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
    public void WorldToScreen_WithDefaultCamera_TransformsCorrectly()
    {
        OrthographicCamera camera = new OrthographicCamera(_graphicsFixture.GraphicsDevice);
        Vector2 worldPosition = new Vector2(100, 150);

        Vector2 screenPosition = camera.WorldToScreen(worldPosition);

        // With default camera (no transformation), screen position should equal world position
        Assert.True(Vector2.Distance(worldPosition, screenPosition) < 0.001f,
            $"Expected screen position {worldPosition}, but got {screenPosition}");
    }

    [Fact]
    public void WorldToScreen_OverloadWithFloats_ReturnsCorrectResult()
    {
        OrthographicCamera camera = new OrthographicCamera(_graphicsFixture.GraphicsDevice);

        Vector2 result1 = camera.WorldToScreen(100f, 150f);
        Vector2 result2 = camera.WorldToScreen(new Vector2(100f, 150f));

        Assert.Equal(result2, result1);
    }

    [Fact]
    public void ScreenToWorld_WithDefaultCamera_TransformsCorrectly()
    {
        OrthographicCamera camera = new OrthographicCamera(_graphicsFixture.GraphicsDevice);
        Vector2 screenPosition = new Vector2(100, 150);

        Vector2 worldPosition = camera.ScreenToWorld(screenPosition);

        // With default camera (no transformation), world position should equal screen position
        Assert.True(Vector2.Distance(screenPosition, worldPosition) < 0.001f,
            $"Expected world position {screenPosition}, but got {worldPosition}");
    }

    [Fact]
    public void ScreenToWorld_OverloadWithFloats_ReturnsCorrectResult()
    {
        OrthographicCamera camera = new OrthographicCamera(_graphicsFixture.GraphicsDevice);

        Vector2 result1 = camera.ScreenToWorld(100f, 150f);
        Vector2 result2 = camera.ScreenToWorld(new Vector2(100f, 150f));

        Assert.Equal(result2, result1);
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

    [Fact]
    public void BoundingRectangle_WithMovement_ReturnsCorrectBounds()
    {
        DefaultViewportAdapter viewport = new DefaultViewportAdapter(_graphicsFixture.GraphicsDevice);
        OrthographicCamera camera = new OrthographicCamera(viewport);

        var movement = new Vector2(2, 3);
        camera.Move(new Vector2(movement.X, 0));
        camera.Move(new Vector2(0, movement.Y));

        RectangleF boundingRectangle = camera.BoundingRectangle;

        Assert.Equal(movement.X, boundingRectangle.Left, 2);
        Assert.Equal(movement.Y, boundingRectangle.Top, 2);
        Assert.Equal(movement.X + viewport.VirtualWidth, boundingRectangle.Right, 2);
        Assert.Equal(movement.Y + viewport.VirtualHeight, boundingRectangle.Bottom, 2);
    }

    [Fact]
    public void BoundingRectangle_WithZoom_ReturnsCorrectBounds()
    {
        OrthographicCamera camera = new OrthographicCamera(_graphicsFixture.GraphicsDevice);
        var viewport = _graphicsFixture.GraphicsDevice.Viewport;
        camera.Zoom = 2f;

        RectangleF boundingRectangle = camera.BoundingRectangle;

        // With 2x zoom, the bounding rectangle should be smaller
        Assert.True(boundingRectangle.Width < viewport.Width);
        Assert.True(boundingRectangle.Height < viewport.Height);
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
}
