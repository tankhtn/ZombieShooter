sampler2D tex[1];

float ElapsedTimeSinceBegin;

float WidthWindow;
float HeightWindow;

texture BloodTexture;
sampler BloodTextureSampler = sampler_state {
	texture = <BloodTexture>;
	AddressU = Wrap;
	AddressV = Wrap;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
};

float DieFactor;

/*****************************************************************************************/

float4 PS_Blood(float4 Position : POSITION0, 
	float2 UV : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(tex[0], UV);
	color.r += DieFactor / 5;
	float4 bloodColor = tex2D(BloodTextureSampler, UV);

	float4 outColor = lerp(color, bloodColor, (1 - bloodColor.g) * DieFactor);

	return outColor;
}

technique blood
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PS_Blood();
    }
}

/*****************************************************************************************/