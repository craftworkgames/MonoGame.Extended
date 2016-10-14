using System;
using System.Reflection;

namespace MonoGame.Extended.Support.Plugins
{
    public delegate void AssemblyLoadEventHandler(object sender, AssemblyLoadEventArgs e);

    public class AssemblyLoadEventArgs : EventArgs
    {
        public Assembly LoadedAssembly
        {
            get { return _assembly; }
        }

        Assembly _assembly;

        public AssemblyLoadEventArgs(Assembly assembly) : base()
        {
            _assembly = assembly;
        }
    }
}
