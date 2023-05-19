#include "ImageProcessingFunc.h"

void UsingCommonType(int arg1, double arg2, float arg3, int* output1, double* output2, float* output3)
{
	std::cout << "receive arguments: arg1 = " << arg1 << ", arg2 = " << arg2 << ", arg3 = " << arg3 << std::endl;

	*output1 = arg1 + 1;
	*output2 = arg2 + 2.0;
	*output3 = arg3 + 3;
}

void UsingStruct(Image* inputImage, Image* outputImage, int arg1, double arg2)
{
	outputImage = new Image();
	
}

void ReleaseImage(Image* inputImage)
{
}