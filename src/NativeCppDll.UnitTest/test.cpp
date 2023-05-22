#include <string>
#include "pch.h"
#include "ImageProcessingFunc.h"
#include "opencv2/opencv.hpp"

TEST(TestDllFunction, TestUsingCommonTypeOutput) 
{
	// arrange
	int arg1 = 1;
	double arg2 = 2.0;
	float arg3 = 3;

	// act
	int output1 = 0;
	double output2 = 0.0;
	float output3 = 0;
	UsingCommonType(arg1, arg2, arg3, &output1, &output2, &output3);

	// assert
	EXPECT_EQ(output1, arg1 + 1);
	EXPECT_DOUBLE_EQ(output2, arg2 + 2.0);
	EXPECT_EQ(output3, arg3 + 3);
}

TEST(TestDllFunction, TestUsingStructOutput)
{
	// arrange
	std::string file = "../Images/lena_gray.bmp";
	std::cout << file << std::endl;
	cv::Mat img = cv::imread(file, cv::IMREAD_GRAYSCALE);
	std::cout << "image rows = " << img.rows << ", cols = " << img.cols << std::endl;

	// act
	Image* input = ConvertCvMat2Image(img);
	Image* output = new Image();
	UsingStruct(input, output, 1, 2.0);

	// assert
	EXPECT_TRUE(output->Width == img.cols);
	EXPECT_TRUE(output->Height == img.rows);
	ReleaseImage(output);
}