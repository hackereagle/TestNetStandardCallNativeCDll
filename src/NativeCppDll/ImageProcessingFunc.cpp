#include "ImageProcessingFunc.h"
//#include "opencv2/opencv.hpp"

//Image* ConvertCvMat2Image(cv::Mat mat)
//{
//	Image* ret = new Image();
//	ret->Height = mat.rows;
//	ret->Width = mat.cols;
//	ret->Data = new unsigned char[ret->Height * ret->Width];
//	memcpy(ret->Data, mat.data, ret->Height * ret->Width);
//
//	return ret;
//}

void UsingCommonType(int arg1, double arg2, float arg3, int* output1, double* output2, float* output3)
{
	std::cout << "In dll, receive arguments: arg1 = " << arg1 << ", arg2 = " << arg2 << ", arg3 = " << arg3 << std::endl;

	*output1 = arg1 + 1;
	*output2 = arg2 + 2.0;
	*output3 = arg3 + 3;
}

void UsingStruct(Image* inputImage, Image* outputImage, int arg1, double arg2)
{
	std::cout << "In dll, input image = {Width = " << inputImage->Width << ", Height = " << inputImage->Height << ", Data[0] = " << (int)*inputImage->Data << "\}, \n"
		<< "arg1 = " << arg1 << ", arg2 = " << arg2 << std::endl;
	
	cv::Mat img(inputImage->Height, inputImage->Width, CV_8UC1, inputImage->Data);
	std::cout << "finish input image convert" << std::endl;
	cv::Mat blurImg;
	try {
		cv::blur(img, blurImg, cv::Size(9, 9));
	}
	catch (cv::Exception& ex) {
		std::cout << "In UsingStruct, occur exception when do blur: " << ex.what() << std::endl;
	}
	catch (std::exception& ex) {
		std::cout << "In UsingStruct, occur exception when do blur: " << ex.what() << std::endl;
	}
	std::cout << "finish blur" << std::endl;
	
	CopyMat2Image(blurImg, outputImage);
	std::cout << "In dll, input image = {Width = " << outputImage->Width << ", Height = " << outputImage->Height << ", Data[0] = " << (int)*outputImage->Data << "\}" << std::endl;
}

void ReleaseImage(Image* inputImage)
{
	inputImage->Height = 0;
	inputImage->Width = 0;
	delete [] inputImage->Data;
	inputImage->Data = nullptr;
}