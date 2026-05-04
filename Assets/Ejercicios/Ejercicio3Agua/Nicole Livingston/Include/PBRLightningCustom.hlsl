#ifndef PBR_CUSTOM_INCLUDED
#define PBR_CUSTOM_INCLUDED

void OrenNayar_float(float3 normalWS, float3 lightDirWS, float3 viewDirWS, float3 roughness, out float diffuse)
{
    float3 NdotL = dot(normalWS, lightDirWS);
    float3 NdotV = dot(normalWS, viewDirWS);
    
    float angleLN = acos(NdotL);
    float angleVN = acos(NdotV);

    float alpha = max(angleVN, angleLN);
    float beta = min(angleVN, angleLN);
    float gamma = cos(angleVN - angleLN);

    float roughnessSq = roughness * roughness;

    float a = 1.0f - 0.5f * (roughnessSq/(roughnessSq + 0.57f));
    float b = 0.45f * (roughnessSq/(roughnessSq + 0.09f));
    float c = sin(alpha) * tan(beta);

    float orenNayar = saturate(NdotL) * (a+(b* max (0.0, gamma) + c));

    diffuse = orenNayar;
}

void ShadeAdditionalLightPBR_float (float2 UVss, float3 normalWS, float3 viewDirWS, float3 positionWS, float smoothness, out float3 diffuse, out float3 specular)
{
    diffuse = 0;
    specular = 0;

    #ifndef SHADERGRAPH_PREVIEW
    uint additionalLightCount = GetAdditionalLightsCount();

    #if USE_FORWARD_PLUS
    InputData inputData =(InputData)0;
    inputData.normalizedScreenSpaceUV = UVss;
    inputData.positionWS = positionWS;
    #endif
    LIGHT_LOOP_BEGIN(additionalLightCount)

        Light light = GetAdditionalLight(lightIndex, positionWS, 1);

        float orenNayar = 0;
        OrenNayar_float(normalWS,light.direction, viewDirWS, 1 - smoothness,orenNayar);
        diffuse += orenNayar * light.color * light.distanceAttenuation * light.shadowAttenuation;
        
    
    LIGHT_LOOP_END
#endif
}

#endif