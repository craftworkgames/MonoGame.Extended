namespace MonoGame.Extended.Content.Pipeline
{
    public class ContentImporterResult<T>
    {
        public ContentImporterResult(string filePath, T data)
        {
            FilePath = filePath;
            Data = data;
        }

        public string FilePath { get; }
        public T Data { get; }
    }
}