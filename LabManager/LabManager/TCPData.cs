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
        Str
    };
    public class TCPData
    {
        protected string _data;
        protected DataType _dataType;
        public TCPData(string data)
        {
            _data = data;
            _dataType = DataType.Unknown;
        }
    }
    public class CMDData : TCPData
    {
        public CMDData(string data) : base(data)
        {
            _dataType = DataType.CMD;
        }
        public string DataStr => _data;
        public byte[] DataByte => Encoding.Default.GetBytes(_data);
    }
    public class DBData : TCPData
    {
        public DBData(string data) : base(data)
        {
        }
    }
    public class StrData : TCPData
    {
        public StrData(string data) : base(data)
        {
        }
    }
}
