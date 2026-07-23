#include "Macros.fxh"
#include "Structures.fxh"

DECLARE_TEXTURE(Texture, 0);

BEGIN_CONSTANTS

float4 DiffuseColor = float4(1, 1, 1, 1);

MATRIX_CONSTANTS

float4x4 WorldViewProjection _vs(c0) _cb(c0);

END_CONSTANTS

VertexShaderOutputPosition VertexShaderFunctionPosition(VertexShaderInputPosition input)
{
	VertexShaderOutputPosition output;
	output.Position = mul(input.Position, WorldViewProjection);
	return output;
}

float4 PixelShaderFunctionPosition(VertexShaderOutputPosition input) : PIXEL_TARGET_SEMANTIC
{
	return DiffuseColor;
}

VertexShaderOutputPositionTexture VertexShaderFunctionPositionTexture(VertexShaderInputPositionTexture input)
{
	VertexShaderOutputPositionTexture output;
	output.Position = mul(input.Position, WorldViewProjection);
	output.TextureCoordinate = input.TextureCoordinate;
	return output;
}

float4 PixelShaderFunctionPositionTexture(VertexShaderOutputPositionTexture input) : PIXEL_TARGET_SEMANTIC
{
	return SAMPLE_TEXTURE(Texture, input.TextureCoordinate) * DiffuseColor;
}

VertexShaderOutputPositionColor VertexShaderFunctionPositionColor(VertexShaderInputPositionColor input)
{
	VertexShaderOutputPositionColor output;
	output.Position = mul(input.Position, WorldViewProjection);
	output.Color = input.Color;
	return output;
}

float4 PixelShaderFunctionPositionColor(VertexShaderOutputPositionColor input) : PIXEL_TARGET_SEMANTIC
{
	return input.Color * DiffuseColor;
}

VertexShaderOutputPositionColorTexture VertexShaderFunctionPositionColorTexture(VertexShaderInputPositionColorTexture input)
{
	VertexShaderOutputPositionColorTexture output;
	output.Position = mul(input.Position, WorldViewProjection);
	output.Color = input.Color;
	output.TextureCoordinate = input.TextureCoordinate;
	return output;
}

float4 PixelShaderFunctionPositionColorTexture(VertexShaderOutputPositionColorTexture input) : PIXEL_TARGET_SEMANTIC
{
	float4 textureColor = SAMPLE_TEXTURE(Texture, input.TextureCoordinate);
	return textureColor * input.Color * DiffuseColor;
}

TECHNIQUE(Position, VertexShaderFunctionPosition, PixelShaderFunctionPosition);
TECHNIQUE(PositionTexture, VertexShaderFunctionPositionTexture, PixelShaderFunctionPositionTexture);
TECHNIQUE(PositionColor, VertexShaderFunctionPositionColor, PixelShaderFunctionPositionColor);
TECHNIQUE(PositionColorTexture, VertexShaderFunctionPositionColorTexture, PixelShaderFunctionPositionColorTexture);
