using System;

namespace ex1
{
    public sealed class Program
    {
        public static void Main()
        {
            //delegates_1.Start();

            /*
            gc_1.MemoryPressureDemo(0); // 0 вызывает нечастую уборку мусора
            gc_1.MemoryPressureDemo(10 * 1024 * 1024); // 10 Mбайт вызывают частую
                                                       // уборку мусора
            gc_1.HandleCollectorDemo();
            */

            ad_1.Marshalling();

            Console.ReadLine();
        }
    }
}