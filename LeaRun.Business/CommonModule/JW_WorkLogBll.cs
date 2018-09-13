//=====================================================================================
// All Rights Reserved , Copyright @ Learun 2017
// Software Developers @ Learun 2017
//=====================================================================================

using LeaRun.Entity;
using LeaRun.Repository;
using LeaRun.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System;
using System.Diagnostics;

namespace LeaRun.Business
{
    /// <summary>
    /// JW_WorkLog
    /// <author>
    ///		<name>she</name>
    ///		<date>2017.05.17 10:50</date>
    /// </author>
    /// </summary>
    public class JW_WorkLogBll : RepositoryFactory<JW_WorkLog>
    {

        /// <summary>
        ///  绑定日常工作日志表
        /// </summary>
        /// <returns></returns>
        public string GridPageApplyJsonMy(JqGridParam jqgridparam)
        {
            int pageIndex = jqgridparam.page;
            int pageSize = jqgridparam.rows;
            Stopwatch watch = CommonHelper.TimerStart();
            string sqlTotal =
            string.Format(
                        @" select * from ( 
                                        select 
                                        ROW_NUMBER() over(order by wl.startdate desc) rowNumber
                                        , wl.JW_WorkLog_id,wl.unit_id,wl.dep_id,wl.user_id,u.realname
                                        ,CONVERT(varchar(20), wl.startdate, 120) startdate
                                        ,CONVERT(varchar(20), wl.enddate, 120) enddate
                                        ,wl.content from JW_WorkLog wl
                                        left join Base_user u on u.userid =wl.user_id
                                        where wl.user_id='{0}'
                                        ) as a   "
                            , ManageProvider.Provider.Current().UserId
                        );
            string sql =
                string.Format(
                    @" select * from ( 
                                        {4}
                                        ) as a  
                                        where rowNumber between {0} and {1}  
                                        order by {2} {3} "
                    , (pageIndex - 1) * pageSize + 1
                    , pageIndex * pageSize
                    , jqgridparam.sidx
                    , jqgridparam.sord
                    , sqlTotal
                    );
            DataTable dt = SqlHelper.DataTable(sql, CommandType.Text);//Repository().FindTableBySql(sql);
            try
            {
                var JsonData = new
                {
                    total = Convert.ToInt32(Math.Ceiling(SqlHelper.DataTable(sqlTotal, CommandType.Text).Rows.Count * 1.0 / jqgridparam.rows)), //总页数
                    page = jqgridparam.page, //当前页码
                    records = dt.Rows.Count, //总记录数
                    costtime = CommonHelper.TimerEnd(watch), //查询消耗的毫秒数
                    rows = dt
                };
                return JsonData.ToJson();
            }
            catch (Exception)
            {
                return null;
            }

        }
        /// <summary>
        /// 删除日常工作日志表
        /// </summary>
        /// <param name="apply_id"></param>
        /// <returns></returns>
        public int DeleteJW_WorkLog(string JW_WorkLog_id)
        {
            string sql = string.Format(@" delete JW_WorkLog where JW_WorkLog_Id ='" + JW_WorkLog_id + "'");
            try
            {
                int r = SqlHelper.ExecuteNonQuery(sql, CommandType.Text);
                return r;
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}