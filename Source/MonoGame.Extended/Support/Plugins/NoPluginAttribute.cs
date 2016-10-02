using System;

namespace MonoGame.Extended.Support.Plugins
{
    /// <summary>Attribute that prevents a class from being seen by the PluginHost</summary>
    /// <remarks>
    ///   When this attribute is attached to a class it will be invisible to the
    ///   PluginHost and not become accessable as a plugin.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class NoPluginAttribute : Attribute
    {

        /// <summary>Initializes an instance of the NoPluginAttributes</summary>
        public NoPluginAttribute() : base() { }
    }
}
