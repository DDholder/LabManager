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

        public void SetDataSet()
        {
            _dataSet.Clear();
            _dataBaseConn.Open();
            _adapter.Fill(_dataSet, _tableName);
            _dataBaseConn.Close();
        }

        public void SetValue(int row, int Columns, object value)
        {
            _dataSet.Tables[0].Rows[row][Columns] = value;
        }
        public void UpdateData()
        {
            var myCommandBuilder = new SqlCommandBuilder(_adapter);
            _adapter.Update(_dataSet, _tableName);
        }
        /// <summary>
        /// 获取或设置数据库连接字符串
        /// </summary>
        public string ConStr { get => _constr; set => _constr = value; }
        public DataSet DataSet { get => _dataSet; set => _dataSet = value; }
        public string TableName { get => _tableName; set => _tableName = value; }
        public string CommandString { get => _commandString; set => _commandString = value; }
    }
}
