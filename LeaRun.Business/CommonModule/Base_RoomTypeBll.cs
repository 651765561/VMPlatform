//=====================================================================================
// All Rights Reserved , Copyright @ Learun 2018
// Software Developers @ Learun 2018
//=====================================================================================

using LeaRun.Entity;
using LeaRun.Repository;
using LeaRun.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;

namespace LeaRun.Business
{
    /// <summary>
    /// Base_RoomType
    /// <author>
    ///		<name>she</name>
    ///		<date>2018.08.30 10:18</date>
    /// </author>
    /// </summary>
    public class Base_RoomTypeBll : RepositoryFactory<Base_RoomType>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DataTable GetBigTypes()
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            DataTable dt = new DataTable();
            strSql.Append(@"select distinct bigtype from Base_RoomType ");
            DataSet ds = Repository().FindDataSetBySql(strSql.ToString());
            if (ds.Tables.Count > 0)
            {
                dt = ds.Tables[0];
            }
            return dt;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="bigtype"></param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public DataTable Search(string type, string name, string bigtype, ref JqGridParam jqgridparam)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append("SELECT *FROM [Base_RoomType]where 1=1");
            if (!string.IsNullOrEmpty(type))
            {
                strSql.Append(" and type=" + type);
            }

            if (!string.IsNullOrEmpty(name))
            {
                strSql.Append(" and name like '%" + name + @"%'");   
            }

            if (!string.IsNullOrEmpty(bigtype))
            {
                strSql.Append(" and bigtype='" + bigtype + "'");
            }

            return Repository().FindTablePageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
        }
    }
}