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
        private static EffectResource _defaultEffect;
        private static string _shaderExtension;

        /// <summary>
        ///     Gets the <see cref="Effects.DefaultEffect" /> embedded into the MonoGame.Extended.Graphics library.
        /// </summary>
        public static EffectResource DefaultEffect => _defaultEffect ?? (_defaultEffect = new EffectResource($"MonoGame.Extended.Graphics.Effects.Resources.DefaultEffect.{_shaderExtension}.mgfxo"));

        static EffectResource()
        {
            DetermineShaderExtension();
        }

        private static void DetermineShaderExtension()
        {
            // use reflection to figure out if Shader.Profile is OpenGL (0) or DirectX (1),
            // may need to be changed / fixed for future shader profiles

            var assembly = typeof(Game).GetTypeInfo().Assembly;
            Debug.Assert(assembly != null);

            var shaderType = assembly.GetType("Microsoft.Xna.Framework.Graphics.Shader");
            Debug.Assert(shaderType != null);
            var shaderTypeInfo = shaderType.GetTypeInfo();
            Debug.Assert(shaderTypeInfo != null);

            // https://github.com/MonoGame/MonoGame/blob/develop/MonoGame.Framework/Graphics/Shader/Shader.cs#L47
            var profileProperty = shaderTypeInfo.GetDeclaredProperty("Profile");
            var value = (int)profileProperty.GetValue(null);

            switch (value)
            {
                case 0:
                    // OpenGL
                    _shaderExtension = "ogl";
                    break;
                case 1:
                    // DirectX
                    _shaderExtension = "dx11";
                    break;
                default:
                    throw new InvalidOperationException("Unknown shader profile.");
            }
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

                    var stream = _assembly.GetManifestResourceStream(_resourceName);
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
    }
}