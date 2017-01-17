#region

using Microsoft.Xna.Framework.Content.Pipeline;

#endregion

namespace MonoGame.Extended.Content.Pipeline
{
    public class ContentLogger
    {
        public static ContentBuildLogger Logger { get; set; }

        public static void Log(string message)
        {
            Logger?.LogMessage(message);
        }
    }
}