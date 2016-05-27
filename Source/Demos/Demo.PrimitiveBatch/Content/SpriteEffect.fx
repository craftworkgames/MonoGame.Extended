// The matrix to use in the vertex shader to transform render geometry.
float4x4 WorldViewProjection;
// The texture to use. It's "registered" to texture slot index 0.
Texture2D<float4> Texture;
// The texure sampler to use.
sampler2D TextureSampler = sampler_state
{
	Texture = (Texture);
	MagFilter = Point;
	MinFilter = Point;
	AddressU = Clamp;
	AddressV = Clamp;
};

// The information layout used as input to the vertex shader. The actual information is passed to the GPU from the CPU.
struct VertexShaderInput
{
	float4 Position : POSITION0;
	float4 Color : COLOR0;
	float2 TextureCoordinate : TEXCOORD0;
};

// The information layout used as output from the vertex shader and input to the pixel shader.
struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float4 Color : COLOR0;
	float2 TextureCoordinate : TEXCOORD0;
};

// The information layout used as output from the pixel shader. The actual information is what is displayed on screen.
struct PixelShaderOutput
{
	float4 Color : COLOR0;
};

// The vertex shader code that transforms render geometry into a space so it can be determined wether it should be rendered or not on screen. The input information is from the CPU.
VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;
	output.Position = mul(input.Position, WorldViewProjection);
	output.Color = input.Color;
	output.TextureCoordinate = input.TextureCoordinate;
	return output;
}

// The pixel shader code that determines the final color of each pixel. The input information is the output of the vertex shader, if, and only if, that information is determined to be on screen.
PixelShaderOutput PixelShaderFunction(VertexShaderOutput input)
{
	PixelShaderOutput output;
	output.Color = tex2D(TextureSampler, input.TextureCoordinate) * input.Color;
	return output;
}

// The technique; a collection of passes to be applied to the CPU information, with each pass specifying the vertex and shader used.
technique PolygonTechnique
{
	pass Pass1
	{
		VertexShader = compile vs_2_0 VertexShaderFunction();
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
}