#pragma once

#include <iostream>
#include <string>

#ifdef DLL_EXPORT
#define DLL_API extern "C" _declspec(dllexport)
#else
#define DLL_API _declspec(dllimport)
#endif // DLL_EXPORT

extern "C"
{
	DLL_API void UsingCommonType(int arg1, double arg2, float arg3, int* output1, double* output2, float* output3);

	typedef struct _image
	{
		int Width = 0;
		int Height = 0;
		unsigned char* Data = nullptr;
	}Image;
	DLL_API void UsingStruct(Image* inputImage, Image* outputImage);

	DLL_API void ReleaseImage(Image* inputImage);
}