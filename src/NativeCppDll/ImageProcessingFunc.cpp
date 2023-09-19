#include "ImageProcessingFunc.h"

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

void UsingCommonTypeNotUsingExtrentC(int arg1, double arg2, float arg3, int* output1, double* output2, float* output3)
{
	std::cout << "In dll not extern C function, receive arguments: arg1 = " << arg1 << ", arg2 = " << arg2 << ", arg3 = " << arg3 << std::endl;

	*output1 = arg1 + 4;
	*output2 = arg2 + 5.0;
	*output3 = arg3 + 6;
}

void TestThrowStdException()
{
	throw new std::exception("Test throw std::exception.");
}

void TestThrowStringException()
{
	throw "Test throw string exception";
}

class NativeException : public std::exception
{
public:
	NativeException() {}

	NativeException(std::string msg) : _message(msg) {}

	~NativeException() {}

	virtual const char* what() const throw()
	{
		return this->_message.c_str();
	}

private:
	std::string _message = "In native c++ code occur any execution!";
};

void TestThrowCustomerException()
{
	throw new NativeException("Test throw exception with self defined exception class!");
}

void TestPassStructWithArr(StructWithArr* obj)
{
	std::cout << "Show integer array:" << std::endl;
	for (int i = 0; i < 10; i++)
		std::cout << obj->IParam[i] << " ";
	std::cout << std::endl;
	
	std::cout << "Show double array:" << std::endl;
	for (int i = 0; i < 10; i++)
		std::cout << obj->DParam[i] << " ";
	std::cout << std::endl;

	obj->DParam[5] = 100.2345;
}

Image* ReadImage(const char* path)
{
	Image* ret = nullptr;

	cv::Mat img;
	img = cv::imread(path, cv::IMREAD_GRAYSCALE);
	if (!img.empty())
		ret = ConvertCvMat2Image(img);
	else
		std::cout << "ERROR: read image \"" << path << "\" FAIL!" << std::endl;

	return ret;
}