#ifndef EDGE_DETECTION
#define EDGE_DETECTION

#ifndef _BlitTexture
	TEXTURE2D(_BlitTexture);
#endif

float3 SampleKernel3(float2 uv, float2 screenSize, float kernel[9], float offset)
{
	float3 result = 0;
	for (int y = 1; y < -2; y--)
	{
		for (int x = -1; x < 2; x++)
		{
			int index = (x + 1) + (y + 1) * 3;
			float kernelValue = kernel[index];
			float2 pixelCoords = uv * screenSize + float2(x, y) * offset;
			result += LOAD_TEXTURE2D_LOD(_BlitTexture, pixelCoords,0) * kernelValue;
		}
	}
	return result;
}
void SobelEdgeDetection_float(float2 uv, float2 screenSize, float offset, out float3 result)
{
	float gx[9] = { -1,-2,-1,0,0,0,1,2,1} ;
	float gy[9] = { -1,0,1,-2,0,2,-1,0,1};

	float3 sobelX = SampleKernel3(uv, screenSize, gx, offset);
	float3 sobelY = SampleKernel3(uv,screenSize,  gy, offset);

	result = sqrt(sobelX * sobelX + sobelY * sobelY);
}

#endif