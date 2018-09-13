//=====================================================================================
// All Rights Reserved , Copyright @ Learun 2016
// Software Developers @ Learun 2016
//=====================================================================================
using LeaRun.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using LeaRun.Entity.CommonModule;
using LeaRun.Utilities;
using System.Diagnostics;
using LeaRun.Entity;
using System.Data.Common;


namespace LeaRun.Business
{
    /// <summary>
    /// CaseTimeOutWarn
    /// <author>
    ///		<name>she</name>
    ///		<date>2016.12.07 10:51</date>
    /// </author>
    /// </summary>
    public class CaseTimeOutWarnBll : RepositoryFactory<CaseTimeOutWarn>
    {
        /// <summary>
        /// 列表加载
        /// </summary>
        /// <param name="ParameterJson"></param>
        /// <param name="unit_id"></param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public string GridPageApplyJson(string ParameterJson, JqGridParam jqgridparam)
        {
            string unit_id = ManageProvider.Provider.Current().CompanyId;
            try
            {
                int pageIndex = jqgridparam.page;
                int pageSize = jqgridparam.rows;
                Stopwatch watch = CommonHelper.TimerStart();
                string sqlTotal =
                    string.Format(
                        @" select * from ( 
                                        select 
                                            ROW_NUMBER() over(order by Usedetail_id) rowNumber
                                            ,ud.Usedetail_id
                                            ,ud.unit_id
                                            ,ud.PoliceArea_id
                                            ,ud.apply_id
                                            ,ud.adduser_id
                                            ,ud.addDate
                                            ,ud.room_id
                                            ,CONVERT(varchar(20), ud.startdate, 120) startdate
                                            ,CONVERT(varchar(20), ud.enddate, 120) enddate 
                                            ,ud.timeoutstate
                                            ,ud.downloadtime
                                            ,ud.isend
                                            ,u.unit,pa.AreaName,r.RoomName
                                        from JW_Usedetail ud 
                                        left join base_unit u on u.base_unit_id=ud.unit_id 
                                        left join base_policearea pa on pa.PoliceArea_id=ud.PoliceArea_id  
                                        left join jw_apply ja on ja.apply_id=ud.apply_id  
                                        left join base_room r on r.room_id=ud.room_id  
                                            where ud.unit_id='{0}'  and ((isend=1 and DATEDIFF(s,ud.startdate,ud.enddate)>43200) OR (isend=0 and DATEDIFF(s,ud.startdate,GETDATE())>43200))
                                        ) as a  
                                       "
                        , unit_id
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
        /// 列表加载
        /// </summary>
        /// <param name="ParameterJson"></param>
        /// <param name="unit_id"></param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public string GridPageApplyJsonQuery(string ParameterJson, JqGridParam jqgridparam, string unit_id, string PoliceArea_id, string applydatestart, string applydateend, string isend)
        {

            //string unit_id = ManageProvider.Provider.Current().CompanyId;
            // string user_id = ManageProvider.Provider.Current().UserId;
            try
            {
                int pageIndex = jqgridparam.page;
                int pageSize = jqgridparam.rows;
                Stopwatch watch = CommonHelper.TimerStart();
                    string sqlTotal =
                    string.Format(
                        @" 
                         select   ROW_NUMBER() over(order by Usedetail_id) rowNumber ,ud.Usedetail_id,ud.unit_id,ud.PoliceArea_id
                        ,ud.apply_id,ud.adduser_id,ud.addDate ,ud.room_id ,CONVERT(varchar(20), ud.startdate, 120) startdate ,CONVERT(varchar(20), ud.enddate, 120) enddate
                        ,ud.timeoutstate ,ud.downloadtime,ud.isend,u.unit,pa.AreaName,r.RoomName
                        from JW_Usedetail ud  left join base_unit u on u.base_unit_id=ud.unit_id left join base_policearea pa on pa.PoliceArea_id=ud.PoliceArea_id
                        left join jw_apply ja on ja.apply_id=ud.apply_id   left join base_room r on r.room_id=ud.room_id  
                        where 1=1 and ((isend=1 and DATEDIFF(s,ud.startdate,ud.enddate)>43200) OR (isend=0 and DATEDIFF(s,ud.startdate,GETDATE())>43200))
                                       "
                          );
                if (unit_id != "")//单位ID
                {
                    sqlTotal = sqlTotal + " and ud.unit_id='" + unit_id + "'";
                }
                if (PoliceArea_id != "")//警务区
                {
                    sqlTotal = sqlTotal + " and ud.PoliceArea_id = '" + PoliceArea_id + "'";
                }
                if (applydatestart != "")//开始时间开始
                {
                    sqlTotal = sqlTotal + " and  ud.startdate> '" + applydatestart + "'";
                }
                if (applydateend != "")//开始时间结束
                {
                    sqlTotal = sqlTotal + " and  ud.startdate< '" + applydateend + "'";
                }
                if (isend != "")//办案状态
                {
                    sqlTotal = sqlTotal + " and ud.isend = " + isend;
                }

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
    }
}