sampler s0;  
texture2D lightMask;  
sampler lightSampler = sampler_state
{
	Texture = <lightMask>;
	AddressU = WRAP;
	AddressV = WRAP;
};

float4 main(float4 pos: SV_POSITION, float4 color1: COLOR0, float2 coords: TEXCOORD0) : SV_TARGET0
{  
    float4 color = tex2D(s0, coords);
    float4 lightColor = tex2D(lightSampler, coords);
    return color * lightColor;
}  

technique Technique0
{
    pass Pass0
    {
        PixelShader = compile ps_4_0_level_9_3 main();
    }
}
