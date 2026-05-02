using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Graphics.Effects
{
    /// <summary>
    ///     Reperesents the bytecode of an <see cref="Effect" /> that is encapsulated inside a compiled assembly.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Files that are encapsulated inside a compiled assembly are commonly known as Manifiest or embedded resources.
    ///         Since embedded resources are added to the assembly at compiled time, they can not be accidentally deleted or
    ///         misplaced. However, if the file needs to be changed, the assembly will need to be re-compiled with the changed
    ///         file.
    ///     </para>
    ///     <para>
    ///         To add an embedded resource file to an assembly, first add it to the project and then change the Build Action
    ///         in the Properties of the file to <code>Embedded Resource</code>. The next time the project is compiled, the
    ///         compiler will add the file to the assembly as an embedded resource. The compiler adds namespace(s) to the
    ///         embedded resource so it matches with the path of where the file was added to the project.
    ///     </para>
    /// </remarks>
    public class EffectResource
    {
        private static EffectResource _defaultEffectDx11;
        private static EffectResource _defaultEffectOgl;

        /// <summary>
        ///     Gets the <see cref="Effects.DefaultEffect" /> embedded into the MonoGame.Extended.Graphics library.
        /// </summary>
        public static EffectResource GetDefaultEffect(GraphicsDevice graphicsDevice)
        {
            string shaderExtension = DetermineShaderExtension(graphicsDevice);
            switch (shaderExtension)
            {
                case "dx11":
                    return _defaultEffectDx11 ??= new EffectResource("MonoGame.Extended.Graphics.Effects.Resources.DefaultEffect.dx11.mgfxo");
                case "ogl":
                    return _defaultEffectOgl ??= new EffectResource("MonoGame.Extended.Graphics.Effects.Resources.DefaultEffect.ogl.mgfxo");
                default:
                    throw new InvalidOperationException($"Unsupported shader extension '{shaderExtension}'.");
            }
        }

        private static string DetermineShaderExtension(GraphicsDevice graphicsDevice)
        {
            ArgumentNullException.ThrowIfNull(graphicsDevice);

            // use reflection to figure out if Shader.Profile is OpenGL (0) or DirectX (1),
            // may need to be changed / fixed for future shader profiles
            Assembly frameworkAssembly = typeof(Game).GetTypeInfo().Assembly;
            Debug.Assert(frameworkAssembly != null);

            Type shaderType = frameworkAssembly.GetType("Microsoft.Xna.Framework.Graphics.Shader");
            if (shaderType != null)
            {
                TypeInfo shaderTypeInfo = shaderType.GetTypeInfo();
                Debug.Assert(shaderTypeInfo != null);

                // https://github.com/MonoGame/MonoGame/blob/develop/MonoGame.Framework/Graphics/Shader/Shader.cs#L47
                PropertyInfo profileProperty = shaderTypeInfo.GetDeclaredProperty("Profile");
                if (profileProperty?.GetValue(null) is object profileValue)
                {
                    switch (Convert.ToInt32(profileValue))
                    {
                        case 0:
                            return "ogl";
                        case 1:
                            return "dx11";
                    }
                }
            }

            if (IsOpenGlAssembly(graphicsDevice.GetType().Assembly))
            {
                return "ogl";
            }

            if (IsDirectXAssembly(graphicsDevice.GetType().Assembly))
            {
                return "dx11";
            }

            foreach (Assembly loadedAssembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (IsOpenGlAssembly(loadedAssembly))
                {
                    return "ogl";
                }

                if (IsDirectXAssembly(loadedAssembly))
                {
                    return "dx11";
                }
            }

#if KNI
            return "ogl";
#endif

            throw new InvalidOperationException("Unable to determine the shader profile for the current graphics platform.");
        }

        private static bool IsOpenGlAssembly(Assembly assembly)
        {
            string assemblyName = assembly.GetName().Name ?? string.Empty;
            if (assemblyName.Contains("DesktopGL", StringComparison.OrdinalIgnoreCase) ||
                assemblyName.Contains("SDL2.GL", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return assembly.GetType("Microsoft.Xna.Platform.Graphics.ConcreteGraphicsContextGL") != null;
        }

        private static bool IsDirectXAssembly(Assembly assembly)
        {
            string assemblyName = assembly.GetName().Name ?? string.Empty;
            if (assemblyName.Contains("WindowsDX", StringComparison.OrdinalIgnoreCase) ||
                assemblyName.Contains("DX11", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return assembly.GetType("Microsoft.Xna.Platform.Graphics.ConcreteGraphicsContextD3D") != null ||
                   assembly.GetType("Microsoft.Xna.Platform.Graphics.ConcreteGraphicsContextDX") != null ||
                   assembly.GetType("Microsoft.Xna.Platform.Graphics.ConcreteGraphicsContextDirectX") != null;
        }

        private readonly string _resourceName;
        private volatile byte[] _bytecode;
        private readonly Assembly _assembly;

        /// <summary>
        ///     Gets the bytecode of the <see cref="Effect" /> file.
        /// </summary>
        /// <value>
        ///     The bytecode of the <see cref="Effect" /> file.
        /// </value>
        public byte[] Bytecode
        {
            get
            {
                if (_bytecode != null)
                    return _bytecode;

                lock (this)
                {
                    if (_bytecode != null)
                        return _bytecode;

                    Stream stream = _assembly.GetManifestResourceStream(_resourceName);
                    if (stream == null)
                    {
                        string resolvedResourceName = ResolveManifestResourceName();
                        if (resolvedResourceName != null)
                        {
                            stream = _assembly.GetManifestResourceStream(resolvedResourceName);
                        }
                    }

                    if (stream == null)
                    {
                        throw new InvalidOperationException($"Embedded effect resource '{_resourceName}' was not found in assembly '{_assembly.FullName}'.");
                    }

                    using (var memoryStream = new MemoryStream())
                    {
                        stream.CopyTo(memoryStream);
                        _bytecode = memoryStream.ToArray();
                    }
                }

                return _bytecode;
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="EffectResource" /> class.
        /// </summary>
        /// <param name="resourceName">The name of the embedded resource. This must include the namespace(s).</param>
        /// <param name="assembly">The assembly which the embedded resource is apart of.</param>
        public EffectResource(string resourceName, Assembly assembly = null)
        {
            _resourceName = resourceName;
            _assembly = assembly ?? typeof(EffectResource).GetTypeInfo().Assembly;
        }

        private string ResolveManifestResourceName()
        {
            string[] resourceNames = _assembly.GetManifestResourceNames();
            for (int i = 0; i < resourceNames.Length; i++)
            {
                if (string.Equals(resourceNames[i], _resourceName, StringComparison.Ordinal))
                {
                    return resourceNames[i];
                }
            }

            int separatorIndex = _resourceName.IndexOf(".Graphics.Effects.Resources.", StringComparison.Ordinal);
            string suffix = separatorIndex >= 0
                ? _resourceName.Substring(separatorIndex + 1)
                : _resourceName;

            string match = null;
            for (int i = 0; i < resourceNames.Length; i++)
            {
                if (resourceNames[i].EndsWith(suffix, StringComparison.Ordinal))
                {
                    if (match != null)
                    {
                        return null;
                    }

                    match = resourceNames[i];
                }
            }

            return match;
        }
    }
}
