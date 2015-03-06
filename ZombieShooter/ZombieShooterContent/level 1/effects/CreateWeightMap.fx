
sampler2D tex[1];

float3 BaseColor;

float4 PixelShaderFunction(float4 Position : POSITION0, 
	float2 UV : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(tex[0], UV);

	float base = BaseColor.r;
	float c = color.r;

	float r = lerp(0, 1, (c - base) / (1 - base));
	r = saturate(r);

	float g = 0;

	float b = lerp(0, 1, (base - c) / base);
	b = saturate(b);

	return float4(r, g, b, 1);
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}