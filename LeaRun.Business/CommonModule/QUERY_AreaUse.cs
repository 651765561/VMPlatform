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
    public class QUERY_AreaUseBll : RepositoryFactory<QUERY_MyZQ>
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
                        @" select (select unit from base_unit where base_unit_id ='e2c79c56-5b58-4c62-b2a9-3bb7492c') as unitname,
                                     count1.count count1,count2.count count2 ,count3.count count3,count4.count count4
                                from 
                                (select 
                                count(*) as count
                                from JW_Usedetail join jw_apply on JW_Usedetail.apply_id=jw_apply.apply_id and jw_apply.type=1
                                and JW_Usedetail.unit_id='{0}'  )count1
                                join 
                                (select 
                                count(*) as count
                                from JW_Usedetail join jw_apply on JW_Usedetail.apply_id=jw_apply.apply_id and jw_apply.type=2
                                and JW_Usedetail.unit_id='{0}' )count2  on 1=1
                                join 
                                (select 
                                count(*) as count
                                from JW_Usedetail join jw_apply on JW_Usedetail.apply_id=jw_apply.apply_id and jw_apply.type=3
                                and JW_Usedetail.unit_id='{0}' )count3  on 1=1
                                join 
                                (select 
                                count(*) as count
                                from checkin_lf 
                                where policearea_id in (select policearea_id from base_policearea where unit_id='{0}' ))count4 on 1=1 "
                        , unit_id
                        );

//                string sql =
//                string.Format(
//                    @" select * from ( 
//                                        {4}
//                                        ) as a  
//                                        where rowNumber between {0} and {1}  
//                                        order by {2} {3} "
//                    , (pageIndex - 1) * pageSize + 1
//                    , pageIndex * pageSize
//                    , jqgridparam.sidx
//                    , jqgridparam.sord
//                    , sqlTotal
//                    );

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
        public string GridPageApplyJsonQuery(string ParameterJson, JqGridParam jqgridparam, string unit_id, string applydatestart, string applydateend,string contianssubordinateunit)
        {

            //string unit_id = ManageProvider.Provider.Current().CompanyId;
            // string user_id = ManageProvider.Provider.Current().UserId;
            try
            {
                int pageIndex = jqgridparam.page;
                int pageSize = jqgridparam.rows;
                Stopwatch watch = CommonHelper.TimerStart();
                string where1 = "";
                string where2 = "";
                string where3 = "";
                string where4 = "";


                if (unit_id != "")//单位ID
                {
                    if (unit_id != Share.UNIT_ID_JS)//不是江苏省院
                    {
                        if (contianssubordinateunit == "1")
                        {
                            where1 = where1 + " and (JW_Usedetail.unit_id ='" + unit_id + "' or (JW_Usedetail.unit_id in (select base_unit_id from base_unit where parent_unit_id='" + unit_id + "' ))) ";
                            where2 = where2 + " and (JW_Usedetail.unit_id ='" + unit_id + "' or (JW_Usedetail.unit_id in (select base_unit_id from base_unit where parent_unit_id='" + unit_id + "' ))) ";
                            where3 = where3 + " and (JW_Usedetail.unit_id ='" + unit_id + "' or (JW_Usedetail.unit_id in (select base_unit_id from base_unit where parent_unit_id='" + unit_id + "' ))) ";
                            where4 = where4 + " and checkin_lf.policearea_id in (select policearea_id from base_policearea where unit_id='" + unit_id + "' or unit_id in  (select base_unit_id from base_unit where parent_unit_id='" + unit_id + "' ))  ";
                        }
                        else
                        {
                            where1 = where1 + " and JW_Usedetail.unit_id ='" + unit_id + "'";
                            where2 = where2 + " and JW_Usedetail.unit_id ='" + unit_id + "'";
                            where3 = where3 + " and JW_Usedetail.unit_id ='" + unit_id + "'";
                            where4 = where4 + " and checkin_lf.policearea_id in (select policearea_id from base_policearea where unit_id='" + unit_id + "' )";
                        }
                    }
                    else
                    {
                        if (contianssubordinateunit == "1")
                        {

                        }
                        else
                        {
                            where1 = where1 + " and JW_Usedetail.unit_id ='" + unit_id + "'";
                            where2 = where2 + " and JW_Usedetail.unit_id ='" + unit_id + "'";
                            where3 = where3 + " and JW_Usedetail.unit_id ='" + unit_id + "'";
                            where4 = where4 + " and checkin_lf.policearea_id in (select policearea_id from base_policearea where unit_id='" + unit_id + "' )";
                        }
                    }
                }
                else
                {
                    if (ManageProvider.Provider.Current().CompanyId != Share.UNIT_ID_JS)//不是江苏省院
                    {
                        where1 = where1 + " and (JW_Usedetail.unit_id ='" + ManageProvider.Provider.Current().CompanyId + "' or (JW_Usedetail.unit_id in (select base_unit_id from base_unit where parent_unit_id='" + ManageProvider.Provider.Current().CompanyId + "' ))) ";
                        where2 = where2 + " and (JW_Usedetail.unit_id ='" + ManageProvider.Provider.Current().CompanyId + "' or (JW_Usedetail.unit_id in (select base_unit_id from base_unit where parent_unit_id='" + ManageProvider.Provider.Current().CompanyId + "' ))) ";
                        where3 = where3 + " and (JW_Usedetail.unit_id ='" + ManageProvider.Provider.Current().CompanyId + "' or (JW_Usedetail.unit_id in (select base_unit_id from base_unit where parent_unit_id='" + ManageProvider.Provider.Current().CompanyId + "' ))) ";
                        where4 = where4 + " and checkin_lf.policearea_id in (select policearea_id from base_policearea where unit_id='" + ManageProvider.Provider.Current().CompanyId + "' or unit_id in  (select base_unit_id from base_unit where parent_unit_id='" + ManageProvider.Provider.Current().CompanyId + "' ))  ";
                    }
                    else
                    {

                    }
                }
                if (applydatestart != "")//实际进入时间
                {
                    where1 = where1 + " and  JW_Usedetail.startdate>= '" + applydatestart + "'";
                    where2 = where2 + " and  JW_Usedetail.startdate>= '" + applydatestart + "'";
                    where3 = where3 + " and  JW_Usedetail.startdate>= '" + applydatestart + "'";
                    where4 = where4 + " and  checkin_lf.check_time>= '" + applydatestart + "'";
                }
                if (applydateend != "")//实际进入时间
                {
                    where1 = where1 + " and  JW_Usedetail.startdate<= '" + applydateend + "'";
                    where2 = where2 + " and  JW_Usedetail.startdate<= '" + applydateend + "'";
                    where3 = where3 + " and  JW_Usedetail.startdate<= '" + applydateend + "'";
                    where4 = where4 + " and  checkin_lf.check_time<= '" + applydateend + "'";
                }


                string sqlTotal =
                   string.Format(
                       @" select (select unit from base_unit where base_unit_id ='e2c79c56-5b58-4c62-b2a9-3bb7492c') as unitname,
                                     count1.count count1,count2.count count2 ,count3.count count3,count4.count count4
                                from 
                                (select 
                                count(*) as count
                                from JW_Usedetail join jw_apply on JW_Usedetail.apply_id=jw_apply.apply_id and jw_apply.type=1
                                {1} )count1
                                join 
                                (select 
                                count(*) as count
                                from JW_Usedetail join jw_apply on JW_Usedetail.apply_id=jw_apply.apply_id and jw_apply.type=2
                                {2} )count2  on 1=1
                                join 
                                (select 
                                count(*) as count
                                from JW_Usedetail join jw_apply on JW_Usedetail.apply_id=jw_apply.apply_id and jw_apply.type=3
                                {3})count3  on 1=1
                                join 
                                (select 
                                count(*) as count
                                from checkin_lf 
                                where  1=1
                                {4} )count4 on 1=1 "
                       , unit_id
                       , where1
                       , where2
                       , where3
                       , where4 
                       );

               
               
             

//                string sql =
//                string.Format(
//                    @" select * from ( 
//                                        {4}
//                                        ) as a  
//                                        where rowNumber between {0} and {1}  
//                                        order by {2} {3} "
//                    , (pageIndex - 1) * pageSize + 1
//                    , pageIndex * pageSize
//                    , jqgridparam.sidx
//                    , jqgridparam.sord
//                    , sqlTotal
//                    );

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
      
    }
}