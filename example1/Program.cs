using System;
using mqueue_bus;
using System.Threading;
using exampl_receiver;
using exampl_transmitter;

/*
В качестве примера передаваемых данных была выбрана строка,
Но возможно использование ЛЮБОГО типа для передачи данных,
библиотека mqueue_bus работает на основе шаблонных классов,
главное требованиеЖ класс приемник и класс передачтик должны
"знать" тип передаваемых данных(он должен находиться в области
их видимости)
*/


namespace example1
{
    class Program
    {
        static void Main(string[] args)
        {
            //создаем очередь сообщений с типом данных "string"
            // и названием "mqueue1"
            //создать очередб можно где угодно
            MqueueBusStorage<string>.createMsgQueue("mqueue1");
            //Ждем пока выполнится остальная часть программы
            Receiver rcv = new Receiver();
            Transmitter trs = new Transmitter();
            Thread.Sleep(2000);
        }
    }
}

namespace exampl_receiver
{
    public class Receiver
    {
        // получаем Worker - класс ответственный за
        // прием и отправку сообщений
        private MsgQueueWorker<string> mque
                = MqueueBusStorage<string>.getMsgQueue("mqueue1");
        public Receiver()
        {
            //связываем событие приема сообщения 
            // с функцией обработчиком
            mque.Msg += receiveMsg;
        }

        //Функция обработки приема сообщения
        private void receiveMsg(string s)
        {
            Console.WriteLine(s);
        }
    }
}

namespace exampl_transmitter
{
    // получаем Worker - класс ответственный за
    // прием и отправку сообщений
    public class Transmitter
    {
        private MsgQueueWorker<string> mque
                = MqueueBusStorage<string>.getMsgQueue("mqueue1");
        public Transmitter()
        {
            for (int i = 0; i <= 10; ++i)
            {
                msg(String.Format("msg number is {0}", i));
            }
        }

        //функция, которая отправит сообщение из этого класса, 
        //посредством очереди сообщений
        public void msg(string msg1)
        {
            //вызываем метод, который добавляет сообщения в очередь
            mque.addMsg(msg1);
        }

    }
}