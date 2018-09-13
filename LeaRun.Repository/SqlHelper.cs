using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace LeaRun.Repository
{
    public partial class SqlHelper
    {
        //获取App.config中的连接字符串
        private static readonly string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["LeaRunFramework_SqlServer"].ConnectionString;


        /// <summary>
        /// ExecuteNonQuery：执行非查询操作
        /// </summary>
        /// <param name="sqlStr">sql语句</param>
        /// <param name="cmdType">命令类型(文本，存储过程)</param>
        /// <param name="param">参数列表</param>
        /// <returns>返回执行语句的受影响行数，失败则返回-1</returns>
        public static int ExecuteNonQuery(string sqlStr, CommandType cmdType, params SqlParameter[] param)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand(sqlStr, conn))
                {
                    cmd.CommandType = cmdType;
                    if (param != null)
                    {
                        cmd.Parameters.AddRange(param);
                    }
                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }


        /// <summary>
        /// ExecuteScalar：返回首行首列数据
        /// </summary>
        /// <param name="sqlStr">sql语句</param>
        /// <param name="cmdType">命令类型(文本，存储过程)</param>
        /// <param name="param">参数列表</param>
        /// <returns>返回首行首列数据</returns>
        public static object ExecuteScalar(string sqlStr, CommandType cmdType, params SqlParameter[] param)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand(sqlStr, conn))
                {
                    cmd.CommandType = cmdType;
                    if (param != null)
                    {
                        cmd.Parameters.AddRange(param);
                    }
                    conn.Open();
                    return cmd.ExecuteScalar();
                }
            }
        }


        /// <summary>
        /// DataTable：返回一个DataTable表
        /// </summary>
        /// <param name="sqlStr">sql语句</param>
        /// <param name="cmdType">命令类型(文本，存储过程)</param>
        /// <param name="param">参数列表</param>
        /// <returns>返回一个DataTable表</returns>
        public static DataTable DataTable(string sqlStr, CommandType cmdType, params SqlParameter[] param)
        {
            DataTable dataTable = new DataTable();
            using (SqlDataAdapter adapter = new SqlDataAdapter(sqlStr, connStr))
            {
                adapter.SelectCommand.CommandType = cmdType;
                if (param != null)
                {
                    adapter.SelectCommand.Parameters.AddRange(param);
                }
                adapter.Fill(dataTable);
                return dataTable;
            }
        }


        /// <summary>
        /// DataSet：返回一个DataTable的集合
        /// </summary>
        /// <param name="sqlStr">sql语句</param>
        /// <param name="cmdType">命令类型(文本，存储过程)</param>
        /// <param name="param">参数列表</param>
        /// <returns>返回一个DataTable的集合</returns>
        public static DataSet DataSet(string sqlStr, CommandType cmdType, params SqlParameter[] param)
        {
            DataSet dataSet = new DataSet();
            using (SqlDataAdapter adapter = new SqlDataAdapter(sqlStr, connStr))
            {
                adapter.SelectCommand.CommandType = cmdType;
                if (param != null)
                {
                    adapter.SelectCommand.Parameters.AddRange(param);
                }
                adapter.Fill(dataSet);
                return dataSet;
            }
        }







        /// <summary>
        /// ExecuteReader：以只进、只读的方式读取结果集中的数据
        /// </summary>
        /// <param name="sqlStr">sql语句</param>
        /// <param name="cmdType">命令类型(文本，存储过程)</param>
        /// <param name="param">参数列表</param>
        /// <returns>返回一个SqlDataReader</returns>
        public static SqlDataReader ExecuteReader(string sqlStr, CommandType cmdType, params SqlParameter[] param)
        {
            SqlConnection conn = new SqlConnection(connStr);
            using (SqlCommand cmd = new SqlCommand(sqlStr, conn))
            {
                cmd.CommandType = cmdType;
                if (param != null)
                {
                    cmd.Parameters.AddRange(param);
                }
                try
                {
                    conn.Open();
                    return cmd.ExecuteReader(CommandBehavior.CloseConnection);
                }
                catch (Exception)
                {
                    conn.Close();
                    conn.Dispose();
                    //throw;
                    return null;
                }
            }
        }

        //调用
        //string sqlStr = "select * from TblEmployee";
        //SqlDataReader reader = ExecuteReader(sqlStr, CommandType.Text);
        //while (reader.Read())
        //{
        //    //MessageBox.Show(reader[1].ToString());
        //    //MessageBox.Show(reader["EmployeeName"].ToString());
        //    MessageBox.Show(reader.GetString(1));//这里的索引1，是根据编写的sql语句的查询结果集确定的，而不是根据数据库表的结构确定的。
        //} 
    }
}
