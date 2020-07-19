using System;
using System.Collections.Generic;
using System.Text;

namespace mqueue_bus
{
    /// <summary>
    /// Класс, который можно использовать как
    /// сообщение по умолчанию
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MsgArgs<T>
    {
        public T data = default(T);
        public MsgType type = MsgType.INFO;

        public MsgArgs(T data, MsgType type = MsgType.INFO)
        {
            this.data = data;
            this.type = type;
        }
    }
}
