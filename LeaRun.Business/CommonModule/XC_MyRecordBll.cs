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
    /// JW_Apply
    /// <author>
    ///		<name>she</name>
    ///		<date>2016.11.17 11:59</date>
    /// </author>
    /// </summary>
    public class XC_MyRecordBll : RepositoryFactory<QUERY_JW_Apply>
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
                        @"  select * from ( 
                                           select 
                                            ROW_NUMBER() over(order by xc.xc_datetime desc) rowNumber, xc.*
                                            ,unit.unit as unitName ,bxc_unit.unit as bxc_unitName,user1.realname from XC_Main xc
                                            left join base_unit unit on xc.xc_unit_id=unit.Base_Unit_id
                                            left join base_unit bxc_unit on xc.bxc_unit_id=bxc_unit.Base_Unit_id
                                            left join base_user user1 on xc.xc_user_id=user1.UserId
                                            where xc.xc_res in (1) and  xc.xc_state in (1,2) and xc.bxc_unit_id='{0}' 
                                        ) as a   "
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
        public string GridPageApplyJsonQuery(string ParameterJson, JqGridParam jqgridparam, string unit_id, string bxc_unit_id, string xc_datetimestart, string xc_datetimeend, string xc_place, string xc_detail, string xc_state, string contianssubordinateunit)
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
                                        select 
                                            ROW_NUMBER() over(order by xc.xc_datetime desc) rowNumber, xc.*
                                            ,unit.unit as unitName ,bxc_unit.unit as bxc_unitName,user1.realname from XC_Main xc
                                            left join base_unit unit on xc.xc_unit_id=unit.Base_Unit_id
                                            left join base_unit bxc_unit on xc.bxc_unit_id=bxc_unit.Base_Unit_id
                                            left join base_user user1 on xc.xc_user_id=user1.UserId
                                            where xc.xc_res in (1) and xc.xc_state in (1,2,3) and xc.bxc_unit_id='{0}'
                                       "
                        , ManageProvider.Provider.Current().CompanyId );
           
                ////if (unit_id != "")//巡查单位ID
                ////{
                ////    if (unit_id != Share.UNIT_ID_JS)//不是江苏省院
                ////    {
                ////        if (contianssubordinateunit == "1")
                ////        {
                ////            sqlTotal = sqlTotal + " and (xc.xc_unit_id ='" + unit_id + "' or (xc.xc_unit_id in (select base_unit_id from base_unit where parent_unit_id='" + unit_id + "' ))) ";
                ////        }
                ////        else
                ////        {
                ////            sqlTotal = sqlTotal + " and xc.xc_unit_id ='" + unit_id + "'";
                ////        }
                ////    }
                ////    else
                ////    {
                ////        if (contianssubordinateunit == "1")
                ////        {

                ////        }
                ////        else
                ////        {
                ////            sqlTotal = sqlTotal + " and xc.xc_unit_id ='" + unit_id + "'";
                ////        }
                ////    }
                ////}
                ////else
                ////{
                ////    if (ManageProvider.Provider.Current().CompanyId != Share.UNIT_ID_JS)//不是江苏省院
                ////    {
                ////        sqlTotal = sqlTotal + " and (xc.xc_unit_id ='" + ManageProvider.Provider.Current().CompanyId + "' or (xc.xc_unit_id in (select base_unit_id from base_unit where parent_unit_id='" + ManageProvider.Provider.Current().CompanyId + "' ))) ";
                ////    }
                ////    else
                ////    {
                        
                ////    } 
                ////}

                if (bxc_unit_id != "")//被巡查单位ID
                {
                    sqlTotal = sqlTotal + " and xc.bxc_unit_id ='" + bxc_unit_id + "'";
                }
                if (xc_datetimestart != "")//巡查时间开始
                {
                    sqlTotal = sqlTotal + " and  xc.xc_datetime>= '" + xc_datetimestart + "'";
                }
                if (xc_datetimeend != "")//巡查时间结束
                {
                    sqlTotal = sqlTotal + " and  xc.xc_datetime<= '" + xc_datetimeend + "'";
                }
                if (xc_place != "")//巡查场所
                {
                    sqlTotal = sqlTotal + " and xc.xc_place = '"+ xc_place+ "'";
                }
                if (xc_detail != "")//巡查明细
                {
                    sqlTotal = sqlTotal + " and xc.xc_detail like '%" + xc_detail + "%'";
                }
                if (xc_state != "")//状态
                {
                    sqlTotal = sqlTotal + " and xc.xc_state = '" + xc_state + "'";
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
        public DataTable GridPageApplyJsonExcel(string unit_id, string bxc_unit_id, string xc_datetimestart, string xc_datetimeend, string xc_place, string xc_detail, string xc_state, string contianssubordinateunit)
        {

            // string unit_id1 = ManageProvider.Provider.Current().CompanyId;
            // string user_id = ManageProvider.Provider.Current().UserId;
            try
            {

                string sqlTotal =
                    string.Format(
                        @" 
                                        select 
                                             unit.unit as '巡查单位' 
                                            ,bxc_unit.unit as '被巡查单位'
                                            ,user1.realname as '巡查人'
                                            ,CONVERT(varchar(20), xc.xc_datetime, 120) as '巡查时间'
                                            ,xc.xc_place as '巡查场所'
                                            ,xc.xc_detail as '巡查结果描述'
                                            ,case xc.xc_state when 0 then '待核定' 
                                               when 1 then '待处理'
                                               when 2 then '处理中'
                                               when 3 then '已处理'
                                            end  as '状态'
                                            from XC_Main xc
                                            left join base_unit unit on xc.xc_unit_id=unit.Base_Unit_id
                                            left join base_unit bxc_unit on xc.bxc_unit_id=bxc_unit.Base_Unit_id
                                            left join base_user user1 on xc.xc_user_id=user1.UserId
                                            where xc.xc_res in (1)  and xc.xc_state in (1,2,3) and xc.bxc_unit_id='{0}'
                                       "
                        , ManageProvider.Provider.Current().CompanyId);

                ////if (unit_id != "")//巡查单位ID
                ////{
                ////    if (unit_id != Share.UNIT_ID_JS)//不是江苏省院
                ////    {
                ////        if (contianssubordinateunit == "1")
                ////        {
                ////            sqlTotal = sqlTotal + " and (xc.xc_unit_id ='" + unit_id + "' or (xc.xc_unit_id in (select base_unit_id from base_unit where parent_unit_id='" + unit_id + "' ))) ";
                ////        }
                ////        else
                ////        {
                ////            sqlTotal = sqlTotal + " and xc.xc_unit_id ='" + unit_id + "'";
                ////        }
                ////    }
                ////    else
                ////    {
                ////        if (contianssubordinateunit == "1")
                ////        {

                ////        }
                ////        else
                ////        {
                ////            sqlTotal = sqlTotal + " and xc.xc_unit_id ='" + unit_id + "'";
                ////        }
                ////    }
                ////}
                ////else
                ////{
                ////    if (ManageProvider.Provider.Current().CompanyId != Share.UNIT_ID_JS)//不是江苏省院
                ////    {
                ////        sqlTotal = sqlTotal + " and (xc.xc_unit_id ='" + ManageProvider.Provider.Current().CompanyId + "' or (xc.xc_unit_id in (select base_unit_id from base_unit where parent_unit_id='" + ManageProvider.Provider.Current().CompanyId + "' ))) ";
                ////    }
                ////    else
                ////    {

                ////    }
                ////}

                if (bxc_unit_id != "")//被巡查单位ID
                {
                    sqlTotal = sqlTotal + " and xc.bxc_unit_id ='" + bxc_unit_id + "'";
                }
                if (xc_datetimestart != "")//巡查时间开始
                {
                    sqlTotal = sqlTotal + " and  xc.xc_datetime>= '" + xc_datetimestart + "'";
                }
                if (xc_datetimeend != "")//巡查时间结束
                {
                    sqlTotal = sqlTotal + " and  xc.xc_datetime<= '" + xc_datetimeend + "'";
                }
                if (xc_place != "")//巡查场所
                {
                    sqlTotal = sqlTotal + " and xc.xc_place = '" + xc_place + "'";
                }
                if (xc_detail != "")//巡查明细
                {
                    sqlTotal = sqlTotal + " and xc.xc_detail like '%" + xc_detail + "%'";
                }
                if (xc_state != "")//状态
                {
                    sqlTotal = sqlTotal + " and xc.xc_state = '" + xc_state + "'";
                }

                DataTable dt = SqlHelper.DataTable(sqlTotal, CommandType.Text);//Repository().FindTableBySql(sql);


                return dt;
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
        public string GridPageApplyJsonDetail(string ParameterJson, JqGridParam jqgridparam, string xc_id)
        {
            string unit_id = ManageProvider.Provider.Current().CompanyId;

            try
            {
                int pageIndex = jqgridparam.page;
                int pageSize = jqgridparam.rows;
                Stopwatch watch = CommonHelper.TimerStart();
                string sqlTotal =
                    string.Format(
                        @"  select * from ( 
                                           select 
                                            ROW_NUMBER() over(order by xc.xc_datetime desc) rowNumber, con.name 
                                            ,con.detail, con.orders, con.place
                                            ,case con.type when 1 then '规范情况' when 2 then '视频情况' end as  type1
                                            from XC_Main xc
                                            left join XC_Detail xcd on xc.xc_id=xcd.xc_id
                                            left join XC_Content con on xcd.xc_content_id=con.xc_content_id
                                             where  xc.xc_id='{0}'
                                        ) as a   "
                        , xc_id
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

        /// <summary>
        /// 获取巡查头信息
        /// </summary>
        /// <param name="xc_id"></param>
        /// <returns></returns>
        public DataTable GetHeadInfo(string xc_id)
        {
            string sql = string.Format(@" 
                select 
                 unit.unit 
                ,bxc_unit.unit 
                ,user1.realname
                ,CONVERT(varchar(20), xc.xc_datetime, 120) xc_datetime
                ,xc.xc_detail
                ,case xc.xc_state when 1 then (select realname from base_user where UserId='{1}' ) else user2.realname end as handle_user_id
                ,case xc.xc_state when 1 then CONVERT(varchar(20), getdate(), 120)  else CONVERT(varchar(20), xc.handle_date, 120) end  as handle_date
                ,xc.handle_detail
                ,xc.xc_state
                from XC_Main xc
                left join base_unit unit on xc.xc_unit_id=unit.Base_Unit_id
                left join base_unit bxc_unit on xc.bxc_unit_id=bxc_unit.Base_Unit_id
                left join base_user user1 on xc.xc_user_id=user1.UserId
                left join base_user user2 on xc.handle_user_id=user2.UserId
                where xc.xc_id = '{0}'
                "
                , xc_id
                , ManageProvider.Provider.Current().UserId
                );
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

        
 
        /// 巡查反馈
        public int SubmitFormMy(string xc_id, string handle_detail, string xc_state)
        {
            string sql = string.Format(@" update XC_Main set handle_user_id=@handle_user_id,handle_date=getdate(),handle_detail=@handle_detail,xc_state=@xc_state where xc_id=@xc_id ");
            SqlParameter[] pars = new SqlParameter[]
            {
                new SqlParameter("@handle_user_id",ManageProvider.Provider.Current().UserId),
                new SqlParameter("@handle_detail",handle_detail),
                new SqlParameter("@xc_state",xc_state),
                new SqlParameter("@xc_id",xc_id)
            };
            try
            {
                int r = SqlHelper.ExecuteNonQuery(sql, CommandType.Text, pars);
                return r;
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}