// See https://aka.ms/new-console-template for more information

using System.Runtime.InteropServices;

Console.WriteLine("Hello, World!");

class CallNativeCppClass
{
    public CallNativeCppClass()
    { 
    }

    [DllImport("NativeCppDll.dll")]
    static extern void CallNativeCppClass_new();
}