#pragma once

#include <iostream>
#include <string>
#include "opencv2/opencv.hpp"

#ifdef DLL_EXPORT
// #define DLL_API extern "C" _declspec(dllexport) 
#define DLL_API _declspec(dllexport) 
#else
// #define DLL_API extern "C" _declspec(dllimport) 
#define DLL_API _declspec(dllimport) 
#endif // DLL_EXPORT

extern "C"
{
	DLL_API void __cdecl UsingCommonType(int arg1, double arg2, float arg3, int* output1, double* output2, float* output3);

	typedef struct _image
	{
		int Width = 0;
		int Height = 0;
		//int Depth = 0; // 1:8bit, 2:16bit
		int Channel = 0;
		unsigned char* Data = nullptr;
	}Image;
	DLL_API void __cdecl UsingStruct(Image* inputImage, Image* outputImage, int arg1, double arg2);

	DLL_API void __cdecl ReleaseImage(Image* inputImage);

	DLL_API void __cdecl TestThrowStdException();
	DLL_API void __cdecl TestThrowStringException();
	DLL_API void __cdecl TestThrowCustomerException();

	typedef struct _structWithArr
	{
		int IParam[10];
		double DParam[10];

		_structWithArr()
		{
			memset(IParam, 0, sizeof(int) * 10);
			memset(DParam, 0.0, sizeof(double) * 10);
		}
	}StructWithArr;
	DLL_API void __cdecl TestPassStructWithArr(StructWithArr* obj);
	DLL_API Image* __cdecl ReadImage(const char* path);
}

DLL_API void __cdecl UsingCommonTypeNotUsingExtrentC(int arg1, double arg2, float arg3, int* output1, double* output2, float* output3);

inline Image* ConvertCvMat2Image(cv::Mat mat)
{
	Image* ret = new Image();
	ret->Height = mat.rows;
	ret->Width = mat.cols;
	ret->Channel = mat.channels();
	ret->Data = new unsigned char[ret->Height * ret->Width * ret->Channel];

	if (mat.data == nullptr)
		throw "In ConvertCvMat2Image, input cv::Mat::data is null";
	if (ret->Data == nullptr)
		throw "In ConvertCvMat2Image, output Image::Data allocation fail";

	memcpy(ret->Data, mat.data, ret->Height * ret->Width * ret->Channel);

	return ret;
}

inline void CopyMat2Image(cv::Mat mat, Image* image)
{
	image->Height = mat.rows;
	image->Width = mat.cols;
	image->Channel = mat.channels();
	image->Data = new unsigned char[image->Height * image->Width * image->Channel];

	if (mat.data == nullptr)
		throw "In ConvertCvMat2Image, input cv::Mat::data is null";
	if (image->Data == nullptr)
		throw "In ConvertCvMat2Image, output Image::Data allocation fail";

	memcpy(image->Data, mat.data, image->Height * image->Width * image->Channel);
}