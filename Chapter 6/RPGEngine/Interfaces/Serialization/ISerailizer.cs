using System;
using System.Collections.Generic;
using System.Text;

namespace RPGEngine.Interfaces.Serialization
{
    public interface ISerailizer
    {
        string Serialize<T>(T obj);
        T DeSerialize<T>(string data);

        byte[] SerializeToBytes<T>(T obj);
        T DeSerializeFromBytes<T>(byte[] data);
    }
}
