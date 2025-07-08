using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TestNetStandardCallNativeCpp
{
    internal class TheOtherPInvokeClass
    {
        private const string DLL_PATH = "../../NativeCppDll.dll";

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


            Console.WriteLine($"output1 = {output1}, output2 = {output2}, output3 = {output3}");
        }
    }
}
