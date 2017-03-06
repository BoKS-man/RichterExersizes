using System;
using System.Windows.Forms;
using System.IO;
// Объявление делегата; экземпляр ссылается на метод
// с параметром типа Int32, возвращающий значение void

namespace ex1
{
    internal delegate void Feedback(Int32 value);
    public sealed class delegates_1
    {
        public static void Start()
        {
            StaticDelegateDemo();
            InstanceDelegateDemo();
            ChainDelegateDemo1(new delegates_1());
            ChainDelegateDemo2(new delegates_1());
        }
        private static void StaticDelegateDemo()
        {
            Console.WriteLine("----- Static Delegate Demo -----");
            Counter(1, 3, null);
            Counter(1, 3, new Feedback(delegates_1.FeedbackToConsole));
            Counter(1, 3, new Feedback(FeedbackToMsgBox)); // Префикс "Program."
                                                           // не обязателен
            Console.WriteLine();
        }
        private static void InstanceDelegateDemo()
        {
            Console.WriteLine("----- Instance Delegate Demo -----");
            delegates_1 p = new delegates_1();
            Counter(1, 3, new Feedback(p.FeedbackToFile));
            Console.WriteLine();
        }
        private static void ChainDelegateDemo1(delegates_1 p)
        {
            Console.WriteLine("----- Chain Delegate Demo 1 -----");
            Feedback fb1 = new Feedback(FeedbackToConsole);
            Feedback fb2 = new Feedback(FeedbackToMsgBox);
            Feedback fb3 = new Feedback(p.FeedbackToFile);
            Feedback fbChain = null;
            fbChain = (Feedback)Delegate.Combine(fbChain, fb1);
            fbChain = (Feedback)Delegate.Combine(fbChain, fb2);
            fbChain = (Feedback)Delegate.Combine(fbChain, fb3);
            Counter(1, 2, fbChain);
            Console.WriteLine();
            fbChain = (Feedback)
            Delegate.Remove(fbChain, new Feedback(FeedbackToMsgBox));
            Counter(1, 2, fbChain);
        }
        private static void ChainDelegateDemo2(delegates_1 p)
        {
            Console.WriteLine("----- Chain Delegate Demo 2 -----");
            Feedback fb1 = new Feedback(FeedbackToConsole);
            Feedback fb2 = new Feedback(FeedbackToMsgBox);
            Feedback fb3 = new Feedback(p.FeedbackToFile);
            Feedback fbChain = null;
            fbChain += fb1;
            fbChain += fb2;
            fbChain += fb3;
            Counter(1, 2, fbChain);
            Console.WriteLine();
            fbChain -= new Feedback(FeedbackToMsgBox);
            Counter(1, 2, fbChain);
        }
        private static void Counter(Int32 from, Int32 to, Feedback fb)
        {
            for (Int32 val = from; val <= to; val++)
            {
                // Если указаны методы обратного вызова, вызываем их
                if (fb != null)
                    fb(val);
            }
        }
        private static void FeedbackToConsole(Int32 value)
        {
            Console.WriteLine("Item=" + value);
        }
        private static void FeedbackToMsgBox(Int32 value)
        {
            MessageBox.Show("Item=" + value);
        }
        private void FeedbackToFile(Int32 value)
        {
            using (StreamWriter sw = new StreamWriter("Status", true))
            {
                sw.WriteLine("Item=" + value);
            }
        }
    }
}