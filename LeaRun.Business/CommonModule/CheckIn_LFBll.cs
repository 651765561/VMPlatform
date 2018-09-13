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
    /// CheckIn_LF
    /// <author>
    ///		<name>she</name>
    ///		<date>2016.10.25 09:42</date>
    /// </author>
    /// </summary>
    public class CheckIn_LFBll : RepositoryFactory<CheckIn_LF>
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
        ///  绑定来访登记
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
                                        ROW_NUMBER() over(order by check_time desc) rowNumber
                                        , lf.checkIn_LF_Id,lf.name,lf.sex,lf.sfz_id,lf.checkInfo,lf.goods,CONVERT(varchar(20), lf.check_time, 120) check_time,
                                        lf.remarks,lf.address,lf.tel,lf.unit,lf.photo,lf.PoliceArea_id, pa.AreaName,lf.alertLever,lf.adduser_id,lf.chuli,
                                        lf.department,lf.info, lf.userJD,u.unit as unitName,bu.RealName  from CheckIn_LF lf
                                        left join Base_PoliceArea pa on pa.PoliceArea_id =lf.PoliceArea_id
                                        left join Base_unit u on u.Base_Unit_id =pa.unit_id
                                        left join Base_User bu on lf.adduser_id =bu.UserId
                                        where lf.adduser_id='{0}'
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
        /// 列表加载
        /// </summary>
        /// <param name="ParameterJson"></param>
        /// <param name="unit_id"></param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public string GridPageApplyJsonMyQuery(string ParameterJson, JqGridParam jqgridparam, string name)
        {

            try
            {
                int pageIndex = jqgridparam.page;
                int pageSize = jqgridparam.rows;
                Stopwatch watch = CommonHelper.TimerStart();
                string sqlTotal =
                string.Format(
                        @" 
                                        select 
                                        ROW_NUMBER() over(order by check_time desc) rowNumber
                                        ,lf.checkIn_LF_Id,lf.name,lf.sex,lf.sfz_id,lf.checkInfo,lf.goods,CONVERT(varchar(20), lf.check_time, 120) check_time,
                                        lf.remarks,lf.address,lf.tel,lf.unit,lf.photo,lf.PoliceArea_id, pa.AreaName,lf.alertLever,lf.adduser_id,lf.chuli,
                                        lf.department,lf.info, lf.userJD from CheckIn_LF lf
                                        left join Base_PoliceArea pa on pa.PoliceArea_id =lf.PoliceArea_id
                                        where lf.adduser_id='{0}'
                                      "
                            , ManageProvider.Provider.Current().UserId
                        );
              
                if (name != "")//姓名
                {
                    sqlTotal = sqlTotal + " and lf.name like '%" + name + "%'";
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

        /// <summary>
        ///  绑定历史来访记录
        /// </summary>
        /// <returns></returns>
        public string GridPageJsonHistory(JqGridParam jqgridparam, string Cardid)
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
                                        ROW_NUMBER() over(order by checkIn_LF_Id) rowNumber
                                       ,lf.checkIn_LF_Id,CONVERT(varchar(20), lf.check_time, 120) check_time ,lf.name ,lf.remarks,u.unit,pa.AreaName,lf.alertLever,lf.adduser_id,us.realname,lf.chuli,
                                       lf.department,lf.info, lf.userJD from CheckIn_LF lf 
                                       left join Base_PoliceArea pa on lf.PoliceArea_id=pa.PoliceArea_id
                                       left join Base_Unit u on pa.unit_id= u.Base_Unit_id
                                       left join Base_User us on us.userid= lf.adduser_id
                                       where lf.sfz_id='{0}'
                                        ) as a   "
                            , Cardid
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
        /// 删除来访登记表
        /// </summary>
        /// <param name="apply_id"></param>
        /// <returns></returns>
        public int DeleteCheckIn_LF(string apply_id)
        {
            string sql = string.Format(@" delete CheckIn_LF where checkIn_LF_Id ='" + apply_id + "'");
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



        /// <summary>
        /// 删除来访登记表
        /// </summary>
        /// <param name="apply_id"></param>
        /// <returns></returns>
        public string CheckIsZQ()
        {

            string sql = string.Format(@" select  count(*) from JW_OnDuty where type=4 and state=0 and DutyUser_id ='" + ManageProvider.Provider.Current().UserId+ "'");
            DataTable dt = SqlHelper.DataTable(sql, CommandType.Text);//Repository().FindTableBySql(sql);
            try
            {
                return dt.Rows[0][0].ToString();
            }
            catch (Exception)
            {
                return "0";
            }
        }
    }
}