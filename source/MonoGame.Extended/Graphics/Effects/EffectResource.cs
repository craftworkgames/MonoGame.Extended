using System;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;

#if !FNA && !KNI
using MonoGame.Framework.Utilities;
#endif

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
        private static EffectResource _defaultEffect;

        /// <summary>
        ///     Gets the <see cref="Effects.DefaultEffect" /> embedded into the MonoGame.Extended.Graphics library.
        /// </summary>
        public static EffectResource GetDefaultEffect(GraphicsDevice graphicsDevice)
        {
#if FNA
            return _defaultEffect ??= new EffectResource("MonoGame.Extended.Graphics.Effects.Resources.DefaultEffect.fxb");
#elif KNI
            return _defaultEffect ??= new EffectResource("MonoGame.Extended.Graphics.Effects.Resources.DefaultEffect.fxo");
#else
            switch (PlatformInfo.GraphicsBackend)
            {
                case GraphicsBackend.DirectX:
                    return _defaultEffect ??= new EffectResource("MonoGame.Extended.Graphics.Effects.Resources.DefaultEffect.dx11.mgfxo");
                case GraphicsBackend.DirectX12:
                    return _defaultEffect ??= new EffectResource("MonoGame.Extended.Graphics.Effects.Resources.DefaultEffect.dx12.mgfxo");
                case GraphicsBackend.OpenGL:
                    return _defaultEffect ??= new EffectResource("MonoGame.Extended.Graphics.Effects.Resources.DefaultEffect.ogl.mgfxo");
                case GraphicsBackend.Vulkan:
                    return _defaultEffect ??= new EffectResource("MonoGame.Extended.Graphics.Effects.Resources.DefaultEffect.vk.mgfxo");
                default:
                    throw new InvalidOperationException($"Unsupported shader extension '{PlatformInfo.GraphicsBackend}'.");
            }
#endif
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
