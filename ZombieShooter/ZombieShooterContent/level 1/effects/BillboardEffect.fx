float4x4 View;
float4x4 Projection;

texture ParticleTexture;
sampler2D texSampler = sampler_state {
	texture = <ParticleTexture>;
};

float2 Size;
float3 Up;
float3 Side;
float3 Position;

float offsetU = 0;
float offsetV = 0;

bool AlphaTest = true;
float AlphaTestValue = 0.5f;
bool AlphaTestGreater = true;

/*********************************************************************************************************/

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float2 UV : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float2 UV : TEXCOORD0;
};

/*********************************************************************************************************/

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;
	float3 position = input.Position + Position;

	float2 offset = float2((input.UV.x - 0.5f) * 2.0f, -(input.UV.y - 0.5f) * 2.0f);

	//position += offset.x * Size.x * Side + offset.y * Size.y * Up;
	float3 side = float3(1, 0, 0);
	float3 up = float3(0, 0.707f, -0.707);
	position += offset.x * Size.x * side + offset.y * Size.y * up;

	output.Position = mul(float4(position, 1), mul(View, Projection));
	input.UV.x *= 0.25;
	input.UV.y *= 0.5;
	output.UV = input.UV + float2(offsetU, offsetV);

	return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 color = tex2D(texSampler, input.UV);
	if (AlphaTest)
		clip((color.a - AlphaTestValue) * (AlphaTestGreater ? 1 : -1));
	return color;
}

/*********************************************************************************************************/

technique Technique1
{
	pass Pass1
	{
		VertexShader = compile vs_2_0 VertexShaderFunction();
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
}