namespace MonoGame.Extended.Content.Pipeline
{
    public class ContentImporterResult<T>
    {
        public T Data { get; private set; }

        public string FilePath { get; private set; }

        public ContentImporterResult(string filePath, T data)
        {
            FilePath = filePath;
            Data = data;
        }
    }
}