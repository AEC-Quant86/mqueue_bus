using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Pipes;

namespace mqueue_bus
{
    //IPC enums -->
    public enum queueType
    {
        LOCAL = 0,
        SHARED,
        BOTH
    }
    public enum queueDir
    {
        INPUT = 0,
        OUTPUT
    }
    //<-- IPC enums
    public class MsgQueueWorker<T>
    {
        /// <summary>
        /// Очередь, содержащая в себе сообщения
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private Queue<T> msgQueue = new Queue<T>();
        private Thread worker;
        public string name;
        private Mutex sendMutex = new Mutex();
        private Mutex addMutex = new Mutex();
        private delegate void Send(T msg);
        private Send send;


        //секция IPC -->
        private Thread receiver;
        private queueType type { get; }
        private queueDir dir { get; }
        //<--секция IPC


        public MsgQueueWorker(string name, queueType type = queueType.LOCAL, queueDir dir = queueDir.OUTPUT)
        {
            this.name = name;
            this.type = type;
            this.dir = dir;
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
            if (dir == queueDir.OUTPUT)
            {
                worker = new Thread(new ThreadStart(doStuff));
                worker.IsBackground = true;
                worker.Start();
            }
            if (type >= queueType.SHARED && dir == queueDir.INPUT)
            {
                receiver = new Thread(new ThreadStart(IPC_Recriver));
                receiver.IsBackground = true;
                receiver.Start();
            }
        }

        /// <summary>
        /// Поток, отправляющий сообщения из очереди
        /// </summary>
        private void doStuff()
        {
            while (true)
            {
                if (msgQueue.Count == 0 || send == null)
                    Thread.Sleep(10);
                else
                {
                    sendMutex.WaitOne();
                    send(msgQueue.Dequeue());
                    sendMutex.ReleaseMutex();
                }
            }
        }

        private void IPC_Recriver()
        {
            while (true)
            {

                using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", name, PipeDirection.In))
                {
                    pipeClient.Connect();
                    using (StreamReader input = new StreamReader(pipeClient))
                    {
                        // Display the read text to the console
                        string temp;
                        while ((temp = input.ReadLine()) != null)
                        {
                            sendMutex.WaitOne();
                            if (stringMsg != null)
                                stringMsg(temp);
                            sendMutex.ReleaseMutex();
                        }
                    }
                }

            }
        }


        //send metods
        private void localMsg(T msg)
        {
            //задержка, пока в локальной очереди не появится хоть один приемник
            while (Msg == null)
                Thread.Sleep(10);
            Msg(msg);
        }
        private void sharedMsg(T msg)
        {
            using (NamedPipeServerStream pipeServer = new NamedPipeServerStream(name, PipeDirection.Out))
            {
                try
                {
                    pipeServer.WaitForConnection();
                    using (StreamWriter sw = new StreamWriter(pipeServer))
                    {
                        sw.AutoFlush = true;
                        sw.WriteLine(msg.ToString());
                    }
                }
                catch { }
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
        //Событие отправки локального собщения и его делегат
        public delegate void SendMsg(T msg);
        public event SendMsg Msg;

        //IPC событие приема (string)
        public delegate void StringMsg(string msg);
        public event StringMsg stringMsg;

        //делегат блокирующего сообщения
        public delegate int BlockingMsg(T msg);
        public BlockingMsg blockingMsg { get; set; }
    }

}

