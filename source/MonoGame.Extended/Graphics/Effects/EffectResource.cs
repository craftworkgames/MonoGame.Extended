using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
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
        private static EffectResource _defaultEffectFna;
        private static EffectResource _defaultEffectKni;
        private static EffectResource _defaultEffectDx11;
        private static EffectResource _defaultEffectDx12;
        private static EffectResource _defaultEffectOgl;
        private static EffectResource _defaultEffectVk;
        private static string _detectedShaderProfile;

        /// <summary>
        ///     Gets the <see cref="Effects.DefaultEffect" /> embedded into the MonoGame.Extended.Graphics library.
        /// </summary>
        public static EffectResource GetDefaultEffect(GraphicsDevice graphicsDevice)
        {
#if FNA
            return _defaultEffectFna ??= new EffectResource("MonoGame.Extended.Graphics.Effects.Resources.DefaultEffect.fxb");
#elif KNI
            return _defaultEffectKni ??= new EffectResource("MonoGame.Extended.Graphics.Effects.Resources.DefaultEffect.fxo");
#else
            string shaderExtension = DetermineShaderExtension(graphicsDevice);
            switch (shaderExtension)
            {
                case "dx11":
                    return _defaultEffectDx11 ??= new EffectResource("MonoGame.Extended.Graphics.Effects.Resources.DefaultEffect.dx11.mgfxo");
                case "dx12":
                    return _defaultEffectDx12 ??= new EffectResource("MonoGame.Extended.Graphics.Effects.Resources.DefaultEffect.dx12.mgfxo");
                case "ogl":
                    return _defaultEffectOgl ??= new EffectResource("MonoGame.Extended.Graphics.Effects.Resources.DefaultEffect.ogl.mgfxo");
                case "vk":
                    return _defaultEffectVk ??= new EffectResource("MonoGame.Extended.Graphics.Effects.Resources.DefaultEffect.vk.mgfxo");
                default:
                    throw new InvalidOperationException($"Unsupported shader extension '{shaderExtension}'.");
            }
#endif
        }

        private static string DetermineShaderExtension(GraphicsDevice graphicsDevice)
        {
            ArgumentNullException.ThrowIfNull(graphicsDevice);

            if (_detectedShaderProfile != null)
            {
                return _detectedShaderProfile;
            }

            // Perform a bytecode compatibility test.
            // As far as I can see, this is the only AOT-compatible approach right now.
            // TODO: We should revisit this once we have a publicly available ShaderProfile property.
            string[] profilesToTest = ["dx12", "dx11", "ogl", "vk"];
            foreach (string profile in profilesToTest)
            {
                try
                {
                    Debug.WriteLine($"Testing shader profile: {profile}");

                    // Load the embedded resource bytecode for this profile.
                    string resourceName = $"MonoGame.Extended.Graphics.Effects.Resources.DefaultEffect.{profile}.mgfxo";
                    byte[] bytecode = new EffectResource(resourceName).Bytecode;

                    // Attempt to create an Effect.
                    // If the GraphicsDevice is Vulkan, and we feed it OpenGL bytecode, 
                    // the underlying driver will throw an exception.
                    Effect testEffect = new Effect(graphicsDevice, bytecode);

                    // If we reach here, the GraphicsDevice successfully parsed the bytecode.
                    return _detectedShaderProfile ??= profile;
                }
                catch
                {
                    // Bytecode was rejected by the current graphics backend:
                    // Try the next possibility.
                    Debug.WriteLine($"Shader profile was rejected: {profile}");

                    continue;
                }
            }

            throw new InvalidOperationException("Unable to determine the shader profile for the current graphics platform.");
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
