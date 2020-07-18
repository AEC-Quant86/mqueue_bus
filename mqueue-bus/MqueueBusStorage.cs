using System;
using System.Collections.Generic;
using System.Text;

namespace mqueue_bus
{
    public static class MqueueBusStorage<T>
    {
        private static Dictionary<string, MsgQueueWorker<T>> busDictionary =
                                    new Dictionary<string, MsgQueueWorker<T>>();

        public static MsgQueueWorker<T> getMsgQueue(string name)
        {
            MsgQueueWorker<T> output = null;
            if (busDictionary.ContainsKey(name))
                busDictionary.TryGetValue(name, out output);
            return output;
        }

        public static void createMsgQueue(string name)
        {
            busDictionary.Add(name, new MsgQueueWorker<T>());
        }
    }
}
