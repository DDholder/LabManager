using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabManager
{
    public enum DataType
    {
        Unknown,
        CMD,
        DB,
        Str,
        Default
    };

    public enum CMDList
    {
        Read,
        Write 
    }
    public class TCPData
    {
        protected byte[] _data;
        protected DataType _dataType;
        protected byte[] head;
        public TCPData(byte[] data)
        {
            _data = data;
            _dataType = DataType.Unknown;
            head =new []{ (byte)_dataType};
        }
    }
    public class CMDData : TCPData
    {
        private byte[] _dataByte = new byte[2];
        public CMDData(byte[] data) : base(data)
        {
            _dataType = DataType.CMD;
            head = new[] { (byte)_dataType };
        }
        public byte[] DataByte => head.Concat(_data).ToArray();
    }
    public class DBData : TCPData
    {
        public DBData(byte[] data) : base(data)
        {
        }
    }
    public class StrData : TCPData
    {
        public StrData(byte[] data) : base(data)
        {
        }
    }
}
