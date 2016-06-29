using System.IO;
using System.Reflection;

namespace MonoGame.Extended.Graphics.Effects
{
    public class EffectResource
    {
        public static readonly EffectResource BatchEffect = new EffectResource(name: "MonoGame.Extended.Graphics.Effects.Resources.ShapeEffect.mgfxo");

        private readonly string _name;
        private volatile byte[] _bytecode;

        private EffectResource(string name)
        {
            _name = name;
        }

        public byte[] Bytecode
        {
            get
            {
                if (_bytecode != null)
                {
                    return _bytecode;
                }

                lock (this)
                {
                    if (_bytecode != null)
                    {
                        return _bytecode;
                    }

                    var assembly = typeof(EffectResource).GetTypeInfo().Assembly;
                    var stream = assembly.GetManifestResourceStream(_name);
                    using (var memoryStream = new MemoryStream())
                    {
                        stream.CopyTo(memoryStream);
                        _bytecode = memoryStream.ToArray();
                    }
                }

                return _bytecode;
            }
        }
    }
}
