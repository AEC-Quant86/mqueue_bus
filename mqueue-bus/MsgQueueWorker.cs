using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;

namespace mqueue_bus
{
    public enum queueType
    {
        LOCAL = 0,
        SHARED,
        BOTH
    }
    public class MsgQueueWorker<T>
    {
        /// <summary>
        /// Очередь, содержащая в себе сообщения
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public string name;
        private queueType type { get; }
        private Queue<T> msgQueue = new Queue<T>();
        private Thread worker;
        private Mutex sendMutex = new Mutex();
        private Mutex addMutex = new Mutex();
        private delegate void Send(T msg);
        private Send send;
        public MsgQueueWorker(string name, queueType type = queueType.LOCAL)
        {
            this.name = name;
            this.type = type;
            switch (type)
            {
                case queueType.LOCAL:
                    send = localMsg;
                    break;
                case queueType.SHARED:
                    send = sharedMsg;
                    break;
                case queueType.BOTH:
                    send = localMsg;
                    send += sharedMsg;
                    break;
            }
            worker = new Thread(new ThreadStart(doStuff));
            worker.IsBackground = true;
            worker.Start();
        }

        /// <summary>
        /// Поток, отправляющий сообщения из очереди
        /// </summary>
        private void doStuff()
        {
            while (true)
            {
                if (msgQueue.Count == 0 || Msg == null)
                    Thread.Sleep(10);
                else
                {
                    sendMutex.WaitOne();
                    send(msgQueue.Dequeue());
                    sendMutex.ReleaseMutex();
                }
            }
        }


        //send metods
        private void localMsg(T msg) => Msg(msg);
        private void sharedMsg(T msg) { }

        //Inputs

        /// <summary>
        /// Добавляет сообщение в очередь
        /// </summary>
        /// <param name="msg"></param>
        public void addMsg(T msg)
        {
            addMutex.WaitOne();
            if (msg != null)
                msgQueue.Enqueue(msg);
            addMutex.ReleaseMutex();
        }

        /// <summary>
        /// Выполняет блокирующую отправку
        /// (вызвавший поток остановится, в одидании возврата
        /// этой функции)
        /// </summary>
        /// <param name="msg"></param>
        public void sendBlockingMsg(T msg)
        {
            sendMutex.WaitOne();
            if (blockingMsg != null)
                blockingMsg(msg);
            sendMutex.ReleaseMutex();
        }

        //Output
        //Событие и его делегат
        public delegate void SendMsg(T msg);
        public event SendMsg Msg;

        //делегат блокирующего сообщения
        public delegate int BlockingMsg(T msg);
        public BlockingMsg blockingMsg { get; set; }
    }
}
