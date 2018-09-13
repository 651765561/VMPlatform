using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data.OracleClient;
using System.Data;
using LeaRun.Utilities;

namespace LeaRun.Repository
{
    /// <summary>
    /// oracle操作数据库类
    /// </summary>
    public  partial class OracleHelper
    {
        /// <summary>
        /// 连接数据库字符串
        /// </summary>
        private static string strDBConnection = "Data Source=141.72.5.5;User Id=bestunion;Password=bestunion;";
        private static OracleConnection conn = null;

        /// <summary>
        /// 打开数据库
        /// </summary>
        private static void Open()
        {
            if (conn == null)
            {
                conn = new OracleConnection(strDBConnection);
            }

            if (conn.State == System.Data.ConnectionState.Closed)
            {
                conn.Open();
            }
        }

        /// <summary>
        /// 关闭数据库
        /// </summary>
        private static void Close()
        {
            if (conn != null && conn.State == System.Data.ConnectionState.Open)
            {
                conn.Close();
            }
        }

        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="strSql">sql语句</param>
        /// <returns>返回执行结果</returns>
        public static int ExcuteNoQuery(string strSql)
        {
            int nResult = -1;
            Open();

            OracleCommand cmd = new OracleCommand(strSql, conn);
            nResult = cmd.ExecuteNonQuery();

            Close();

            return nResult;
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="strSql">sql语句</param>
        /// <returns>返回DataTable</returns>
        public static DataTable GetTable(string strSql)
        {
            Open();
            OracleDataAdapter adapter = new OracleDataAdapter(strSql, conn);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            Close();
            return dt;
        }

      
    }
}
