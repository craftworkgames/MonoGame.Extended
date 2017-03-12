// Vertex shader input structures.

struct VertexShaderInputPosition
{
    float4 Position : SV_Position;
};

struct VertexShaderInputPositionColor
{
    float4 Position : SV_Position;
    float4 Color : COLOR;
};

struct VertexShaderInputPositionTexture
{
    float4 Position : SV_Position;
    float2 TextureCoordinate : TEXCOORD0;
};

struct VertexShaderInputPositionColorTexture
{
    float4 Position : SV_Position;
	float4 Color : COLOR;
    float2 TextureCoordinate : TEXCOORD0;
};

// Vertex shader output structures.

struct VertexShaderOutputPosition
{
    float4 Position : SV_Position;
};

struct VertexShaderOutputPositionColor
{
    float4 Position : SV_Position;
    float4 Color : COLOR0;
};

struct VertexShaderOutputPositionTexture
{
    float4 Position : SV_Position;
    float2 TextureCoordinate : TEXCOORD0;
};

struct VertexShaderOutputPositionColorTexture
{
    float4 Position : SV_Position;
    float4 Color : COLOR0;
	float2 TextureCoordinate : TEXCOORD0;
};
