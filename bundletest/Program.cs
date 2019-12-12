using System;
using System.Runtime.Loader;
using System.Runtime.InteropServices;
using System.IO;

namespace bundletest
{
    class Program
    {
        [DllImport("lib1")]
        static extern void say_hello();
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();
        [DllImport("hoge")]
        public static extern void TestFunction();
        static void Main(string[] args)
        {
            // new Grpc.Core.CallOptions();
            System.Runtime.Loader.AssemblyLoadContext.Default.ResolvingUnmanagedDll += (asm, name) =>
            {
                Console.WriteLine($"asm = {asm.GetName()}, name = {name}");
                if(name == "lib1")
                {
                    var dllPath = Path.Combine(Path.GetDirectoryName(asm.Location), "runtimes", "win-x64", "native", "lib1.dll");
                    return NativeLibrary.Load(dllPath);
                }
                return IntPtr.Zero;
            };
            var attributes = new[] { "PLATFORM_RESOURCE_ROOTS", "NATIVE_DLL_SEARCH_DIRECTORIES", "APP_PATHS", "APP_NI_PATHS" };
            foreach (var attr in attributes)
            {
                var v = AppContext.GetData(attr);
                Console.WriteLine($"{attr} = '{v}'");
            }
            PInvoke.User32.MessageBox(IntPtr.Zero, "hoge", "piyo", PInvoke.User32.MessageBoxOptions.MB_OK);
            Console.WriteLine(typeof(Program).Assembly.Location);
            Console.WriteLine($"{System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory()}");
            GetForegroundWindow();
            say_hello();
        }
    }
}
