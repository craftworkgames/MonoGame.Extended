namespace MonoGame.Extended.Tests.Fixtures;

/// <summary>
/// Defines a shared test collection for graphics tests to prevent SDL initialization conflicts.
///
/// IMPORTANT: This solves the critical issue where multiple graphics test classes would crash
/// when run together due to SDL trying to initialize multiple times simultaneously.
///
/// The problem:
/// - Each test class with IClassFixture<GameFixture> creates its own GameFixture instance
/// - Each GameFixture creates a new MonoGame Game instance
/// - Multiple Game instances try to initialize SDL at the same time
/// - This causes a fatal 0xC0000005 error in Sdl.Init()
///
/// The solution:
/// - Use ICollectionFixture instead of IClassFixture
/// - All graphics test classes use [Collection("Graphics")] attribute
/// - This ensures only ONE GameFixture instance is shared across all graphics tests
/// - SDL is initialized only once, preventing conflicts
///
/// Usage:
/// - Mark any test class that needs graphics with [Collection("Graphics")]
/// - The test class constructor receives GameFixture as a parameter
/// - All tests in the collection share the same GameFixture instance
/// </summary>
[CollectionDefinition("GraphicsTest")]
public class GraphicsTestCollection : ICollectionFixture<GraphicsTestFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
