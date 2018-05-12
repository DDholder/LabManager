using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabManager
{
    public class SystemBase
    {
        protected static readonly string LogPath = AppDomain.CurrentDomain.BaseDirectory + @"\log";
        protected const string OpLogName = @"\OperationLog.log";
        protected const string ErrLogName = @"\ErrLog.log";

        protected void RecordOperateLog(string tips = "无")
        {
            StackTrace stackTrace = new StackTrace(true);
            string time = @"调用时间:" + DateTime.Now +"       ";
            string typeName = @"调用类名:" + stackTrace.GetFrame(1).GetMethod().DeclaringType?.FullName+"       ";
            string funcName = @"调用函数名:" + stackTrace.GetFrame(1).GetMethod().Name + Environment.NewLine;
            tips = tips.Insert(0, "备注:") + Environment.NewLine;
            string value = time + typeName + funcName + tips + Environment.NewLine;
            SaveLog(OpLogName, value);
        }

        protected void RecordErrLog(string message)
        {
            string time = @"调用时间:" + DateTime.Now + Environment.NewLine;
            string value = time + message + Environment.NewLine + Environment.NewLine;
            SaveLog(ErrLogName, value);
        }

        private void SaveLog(string name, string data)
        {
            string fullName = LogPath + name;
            byte[] fileByte = Encoding.Default.GetBytes(data);
            if (!Directory.Exists(LogPath))
                Directory.CreateDirectory(LogPath);
            var fs = File.Exists(fullName) ? new FileStream(fullName, FileMode.Append) : new FileStream(fullName, FileMode.Create);
            fs.Write(fileByte, 0, fileByte.Length);
            fs.Flush();
            fs.Close();
        }
    }
}
