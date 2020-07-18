using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;

namespace mqueue_bus
{
    public class MsgQueueWorker<T>
    {
        Queue<T> msgQueue = new Queue<T>();
        private Thread worker;
        private Mutex sendMutex = new Mutex();
        public MsgQueueWorker()
        {
            worker = new Thread(new ThreadStart(doStuff));
            worker.IsBackground = true;
            worker.Start();
        }

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


        //Input
        public void addMsg(T msg)
        {
            if (msg != null)
                msgQueue.Enqueue(msg);
        }

        public void sendBlockingMsg(T msg)
        {
            sendMutex.WaitOne();
            if (blockingMsg != null)
                blockingMsg(msg);
            sendMutex.ReleaseMutex();
        }

        //Output
        public delegate void SendMsg(T msg);
        public event SendMsg Msg;
        public delegate int BlockingMsg(T msg);
        public BlockingMsg blockingMsg { get; set; }
    }
}
