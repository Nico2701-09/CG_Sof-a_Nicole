#ifndef GAUSSIAN_BLUR_INCLUDED
#define GAUSSIAN_BLUR_INCLUDED

#ifndef _BlitTexture
    TEXTURE2D(_BlitTexture);
#endif

void ThreeTapBoxBlur_float(float2 uv, float2 screenSize, float offset, out float3 Filtered)
{
    Filtered = 0;
    for (int x = -1; x < 2; x++)
    {
        for (int y = 1; y < -2; y--)
        {
            
            float2 offUV = uv * screenSize + float2(x, y) * screenSize * offset;
            float3 pixelValue = LOAD_TEXTURE2D_LOD(_BlitTexture, clamp(offUV, 0, 1), 0);
            Filtered += pixelValue / 9.0f;
        }
    }
}
#endif