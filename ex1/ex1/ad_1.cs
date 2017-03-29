using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ex1
{
    public static class ad_1
    {
        public static void Marshalling()
        {
            // Получаем ссылку на домен, в котором исполняется вызывающий поток
            AppDomain adCallingThreadDomain = Thread.GetDomain();
            // Каждому домену присваивается значимое имя, облегчающее отладку
            // Получаем имя домена и выводим его
            String callingDomainName = adCallingThreadDomain.FriendlyName;
            Console.WriteLine(
            "Default AppDomain's friendly name={0}", callingDomainName);
            // Получаем и выводим сборку в домене, содержащем метод Main.
            String exeAssembly = Assembly.GetEntryAssembly().FullName;
            Console.WriteLine("Main assembly={0}", exeAssembly);
            // Определяем локальную переменную, ссылающуюся на домен
            AppDomain ad2 = null;
            // ПРИМЕР 1. Доступ к объектам другого домена приложений
            // с продвижением по ссылке
            Console.WriteLine("{0}Demo #1", Environment.NewLine);
            // Создаем новый домен (с теми же параметрами защиты и конфигурирования)
            ad2 = AppDomain.CreateDomain("AD #2", null, null);
            MarshalByRefType mbrt = null;
            // Загружаем нашу сборку в новый домен, конструируем объект
            // и продвигаем его обратно в наш домен
            // (в действительности мы получаем ссылку на представитель)
            mbrt = (MarshalByRefType)
            ad2.CreateInstanceAndUnwrap(exeAssembly, "MarshalByRefType");
            Console.WriteLine("Type={0}", mbrt.GetType()); // CLR неверно
                                                           // определяет тип
                                                           // Убеждаемся, что получили ссылку на объект-представитель
            Console.WriteLine(
                "Is proxy={0}", RemotingServices.IsTransparentProxy(mbrt));
            // Все выглядит так, как будто мы вызываем метод экземпляра
            // MarshalByRefType, но на самом деле мы вызываем метод типа
            // представителя. Именно представитель переносит поток в тот домен,
            // в котором находится объект, и вызывает метод для реального объекта
            mbrt.SomeMethod();
            // Выгружаем новый домен
            AppDomain.Unload(ad2);
            // mbrt ссылается на правильный объект-представитель;
            // объект-представитель ссылается на неправильный домен
            try
            {
                // Вызываем метод, определенный в типе представителя
                // Поскольку домен приложений неправильный, появляется исключение
                mbrt.SomeMethod();
                Console.WriteLine("Successful call.");
            }
            catch (AppDomainUnloadedException)
            {
                Console.WriteLine("Failed call.");
            }
            // ПРИМЕР 2. Доступ к объектам другого домена
            // с продвижением по значению
            Console.WriteLine("{0}Demo #2", Environment.NewLine);
            // Создаем новый домен (с такими же параметрами защиты
            // и конфигурирования, как в текущем)
            ad2 = AppDomain.CreateDomain("AD #2", null, null);
            // Загружаем нашу сборку в новый домен, конструируем объект
            // и продвигаем его обратно в наш домен
            // (в действительности мы получаем ссылку на представитель)
            mbrt = (MarshalByRefType)
            ad2.CreateInstanceAndUnwrap(exeAssembly, "MarshalByRefType");
            // Метод возвращает КОПИЮ возвращенного объекта;
            // продвижение объекта происходило по значению, а не по ссылке
            MarshalByValType mbvt = mbrt.MethodWithReturn();
            // Убеждаемся, что мы НЕ получили ссылку на объект-представитель
            Console.WriteLine(
            "Is proxy={0}", RemotingServices.IsTransparentProxy(mbvt));
            // Кажется, что мы вызываем метод экземпляра MarshalByRefType,
            // и это на самом деле так
            Console.WriteLine("Returned object created " + mbvt.ToString());
            // Выгружаем новый домен
            AppDomain.Unload(ad2);
            // mbrt ссылается на действительный объект;
            // выгрузка домена не имеет никакого эффекта
            try
            {
                // Вызываем метод объекта; исключение не генерируется
                Console.WriteLine("Returned object created " + mbvt.ToString());
                Console.WriteLine("Successful call.");
            }
            catch (AppDomainUnloadedException)
            {
                Console.WriteLine("Failed call.");
            }
            // ПРИМЕР 3. Доступ к объектам другого домена
            // без использования механизма продвижения
            Console.WriteLine("{0}Demo #3", Environment.NewLine);
            // Создаем новый домен (с такими же параметрами защиты
            // и конфигурирования, как в текущем)
            ad2 = AppDomain.CreateDomain("AD #2", null, null);
            // Загружаем нашу сборку в новый домен, конструируем объект
            // и продвигаем его обратно в наш домен
            // (в действительности мы получаем ссылку на представитель)
            mbrt = (MarshalByRefType)
            ad2.CreateInstanceAndUnwrap(exeAssembly, "MarshalByRefType");
            // Метод возвращает объект, продвижение которого невозможно
            // Генерируется исключение
            NonMarshalableType nmt = mbrt.MethodArgAndReturn(callingDomainName);
            // До выполнения этого кода дело не дойдет...
        }
        // Экземпляры допускают продвижение по ссылке через границы доменов
        public sealed class MarshalByRefType : MarshalByRefObject
        {
            public MarshalByRefType()
            {
                Console.WriteLine("{0} ctor running in {1}",
                this.GetType().ToString(), Thread.GetDomain().FriendlyName);
            }
            public void SomeMethod()
            {
                Console.WriteLine("Executing in " + Thread.GetDomain().FriendlyName);
            }
            public MarshalByValType MethodWithReturn()
            {
                Console.WriteLine("Executing in " + Thread.GetDomain().FriendlyName);
                MarshalByValType t = new MarshalByValType();
                return t;
            }
            public NonMarshalableType MethodArgAndReturn(String callingDomainName)
            {
                // ПРИМЕЧАНИЕ: callingDomainName имеет атрибут [Serializable]
                Console.WriteLine("Calling from '{0}' to '{1}'.",
                callingDomainName, Thread.GetDomain().FriendlyName);
                NonMarshalableType t = new NonMarshalableType();
                return t;
            }
        }
        // Экземпляры допускают продвижение по значению через границы доменов
        [Serializable]
        public sealed class MarshalByValType : Object {
            private DateTime m_creationTime = DateTime.Now;
            // ПРИМЕЧАНИЕ: DateTime помечен атрибутом [Serializable]
            public MarshalByValType() {
                Console.WriteLine("{0} ctor running in {1}, Created on {2:D}",
                this.GetType().ToString(),
                Thread.GetDomain().FriendlyName,
                m_creationTime);
            }
            public override String ToString() {
                return m_creationTime.ToLongDateString();
            }
        }
        // Экземпляры не допускают продвижение между доменами
        // [Serializable]
        public sealed class NonMarshalableType : Object {
            public NonMarshalableType() {
                Console.WriteLine("Executing in " + Thread.GetDomain().FriendlyName);
            }
        }
        /*
Собрав и выполнив это приложение, мы получим следующее:
Default AppDomain's friendly name= Ch22-1-AppDomains.exe
Main assembly=Ch22-1-AppDomains, Version=0.0.0.0,
Culture=neutral, PublicKeyToken=null
Demo #1
MarshalByRefType ctor running in AD #2
Type=MarshalByRefType
Is proxy=True
Executing in AD #2
Failed call.
Demo #2
MarshalByRefType ctor running in AD #2
Executing in AD #2
MarshalByValType ctor running in AD #2, Created on Friday, August 07, 2009
Is proxy=False
Returned object created Friday, August 07, 2009
Returned object created Friday, August 07, 2009
Successful call.
Demo #3
MarshalByRefType ctor running in AD #2

*/
            }
    }
