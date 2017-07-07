using System;
using System.Runtime.Serialization.Formatters;
using MonoGame.Extended.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MonoGame.Extended.Particles.Serialization
{
    public sealed class ParticleJsonSerializer : JsonSerializer
    {
        public ParticleJsonSerializer(ITextureRegionService textureRegionService, NullValueHandling nullValueHandling = NullValueHandling.Include)
        {
            Converters.Add(new Vector2JsonConverter());
            Converters.Add(new Size2JsonConverter());
            Converters.Add(new ColorJsonConverter());
            Converters.Add(new TextureRegion2DJsonConverter(textureRegionService));
            Converters.Add(new ProfileJsonConverter());
            Converters.Add(new ModifierJsonConverter());
            Converters.Add(new InterpolatorJsonConverter());
            Converters.Add(new TimeSpanJsonConverter());
            Converters.Add(new RangeJsonConverter<int>());
            Converters.Add(new RangeJsonConverter<float>());
            Converters.Add(new RangeJsonConverter<HslColor>());
            Converters.Add(new HslColorJsonConverter());
            Converters.Add(new ModifierExecutionStrategyJsonConverter());
            ContractResolver = new ShortNameJsonContractResolver();

                //Binder = new ShortTypeNameSerializationBinder();

            NullValueHandling = nullValueHandling;
            TypeNameHandling = TypeNameHandling.Auto;
            Formatting = Formatting.Indented;
        }
    }

    //public class ShortTypeNameSerializationBinder : DefaultSerializationBinder
    //{
    //    public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
    //    {
    //        assemblyName = null;
    //        typeName = serializedType.Name;
    //        //base.BindToName(serializedType, out assemblyName, out typeName);
    //    }

    //    public override Type BindToType(string assemblyName, string typeName)
    //    {
    //        return base.BindToType(assemblyName, typeName);
    //    }
    //}
}
