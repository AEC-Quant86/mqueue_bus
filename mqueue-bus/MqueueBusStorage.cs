using System;
using System.Collections.Generic;
using System.Text;

namespace mqueue_bus
{
    public static class MqueueBusStorage<T>
    {
        /// <summary>
        /// Содержит в себе именованные очереди сообщений
        /// </summary>
        /// <returns></returns>
        private static Dictionary<string, MsgQueueWorker<T>> busDictionary =
                                    new Dictionary<string, MsgQueueWorker<T>>();

        /// <summary>
        /// Возвращает очередь определенного типа,
        /// по имени
        /// </summary>
        /// <param name="name">Имя очереди</param>
        /// <returns>"Воркер" очереди</returns>
        public static MsgQueueWorker<T> getMsgQueue(string name)
        {
            MsgQueueWorker<T> output = null;
            if (busDictionary.ContainsKey(name))
                busDictionary.TryGetValue(name, out output);
            return output;
        }

        /// <summary>
        /// Создает именованную очередь
        /// определенного типа
        /// </summary>
        /// <param name="name"></param>
        public static void createMsgQueue(string name)
        {
            busDictionary.Add(name, new MsgQueueWorker<T>());
        }
    }
}
