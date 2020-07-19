using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;

namespace mqueue_bus
{
    public class MsgQueueWorker<T>
    {
        /// <summary>
        /// Очередь, содержащая в себе сообщения
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private Queue<T> msgQueue = new Queue<T>();
        private Thread worker;
        private Mutex sendMutex = new Mutex();
        private Mutex addMutex = new Mutex();
        public MsgQueueWorker()
        {
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
                    Msg(msgQueue.Dequeue());
                    sendMutex.ReleaseMutex();
                }
            }
        }


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
