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
using System;
using System.Data.SqlClient;
using LeaRun.DataAccess;
using System.Data;
using System.Diagnostics;

namespace LeaRun.Business
{
    /// <summary>
    /// PeopleControl
    /// <author>
    ///		<name>she</name>
    ///		<date>2017.01.16 12:37</date>
    /// </author>
    /// </summary>
    public class PeopleControlBll : RepositoryFactory<People>
    {

        /// <summary>
        /// 列表加载
        /// </summary>
        /// <param name="ParameterJson"></param>
        /// <param name="unit_id"></param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public string GetPeople(string ParameterJson, JqGridParam jqgridparam)
        {
            string unit_id = ManageProvider.Provider.Current().CompanyId;
            string user_id = ManageProvider.Provider.Current().UserId;
            try
            {
                int pageIndex = jqgridparam.page;
                int pageSize = jqgridparam.rows;
                Stopwatch watch = CommonHelper.TimerStart();
                string sql = "select p.People_id,p.designation,r.name as room_id,p.name,p.sex,p.selftype,p.selfmoney,";
                sql = sql + " CONVERT(varchar(20), p.startdate, 120) startdate,CONVERT(varchar(20), p.enddate, 120) enddate,p.limit,p.punish,p.note,p.adduser,p.adddate  from people p";
                sql = sql + " left join base_room r on r.room_id=p.room_id";
                sql = sql + " where p.room_id in (select room_id from base_room where user_id='" + user_id  + "')";
                sql = sql + " order by p.People_id";

                DataTable dt = DbHelper.GetDataSet(CommandType.Text, sql).Tables[0];//Repository().FindTableBySql(sql);

                var JsonData = new
                {
                    total = Convert.ToInt32(Math.Ceiling(dt.Rows.Count * 1.0 / jqgridparam.rows)), //总页数
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
        public string GridPageJsonMyQuery(string ParameterJson, JqGridParam jqgridparam, string designation,string area_id,string room_id, string name, string sex, string startdate, string enddate)
        {

            //string unit_id = ManageProvider.Provider.Current().CompanyId;
            string user_id = ManageProvider.Provider.Current().UserId;
            try
            {
                int pageIndex = jqgridparam.page;
                int pageSize = jqgridparam.rows;
                Stopwatch watch = CommonHelper.TimerStart();
                string sql = "select p.People_id,p.designation,r.name as room_id,p.name,p.sex,p.selftype,p.selfmoney,";
                sql = sql + " CONVERT(varchar(20), p.startdate, 120) startdate,CONVERT(varchar(20), p.enddate, 120) enddate,p.limit,p.punish,p.note,p.adduser,p.adddate  from people p";
                sql = sql + " left join base_room r on r.room_id=p.room_id";
                sql = sql + " where p.room_id in (select room_id from base_room where user_id='" + user_id + "')";
              //  sql = sql + " order by p.People_id";


                if (designation != "")//番号
                {
                    sql = sql + " and p.designation like '%" + designation.Trim() + "%'";
                }
                if (area_id != "")//监区
                {
                    sql = sql + " and r.area_id='" + area_id + "'";
                }
                if (room_id != "")//监室
                {
                    sql = sql + " and p.room_id='" + room_id + "'";
                }
                if (name != "")//姓名
                {
                    sql = sql + " and p.name like '%" + name.Trim() + "%'";
                }
                if (sex != "")//性别
                {
                    sql = sql + " and p.sex = '" + sex + "'";
                }
                if (startdate != "")//入所开始日期
                {
                    sql = sql + " and  p.startdate> '" + startdate + "'";
                }
                if (enddate != "")//入所结束日期
                {
                    sql = sql + " and  p.startdate< '" + enddate + "'";
                }
                sql = sql + " order by p.People_id " + jqgridparam.sord;


                DataTable dt = DbHelper.GetDataSet(CommandType.Text,sql ).Tables[0];//Repository().FindTableBySql(sql);

                var JsonData = new
                {
                    total = Convert.ToInt32(Math.Ceiling(dt.Rows.Count * 1.0 / jqgridparam.rows)), //总页数
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
        /// 新增，编辑
        /// </summary>
        /// <param name="jwApply"></param>
        /// <param name="submitType"></param>
        /// <returns></returns>
        public int AddPeople(People people, string strKeyValue)
        {
            if (strKeyValue == "")//新增
            {
                string area_id = Guid.NewGuid().ToString();
                string sql =
                string.Format(@"insert into people(People_id,designation,room_id,name,sex,selftype,selfmoney,startdate,enddate,limit,punish,note,adduser,adddate,state ) 
                                              values (@People_id,@designation,@room_id,@name,@sex,@selftype,@selfmoney,@startdate,@enddate,@limit,@punish,@note ,@adduser,@adddate,@state)");
             
                SqlParameter[] pars = new SqlParameter[]
                {       
                    new SqlParameter("@People_id",Guid.NewGuid().ToString()),
                    new SqlParameter("@designation",people.designation),
                    new SqlParameter("@room_id",people.room_id),
                    new SqlParameter("@name",people.name),
                    new SqlParameter("@sex",people.sex),
                    new SqlParameter("@selftype",people.selftype),
                    new SqlParameter("@selfmoney",people.selfmoney==null?0:people.selfmoney),
                    new SqlParameter("@startdate",people.startdate),
                    new SqlParameter("@enddate",people.enddate==null?(object)DBNull.Value:people.enddate),
                    new SqlParameter("@limit",people.limit==null?500:people.limit),
                    new SqlParameter("@punish",people.punish),
                    new SqlParameter("@note",people.note),
                    new SqlParameter("@adduser",ManageProvider.Provider.Current().UserId),
                    new SqlParameter("@adddate",DateTime.Now),
                    new SqlParameter("@state",1)
                  
                };

                try
                {
                    int r = DbHelper.ExecuteNonQuery(CommandType.Text, sql, pars);
                    return r;
                }
                catch (Exception ex)
                {
                    return 0;
                }
            }
            else
            {
                string sql =
                      string.Format(@"update people set People_id=@People_id,designation=@designation,room_id=@room_id,name=@name,sex=@sex,
                                                        selftype=@selftype,selfmoney=@selfmoney,startdate=@startdate,enddate=@enddate,
                                                        limit=@limit,punish=@punish,note=@note 
                                       where  People_id=@People_id ");
                SqlParameter[] pars = new SqlParameter[]
                {
                    new SqlParameter("@People_id",strKeyValue),
                    new SqlParameter("@designation",people.designation),
                    new SqlParameter("@room_id",people.room_id),
                    new SqlParameter("@name",people.name),
                    new SqlParameter("@sex",people.sex),
                    new SqlParameter("@selftype",people.selftype),
                    new SqlParameter("@selfmoney",people.selfmoney==null?0:people.selfmoney),
                    new SqlParameter("@startdate",people.startdate),
                    new SqlParameter("@enddate",people.enddate==null?(object)DBNull.Value:people.enddate),
                    new SqlParameter("@limit",people.limit==null?500:people.limit),
                    new SqlParameter("@punish",people.punish),
                    new SqlParameter("@note",people.note),
                  
                };

                try
                {
                    int r = DbHelper.ExecuteNonQuery(CommandType.Text, sql, pars);
                    return r;
                }
                catch (Exception ex)
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// 番号验证
        /// </summary>
        /// <param name="unitId"></param>
        /// <returns></returns>
        public DataTable checkDesignation(string KeyValue)
        {
            string sql = string.Format(@" 
                select 1 from people p
                WHERE p.designation='{0}' and p.state=1 and p.room_id in (select room_id from base_room where user_id='{1}')
                "
                , KeyValue
                , ManageProvider.Provider.Current().UserId
                );
            try
            {
                DataTable dt = DbHelper.GetDataSet(CommandType.Text, sql).Tables[0];
                return dt;
            }
            catch (Exception)
            {
                return null;
            }
        }




        /// <summary>
        /// 获得AREA_ID,ROOM_ID
        /// </summary>
        /// <param name="KeyValue"></param>
        /// <returns></returns>
        public DataTable GetAreaRoom(string KeyValue)
        {
            string sql = string.Format(@" 
                select a.area_id,p.room_id from people p
                left join base_room r on r.room_id=p.room_id
                left join base_area a on r.area_id=a.area_id
                WHERE p.people_id='{0}' and p.state=1 and p.room_id in (select room_id from base_room where user_id='{1}')
                "
                , KeyValue
                , ManageProvider.Provider.Current().UserId
                );
            try
            {
                DataTable dt = DbHelper.GetDataSet(CommandType.Text, sql).Tables[0];
                return dt;
            }
            catch (Exception)
            {
                return null;
            }
        }





    }
}