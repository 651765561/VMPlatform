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
    /// QUERY_JW_DailyReport
    /// <author>
    ///		<name>she</name>
    ///		<date>2016.12.09 10:07</date>
    /// </author>
    /// </summary>
    public class QUERY_JW_DailyReportBll : RepositoryFactory<QUERY_JW_DailyReport>
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
            string user_id = ManageProvider.Provider.Current().UserId;
            try
            {
                int pageIndex = jqgridparam.page;
                int pageSize = jqgridparam.rows;
                Stopwatch watch = CommonHelper.TimerStart();
                 string sqlTotal =
                    string.Format(
                        @" 
                                        select 
                                            ROW_NUMBER() over(order by adddate desc) rowNumber,
                                            DailyReport_id,unit_id,adduser_id,convert(varchar(10),adddate,120) adddate
                                           ,reportYear,reportNum,reportAllNum,basicinfo,importantinfo
                                           ,dailyinfo,submit,deliver,editing,review,issue
                                           ,case JW_DailyReport.needReport when '1' then '上报' else '不上报' end needReport
                                           ,case JW_DailyReport.needPublish when '1' then '发布' else '不发布' end needPublish
                                           ,unit.unit,use1.RealName
                                           FROM JW_DailyReport LEFT JOIN BASE_Unit unit ON unit.BASE_UNIT_ID=unit_id 
                                           LEFT JOIN BASE_User use1 ON use1.UserId=adduser_id
                                         "
                        );

                                           
                //if (unit_id != Share.UNIT_ID_JS)
                //{
                //    sqlTotal = sqlTotal + " WHERE (JW_DailyReport.adduser_id='" + unit_id + "' or ( JW_DailyReport.needPublish='1'  AND JW_DailyReport.unit_id in (select base_unit_id from base_unit where base_unit_ID='" + unit_id + "' or parent_unit_id ='" + unit_id + "') ) )";
                //}
                //else
                //{
                //    sqlTotal = sqlTotal + " WHERE (JW_DailyReport.adduser_id='" + unit_id + "' or (  JW_DailyReport.needPublish='1' ) )";
                
                //}

                 //在原有的权限基础上增加所有市院区院都可以看到省院发布的日报
                 if (unit_id != Share.UNIT_ID_JS)
                 {
                     sqlTotal = sqlTotal + " WHERE (JW_DailyReport.adduser_id='" + unit_id + "' or ( JW_DailyReport.needPublish='1'  AND ( JW_DailyReport.unit_id in (select base_unit_id from base_unit where base_unit_ID='" + unit_id + "' or parent_unit_id ='" + unit_id + "') or JW_DailyReport.unit_id='"+Share.UNIT_ID_JS+"')) )";
                 }
                 else
                 {
                     sqlTotal = sqlTotal + " WHERE (JW_DailyReport.adduser_id='" + unit_id + "' or (  JW_DailyReport.needPublish='1' ) )";

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





        /// <summary>
        /// 列表加载
        /// </summary>
        /// <param name="ParameterJson"></param>
        /// <param name="unit_id"></param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public string GridPageApplyJsonQuery(string ParameterJson, JqGridParam jqgridparam, string unit_id, string applydatestart, string applydateend, string basicinfo, string importantinfo, string dailyinfo, string contianssubordinateunit)
        {
         
            try
            {
                string user_id = ManageProvider.Provider.Current().UserId;
                int pageIndex = jqgridparam.page;
                int pageSize = jqgridparam.rows;
                Stopwatch watch = CommonHelper.TimerStart();
                  string sqlTotal =
                    string.Format(
                        @" select 
                                            ROW_NUMBER() over(order by adddate desc) rowNumber,
                                            DailyReport_id,unit_id,adduser_id,convert(varchar(10),adddate,120) adddate
                                            ,reportYear,reportNum,reportAllNum,basicinfo,importantinfo
                                            ,dailyinfo,submit,deliver,editing,review,issue
                                            ,case JW_DailyReport.needReport when '1' then '上报' else '不上报' end needReport
                                            ,case JW_DailyReport.needPublish when '1' then '发布' else '不发布' end needPublish
                                            ,unit.unit,use1.RealName
                                            FROM JW_DailyReport LEFT JOIN BASE_Unit unit ON unit.BASE_UNIT_ID=unit_id 
                                            LEFT JOIN BASE_User use1 ON use1.UserId=adduser_id
                                            WHERE 1=1
                                          "
                        );

                 
                //if (unit_id != "")//单位ID
                //{
                //    sqlTotal = sqlTotal + " and JW_DailyReport.unit_id ='" + unit_id + "'";
                //}
                  if (unit_id != "")//单位ID
                  {
                      if (unit_id != Share.UNIT_ID_JS)//不是江苏省院
                      {
                          if (contianssubordinateunit == "1")
                          {
                              sqlTotal = sqlTotal + " and ( JW_DailyReport.needPublish='1' and JW_DailyReport.unit_id ='" + unit_id + "' or (JW_DailyReport.unit_id in (select base_unit_id from base_unit where parent_unit_id='" + unit_id + "' ) or JW_DailyReport.unit_id='" + Share.UNIT_ID_JS + "')) ";
                          }
                          else
                          {
                              sqlTotal = sqlTotal + " and JW_DailyReport.needPublish='1' and ( JW_DailyReport.unit_id ='" + unit_id + "' or JW_DailyReport.unit_id='" + Share.UNIT_ID_JS + "')";
                          }
                      }
                      else
                      {
                          if (contianssubordinateunit == "1")
                          {
                              sqlTotal = sqlTotal + " and  JW_DailyReport.needPublish='1' ";
                          }
                          else
                          {
                              sqlTotal = sqlTotal + " and  JW_DailyReport.needPublish='1' and (JW_DailyReport.unit_id ='" + unit_id + "' or JW_DailyReport.unit_id='" + Share.UNIT_ID_JS + "')";
                          }
                      }
                  }
                  else
                  {
                      if (ManageProvider.Provider.Current().CompanyId != Share.UNIT_ID_JS)//不是江苏省院
                      {
                          sqlTotal = sqlTotal + " and ( JW_DailyReport.needPublish='1' and JW_DailyReport.unit_id in (select base_unit_id from base_unit where base_unit_ID='" + ManageProvider.Provider.Current().CompanyId + "' or parent_unit_id ='" + ManageProvider.Provider.Current().CompanyId + "') or JW_DailyReport.unit_id='" + Share.UNIT_ID_JS + "')) )";
                                                            
                      }
                      else
                      {
                          sqlTotal = sqlTotal + " and  JW_DailyReport.needPublish='1' or JW_DailyReport.unit_id='" + Share.UNIT_ID_JS + "' ";
                      } 
                  }

                if (applydatestart != "")//发布开始日期
                {
                    sqlTotal = sqlTotal + " and  JW_DailyReport.adddate> '" + applydatestart + "'";
                }
                if (applydateend != "")//发布结束日期
                {
                    sqlTotal = sqlTotal + " and  JW_DailyReport.adddate< '" + applydateend + "'";
                }
                if (basicinfo != "")//
                {
                    sqlTotal = sqlTotal + " and JW_DailyReport.basicinfo like '%" + basicinfo.Trim() + "%'";
                }
                if (importantinfo != "")//
                {
                    sqlTotal = sqlTotal + " and JW_DailyReport.importantinfo like '%" + importantinfo + "%'";
                }
                if (dailyinfo != "")//身份证号
                {
                    sqlTotal = sqlTotal + " and JW_DailyReport.dailyinfo like '%" + dailyinfo + "%'";
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


        /// <summary>
        /// 列表加载
        /// </summary>
        /// <param name="ParameterJson"></param>
        /// <param name="unit_id"></param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public DataTable GridPageApplyJsonExcel(string unit_id, string applydatestart, string applydateend, string basicinfo, string importantinfo, string dailyinfo, string contianssubordinateunit)
        {

            //string unit_id = ManageProvider.Provider.Current().CompanyId;
            // string user_id = ManageProvider.Provider.Current().UserId;
            try
            {

                string sqlTotal =
                    string.Format(
                        @" 
                                       select 
                                         
                                             unit.unit as '发布单位'
                                            ,use1.RealName as '报告人员'
                                            ,convert(varchar(10),adddate,120) as '发布日期'
                                            ,reportYear as '报告年份'
                                            ,reportNum as '报告期数'
                                            ,reportAllNum as '报告总期数'
                                            ,replace(basicinfo,'&nbsp;','') as '警务基本信息'
                                            ,replace(importantinfo,'&nbsp;','') as '重要警务信息'
                                            ,replace(dailyinfo,'&nbsp;','') as '日常工作信息'
                                            ,submit as '报'
                                            ,deliver as '送'
                                            ,editing as '编校'
                                            ,review as '审核'
                                            ,issue as '签发'
                                            ,case JW_DailyReport.needReport when '1' then '上报' else '不上报' end as '是否上报'
                                            ,case JW_DailyReport.needPublish when '1' then '发布' else '不发布' end as '是否发布' 
                                            FROM JW_DailyReport LEFT JOIN BASE_Unit unit ON unit.BASE_UNIT_ID=unit_id 
                                            LEFT JOIN BASE_User use1 ON use1.UserId=adduser_id
                                            WHERE 1=1
                                          "
                        );


                //if (unit_id != "")//单位ID
                //{
                //    sqlTotal = sqlTotal + " and JW_DailyReport.unit_id ='" + unit_id + "'";
                //}
                if (unit_id != "")//单位ID
                {
                    if (unit_id != Share.UNIT_ID_JS)//不是江苏省院
                    {
                        if (contianssubordinateunit == "1")
                        {
                            sqlTotal = sqlTotal + " and ( JW_DailyReport.needPublish='1' and JW_DailyReport.unit_id ='" + unit_id + "' or (JW_DailyReport.unit_id in (select base_unit_id from base_unit where parent_unit_id='" + unit_id + "' ) or JW_DailyReport.unit_id='" + Share.UNIT_ID_JS + "')) ";
                        }
                        else
                        {
                            sqlTotal = sqlTotal + " and JW_DailyReport.needPublish='1' and ( JW_DailyReport.unit_id ='" + unit_id + "' or JW_DailyReport.unit_id='" + Share.UNIT_ID_JS + "')";
                        }
                    }
                    else
                    {
                        if (contianssubordinateunit == "1")
                        {
                            sqlTotal = sqlTotal + " and  JW_DailyReport.needPublish='1' ";
                        }
                        else
                        {
                            sqlTotal = sqlTotal + " and  JW_DailyReport.needPublish='1' and (JW_DailyReport.unit_id ='" + unit_id + "' or JW_DailyReport.unit_id='" + Share.UNIT_ID_JS + "')";
                        }
                    }
                }
                else
                {
                    if (ManageProvider.Provider.Current().CompanyId != Share.UNIT_ID_JS)//不是江苏省院
                    {
                        sqlTotal = sqlTotal + " and ( JW_DailyReport.needPublish='1' and JW_DailyReport.unit_id in (select base_unit_id from base_unit where base_unit_ID='" + ManageProvider.Provider.Current().CompanyId + "' or parent_unit_id ='" + ManageProvider.Provider.Current().CompanyId + "') or JW_DailyReport.unit_id='" + Share.UNIT_ID_JS + "')) )";

                    }
                    else
                    {
                        sqlTotal = sqlTotal + " and  JW_DailyReport.needPublish='1' or JW_DailyReport.unit_id='" + Share.UNIT_ID_JS + "' ";
                    }
                }

                if (applydatestart != "")//发布开始日期
                {
                    sqlTotal = sqlTotal + " and  JW_DailyReport.adddate> '" + applydatestart + "'";
                }
                if (applydateend != "")//发布结束日期
                {
                    sqlTotal = sqlTotal + " and  JW_DailyReport.adddate< '" + applydateend + "'";
                }
                if (basicinfo != "")//
                {
                    sqlTotal = sqlTotal + " and JW_DailyReport.basicinfo like '%" + basicinfo.Trim() + "%'";
                }
                if (importantinfo != "")//
                {
                    sqlTotal = sqlTotal + " and JW_DailyReport.importantinfo like '%" + importantinfo + "%'";
                }
                if (dailyinfo != "")//身份证号
                {
                    sqlTotal = sqlTotal + " and JW_DailyReport.dailyinfo like '%" + dailyinfo + "%'";
                }
                DataTable dt = SqlHelper.DataTable(sqlTotal, CommandType.Text);//Repository().FindTableBySql(sql);


                return dt;
            }
            catch (Exception)
            {
                return null;
            }



        }
    }
}