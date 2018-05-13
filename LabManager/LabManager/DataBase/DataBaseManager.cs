using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace LabManager.DataBase
{
    /// <inheritdoc />
    /// <summary>
    /// 数据库管理类
    /// </summary>
    public class DataBaseManager : SystemBase
    {
        private string _constr;
        private readonly SqlConnection _dataBaseConn;
        private readonly SqlDataAdapter _adapter;
        private string _tableName;
        private string _commandString;
        private DataSet _dataSet = new DataSet();
        public string ConStr { get => _constr; set => _constr = value; }
        /// <summary>
        /// 获取或设置数据集
        /// </summary>
        public DataSet DataSet { get => _dataSet; set => _dataSet = value; }
        /// <summary>
        /// 获取或设置表
        /// </summary>
        public string TableName { get => _tableName; set => _tableName = value; }
        /// <summary>
        /// 获取或设置命令字符串
        /// </summary>
        public string CommandString { get => _commandString; set => _commandString = value; }
        /// <summary>
        /// 初始化方法
        /// </summary>
        /// <param name="connectString">数据库的连接字符串</param>
        /// <param name="tableName">要连接的表名</param>
        public DataBaseManager(string connectString, string tableName)
        {

            try
            {
                _constr = connectString;
                _tableName = "[" + tableName + "]";
                _dataBaseConn = new SqlConnection(_constr);
                _commandString = @"select * from " + _tableName;
                _adapter = new SqlDataAdapter(_commandString, _dataBaseConn);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                RecordErrLog(ex.ToString());
            }
        }      
        /// <summary>
        /// 设置与数据库关联的数据集
        /// </summary>
        /// <param name="dataSet">要关联的数据集</param>
        public void SetDataSet(DataSet dataSet)
        {
            try
            {
                _dataSet = dataSet.Copy();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                RecordErrLog(ex.ToString());
            }

        }
        /// <summary>
        /// 根据设置填充数据集
        /// </summary>
        public void SetDataSet()
        {
            try
            {
                _dataSet.Clear();
                _dataBaseConn.Open();
                _adapter.Fill(_dataSet, _tableName);
                _dataBaseConn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                RecordErrLog(ex.ToString());
            }
        }
        /// <summary>
        /// 修改数据库里的值
        /// </summary>
        /// <param name="row">行号</param>
        /// <param name="Columns">列号</param>
        /// <param name="value">值</param>
        public void SetValue(int row, int Columns, object value)
        {
            try
            {
                _dataSet.Tables[0].Rows[row][Columns] = value;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                RecordErrLog(ex.ToString());
            }
        }
        /// <summary>
        /// 更新数据库
        /// </summary>
        public void UpdateData()
        {
            try
            {
                var myCommandBuilder = new SqlCommandBuilder(_adapter);
                _adapter.Update(_dataSet, _tableName);
                RecordOperateLog("更新数据库");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                RecordErrLog(ex.ToString());
            }
        }
        /// <summary>
        /// 获取或设置数据库连接字符串
        /// </summary>

    }
}
