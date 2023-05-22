#pragma once

#include <iostream>
#include <string>
#include "opencv2/opencv.hpp"

#ifdef DLL_EXPORT
#define DLL_API extern "C" _declspec(dllexport) 
#else
#define DLL_API _declspec(dllimport) 
#endif // DLL_EXPORT

extern "C"
{
	DLL_API void __cdecl UsingCommonType(int arg1, double arg2, float arg3, int* output1, double* output2, float* output3);

	typedef struct _image
	{
		int Width = 0;
		int Height = 0;
		unsigned char* Data = nullptr;
	}Image;
	DLL_API void __cdecl UsingStruct(Image* inputImage, Image* outputImage, int arg1, double arg2);

	DLL_API void __cdecl ReleaseImage(Image* inputImage);
}

inline Image* ConvertCvMat2Image(cv::Mat mat)
{
	Image* ret = new Image();
	ret->Height = mat.rows;
	ret->Width = mat.cols;
	ret->Data = new unsigned char[ret->Height * ret->Width];

	if (mat.data == nullptr)
		throw "In ConvertCvMat2Image, input cv::Mat::data is null";
	if (ret->Data == nullptr)
		throw "In ConvertCvMat2Image, output Image::Data allocation fail";

	memcpy(ret->Data, mat.data, ret->Height * ret->Width);

	return ret;
}

inline void CopyMat2Image(cv::Mat mat, Image* image)
{
	image->Height = mat.rows;
	image->Width = mat.cols;
	image->Data = new unsigned char[image->Height * image->Width];

	if (mat.data == nullptr)
		throw "In ConvertCvMat2Image, input cv::Mat::data is null";
	if (image->Data == nullptr)
		throw "In ConvertCvMat2Image, output Image::Data allocation fail";

	memcpy(image->Data, mat.data, image->Height * image->Width);
}