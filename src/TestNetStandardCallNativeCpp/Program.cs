// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Runtime.InteropServices;

Console.WriteLine("Beging testing .NET Standard using native c++ dll.");
var test = new TestCallNativeCppClass();
test.TestUsingCommonTypeOutput();
Console.ReadLine();

class CallNativeCppClass
{
    public CallNativeCppClass()
    { 
    }

    [DllImport("../../NativeCppDll.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void UsingCommonType(int arg1, double arg2, float arg3, out int output1, out double output2, out float output3);
}

class TestCallNativeCppClass
{
    public TestCallNativeCppClass()
    { 
    }

    public void TestUsingCommonTypeOutput()
    {
        Console.WriteLine("Test ussing common type and receive output!");
        int arg1 = 1;
        double arg2 = 2.0;
        float arg3 = 3;
        int output1 = 0;
        double output2 = 0.0;
        float output3 = 0;

        CallNativeCppClass.UsingCommonType(arg1, arg2, arg3, out output1, out output2, out output3);

        Debug.Assert(output1 == 2);
        Debug.Assert(output2 == 4.0);
        Debug.Assert(output3 == 6);

        Console.WriteLine($"output1 = {output1}, output2 = {output2}, output3 = {output3}");
    }

    public void TestPassSeftDefineStruct()
    { 
    }
}