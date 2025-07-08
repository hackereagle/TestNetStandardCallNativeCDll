// See https://aka.ms/new-console-template for more information

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using TestNetStandardCallNativeCpp;

Console.WriteLine("Beging testing .NET Standard using native c++ dll.");
var test = new TestCallNativeCppClass();
test.TestUsingCommonTypeOutput();
test.TestPassSeftDefineStruct();
test.TestUsingCommonTypeNotUsingExternCOutput();
test.TestNativeCppDllThrowStdException();
test.TestNativeCppDllThrowStringException();
test.TestNativeCppDllThrowCustomerException();
test.TestPassStructWithArr();
test.TestReturnStruct();
//test.TestResultStructHaveFixedSizeArray();
for (int i = 0; i < 10000; i++)
{
    test.TestResultStructHaveFixedSizeArray();
    //Console.Read();
    Thread.Sleep(1000);
}
Console.WriteLine("Finished struct having fixed size array pressure testing");

test.TestReturnStructDynamicArray();
//for (int i = 0; i < 3000; i++)
//{
//    test.TestReturnStructDynamicArray();
//    //Console.Read();
//    Thread.Sleep(1000);
//}
Console.WriteLine("Finished struct having dynamic size array pressure testing");

Console.ReadLine();


class TestCallNativeCppClass
{
    private const string DLL_PATH = "../../NativeCppDll.dll";
    public TestCallNativeCppClass()
    { 
    }

    [DllImport(DLL_PATH, EntryPoint = "UsingCommonType", CallingConvention = CallingConvention.Cdecl)]
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


    [DllImport(DLL_PATH, EntryPoint = "UsingStruct", CallingConvention = CallingConvention.Cdecl)]
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


    [DllImport(DLL_PATH, EntryPoint = "?UsingCommonTypeNotUsingExtrentC@@YAXHNMPEAHPEANPEAM@Z", CallingConvention = CallingConvention.Cdecl)]
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

    [DllImport(DLL_PATH, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
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

    [DllImport(DLL_PATH, CallingConvention = CallingConvention.Cdecl)]
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

    [DllImport(DLL_PATH, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
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

    [DllImport(DLL_PATH, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
#if USE_STRUCT_INSTEAD_OF_INTPTR
    private static extern void TestPassStructWithArr(ref StructWithArr obj);
#else
    private static extern void TestPassStructWithArr(IntPtr obj);
#endif

    public void TestPassStructWithArr()
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

    [DllImport(DLL_PATH, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr ReadImage([MarshalAs(UnmanagedType.LPStr)]string path);

    [DllImport(DLL_PATH, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private static extern void ReleaseImage(IntPtr img);
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

        // Rlease resource
        ReleaseImage(img);
    }

    [DllImport(DLL_PATH, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private static extern int TestSetOutputToFixeArrayStruct(int arg1, double arg2, float arg3, IntPtr output);

    public void TestResultStructHaveFixedSizeArray()
    {
        Console.WriteLine("\n\n***** Test Result Struct Have Fixed Size Array *****");

        MyResult result = new MyResult();
        GCHandle handle = GCHandle.Alloc(result, GCHandleType.Pinned);
        IntPtr ptr = handle.AddrOfPinnedObject();
        TestSetOutputToFixeArrayStruct(10, 22.0, 330.0f, ptr);

        //result = (MyResult)Marshal.PtrToStructure(ptr, typeof(MyResult))!; // This is not necessary, because we use GCHandle to pin the struct. And this seems to cause a memory leak.
        result = (MyResult)handle.Target!;

        MyPoint point = result.GetPoint(0);
        Debug.Assert(point.X == 10 + 100);
        Debug.Assert(point.Y == 10 + 200);
        point = result.GetPoint(2500);
        Debug.Assert(point.X == 22 + 100);
        Debug.Assert(point.Y == 22 + 100);
        point = result.GetPoint(4999);
        Debug.Assert(point.X == 330 + 100);
        Debug.Assert(point.Y == 330 + 200);

        result.Release();
        handle.Free();
    }

    [DllImport(DLL_PATH, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
	private static extern IntPtr TestReturnStructDynamicArray(int arg1, double arg2, float arg3);

    [DllImport(DLL_PATH, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
	private static extern void ReleaseMyResult2(IntPtr result);

    public void TestReturnStructDynamicArray()
    {
        Console.WriteLine("\n\n***** Test Return Struct Dynamic Array *****");
        IntPtr ptr = TestReturnStructDynamicArray(10, 22.0, 330.0f);
        MyResult2 result = (MyResult2)Marshal.PtrToStructure(ptr, typeof(MyResult2))!;
        Console.WriteLine($"Count = {result.Count}");

        MyPoint point = result.GetPoint(0);
        Debug.Assert(point.X == 10 + 100);
        Debug.Assert(point.Y == 10 + 200);
        point = result.GetPoint(2);
        Debug.Assert(point.X == 22 + 100);
        Debug.Assert(point.Y == 22 + 200);
        point = result.GetPoint(result.Count - 1);
        Debug.Assert(point.X == 330 + 100);
        Debug.Assert(point.Y == 330 + 200);
        // Rlease resource
        ReleaseMyResult2(ptr);
    }
}