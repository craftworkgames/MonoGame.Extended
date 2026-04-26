using System.IO;

namespace MonoGame.Extended.Content;

/// <summary>
/// Opens a stream for an external resource referenced by another asset.
/// </summary>
/// <param name="path">
/// The resource path requested by the asset loader. The path may be absolute or relative,
/// depending on the loader and resolver being used.
/// </param>
/// <returns>
/// A readable stream positioned at the beginning of the resource. The asset loader that
/// invokes the resolver is responsible for disposing the returned stream.
/// </returns>
public delegate Stream ExternalResourceResolver(string path);
