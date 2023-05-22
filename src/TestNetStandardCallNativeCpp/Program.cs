﻿// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

Console.WriteLine("Beging testing .NET Standard using native c++ dll.");
var test = new TestCallNativeCppClass();
test.TestUsingCommonTypeOutput();
test.TestPassSeftDefineStruct();
Console.ReadLine();


class TestCallNativeCppClass
{
    public TestCallNativeCppClass()
    { 
    }

    [DllImport("../../NativeCppDll.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void UsingCommonType(int arg1, double arg2, float arg3, out int output1, out double output2, out float output3);

    public void TestUsingCommonTypeOutput()
    {
        Console.WriteLine("***** Test ussing common type and receive output!");
        int arg1 = 1;
        double arg2 = 2.0;
        float arg3 = 3;
        int output1 = 0;
        double output2 = 0.0;
        float output3 = 0;

        UsingCommonType(arg1, arg2, arg3, out output1, out output2, out output3);

        Debug.Assert(output1 == 2);
        Debug.Assert(output2 == 4.0);
        Debug.Assert(output3 == 6);

        Console.WriteLine($"output1 = {output1}, output2 = {output2}, output3 = {output3}");
    }

    [StructLayout(LayoutKind.Sequential)]
    struct ImageStruct
    {
        public int Width;
        public int Height;
        public byte[] Date;
    }

    ImageStruct ConvertBitmap2ImageStruct(Bitmap bm)
    { 
        ImageStruct ret = new ImageStruct();
        ret.Width = bm.Width;
        ret.Height = bm.Height;
        ret.Date = new byte[bm.Width * bm.Height];

        BitmapData bd = bm.LockBits(new Rectangle(0, 0, bm.Width, bm.Height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
        int stride = bd.Stride;
        int skipByte = stride - bd.Width;
        int size = bm.Width * bm.Height;
        unsafe
        {
            byte* p = (byte*)bd.Scan0.ToPointer();
            int colIdx = 0;
            for (int i = 0; i < size; i++)
            {
                ret.Date[i] = *p;
                p = p + 1;
                colIdx = colIdx + 1;
                if (colIdx == bm.Width - 1)
                { 
                    p = p + skipByte;
                    colIdx = 0;
                }
            }
        }
        bm.UnlockBits( bd );

        return ret;
    }

    [DllImport("../../NativeCppDll.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void UsingStruct(ImageStruct inputImage, ref ImageStruct outputImage, int arg1, double arg2);

    public void TestPassSeftDefineStruct()
    {
        Console.WriteLine($"\n\n***** Test pass seft definition struct!");
        System.Drawing.Bitmap img = new System.Drawing.Bitmap(AppDomain.CurrentDomain.BaseDirectory + "../../../Images/lena_gray.bmp");
        Console.WriteLine($"Read image: width = {img.Width}, height = {img.Height}, pixel format = {img.PixelFormat.ToString()}");
        ImageStruct input = ConvertBitmap2ImageStruct(img);
        ImageStruct output = new ImageStruct();

        UsingStruct(input, ref output, 1, 2.0);
    }
}