float4x4 WorldViewProjection;
Texture2D Texture : register(t0);
sampler TextureSampler : register(s0)
{
	Texture = (Texture);
};

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float4 Color : COLOR0;
	float2 TexureCoordinate : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : SV_Position;
	float4 Color : COLOR0;
	float2 TexureCoordinate : TEXCOORD0;
};

struct PixelShaderOutput
{
	float4 Color : COLOR0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;
	output.Position = mul(input.Position, WorldViewProjection);
	output.Color = input.Color;
	output.TexureCoordinate = input.TexureCoordinate;
	return output;
}

PixelShaderOutput PixelShaderFunction(VertexShaderOutput input)
{
	PixelShaderOutput output;
	output.Color = tex2D(TextureSampler, input.TexureCoordinate) * input.Color;
	return output;
}

technique
{
	pass
	{
		VertexShader = compile vs_2_0 VertexShaderFunction();
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
}