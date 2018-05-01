using System.Data;
using System.Data.SqlClient;

namespace LabManager.DataBase
{
    public class DataBaseManager
    {
        private string _constr;
        private readonly SqlConnection _dataBaseConn;
        private readonly SqlDataAdapter _adapter;
        private string _tableName;
        private string _commandString;
        public DataBaseManager(string connectString, string tableName)
        {
            _constr = connectString;
            _tableName = "[" + tableName + "]";
            _dataBaseConn = new SqlConnection(_constr);
            _commandString = @"select * from " + _tableName;
            _adapter = new SqlDataAdapter(_commandString, _dataBaseConn);
        }

        private DataSet _dataSet = new DataSet();

        public void SetDataSet(DataSet dataSet)
        {
            _dataSet = dataSet.Copy();
        }
        /// <summary>
        /// 根据设置数据集
        /// </summary>
        public void SetDataSet()
        {
            _dataSet.Clear();
            _dataBaseConn.Open();
            _adapter.Fill(_dataSet, _tableName);
            _dataBaseConn.Close();
        }
        /// <summary>
        /// 修改数据库里的值
        /// </summary>
        /// <param name="row">行号</param>
        /// <param name="Columns">列号</param>
        /// <param name="value">值</param>
        public void SetValue(int row, int Columns, object value)
        {
            _dataSet.Tables[0].Rows[row][Columns] = value;
        }
        /// <summary>
        /// 更新数据库
        /// </summary>
        public void UpdateData()
        {
            var myCommandBuilder = new SqlCommandBuilder(_adapter);
            _adapter.Update(_dataSet, _tableName);
        }
        /// <summary>
        /// 获取或设置数据库连接字符串
        /// </summary>
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
    }
}
