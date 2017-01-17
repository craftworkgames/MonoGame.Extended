namespace MonoGame.Extended.Content.Pipeline
{
    public class ContentImporterResult<T>
    {
        public string FilePath { get; private set; }
        public T Data { get; private set; }

        public ContentImporterResult(string filePath, T data)
        {
            FilePath = filePath;
            Data = data;
        }
    }
}