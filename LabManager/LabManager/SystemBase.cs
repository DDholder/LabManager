using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabManager
{
    /// <summary>
    /// 基础系统基类
    /// </summary>
    public class SystemBase
    {
        /// <summary>
        /// 日志文件保存目录
        /// </summary>
        protected static readonly string LogPath = AppDomain.CurrentDomain.BaseDirectory + @"\log";
        /// <summary>
        /// 操作日志文件名称
        /// </summary>
        protected const string OpLogName = @"\OperationLog.log";
        /// <summary>
        /// 错误日志文件名称
        /// </summary>
        protected const string ErrLogName = @"\ErrLog.log";
        /// <summary>
        /// 添加操作记录
        /// </summary>
        /// <param name="tips">备注</param>
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
        /// <summary>
        /// 添加错误记录
        /// </summary>
        /// <param name="message">错误信息</param>
        protected void RecordErrLog(string message)
        {
            string time = @"调用时间:" + DateTime.Now + Environment.NewLine;
            string value = time + message + Environment.NewLine + Environment.NewLine;
            SaveLog(ErrLogName, value);
        }
        /// <summary>
        /// 向日志文件中添加信息并保存
        /// </summary>
        /// <param name="name">日志文件路径</param>
        /// <param name="data">要添加的信息</param>
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
