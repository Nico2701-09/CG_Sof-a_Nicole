#ifndef CUSTOM_LIGHTNING_01_INCLUDED
#define CUSTOM_LIGHTNING_01_INCLUDED

//#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

void GetMainLight_float(float3 positionWS, out float3 direction, out float3 color, out float shadowAttenuation)
{
    #if defined(SHADERGRAPH_PREVIEW)
        direction = normalize(float3(1, 1, -1));
        color = float3(1, 1, 1);
        shadowAttenuation = 1.0f;    
    #else
        float4 shadowCoord = TransformWorldToShadowCoord(positionWS);
        Light mainLight = GetMainLight(shadowCoord);
        direction = mainLight.direction;
        color = mainLight.color; 
        shadowAttenuation = mainLight.shadowAttenuation;
    #endif
}

void SimpleLightingAdditional_float(float2 UVss, float3 normalWS, float3 viewDirWS, float3 positionWS, float smoothness, out float3 diffuse, out float3 specular)
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

            float lambert = saturate(dot(normalWS, light.direction));
            diffuse += lambert * light.color * light.distanceAttenuation * light.shadowAttenuation;

            float3 h = normalize(light.direction + viewDirWS);
            float blinnPhong = saturate(dot(normalWS, h));
            float specularPower = exp2((smoothness + 0.01f) * 6);
            specular += pow(blinnPhong, specularPower) * light.color * light.distanceAttenuation * light.shadowAttenuation * lambert;
    
        LIGHT_LOOP_END
    #endif
}

#endif