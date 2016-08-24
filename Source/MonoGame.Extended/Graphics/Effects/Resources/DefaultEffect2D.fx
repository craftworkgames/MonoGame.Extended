// The matrix that transforms geometry in the vertex shader
float4x4 WorldViewProjection;
// The texture used for texture sampling in the pixel shader
Texture2D<float4> Texture;
// The sampler used for texture sampling in the pixel shader
SamplerState TextureSampler;

// The information layout used as input to the vertex shader. The actual information is passed to the GPU from the CPU
struct VertexShaderInput
{
	float4 Position : POSITION0;
	float4 Color : COLOR0;
	float2 TexureCoordinate : TEXCOORD0;
};

// The information layout used as output from the vertex shader and input to the pixel shader
struct VertexShaderOutput
{
	float4 Position : SV_Position;
	float4 Color : COLOR0;
	float2 TexureCoordinate : TEXCOORD0;
};

// The information layout used as output from the pixel shader. The actual information is what is displayed on screen
struct PixelShaderOutput
{
	float4 Color : COLOR0;
};

// The vertex shader code that transforms render geometry. The input information is from the CPU
VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;
	output.Position = mul(input.Position, WorldViewProjection);
	output.Color = input.Color;
	output.TexureCoordinate = input.TexureCoordinate;
	return output;
}

// The pixel shader code that determines the final color of each pixel. The input information is the output of the vertex shader
PixelShaderOutput PixelShaderFunction(VertexShaderOutput input) : SV_Target0
{
	PixelShaderOutput output;
	output.Color = Texture.Sample(TextureSampler, input.TexureCoordinate) * input.Color;
	return output;
}

// The technique; a collection of passes to be applied, each specifying the vertex and shader to use
technique
{
	pass
	{
		VertexShader = compile vs_2_0 VertexShaderFunction();
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
}