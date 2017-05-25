# ContentManager extensions

## ContentManager.OpenStream

`System.IO.Stream ContentManager.OpenStream(string filename)`

**OpenStream** allows easy access to `TitleContainer.OpenStream` so you can use the `Game.Content` object to load compiled resources *and* included resources.

```csharp
// my-file.txt is in the RootDirectory
var stream = Content.OpenStream("my-file.txt");
// do something with file
stream.Close();
```

## ContentManager.GetGraphicsDevice

`GraphicsDevice ContentManager.GetGraphicsDevice()`

**GetGraphicsDevice** returns the current `GraphicsDevice` from the services.

```csharp
var graphicsDevice = Content.GetGraphicsDevice();
var width = graphicsDevice.DisplayMode.Width;
```

# ContentReader extensions

The `ContentReader` extensions help when writing your own content pipeline readers.

## ContentReader.GetGraphicsDevice

`GraphicsDevice ContentManager.GetGraphicsDevice()`

**GetGraphicsDevice** returns the current `GraphicsDevice` to help when loading content for the current display.

```csharp
public class MyTypeReader : ContentTypeReader<MyType> {
    protected override MyType Read(ContentReader reader, MyType existingInstance)
    {
        var graphicsDevice = reader.GetGraphicsDevice();
        // ...
    }
}
```

## ContentReader.GetRelativeAssetName

`string ContentReader.GetRelativeAssetName(string relativeName)`

**GetRelativeAssetName** helps when your content type loads a different type, and you want to know the name to give `ContentManager.Load`.


```csharp
public class MyTypeReader : ContentTypeReader<MyType> {
    protected override MyType Read(ContentReader reader, MyType existingInstance)
    {
        var assetName = reader.GetRelativeAssetName(reader.ReadString());
        var other = reader.ContentManager.Load<OtherType>(assetName);
        // ...
    }
}
```

