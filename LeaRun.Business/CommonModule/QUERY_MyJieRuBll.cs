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
using System.Diagnostics;
using System.Data;
using System;

namespace LeaRun.Business
{
    /// <summary>
    /// QUERY_MyJieRu
    /// <author>
    ///		<name>she</name>
    ///		<date>2017.02.27 17:53</date>
    /// </author>
    /// </summary>
    public class QUERY_MyJieRuBll : RepositoryFactory<QUERY_MyZQ>
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
                           @" select u.base_unit_id,u.unit,rt.bigtype,rt.Name,count(*) as num,rt.orders from
                                         dbo.Base_MonitorApplication ma 
                                         join dbo.Base_RoomType rt
                                        on ma.object_id=rt.RoomType_id 
                                        join Base_MonitorChannels mc
                                        on mc.MonitorChannels_id=ma.MonitorChannels_id join Base_MonitorServer ms
                                        on ms.MonitorServer_id=mc.MonitorServer_id join base_unit u 
                                        on u.Base_Unit_id=ms.Unit_id
                                        where unit_id='{0}' 
                                        group by u.base_unit_id,u.unit,u.code,rt.bigtype,rt.Name,rt.orders order by u.code,u.unit

                                        "
                        , unit_id
                         );

//                string sql =
//                 string.Format(
//                     @" select * from ( 
//                                        {4}
//                                 where rowNumber between {0} and {1}  
//                                 order by {2} {3} ) as a  
//                                      "
//                     , (pageIndex - 1) * pageSize + 1
//                     , pageIndex * pageSize
//                     , jqgridparam.sidx
//                     , jqgridparam.sord
//                     , sqlTotal
//                     );
                DataTable dt = SqlHelper.DataTable(sqlTotal, CommandType.Text);//Repository().FindTableBySql(sql);

//                string sql2 =
//              string.Format(
//                @" select * from ( 
//                                        {4}
//                                        ) as a  
//                                      "
//                , (pageIndex - 1) * pageSize + 1
//                , pageIndex * pageSize
//                , jqgridparam.sidx
//                , jqgridparam.sord
//                , sqlTotal
//                );
//                DataTable dt2 = SqlHelper.DataTable(sql2, CommandType.Text);//Repository().FindTableBySql(sql);

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
        public string GridPageApplyJsonQuery(string ParameterJson, JqGridParam jqgridparam, string unit_id, string bigtype, string name, string  contianssubordinateunit)
        {

            try
            {
                string user_id = ManageProvider.Provider.Current().UserId;
                int pageIndex = jqgridparam.page;
                int pageSize = jqgridparam.rows;
                Stopwatch watch = CommonHelper.TimerStart();
                 string sqlTotal =
                    string.Format(
                          @" select u.base_unit_id,u.unit,rt.bigtype,rt.Name,count(*) as num,rt.orders from
                                        dbo.Base_MonitorApplication ma 
                                        join dbo.Base_RoomType    rt  on ma.object_id=rt.RoomType_id 
                                        join Base_MonitorChannels mc  on mc.MonitorChannels_id=ma.MonitorChannels_id 
                                        join Base_MonitorServer   ms  on ms.MonitorServer_id=mc.MonitorServer_id 
                                        join base_unit            u   on u.Base_Unit_id=ms.Unit_id
                                        where 1=1
                                     "

                        );

                 if (unit_id != "")//单位ID
                 {
                     if (unit_id != Share.UNIT_ID_JS)//不是江苏省院
                     {
                         if (contianssubordinateunit == "1")
                         {
                             sqlTotal = sqlTotal + " and (u.base_unit_id ='" + unit_id + "' or (u.base_unit_id in (select base_unit_id from base_unit where parent_unit_id='" + unit_id + "' ))) ";
                         }
                         else
                         {
                             sqlTotal = sqlTotal + " and u.base_unit_id ='" + unit_id + "'";
                         }
                     }
                     else
                     {
                         if (contianssubordinateunit == "1")
                         {

                         }
                         else
                         {
                             sqlTotal = sqlTotal + " and u.base_unit_id ='" + unit_id + "'";
                         }
                     }
                 }
                 else
                 {
                     if (ManageProvider.Provider.Current().CompanyId != Share.UNIT_ID_JS)//不是江苏省院
                     {
                         sqlTotal = sqlTotal + " and (u.base_unit_id ='" + ManageProvider.Provider.Current().CompanyId + "' or (u.base_unit_id in (select base_unit_id from base_unit where parent_unit_id='" + ManageProvider.Provider.Current().CompanyId + "' ))) ";
                     }
                     else
                     {
                     }
                 }
                 if (bigtype != "")//大类型
                {
                    sqlTotal = sqlTotal + " and  rt.bigtype= '" + bigtype + "'";
                }
                 if (name != "")//小类型
                {
                    sqlTotal = sqlTotal + " and  rt.name= '" + name + "'";
                }
                 sqlTotal = sqlTotal + " group by u.base_unit_id,u.unit,u.code,rt.bigtype,rt.Name,rt.orders order by u.code,u.unit";

//                 string sql =
//                  string.Format(
//                      @" select * from ( 
//                                        {4}
//                          where rowNumber between {0} and {1}  
//                                        order by {2} {3}                  ) as a  
//                                        "
//                      , (pageIndex - 1) * pageSize + 1
//                      , pageIndex * pageSize
//                      , jqgridparam.sidx
//                      , jqgridparam.sord
//                      , sqlTotal
//                      );

                 DataTable dt = SqlHelper.DataTable(sqlTotal, CommandType.Text);//Repository().FindTableBySql(sql);

//                 string sql2 =
//               string.Format(
//                 @" select * from ( 
//                                        {4}
//                                        ) as a  
//                                      "
//                 , (pageIndex - 1) * pageSize + 1
//                 , pageIndex * pageSize
//                 , jqgridparam.sidx
//                 , jqgridparam.sord
//                 , sqlTotal
//                 );
//                 DataTable dt2 = SqlHelper.DataTable(sql2, CommandType.Text);//Repository().FindTableBySql(sql);

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
        /// 导入大统一数据
        /// </summary>
        /// <returns></returns>
        public DataTable inputData()
        {
            string sql = string.Format(@" select * from tyyw_gg_ajjbxx");
            try
            {
                
                DataTable dt =  OracleHelper.GetTable(sql);
                return dt;
            }
            catch (Exception)
            {
                return null;
            }

        }

    }
}