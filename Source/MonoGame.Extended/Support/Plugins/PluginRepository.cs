using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using PCLStorage;

namespace MonoGame.Extended.Support.Plugins
{
    /// <summary>Stores loaded plugins</summary>
    /// <remarks>
    ///   This class manages a set of assemblies that have been dynamically loaded 
    ///   as plugins. It usually is shared by multiple PluginHosts that handle
    ///   different interfaces of one plugin type.
    /// </remarks>
    public class PluginRepository
    {
        #region class DefaultAssemblyLoader

        /// <summary>Default assembly loader used to read assemblies from files</summary>
        public class DefaultAssemblyLoader : IAssemblyLoader
        {

            /// <summary>Initializes a new default assembly loader</summary>
            /// <remarks>
            ///   Made protected to provide users with a small incentive for using
            ///   the Instance property instead of creating new instances all around.
            /// </remarks>
            protected DefaultAssemblyLoader() { }

            /// <summary>Loads an assembly from a file system path</summary>
            /// <param name="path">Path the assembly will be loaded from</param>
            /// <returns>The loaded assembly</returns>
            protected virtual Assembly LoadAssemblyFromFile(AssemblyName name)
            {
                return Assembly.Load(name);
            }

            /// <summary>Tries to loads an assembly from a file</summary>
            /// <param name="path">Path to the file that is loaded as an assembly</param>
            /// <param name="loadedAssembly">
            ///   Output parameter that receives the loaded assembly or null
            /// </param>
            /// <returns>True if the assembly was loaded successfully, otherwise false</returns>
            public bool TryLoadFile(AssemblyName name, out Assembly loadedAssembly)
            {

                // A lot of errors can occur when attempting to load an assembly...
                try
                {
                    loadedAssembly = LoadAssemblyFromFile(name);
                    return true;
                }
#if WINDOWS
        // File not found - Most likely a missing dependency of the assembly we
        // attempted to load since the assembly itself has been found by the GetFiles() method
        catch(DllNotFoundException) {
          reportError(
            "Assembly '" + path + "' or one of its dependencies is missing"
          );
        }
#endif
                // Unauthorized acccess - Either the assembly is not trusted because it contains
                // code that imposes a security risk on the system or a user rights problem
                catch (UnauthorizedAccessException)
                {
                    reportError(
                      "Not authorized to load assembly '" + name.Name + "', " +
                      "possible rights problem"
                    );
                }
                // Bad image format - This exception is often thrown when the assembly we
                // attempted to load requires a different version of the .NET framework
                catch (BadImageFormatException)
                {
                    reportError(
                      "'" + name.Name + "' is not a .NET assembly, requires a different version " +
                      "of the .NET Runtime or does not support the current instruction set (x86/x64)"
                    );
                }
                // Unknown error - Our last resort is to show a default error message
                catch (Exception exception)
                {
                    reportError(
                      "Failed to load plugin assembly '" + name.Name + "': " + exception.Message
                    );
                }

                loadedAssembly = null;
                return false;

            }

            /// <summary>The only instance of the DefaultAssemblyLoader</summary>
            public static readonly DefaultAssemblyLoader Instance =
              new DefaultAssemblyLoader();

        }

        #endregion // class DefaultAssemblyLoader

        /// <summary>Triggered whenever a new assembly is loaded into this repository</summary>
        public event AssemblyLoadEventHandler AssemblyLoaded;

        /// <summary>Initializes a new instance of the plugin repository</summary>
        public PluginRepository() : this(DefaultAssemblyLoader.Instance) { }

        /// <summary>Initializes a new instance of the plugin repository</summary>
        /// <param name="loader">
        ///   Loader to use for loading assemblies into this repository
        /// </param>
        public PluginRepository(IAssemblyLoader loader)
        {
            _assemblies = new List<Assembly>();
            _assemblyLoader = loader;
        }

        /// <summary>Loads all plugins matching a wildcard specification</summary>
        /// <param name="wildcard">Path of one or more plugins via wildcard</param>
        /// <remarks>
        ///   This function always assumes that a plugin is optional. This means that
        ///   even when you specify a unique file name and a matching file is not found,
        ///   no exception will be raised and the error is silently ignored.
        /// </remarks>
        public void AddFiles(string wildcard)
        {
            string directory = Path.GetDirectoryName(wildcard);
            string search = Path.GetFileName(wildcard);

            // If no directory was specified, use the current working directory
            if ((directory == null) || (directory == string.Empty))
            {
                directory = ".";
            }

            // We'll scan the specified directory for all files matching the specified
            // wildcard. If only a single file is specified, only that file will match
            // the supposed wildcard and everything works as expected
            var assemblyFiles = FileSystem.Current.LocalStorage.GetFilesAsync().Result;
            foreach (var assemblyFile in assemblyFiles)
            {
                AssemblyName name = new AssemblyName(assemblyFile.Path);
                
                Assembly loadedAssembly;
                if (_assemblyLoader.TryLoadFile(name, out loadedAssembly))
                {
                    AddAssembly(loadedAssembly);
                }

            }
        }

        /// <summary>Adds the specified assembly to the repository</summary>
        /// <remarks>
        ///   Also used internally, so any assembly that is to be put into the repository,
        ///   not matter how, wanders through this method
        /// </remarks>
        public void AddAssembly(Assembly assembly)
        {
            _assemblies.Add(assembly);

            // Trigger event in case any subscribers have been registered
            if (AssemblyLoaded != null)
            {
                AssemblyLoaded(this, new AssemblyLoadEventArgs(assembly));
            }
        }

        /// <summary>List of all loaded plugin assemblies in the repository</summary>
        public List<Assembly> LoadedAssemblies
        {
            get { return _assemblies; }
        }

        /// <summary>Reports an error to the debugging console</summary>
        /// <param name="error">Error message that will be reported</param>
        private static void reportError(string error)
        {
#if WINDOWS
      Trace.WriteLine(error);
#endif
        }

        /// <summary>Loaded plugin assemblies</summary>
        private List<Assembly> _assemblies;
        /// <summary>Takes care of loading assemblies for the repositories</summary>
        private IAssemblyLoader _assemblyLoader;

    }
}
