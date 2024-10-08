﻿// See https://aka.ms/new-console-template for more information

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

Console.WriteLine("Beging testing .NET Standard using native c++ dll.");
var test = new TestCallNativeCppClass();
test.TestUsingCommonTypeOutput();
test.TestPassSeftDefineStruct();
test.TestUsingCommonTypeNotUsingExternCOutput();
test.TestNativeCppDllThrowStdException();
test.TestNativeCppDllThrowStringException();
test.TestNativeCppDllThrowCustomerException();
test.TestPassstructWithArr();
test.TestReturnStruct();
Console.ReadLine();


class TestCallNativeCppClass
{
    public TestCallNativeCppClass()
    { 
    }

    [DllImport("../../NativeCppDll.dll", EntryPoint = "UsingCommonType", CallingConvention = CallingConvention.Cdecl)]
    private static extern void UsingCommonType(int arg1, double arg2, float arg3, out int output1, out double output2, out float output3);

    public void TestUsingCommonTypeOutput()
    {
        Console.WriteLine("***** Test ussing common type and receive output!");
        // arrange
        int arg1 = 1;
        double arg2 = 2.0;
        float arg3 = 3;
        int output1 = 0;
        double output2 = 0.0;
        float output3 = 0;

        // act
        UsingCommonType(arg1, arg2, arg3, out output1, out output2, out output3);

        // assert
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
        public IntPtr Data;

        GCHandle _pinnedArray;
        public ImageStruct(Bitmap bm)
        { 
            this.Width = bm.Width;
            this.Height = bm.Height;
            byte[] Date = new byte[bm.Width * bm.Height];

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
                    Date[i] = *p;
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

            //GCHandle pinnedArray = GCHandle.Alloc( Date, GCHandleType.Pinned);
            //this.Data = pinnedArray.AddrOfPinnedObject();
            //pinnedArray.Free();
            _pinnedArray = GCHandle.Alloc( Date, GCHandleType.Pinned);
            this.Data = _pinnedArray.AddrOfPinnedObject();
            _pinnedArray.Free();
        }

        public Bitmap ToBitmap()
        { 
            Bitmap ret = new Bitmap(this.Width, this.Height, PixelFormat.Format8bppIndexed);

            ColorPalette cp = ret.Palette;
            for (int i = 0; i < 256; i++)
                cp.Entries[i] = System.Drawing.Color.FromArgb(255, i, i, i);

            BitmapData bd = ret.LockBits(new Rectangle(0, 0, ret.Width, ret.Height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
            int stride = bd.Stride;
            int skipByte = stride - bd.Width;
            int size = ret.Width * ret.Height;
            unsafe
            {
                byte* p = (byte*)bd.Scan0.ToPointer();
                byte* thisImgPtr = (byte*)Data.ToPointer();
                int colIdx = 0;
                for (int i = 0; i < size; i++)
                {
                    *p = *thisImgPtr;
                    p = p + 1;
                    thisImgPtr = thisImgPtr + 1;

                    colIdx = colIdx + 1;
                    if (colIdx == ret.Width - 1)
                    { 
                        p = p + skipByte;
                        colIdx = 0;
                    }
                }
            }
            ret.UnlockBits( bd );

            return ret;
        }

        public void Release()
        { 
            if (_pinnedArray.IsAllocated)
                _pinnedArray.Free();
        }
    }

    [DllImport("../../NativeCppDll.dll", EntryPoint = "UsingStruct", CallingConvention = CallingConvention.Cdecl)]
    private static extern void UsingStruct(ImageStruct inputImage, ref ImageStruct outputImage, int arg1, double arg2);

    public void TestPassSeftDefineStruct()
    {
        Console.WriteLine($"\n\n***** Test pass seft definition struct!");
        System.Drawing.Bitmap img = new System.Drawing.Bitmap(AppDomain.CurrentDomain.BaseDirectory + "../../../Images/lena_gray.bmp");
        Console.WriteLine($"Read image: width = {img.Width}, height = {img.Height}, pixel format = {img.PixelFormat.ToString()}");
        ImageStruct input = new ImageStruct(img);
        ImageStruct output = new ImageStruct();

        UsingStruct(input, ref output, 1, 2.0);

        int firstDataVal = 0;
        unsafe 
        { 
            firstDataVal = *(byte*)output.Data.ToPointer();
        }

        Debug.Assert(output.Width == 512);
        Debug.Assert(output.Height == 512);
        Debug.Assert(firstDataVal == 134);
        Console.WriteLine($"After UsingStruct, output image: width = {output.Width}, height = {output.Height}, Data[0] = {firstDataVal}");
        input.Release();
    }


    [DllImport("../../NativeCppDll.dll", EntryPoint = "?UsingCommonTypeNotUsingExtrentC@@YAXHNMPEAHPEANPEAM@Z", CallingConvention = CallingConvention.Cdecl)]
    private static extern void UsingCommonTypeNotUsingExtrentC(int arg1, double arg2, float arg3, out int output1, out double output2, out float output3);

    public void TestUsingCommonTypeNotUsingExternCOutput()
    {
        Console.WriteLine("\n\n***** Test using common type not using extern C and receive output!");
        // arrange
        int arg1 = 1;
        double arg2 = 2.0;
        float arg3 = 3;
        int output1 = 0;
        double output2 = 0.0;
        float output3 = 0;

        // act
        UsingCommonTypeNotUsingExtrentC(arg1, arg2, arg3, out output1, out output2, out output3);

        // assert
        Debug.Assert(output1 == 5);
        Debug.Assert(output2 == 7.0);
        Debug.Assert(output3 == 9);

        Console.WriteLine($"output1 = {output1}, output2 = {output2}, output3 = {output3}");
    }

    [DllImport("../../NativeCppDll.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private static extern void TestThrowStdException();
    public void TestNativeCppDllThrowStdException()
    {
        Console.WriteLine("\n\n***** Test catch cpp dll throw std::exception!");
        try
        {
            TestThrowStdException();
        }
        catch (SEHException ex)
        {
            Console.WriteLine($"Catch {ex.GetType().Name}: {ex.Message}, error code = {ex.ErrorCode}\nstack: {ex.StackTrace}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Catch {ex.GetType().Name}: {ex.Message}\nstack: {ex.StackTrace}");
        }
    }

    [DllImport("../../NativeCppDll.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void TestThrowStringException();

    public void TestNativeCppDllThrowStringException()
    {
        Console.WriteLine("\n\n***** Test catch cpp dll throw std::string exception!");
        try
        {
            TestThrowStringException();
        }
        catch (SEHException ex)
        {
            Console.WriteLine($"Catch {ex.GetType().Name}: {ex.Message}, error code = {ex.ErrorCode}\nstack: {ex.StackTrace}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Catch {ex.GetType().Name}: {ex.Message}\nstack: {ex.StackTrace}");
        }
    }

    [DllImport("../../NativeCppDll.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
    private static extern void TestThrowCustomerException();

    public void TestNativeCppDllThrowCustomerException()
    {
        Console.WriteLine("\n\n***** Test catch cpp dll throw std::NativeException!");
        try
        {
            TestThrowCustomerException();
        }
        catch (SEHException ex)
        {
            Console.WriteLine($"Catch {ex.GetType().Name}: {ex.Message}, error code = {ex.ErrorCode}\nstack: {ex.StackTrace}");
        }
        catch (Win32Exception ex)
        {
            Console.WriteLine($"Catch {ex.GetType().Name}: {ex.Message}\nstack: {ex.StackTrace}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Catch {ex.GetType().Name}: {ex.Message}\nstack: {ex.StackTrace}");
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	struct StructWithArr
	{
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
		public int[] IParam;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
		public double[] DParam;
	};
    [DllImport("../../NativeCppDll.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
#if USE_STRUCT_INSTEAD_OF_INTPTR
    private static extern void TestPassStructWithArr(ref StructWithArr obj);
#else
    private static extern void TestPassStructWithArr(IntPtr obj);
#endif

    public void TestPassstructWithArr()
    {
        Console.WriteLine("\n\n***** Test pass struct which have array member to cpp dll *****");
#if USE_STRUCT_INSTEAD_OF_INTPTR
        StructWithArr st = new StructWithArr();
        st.IParam = new int[10];
        st.DParam = new double[10];

        for (int i = 0; i < 10; i++)
            st.IParam[i] = i;

        for (int i = 0; i < 10; i++)
            st.DParam[i] = Convert.ToDouble(i) + (Convert.ToDouble(i) * 0.1);

        TestPassStructWithArr(ref st);
        Console.WriteLine($"5-th element = {st.DParam[5]}");
        Debug.Assert(st.DParam[5] == 100.2345);
#else
        // Due to a fact that struct have array and array is reference, so cannot only pin struct.
        StructWithArr st = new StructWithArr();
        st.IParam = new int[10];
        st.DParam = new double[10];

        for (int i = 0; i < 10; i++)
            st.IParam[i] = i;

        for (int i = 0; i < 10; i++)
            st.DParam[i] = Convert.ToDouble(i) + (Convert.ToDouble(i) * 0.1);

        GCHandle handle = GCHandle.Alloc(st, GCHandleType.Pinned);
        IntPtr ptr = handle.AddrOfPinnedObject();
        TestPassStructWithArr(ptr);
        handle.Free();
        Console.WriteLine($"5-th element = {st.DParam[5]}");
#endif
    }

    [DllImport("../../NativeCppDll.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr ReadImage([MarshalAs(UnmanagedType.LPStr)]string path);
    public void TestReturnStruct()
    { 
        Console.WriteLine("\n\n***** Test Return Struct Native Cpp Function *****");

        string path = AppDomain.CurrentDomain.BaseDirectory + "../../../Images/lena_gray.bmp";
        IntPtr img = ReadImage(path);
        ImageStruct managedImg = (ImageStruct)Marshal.PtrToStructure(img, typeof(ImageStruct))!;
        Debug.Assert(managedImg.Width == 512);
        Console.WriteLine($"Width = {managedImg.Width}");
        Debug.Assert(managedImg.Height == 512);
        Console.WriteLine($"Height = {managedImg.Height}");

        Bitmap bm = managedImg.ToBitmap();
        Debug.Assert(managedImg.Width == 512);
        Console.WriteLine($"Bitmap Width = {bm.Width}");
        Debug.Assert(managedImg.Height == 512);
        Console.WriteLine($"Bitmap Height = {bm.Height}");

    }
}