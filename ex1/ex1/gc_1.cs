using System;
using System.Runtime.InteropServices;
public static class gc_1
{
    public static void MemoryPressureDemo(Int32 size)
    {
        Console.WriteLine();
        Console.WriteLine("MemoryPressureDemo, size={0}", size);
        // Создание набора объектов с указанием их логического размера
        for (Int32 count = 0; count < 15; count++)
        {
            new BigNativeResource(size);
        }
        // В демонстрационных целях очищаем все
        GC.Collect();
    }
    private sealed class BigNativeResource
    {
        private Int32 m_size;
        public BigNativeResource(Int32 size)
        {
            m_size = size;
            // Пусть уборщик думает, что объект занимает больше памяти
            if (m_size > 0) GC.AddMemoryPressure(m_size);
            Console.WriteLine("BigNativeResource create.");
        }
        ~BigNativeResource()
        {
            // Пусть уборщик думает, что объект освободил больше памяти
            if (m_size > 0) GC.RemoveMemoryPressure(m_size);
            Console.WriteLine("BigNativeResource destroy.");
        }
    }
    public static void HandleCollectorDemo()
    {
        Console.WriteLine();
        Console.WriteLine("HandleCollectorDemo");
        for (Int32 count = 0; count < 10; count++) new LimitedResource();
        // В демонстрационных целях очищаем все
        GC.Collect();
    }
    private sealed class LimitedResource
    {
        // Создаем объект HandleCollector и передаем ему указание
        // перейти к очистке,когда в куче появится два или более
        // объекта LimitedResource
        private static HandleCollector s_hc =
        new HandleCollector("LimitedResource", 2);
        public LimitedResource()
        {
            // Сообщаем HandleCollector, что в кучу добавлен еще
            // один объект LimitedResource
            s_hc.Add();
            Console.WriteLine("LimitedResource create. Count={0}", s_hc.Count);
        }
        ~LimitedResource()
        {
            // Сообщаем HandleCollector, что один объект LimitedResource
            // удален из кучи
            s_hc.Remove();
            Console.WriteLine("LimitedResource destroy. Count={0}", s_hc.Count);
        }
    }
}