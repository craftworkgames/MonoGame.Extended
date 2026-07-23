// Vertex shader input structures.

struct VertexShaderInputPosition
{
    float4 Position : POSITION_INPUT_SEMANTIC;
};

struct VertexShaderInputPositionColor
{
    float4 Position : POSITION_INPUT_SEMANTIC;
    float4 Color : COLOR0;
};

struct VertexShaderInputPositionTexture
{
    float4 Position : POSITION_INPUT_SEMANTIC;
    float2 TextureCoordinate : TEXCOORD0;
};

struct VertexShaderInputPositionColorTexture
{
    float4 Position : POSITION_INPUT_SEMANTIC;
	float4 Color : COLOR0;
    float2 TextureCoordinate : TEXCOORD0;
};

// Vertex shader output structures.

struct VertexShaderOutputPosition
{
    float4 Position : POSITION_OUTPUT_SEMANTIC;
};

struct VertexShaderOutputPositionColor
{
    float4 Position : POSITION_OUTPUT_SEMANTIC;
    float4 Color : COLOR0;
};

struct VertexShaderOutputPositionTexture
{
    float4 Position : POSITION_OUTPUT_SEMANTIC;
    float2 TextureCoordinate : TEXCOORD0;
};

struct VertexShaderOutputPositionColorTexture
{
    float4 Position : POSITION_OUTPUT_SEMANTIC;
    float4 Color : COLOR0;
	float2 TextureCoordinate : TEXCOORD0;
};
