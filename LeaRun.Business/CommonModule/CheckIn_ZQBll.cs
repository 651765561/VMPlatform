//=====================================================================================
// All Rights Reserved , Copyright @ Learun 2016
// Software Developers @ Learun 2016
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
    /// CheckIn_ZQ
    /// <author>
    ///		<name>she</name>
    ///		<date>2016.10.25 13:16</date>
    /// </author>
    /// </summary>
    public class CheckIn_ZQBll : RepositoryFactory<CheckIn_ZQ>
    {
        /// <summary>
        ///  绑定警务区
        /// </summary>
        /// <returns></returns>
        public DataTable GetPoliceAreaListJson()
        {
            string sql = string.Format(@" select PoliceArea_id,AreaName from Base_PoliceArea where unit_id='" + ManageProvider.Provider.Current().CompanyId + "' and  AreaType in (3,4)");
            try
            {
                DataTable dt = SqlHelper.DataTable(sql, CommandType.Text);
                return dt;
            }
            catch (Exception)
            {
                return null;
            }

        }

        /// <summary>
        ///  绑定执勤登记
        /// </summary>
        /// <returns></returns>
        public string GridPageApplyJsonMy(JqGridParam jqgridparam)
        {  
            try
            {
                int pageIndex = jqgridparam.page;
                int pageSize = jqgridparam.rows;
                Stopwatch watch = CommonHelper.TimerStart();
                string sqlTotal =
                  string.Format(
                        @" select * from ( 
                                        select 
                                        ROW_NUMBER() over(order by checkIn_ZQ_Id) rowNumber
                                       ,pa.AreaName areaname,zq.checkIn_ZQ_Id,zq.PoliceArea_id,zq.userName,CONVERT(varchar(20), zq.startTime, 120) startTime,CONVERT(varchar(20), zq.endTime, 120) endTime,zq.matters  from CheckIn_ZQ zq
                                       left join Base_PoliceArea pa on pa.PoliceArea_id =zq.PoliceArea_id
                                       where zq.userId='{0}' 
                                        ) as a  "
                         ,ManageProvider.Provider.Current().UserId
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
        /// 删除执勤登记表
        /// </summary>
        /// <param name="apply_id"></param>
        /// <returns></returns>
        public int DeleteCheckIn_ZQ(string apply_id)
        {
            string sql = string.Format(@" delete CheckIn_ZQ where checkIn_ZQ_Id ='" + apply_id + "'");
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