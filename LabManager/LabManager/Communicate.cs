using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace LabManager
{
    /// <summary>
    /// 通信服务类
    /// </summary>
    public static class Communicate
    {
        public enum CmdType
        {
            Write,
            Read
        }
        /// <summary>
        /// 数据库命令结构包
        /// </summary>
        [Serializable]
        public struct DataBaseCmd
        {
            public int Operate;
            public int Row;
            public int Columns;
            public object Value;
            public DataBaseCmd(CmdType operate, int row, int columns, object value = null)
            {
                Operate = (int)operate;
                Row = row;
                Columns = columns;
                Value = value;
            }
        }
        /// <summary>
        /// object序列化
        /// </summary>
        /// <param name="obj">要序列化的object</param>
        /// <returns>返回序列化后的byte[]</returns>
        public static byte[] Object2Bytes(object obj)
        {
            byte[] buff;
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter iFormatter = new BinaryFormatter();
                iFormatter.Serialize(ms, obj);
                buff = ms.GetBuffer();
            }
            return buff;
        }

        /// <summary>
        /// 将byte[]反序列化为objec实例
        /// </summary>
        /// <param name="buff">待序列化的byte[]</param>
        /// <returns>反序列化后的objec实例</returns>
        public static object Bytes2Object(byte[] buff)
        {
            object obj;
            using (MemoryStream ms = new MemoryStream(buff))
            {
                BinaryFormatter iFormatter = new BinaryFormatter();
                obj = iFormatter.Deserialize(ms);
            }
            return obj;
        }

    }
}
