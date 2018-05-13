using System;
using System.Runtime.InteropServices;

namespace LabManager
{
    /// <summary>
    /// 通信服务类
    /// </summary>
    public static class Communicate
    {
        public struct Tryclass
        {
            public int num;
            public int id;
        }
        /// <summary>
        /// 将struct转为byte[]
        /// </summary>
        /// <param name="structObj">待转换的struct</param>
        /// <returns>转换完成后的byte[]</returns>
        public static byte[] StructToBytes(object structObj)
        {
            int size = Marshal.SizeOf(structObj);
            IntPtr buffer = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.StructureToPtr(structObj, buffer, false);
                byte[] bytes = new byte[size];
                Marshal.Copy(buffer, bytes, 0, size);
                return bytes;
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }
        /// <summary>
        /// 将byte[]还原为struct
        /// </summary>
        /// <param name="bytes">待转换的byte[]</param>
        /// <param name="type">struct的Type</param>
        /// <returns>转换后的struct</returns>
        public static object BytesToStruct(byte[] bytes, Type type)
        {
            int size = Marshal.SizeOf(type);
            IntPtr buffer = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.Copy(bytes, 0, buffer, size);
                return Marshal.PtrToStructure(buffer, type);
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }
    }
}
