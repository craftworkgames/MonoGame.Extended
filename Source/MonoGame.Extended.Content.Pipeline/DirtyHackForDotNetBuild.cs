namespace MonoGame.Extended.Content.Pipeline
{
    public static class DirtyHackForDotNetBuild
    {
        public static string Message = 
            "This is a dirty hack to work around issue #495.\r\n" +
            "It forces the MonoGame.Extended.Content.Pipeline project to be built\r\n" +
            "before the MonoGame Content Pipeline tool tries to use it.";
    }
}