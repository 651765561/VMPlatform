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
    /// QUERY_MyZQ
    /// <author>
    ///		<name>she</name>
    ///		<date>2017.02.27 17:53</date>
    /// </author>
    /// </summary>
    public class QUERY_MyZQBll : RepositoryFactory<QUERY_MyZQ>
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
                         @" select * from ( 
                                        select 
                                            ROW_NUMBER() over(order by SendDate DESC) rowNumber
                                                ,sp.unit_id
                                                ,unit.unit
                                                ,sp.SendUser_id
                                                ,u2.RealName as sendUserName
                                                ,sp.SendDate
                                                ,sp.type
                                                ,sp.Object_id
                                                ,sp.SendPolice_id
                                                ,sp.state
                                                ,u.RealName
                                                ,sp.backreason
                                                ,CONVERT(varchar(20), sp.actdate, 120) actdate
                                                ,CONVERT(varchar(20), sp.enddate, 120) enddate
                                                ,replace(sp.actdetail,'!@#$%^&*()','') actdetail
                                                ,sp.user_id
                                        from JW_SendPolice sp 
                                        left join base_user u on u.UserId=sp.user_id
                                        left join base_user u2 on u2.UserId=sp.SendUser_id
                                        left join base_unit unit on unit.base_unit_id=sp.unit_id
                                        where sp.user_id='{0}'  and sp.type in (1,2,3,8,9,4,5)

                                   

                                        ) as a   "
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

        /// <summary>
        /// 列表加载
        /// </summary>
        /// <param name="ParameterJson"></param>
        /// <param name="unit_id"></param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public string GridPageApplyJsonQuery(string ParameterJson, JqGridParam jqgridparam, string type, string SendDate_start, string SendDate_end, string state)
        {

            try
            {
                string user_id = ManageProvider.Provider.Current().UserId;
                int pageIndex = jqgridparam.page;
                int pageSize = jqgridparam.rows;
                Stopwatch watch = CommonHelper.TimerStart();
                 string sqlTotal =
                    string.Format(
                        @" 
                                      select    ROW_NUMBER() over(order by SendDate DESC) rowNumber
                                      ,sp.unit_id ,unit.unit ,sp.SendUser_id ,u2.RealName as sendUserName  ,sp.SendDate
                                      ,sp.type,sp.Object_id ,sp.state ,u.RealName ,sp.backreason ,CONVERT(varchar(20), sp.actdate, 120) actdate
                                                ,CONVERT(varchar(20), sp.enddate, 120) enddate
                                                ,replace(sp.actdetail,'!@#$%^&*()','') actdetail ,sp.SendPolice_id,sp.user_id from JW_SendPolice sp 
                                      left join base_user u on u.UserId=sp.user_id
                                      left join base_user u2 on u2.UserId=sp.SendUser_id
                                      left join base_unit unit on unit.base_unit_id=sp.unit_id
                                      where sp.user_id='{0}' and sp.type in (1,2,3,8,9,4,5) 

                                     "
                        , user_id
                        );
                 //and type not in (1,2,3)
                                   
                 if (type != "")//派警区分
                {
                    sqlTotal = sqlTotal + " and sp.type = '" + type + "'";
                }
                 if (SendDate_start != "")//派警开始日期
                {
                    sqlTotal = sqlTotal + " and  sp.SendDate> '" + SendDate_start + "'";
                }
                 if (SendDate_end != "")//派警结束日期
                {
                    sqlTotal = sqlTotal + " and  sp.SendDate< '" + SendDate_end + "'";
                }
                 if (state != "")
                {
                    sqlTotal = sqlTotal + " and sp.state = " + state;
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
        /// 获得登陆者信息
        /// </summary>
        /// <param name="unitId"></param>
        /// <returns></returns>
        public DataTable GetLoginUserId(string UserId)
        {
            string sql = string.Format(@" 
                select 
                UNIT.Base_Unit_id, UNIT.unit,DEP.Dep_id,DEP.Name,USE1.UserId,USE1.RealName,Telephone
                from Base_User USE1
                join Base_Unit UNIT ON UNIT.Base_Unit_id=USE1.CompanyId
                join Base_Department DEP  ON DEP.Dep_id=USE1.dep_id
                WHERE USE1.UserId='{0}' 
                "
                , UserId
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




       


        /// <summary>
        /// 列表加载明细TITLE
        /// </summary>
        /// <param name="ParameterJson"></param>
        /// <param name="unit_id"></param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public DataTable GetJWAPPLY(string apply_id, string user_id)
        {
            string sql = string.Format(@" 
                                        select 
                                            AreaName,CONVERT(varchar(20), sp.actdate, 120) actdate,
                                            CONVERT(varchar(20), sp.enddate, 120) enddate,replace(sp.actdetail,'!@#$%^&*()','') actdetail
                                            from JW_OnDuty od
                                            left join jw_sendpolice sp on sp.object_id=od.ONDUTY_id
                                            left join base_PoliceArea pa on pa.PoliceArea_id=od.PoliceArea_id
                                            where sp.sendpolice_id='{0}' 
                                       "
                         , apply_id
                         , user_id
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


        /// <summary>
        /// 列表加载
        /// </summary>
        /// <param name="ParameterJson"></param>
        /// <param name="unit_id"></param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public DataTable GridPageApplyJsonExcel(string type, string SendDate_start, string SendDate_end, string state)
        {

            //string unit_id = ManageProvider.Provider.Current().CompanyId;
             string user_id = ManageProvider.Provider.Current().UserId;
            try
            {

                string sqlTotal =
                    string.Format(
                        @" 
                                         select    
                                       unit.unit as '派警单位'
                                      ,u2.RealName as '派警人'
                                      ,CONVERT(varchar(20), sp.SendDate, 120) as '派警日期'
                                      ,case sp.type    when 1 then '办案区'
                                                        when 2 then '刑检办案区'
                                                        when 3 then '指定居所'
                                                        when 4 then '用警申请'
                                                        when 5 then '直接派警'
                                                        when 6 then '调警申请'
                                                        when 7 then '调警令'
                                                        when 8 then '控申接待'
                                                        when 9 then '机关安保'
                                        end as '派警方式'
                                       ,CONVERT(varchar(20), sp.actdate, 120) as '执勤开始时间'
                                       ,CONVERT(varchar(20), sp.enddate, 120) as '执勤结束时间'
                                       ,replace(sp.actdetail,'!@#$%^&*()','') as '执勤情况'
                                       ,sp.backreason as '法警退回原因'
                                       from JW_SendPolice sp 
                                       left join base_user u on u.UserId=sp.user_id
                                       left join base_user u2 on u2.UserId=sp.SendUser_id
                                       left join base_unit unit on unit.base_unit_id=sp.unit_id
                                       where sp.user_id='{0}' and sp.type in (1,2,3,8,9,4,5) 

                                     "
                        , user_id
                        );
                //and type not in (1,2,3)

                if (type != "")//派警区分
                {
                    sqlTotal = sqlTotal + " and sp.type = '" + type + "'";
                }
                if (SendDate_start != "")//派警开始日期
                {
                    sqlTotal = sqlTotal + " and  sp.SendDate> '" + SendDate_start + "'";
                }
                if (SendDate_end != "")//派警结束日期
                {
                    sqlTotal = sqlTotal + " and  sp.SendDate< '" + SendDate_end + "'";
                }
                if (state != "")
                {
                    sqlTotal = sqlTotal + " and sp.state = " + state;
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