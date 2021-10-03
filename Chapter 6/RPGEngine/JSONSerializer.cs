using System.Text;

using System.Text.Json;
using System.Text.Json.Serialization;

using RPGEngine.Interfaces.Serialization;

namespace RPGEngine
{
    public class JSONSerializer : ISerailizer
    {
        /// <summary>
        /// Serialize a given object
        /// </summary>
        /// <typeparam name="T">The type to serialize</typeparam>
        /// <param name="obj">The object to serialize. It must be compatible with the given JsonConvert.SerializeObject</param>
        /// <returns>A serialized string of obj</returns>
        public virtual string Serialize<T>(T obj)
        {
            return JsonSerializer.Serialize(obj);
        }

        /// <summary>
        /// Method to de-serialize a string back into an instance of T
        /// </summary>
        /// <typeparam name="T">The type to de-serialize to</typeparam>
        /// <param name="data">The data string to de-serialize</param>
        /// <returns>De serialized instance of T</returns>
        public virtual T DeSerialize<T>(string data)
        {
            return JsonSerializer.Deserialize<T>(data);
        }

        /// <summary>
        /// Serializes an instance of T into a byte[]
        /// </summary>
        /// <typeparam name="T">TYpe to be serialized</typeparam>
        /// <param name="obj">Instance of T to be serialized</param>
        /// <returns>byte array of serialized data</returns>
        public virtual byte[] SerializeToBytes<T>(T obj)
        {
            return Encoding.UTF8.GetBytes(Serialize<T>(obj));
        }

        /// <summary>
        /// De-serialize a byts array to an instance of T
        /// </summary>
        /// <typeparam name="T">The type to de-serialize to</typeparam>
        /// <param name="data">byte array to be de-serialized</param>
        /// <returns>De-serialized instance of T</returns>
        public virtual T DeSerializeFromBytes<T>(byte[] data)
        {
            return DeSerialize<T>(Encoding.UTF8.GetString(data));
        }
    }
}
