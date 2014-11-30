sampler s0;  
texture lightMask;  
sampler lightSampler = sampler_state{Texture = lightMask;};  
  
float4 main(float2 coords: TEXCOORD0) : COLOR0  
{  
    float4 color = tex2D(s0, coords);  
    float4 lightColor = tex2D(lightSampler, coords);  
    return color * lightColor;  
}  

technique Technique0
{
    pass Pass0
    {
        PixelShader = compile ps_2_0 main();
    }
}
