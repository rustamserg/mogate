sampler TextureSampler : register(s0);
float ColorAmount;

float4 main(float2 texCoord : TEXCOORD0) : COLOR0
{
    float4 tex = tex2D(TextureSampler, texCoord); 
    float3 colrgb = tex.rgb;
    colrgb.rgb = lerp(float3(0, 0, 0), colrgb, ColorAmount);

    return float4(colrgb.rgb, tex.a);
}

technique Technique0
{
    pass Pass0
    {
        PixelShader = compile ps_2_0 main();
    }
}
