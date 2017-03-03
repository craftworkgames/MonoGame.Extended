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
        private static EffectResource _defaultEffect;

        /// <summary>
        ///     Gets the <see cref="Effects.DefaultEffect" /> embedded into the MonoGame.Extended.Graphics library.
        /// </summary>
        public static EffectResource DefaultEffect => _defaultEffect ?? (_defaultEffect = new EffectResource("MonoGame.Extended.Graphics.Effects.Resources.DefaultEffect.ogl.mgfxo"));

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