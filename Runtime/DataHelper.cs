using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace BB.BardicFramework
{
    public static class DataHelper
    {

        //https://stackoverflow.com/questions/1446547/how-to-convert-an-object-to-a-byte-array-in-c-sharp
        public static byte[] To(object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public static T To<T>(byte[] arrBytes)
        {
            using (var memStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                var obj = binForm.Deserialize(memStream);
                return (T)obj;
            }
        }
#if UNITY_EDITOR
        public static byte[] To(SerializedProperty property)
        {
            byte[] data = default;
            return data;
        }
        public static T To<T>(SerializedProperty prop) => To<T>(To(prop));

        public static void Set(SerializedProperty property, byte[] data)
        {
        }

#endif
    }

}