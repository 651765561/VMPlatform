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
    /// JW_DailyReport
    /// <author>
    ///		<name>she</name>
    ///		<date>2016.11.21 14:06</date>
    /// </author>
    /// </summary>
    public class XC_DailyReportBll : RepositoryFactory<XC_DailyReport>
    {
        public int DeleteJW_DailyReport(string DailyReport_id)
        {
            StringBuilder strSql = new StringBuilder();

            strSql.Append("DELETE  FROM  JW_DailyReport  where DailyReport_id='" + DailyReport_id + "'");
            return Repository().ExecuteBySql(strSql);
        }




        public string GridPageJsonMy(JqGridParam jqgridparam)
        {
            try
            {
                string unit_id = ManageProvider.Provider.Current().CompanyId;
                string user_id = ManageProvider.Provider.Current().UserId;
                int pageIndex = jqgridparam.page;
                int pageSize = jqgridparam.rows;
                Stopwatch watch = CommonHelper.TimerStart();
                string sqlTotal =
                    string.Format(
                        @" select * from ( 
                                            select 
                                                ROW_NUMBER() over(order by adddate desc) rowNumber,
                                                 DailyReport_id,unit_id,adduser_id,convert(varchar(20),adddate,120) adddate
                                                ,reportYear,reportNum,reportAllNum,basicinfo,importantinfo 
                                                ,dailyinfo,submit,deliver,editing,review,issue
                                                ,case JW_DailyReport.needReport when 1 then '上报' else '不上报' end needReport
                                                ,case JW_DailyReport.needPublish when 1 then '发布' else '不发布' end needPublish
                                                ,unit.unit,use1.RealName
                                                 FROM JW_DailyReport LEFT JOIN BASE_Unit unit ON unit.BASE_UNIT_ID=unit_id 
                                                 LEFT JOIN BASE_User use1 ON use1.UserId=adduser_id   
                                                 where adduser_id='{0}'
                                               ) as a  where 1=1
                                           "
                            , user_id
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

                   string sql2 =
               string.Format(
                 @" select * from ( 
                                        {4}
                                        ) as a  
                                      "
                 , (pageIndex - 1) * pageSize + 1
                 , pageIndex * pageSize
                 , jqgridparam.sidx
                 , jqgridparam.sord
                 , sqlTotal
                 );
                   DataTable dt2 = SqlHelper.DataTable(sql2, CommandType.Text);//Repository().FindTableBySql(sql);

                   var JsonData = new
                   {
                       total = Convert.ToInt32(Math.Ceiling(SqlHelper.DataTable(sqlTotal, CommandType.Text).Rows.Count * 1.0 / jqgridparam.rows)), //总页数
                       page = jqgridparam.page, //当前页码
                       records = dt2.Rows.Count, //总记录数
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

        public DataSet Getbasicinfo(string strDate)
        {
            DataSet ds = new DataSet();
            StringBuilder strSql_basicinfo1 = new StringBuilder();
            StringBuilder strSql_basicinfo2 = new StringBuilder();
            StringBuilder strSql_basicinfo3 = new StringBuilder();

            strSql_basicinfo1.Append(" select  count(1)  from  JW_SendPolice where SendDate>='" + strDate + " 00:00:00' and SendDate<='" + strDate + " 23:59:59'");
            strSql_basicinfo2.Append(" select  count(1)  from  JW_Apply where  fact_indate>='" + strDate + " 00:00:00' and fact_indate<='" + strDate + " 23:59:59'");
            strSql_basicinfo3.Append(" select  count(1) from CheckIn_LF where check_time>='" + strDate + " 00:00:00' and check_time<='" + strDate + " 23:59:59'");
           
            try
            {
                DataTable dt_basicinfo1 = SqlHelper.DataTable(strSql_basicinfo1.ToString(), CommandType.Text);
                DataTable dt_basicinfo2 = SqlHelper.DataTable(strSql_basicinfo2.ToString(), CommandType.Text);
                DataTable dt_basicinfo3 = SqlHelper.DataTable(strSql_basicinfo3.ToString(), CommandType.Text);
                ds.Tables.Add(dt_basicinfo1);
                ds.Tables.Add(dt_basicinfo2);
                ds.Tables.Add(dt_basicinfo3);
                return ds;
                
            }
            catch (Exception)
            {
                return null;
            }

        }
        public DataSet Getxcinfo(string strDate)
        {
            DataSet ds = new DataSet();
            StringBuilder strSql_basicinfo1 = new StringBuilder();//全省的市
            StringBuilder strSql_basicinfo2 = new StringBuilder();//全省的单位
            StringBuilder strSql_basicinfo3 = new StringBuilder();//办案区的市
            StringBuilder strSql_basicinfo4 = new StringBuilder();//办案区单位
            StringBuilder strSql_basicinfo5 = new StringBuilder();//办案区案件数
            StringBuilder strSql_basicinfo6 = new StringBuilder();//办案区不规范案件数
            StringBuilder strSql_basicinfo7 = new StringBuilder();//控申市
            StringBuilder strSql_basicinfo8 = new StringBuilder();//控申单位
            StringBuilder strSql_basicinfo9 = new StringBuilder();//控申不规范单位

            strSql_basicinfo1.Append(" select distinct b.unit,sortcode from XC_Main x join base_unit b on x.bxc_unit_id=b.base_unit_id where parent_unit_id in ('e2c79c56-5b58-4c62-b2a9-3bb7492c') and xc_type=1 and xc_datetime>='" + strDate + " 00:00:00' and xc_datetime<='" + strDate + " 23:59:59' union select distinct unit,sortcode from base_unit where base_unit_id in (select b.parent_unit_id from XC_Main x join base_unit b on x.bxc_unit_id=b.base_unit_id and xc_type=1 and xc_datetime>='" + strDate + " 00:00:00' and xc_datetime<='" + strDate + " 23:59:59') order by sortcode");
            strSql_basicinfo2.Append(" select distinct bxc_unit_id  from  XC_Main where xc_type=1 and xc_datetime>='" + strDate + " 00:00:00' and xc_datetime<='" + strDate + " 23:59:59'");
            strSql_basicinfo3.Append(" select distinct b.unit,sortcode from XC_Main x join base_unit b on x.bxc_unit_id=b.base_unit_id where parent_unit_id in ('e2c79c56-5b58-4c62-b2a9-3bb7492c') and xc_place='办案场所' and xc_type=1 and xc_datetime>='" + strDate + " 00:00:00' and xc_datetime<='" + strDate + " 23:59:59' union select distinct unit,sortcode from base_unit where base_unit_id in (select b.parent_unit_id from XC_Main x join base_unit b on x.bxc_unit_id=b.base_unit_id and xc_place='办案场所' and xc_type=1 and xc_datetime>='" + strDate + " 00:00:00' and xc_datetime<='" + strDate + " 23:59:59') order by sortcode");
            strSql_basicinfo4.Append(" select distinct bxc_unit_id  from  XC_Main where xc_place='办案场所' and xc_type=1 and xc_datetime>='" + strDate + " 00:00:00' and xc_datetime<='" + strDate + " 23:59:59'");
            strSql_basicinfo5.Append(" select distinct xc_case_id  from  XC_Main where xc_case_id!='' and  xc_place='办案场所' and xc_type=1 and xc_datetime>='" + strDate + " 00:00:00' and xc_datetime<='" + strDate + " 23:59:59'");
            strSql_basicinfo6.Append(" select distinct xc_case_id  from  XC_Main where xc_res=1 and xc_place='办案场所' and xc_type=1 and xc_datetime>='" + strDate + " 00:00:00' and xc_datetime<='" + strDate + " 23:59:59'");
            /////////
            strSql_basicinfo7.Append(" select distinct b.unit,sortcode from XC_Main x join base_unit b on x.bxc_unit_id=b.base_unit_id where parent_unit_id in ('e2c79c56-5b58-4c62-b2a9-3bb7492c') and xc_place='控申接待' and xc_type=1 and xc_datetime>='" + strDate + " 00:00:00' and xc_datetime<='" + strDate + " 23:59:59' union select distinct unit,sortcode from base_unit where base_unit_id in (select b.parent_unit_id from XC_Main x join base_unit b on x.bxc_unit_id=b.base_unit_id and xc_place='控申接待' and xc_type=1 and xc_datetime>='" + strDate + " 00:00:00' and xc_datetime<='" + strDate + " 23:59:59') order by sortcode");
            strSql_basicinfo8.Append(" select distinct bxc_unit_id  from  XC_Main where xc_place='控申接待' and xc_type=1 and xc_datetime>='" + strDate + " 00:00:00' and xc_datetime<='" + strDate + " 23:59:59'");
            strSql_basicinfo9.Append(" select distinct bxc_unit_id  from  XC_Main where xc_res=1 and xc_place='控申接待' and xc_type=1 and xc_datetime>='" + strDate + " 00:00:00' and xc_datetime<='" + strDate + " 23:59:59'");
            try
            {
                DataTable dt_shi1 = SqlHelper.DataTable(strSql_basicinfo1.ToString(), CommandType.Text);
                DataTable dt_danwei1 = SqlHelper.DataTable(strSql_basicinfo2.ToString(), CommandType.Text);
                DataTable dt_basicinfo3 = SqlHelper.DataTable(strSql_basicinfo3.ToString(), CommandType.Text);
                DataTable dt_basicinfo4 = SqlHelper.DataTable(strSql_basicinfo4.ToString(), CommandType.Text);
                DataTable dt_basicinfo5 = SqlHelper.DataTable(strSql_basicinfo5.ToString(), CommandType.Text);
                DataTable dt_basicinfo6 = SqlHelper.DataTable(strSql_basicinfo6.ToString(), CommandType.Text);
                DataTable dt_basicinfo7 = SqlHelper.DataTable(strSql_basicinfo7.ToString(), CommandType.Text);
                DataTable dt_basicinfo8 = SqlHelper.DataTable(strSql_basicinfo8.ToString(), CommandType.Text);
                DataTable dt_basicinfo9 = SqlHelper.DataTable(strSql_basicinfo9.ToString(), CommandType.Text);

                ds.Tables.Add(dt_shi1);
                ds.Tables.Add(dt_danwei1);
                ds.Tables.Add(dt_basicinfo3);
                ds.Tables.Add(dt_basicinfo4);
                ds.Tables.Add(dt_basicinfo5);
                ds.Tables.Add(dt_basicinfo6);
                ds.Tables.Add(dt_basicinfo7);
                ds.Tables.Add(dt_basicinfo8);
                ds.Tables.Add(dt_basicinfo9);
                return ds;

            }
            catch (Exception)
            {
                return null;
            }

        }
        public DataSet Getoperationinfo(string strDate)
        {
            DataSet ds = new DataSet();
            StringBuilder strSql_basicinfo1 = new StringBuilder();//履职
            StringBuilder strSql_basicinfo2 = new StringBuilder();//办案
            StringBuilder strSql_basicinfo3 = new StringBuilder();//控申
            StringBuilder strSql_basicinfo4 = new StringBuilder();//不规范
            //StringBuilder strSql_basicinfo5 = new StringBuilder();//办案区案件数
            //StringBuilder strSql_basicinfo6 = new StringBuilder();//办案区不规范案件数
            //StringBuilder strSql_basicinfo7 = new StringBuilder();//控申市
            //StringBuilder strSql_basicinfo8 = new StringBuilder();//控申单位
            //StringBuilder strSql_basicinfo9 = new StringBuilder();//控申不规范单位

            strSql_basicinfo1.Append(" select * from JW_SendPolice where actdate>='" + strDate + " 00:00:00' and actdate<='" + strDate + " 23:59:59'");
            strSql_basicinfo2.Append("  select * from JW_Apply where fact_indate>='" + strDate + " 00:00:00' and fact_indate<='" + strDate + " 23:59:59'");
            strSql_basicinfo3.Append("  select * from CheckIn_LF where check_time>='" + strDate + " 00:00:00' and check_time<='" + strDate + " 23:59:59'");
            strSql_basicinfo4.Append("  select * from XC_Main where xc_type=2 and xc_state<>0 and xc_datetime>='" + strDate + " 00:00:00' and xc_datetime<='" + strDate + " 23:59:59'");
            try
            {
                DataTable dt_basicinfo1 = SqlHelper.DataTable(strSql_basicinfo1.ToString(), CommandType.Text);
                DataTable dt_basicinfo2 = SqlHelper.DataTable(strSql_basicinfo2.ToString(), CommandType.Text);
                DataTable dt_basicinfo3 = SqlHelper.DataTable(strSql_basicinfo3.ToString(), CommandType.Text);
                DataTable dt_basicinfo4 = SqlHelper.DataTable(strSql_basicinfo4.ToString(), CommandType.Text);
                //DataTable dt_basicinfo5 = SqlHelper.DataTable(strSql_basicinfo5.ToString(), CommandType.Text);
                //DataTable dt_basicinfo6 = SqlHelper.DataTable(strSql_basicinfo6.ToString(), CommandType.Text);
                //DataTable dt_basicinfo7 = SqlHelper.DataTable(strSql_basicinfo7.ToString(), CommandType.Text);
                //DataTable dt_basicinfo8 = SqlHelper.DataTable(strSql_basicinfo8.ToString(), CommandType.Text);
                //DataTable dt_basicinfo9 = SqlHelper.DataTable(strSql_basicinfo9.ToString(), CommandType.Text);

                ds.Tables.Add(dt_basicinfo1);
                ds.Tables.Add(dt_basicinfo2);
                ds.Tables.Add(dt_basicinfo3);
                ds.Tables.Add(dt_basicinfo4);
                //ds.Tables.Add(dt_basicinfo5);
                //ds.Tables.Add(dt_basicinfo6);
                //ds.Tables.Add(dt_basicinfo7);
                //ds.Tables.Add(dt_basicinfo8);
                //ds.Tables.Add(dt_basicinfo9);
                return ds;

            }
            catch (Exception)
            {
                return null;
            }

        }
        public DataSet Getvideoinfo(string strDate)
        {
            DataSet ds = new DataSet();
            //StringBuilder strSql_basicinfo1 = new StringBuilder();//全省的市
            //StringBuilder strSql_basicinfo2 = new StringBuilder();//全省的单位
            StringBuilder strSql_basicinfo3 = new StringBuilder();//办案区的市
            StringBuilder strSql_basicinfo4 = new StringBuilder();//办案区单位
            StringBuilder strSql_basicinfo5 = new StringBuilder();//办案区巡查次数
            StringBuilder strSql_basicinfo6 = new StringBuilder();//办案区不通次数
            StringBuilder strSql_basicinfo7 = new StringBuilder();//控申市
            StringBuilder strSql_basicinfo8 = new StringBuilder();//控申单位
            StringBuilder strSql_basicinfo9 = new StringBuilder();//控申巡查次数
            StringBuilder strSql_basicinfo10 = new StringBuilder();//控申不通次数

            //strSql_basicinfo1.Append(" select distinct b.unit,sortcode from XC_Main x join base_unit b on x.bxc_unit_id=b.base_unit_id where parent_unit_id in ('e2c79c56-5b58-4c62-b2a9-3bb7492c') and xc_type=1 and xc_datetime>='" + strDate + " 00:00:00' and xc_datetime<='" + strDate + " 23:59:59' union select distinct unit,sortcode from base_unit where base_unit_id in (select b.parent_unit_id from XC_Main x join base_unit b on x.bxc_unit_id=b.base_unit_id and xc_type=1 and xc_datetime>='" + strDate + " 00:00:00' and xc_datetime<='" + strDate + " 23:59:59') order by sortcode");
            //strSql_basicinfo2.Append(" select distinct bxc_unit_id  from  XC_Main where xc_type=1 and xc_datetime>='" + strDate + " 00:00:00' and xc_datetime<='" + strDate + " 23:59:59'");
            strSql_basicinfo3.Append(" select distinct b.unit,sortcode from XC_Main x join base_unit b on x.bxc_unit_id=b.base_unit_id where parent_unit_id in ('e2c79c56-5b58-4c62-b2a9-3bb7492c') and xc_place='办案场所' and xc_type=1 and xc_datetime>='" + strDate + " 00:00:00' and xc_datetime<='" + strDate + " 23:59:59' union select distinct unit,sortcode from base_unit where base_unit_id in (select b.parent_unit_id from XC_Main x join base_unit b on x.bxc_unit_id=b.base_unit_id and xc_place='办案场所' and xc_type=1 and xc_datetime>='" + strDate + " 00:00:00' and xc_datetime<='" + strDate + " 23:59:59') order by sortcode");
            strSql_basicinfo4.Append(" select distinct bxc_unit_id  from  XC_Main where xc_place='办案场所' and xc_type=1 and xc_datetime>='" + strDate + " 00:00:00' and xc_datetime<='" + strDate + " 23:59:59'");
            strSql_basicinfo5.Append(" select distinct xc_room_id  from  XC_Main where xc_place='办案场所' and xc_type=1 and xc_datetime>='" + strDate + " 00:00:00' and xc_datetime<='" + strDate + " 23:59:59'");
            strSql_basicinfo6.Append(" select distinct xc_room_id  from  XC_Main x join XC_Detail d on x.xc_id=d.xc_id where d.name='监控不通' and xc_res=1 and xc_place='办案场所' and xc_type=1 and xc_datetime>='" + strDate + " 00:00:00' and xc_datetime<='" + strDate + " 23:59:59'");
            /////////
            strSql_basicinfo7.Append(" select distinct b.unit,sortcode from XC_Main x join base_unit b on x.bxc_unit_id=b.base_unit_id where parent_unit_id in ('e2c79c56-5b58-4c62-b2a9-3bb7492c') and xc_place='控申接待' and xc_type=1 and xc_datetime>='" + strDate + " 00:00:00' and xc_datetime<='" + strDate + " 23:59:59' union select distinct unit,sortcode from base_unit where base_unit_id in (select b.parent_unit_id from XC_Main x join base_unit b on x.bxc_unit_id=b.base_unit_id and xc_place='控申接待' and xc_type=1 and xc_datetime>='" + strDate + " 00:00:00' and xc_datetime<='" + strDate + " 23:59:59') order by sortcode");
            strSql_basicinfo8.Append(" select distinct bxc_unit_id  from  XC_Main where xc_place='控申接待' and xc_type=1 and xc_datetime>='" + strDate + " 00:00:00' and xc_datetime<='" + strDate + " 23:59:59'");
            strSql_basicinfo9.Append(" select distinct xc_room_id  from  XC_Main where xc_place='控申接待' and xc_type=1 and xc_datetime>='" + strDate + " 00:00:00' and xc_datetime<='" + strDate + " 23:59:59'");
            strSql_basicinfo10.Append(" select distinct xc_room_id  from  XC_Main x join XC_Detail d on x.xc_id=d.xc_id where d.name='监控不通' and xc_res=1 and xc_place='控申接待' and xc_type=1 and xc_datetime>='" + strDate + " 00:00:00' and xc_datetime<='" + strDate + " 23:59:59'");
            try
            {
                //DataTable dt_shi1 = SqlHelper.DataTable(strSql_basicinfo1.ToString(), CommandType.Text);
                //DataTable dt_danwei1 = SqlHelper.DataTable(strSql_basicinfo2.ToString(), CommandType.Text);
                DataTable dt_basicinfo3 = SqlHelper.DataTable(strSql_basicinfo3.ToString(), CommandType.Text);
                DataTable dt_basicinfo4 = SqlHelper.DataTable(strSql_basicinfo4.ToString(), CommandType.Text);
                DataTable dt_basicinfo5 = SqlHelper.DataTable(strSql_basicinfo5.ToString(), CommandType.Text);
                DataTable dt_basicinfo6 = SqlHelper.DataTable(strSql_basicinfo6.ToString(), CommandType.Text);
                DataTable dt_basicinfo7 = SqlHelper.DataTable(strSql_basicinfo7.ToString(), CommandType.Text);
                DataTable dt_basicinfo8 = SqlHelper.DataTable(strSql_basicinfo8.ToString(), CommandType.Text);
                DataTable dt_basicinfo9 = SqlHelper.DataTable(strSql_basicinfo9.ToString(), CommandType.Text);
                DataTable dt_basicinfo10 = SqlHelper.DataTable(strSql_basicinfo10.ToString(), CommandType.Text);

                //ds.Tables.Add(dt_shi1);
                //ds.Tables.Add(dt_danwei1);
                ds.Tables.Add(dt_basicinfo3);
                ds.Tables.Add(dt_basicinfo4);
                ds.Tables.Add(dt_basicinfo5);
                ds.Tables.Add(dt_basicinfo6);
                ds.Tables.Add(dt_basicinfo7);
                ds.Tables.Add(dt_basicinfo8);
                ds.Tables.Add(dt_basicinfo9);
                ds.Tables.Add(dt_basicinfo10);
                return ds;

            }
            catch (Exception)
            {
                return null;
            }

        }
    }
}