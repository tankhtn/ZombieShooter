float4x4 World;
float4x4 View;
float4x4 Projection;

float3 DiffuseColor = float3(0, 0, 0);

texture BasicTexture;
sampler BasicTextureSampler = sampler_state {
	texture = <BasicTexture>;
	AddressU = Wrap;
	AddressV = Wrap;
	MinFilter = Anisotropic;
	MagFilter = Anisotropic;
};
bool TextureEnabled;

float3 LightDirection = float3(1, -1, -1);
float3 LightColor = float3(0.8f, 0.8f, 0.8f);

float ElapsedTime;

bool SelectedBuilding = false;

/*******************************************************************************************/

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float2 UV : TEXCOORD0;
	float3 Normal : NORMAL0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float2 UV : TEXCOORD0;
	float3 Normal : TEXCOORD1;
};

/*******************************************************************************************/

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

	output.Normal = mul(input.Normal, World);
	output.UV = input.UV;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float3 color = DiffuseColor;

	if(TextureEnabled)
	{
		color = tex2D(BasicTextureSampler, input.UV);
	}

	if(SelectedBuilding)
		color.r = sin(color.r + ElapsedTime);

	float light = 0.15f;
	light += saturate(-dot(normalize(input.Normal), normalize(LightDirection)));

    return float4(color * light, 1);
}

/*******************************************************************************************/

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
