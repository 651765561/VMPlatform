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

namespace LeaRun.Business
{
    /// <summary>
    /// XC_Content
    /// <author>
    ///		<name>she</name>
    ///		<date>2018.08.01 14:07</date>
    /// </author>
    /// </summary>
    public class XC_ContentBll : RepositoryFactory<XC_Content>
    {
        /// <summary>
        /// 获取巡查场所
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public DataTable GetXunChaChangSuo(string types)
        {
            string sql = string.Format(@" select place from XC_Content  where type  in(" + types + @") group by place");
            DataTable dt = SqlHelper.DataTable(sql, CommandType.Text);
            return dt;
        }
    }
}